using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Singleton;
using YooAsset;

/// <summary>
/// 初始化资源包
/// </summary>
internal class FsmInitialize : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("初始化资源包！");
        UniSingleton.StartCoroutine(InitPackage());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator InitPackage()
    {
        //yield return new WaitForSeconds(1f);

        var playMode = PatchManager.Instance.PlayMode;

        // 创建默认的资源包
        string packageName = VersionSettings.PackageName;
        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);
        }

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            //TODO: ？？？离线运行模式
            var createParameters = new OfflinePlayModeParameters();
            createParameters.DecryptionServices = new GameDecryptionServices();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            var createParameters = new HostPlayModeParameters();
            createParameters.DecryptionServices = new GameDecryptionServices();

            createParameters.QueryServices = new GameQueryServices();
            //! 与CDN 资源服务器有关
            createParameters.DefaultHostServer = GetHostServerURL();
            createParameters.FallbackHostServer = GetHostServerURL();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        yield return initializationOperation;
        if (initializationOperation.Status == EOperationStatus.Succeed)
        {
            //！ 初始化步骤成功后，进行版本更新？？
            _machine.ChangeState<FsmUpdateVersion>();
        }
        else
        {
            Debug.LogWarning($"{initializationOperation.Error}");
            PatchEventDefine.InitializeFailed.SendEventMessage();
        }
    }

    private string GetProjectName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "umi_apk_dev";
            case RuntimePlatform.IPhonePlayer:
                return "umi_ipa_dev";
            default:
                return "umi_pc_dev";
        }
    }


    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        VersionSettings.CDN_URL = "http://127.0.0.1:8000";
#if !UNITY_EDITOR
        //已构建版本  CDN 地址
        if(VersionSettings.ReleaseLocalServerURL)
        {
            VersionSettings.CDN_URL = "http://127.0.0.1:8000";
        }
        else
        {
             string ab_folder = GetRemoteABFolder();
             VersionSettings.CDN_URL = $"http://192.168.210.41/share/{ab_folder}/ab"; 
        }




#else
        //Editor 下 CDN 地址

        if (VersionSettings.EditorRomoteServerURL)
        {
            string ab_folder = GetRemoteABFolder();
            VersionSettings.CDN_URL = $"http://192.168.210.41/share/{ab_folder}/ab";
        }
        else
        {
            VersionSettings.CDN_URL = "http://127.0.0.1:8000";
        }

#endif




        string PackageName = VersionSettings.PackageName;
        string appVersion = VersionSettings.AssetFolderName;


        //appVersion = VersionSettings.AssetFolderName;

#if UNITY_EDITOR
        return $"{VersionSettings.CDN_URL}/{PackageName}/{appVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{VersionSettings.CDN_URL}/{PackageName}/{appVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{VersionSettings.CDN_URL}/{PackageName}/{appVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{VersionSettings.CDN_URL}/{PackageName}/{appVersion}";
		else
			return $"{VersionSettings.CDN_URL}/{PackageName}/{appVersion}";
#endif
    }

    private string GetRemoteABFolder()
    {
        string str = "exe";
#if !UNITY_EDITOR
		if (Application.platform == RuntimePlatform.Android)
			str = "apk";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			str = "ipa";
		else
			str = "exe";
#endif

#if UNITY_EDITOR
        str = "apk";
#endif


        return string.Format(VersionSettings.Remote_AB_Folder, str);
    }

    /// <summary>
    /// 资源文件解密服务类
    /// </summary>
    private class GameDecryptionServices : IDecryptionServices
    {
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return bundleStream;
        }

        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }
}