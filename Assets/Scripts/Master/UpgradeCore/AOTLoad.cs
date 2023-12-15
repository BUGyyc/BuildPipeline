using System;
using System.Collections.Generic;
using HybridCLR;
using UnityEngine;


public class AOTLoad
{


    public static async void LoadMetadataForAOTAssemblies(Action overFun)
    {

#if UNITY_EDITOR
        if (VersionSettings.EditorRomoteServerURL)
        {
            //Editor 连接 CDN 测试完整热更
        }
        else
        {
            Debug.Log($" ----------------Editor下， 关闭了热更代码 ---------------- ");

            overFun?.Invoke();
            return;
        }

#endif

        //if (VersionSettings.HybridCLR_Enable == false)
        //{
        //    Debug.Log($" ---------------- 关闭了热更代码 -----HybridCLR_Enable = false----------- ");

        //    overFun?.Invoke();
        //    return;
        //}


        List<string> aotMetaAssemblyFiles = new List<string>()
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll",
            };

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 


        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            Debug.Log($" start LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode}");

            var aotHandler = YooAsset.YooAssets.LoadRawFileAsync("Assets/DataBytes/CodeBytes/" + aotDllName + ".bytes");

            await aotHandler.Task;
            byte[] dllBytes = aotHandler.GetRawFileData();
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }

        Debug.Log($"start load :CodeBytes/Hot.dll.bytes. ");

        var handler = YooAsset.YooAssets.LoadRawFileAsync("Assets/DataBytes/CodeBytes/Hot.dll.bytes");

        await handler.Task;

        Debug.Log($"Load DLL Task Complete ");

        //TextAsset hotCodeTx = await Addressables.LoadAssetAsync<TextAsset>("CodeBytes/Hot.dll.bytes").Task;
        byte[] hotDllBytes = handler.GetRawFileData(); //hotCodeTx.bytes;


        Debug.Log($"Load DLL Bytes data ");

        var hotUpdateAss = System.Reflection.Assembly.Load(hotDllBytes);

        Debug.Log($"Assembly DLL Complete ");

        Type type = hotUpdateAss.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);

        Debug.Log($"start load over :CodeBytes/Hot.dll.bytes. ");


        overFun?.Invoke();


        //Addressables.Release(hotCodeTx);

    }


}
