using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
//using GameEditor;
using YooAsset.Editor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
//using UMI;

namespace BuildPipelineCore
{
    /// <summary>
    /// 自动化构建工具类
    /// </summary>
    public static class AutoBuildUtils
    {

        ///// <summary>
        ///// 脚本入口：构建android项目
        ///// </summary>
        //public static void BuildAndroidProject()
        //{



        //    AutoBuilderBase builder = new AutoBuilderAndroid();
        //    AssetBundleBuilderWindow.BuildInternal(builder.Build);
        //}

        ///// <summary>
        ///// 脚本入口：构建ios项目
        ///// </summary>
        //public static void BuildIosProject()
        //{
        //    AutoBuilderBase builder = new AutoBuilderIos();
        //    AssetBundleBuilderWindow.BuildInternal(builder.Build);
        //}

        ///// <summary>
        ///// 脚本入口：构建window项目
        ///// </summary>
        //public static void BuildWindowProject()
        //{
        //    AutoBuilderBase builder = new AutoBuildWindow();
        //    AssetBundleBuilderWindow.BuildInternal(builder.Build);
        //}



        //public static void BuildDLL(Action BuildAssetBundleAction)
        //{
        //    const string path = "Assets/DataBytes/CodeBytes";

        //    PrebuildCommand.GenerateAll();

        //    if (Directory.Exists(path) == false)
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    //BuildAssetsCommand.CopyABAOTHotUpdateDlls(target);

        //    Debug.Log("====> 复制热更新资源和代码");
        //    BuildAssetsCommand.BuildAndCopyABAOTHotUpdateDlls();

        //    LogMaster.BP("=======Build DLL Over=======");

        //    AssetDatabase.Refresh();
        //    if (BuildAssetBundleAction != null)
        //    {
        //        LogMaster.BP("=======Build BuildAssetBundleAction Start=======");
        //        BuildAssetBundleAction.Invoke();
        //    }
        //}

        public static void BuildAssetBundle(Action BuildAppAction, ECopyBuildinFileOption buildOpt)
        {
            AssetBundleBuilderWindow.BuildInternal(BuildAppAction, buildOpt);
        }


        public static void BuildDLLAndCopy(Action buildAssetBundleAction = null)
        {
            LogMaster.BP("构建DLL Begin");

            LogMaster.BP("构建DLL  GenerateAll  Begin");
            PrebuildCommand.GenerateAll();
            LogMaster.BP("构建DLL  GenerateAll  End");

       

            LogMaster.BP("构建DLL End");

            if (buildAssetBundleAction != null)
            {
                buildAssetBundleAction.Invoke();
            }
        }
    }
}