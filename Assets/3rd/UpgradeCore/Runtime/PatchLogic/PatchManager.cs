﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Event;
using UniFramework.Singleton;
using YooAsset;
//using UMI;

/// <summary>
/// ! 补丁管理器
/// </summary>
public class PatchManager : SingletonInstance<PatchManager>, ISingleton
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public EPlayMode PlayMode { private set; get; }

    /// <summary>
    /// 包裹的版本信息
    /// </summary>
    public string PackageVersion { set; get; }

    /// <summary>
    /// 下载器
    /// </summary>
    public ResourceDownloaderOperation Downloader { set; get; }


    private bool _isRun = false;
    private EventGroup _eventGroup = new EventGroup();
    private StateMachine _machine;

    void ISingleton.OnCreate(object createParam)
    {
    }
    void ISingleton.OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }
    void ISingleton.OnUpdate()
    {
        if (_machine != null)
            _machine.Update();
    }

    /// <summary>
    /// 开启流程
    /// </summary>
    public void Run(EPlayMode playMode, Action call = null)
    {
        if (_isRun == false)
        {
            _isRun = true;
            PlayMode = playMode;

            // 注册监听事件
            _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

            _eventGroup.AddListener<UserEventDefine.UserCancelUpgrade>(OnHandleEventMessage);


            //! 补丁的关键流程
            Debug.Log("开启补丁更新流程...");
            _machine = new StateMachine(this);
            _machine.AddNode<FsmPatchPrepare>();
            _machine.AddNode<FsmInitialize>();
            _machine.AddNode<FsmUpdateVersion>();
            _machine.AddNode<FsmUpdateManifest>();
            _machine.AddNode<FsmCreateDownloader>();
            _machine.AddNode<FsmDownloadFiles>();
            _machine.AddNode<FsmDownloadOver>();
            _machine.AddNode<FsmClearCache>();
            _machine.AddNode<FsmPatchDone>();
            _machine.Run<FsmPatchPrepare>();
        }
        else
        {
            Debug.LogWarning("补丁更新已经正在进行中!");
        }
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is UserEventDefine.UserTryInitialize)
        {
            _machine.ChangeState<FsmInitialize>();
        }
        else if (message is UserEventDefine.UserBeginDownloadWebFiles)
        {
            _machine.ChangeState<FsmDownloadFiles>();
        }
        else if (message is UserEventDefine.UserTryUpdatePackageVersion)
        {
            _machine.ChangeState<FsmUpdateVersion>();
        }
        else if (message is UserEventDefine.UserTryUpdatePatchManifest)
        {
            _machine.ChangeState<FsmUpdateManifest>();
        }
        else if (message is UserEventDefine.UserTryDownloadWebFiles)
        {
            _machine.ChangeState<FsmCreateDownloader>();
        }
        else if (message is UserEventDefine.UserCancelUpgrade)
        {
            Debug.Log("跳过此次热更");

            UpgradeComplete();
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }

    public static void UpgradeComplete()
    {
        HotUpdateOverToStart();
    }

    private static void UpdateCompleteCall()
    {
        //PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");

        Debug.Log("UpdateCompleteCall -----> 开始游戏 ");


        YooAssets.UnloadUnusedAB();

        var handler = YooAssets.LoadSceneAsync("Login");

        handler.Completed += (SceneOperationHandle handle) =>
        {
            
            
        };

        //str = string.Empty;
        //Debug.Log("  Start to load HotEntry");

        //var handler = YooAssets.LoadAssetAsync<GameObject>("HotEntry");
        //handler.Completed += (AssetOperationHandle handle) =>
        //{
        //    Debug.Log("HotEntry Load Complete");
        //    if (handle.AssetObject == null)
        //        return;
        //    // 实例化对象
        //    var obj = handle.InstantiateSync();

        //    //UnloadUpdateUI(obj);
        //};
    }

    public static void HotUpdateOverToStart()
    {
        AOTLoad.LoadMetadataForAOTAssemblies(UpdateCompleteCall);
    }
}