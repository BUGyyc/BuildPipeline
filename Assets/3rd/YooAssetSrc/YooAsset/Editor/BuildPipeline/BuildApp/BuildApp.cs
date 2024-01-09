using UnityEngine;
using UnityEditor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using static UnityEngine.GraphicsBuffer;

namespace BuildPipelineCore
{
    public static class BuildApp
    {

        public const string APP_BUILD_OUTPUT_PATH = "../Release/";

        /// <summary>
        /// ! 单纯的首包构建，无多余AB
        /// </summary>
        [MenuItem("BuildPipeline/Build Single App")]
        public static void BuildSingleApp()
        {
            var buildTask = new AutoBuildWindow();
            buildTask.Build(true);
        }

        /// <summary>
        /// !
        /// </summary>
        [MenuItem("BuildPipeline/Build DLL + AssetBundle")]
        public static void BuildDLL_AssetBundle()
        {
            AutoBuildUtils.BuildDLLAndCopy(() =>
            {
                AutoBuildUtils.BuildAssetBundle(null, YooAsset.Editor.ECopyBuildinFileOption.None); ;
            });
        }



        [MenuItem("BuildPipeline/（本地）构建 BuildIn 包")]
        public static void BuildDLLAssetBundleAPP_BuildIn()
        {
            AutoBuildUtils.BuildDLLAndCopy(() =>
            {
                AutoBuildUtils.BuildAssetBundle(() =>
                {
                    var buildTask = new AutoBuildWindow();
                    buildTask.Build(true, null);
                }, YooAsset.Editor.ECopyBuildinFileOption.ClearAndCopyAll);
            });
        }

        [MenuItem("BuildPipeline/（本地）(弃用) --- 构建 BuildIn 包")]
        public static void _BuildDLLAssetBundleAPP_BuildIn()
        {
            AutoBuildUtils.BuildDLLAndCopy(() =>
            {
                AutoBuildUtils.BuildAssetBundle(() =>
                {
                    var buildTask = new AutoBuildWindow();
                    buildTask.Build(true);
                }, YooAsset.Editor.ECopyBuildinFileOption.ClearAndCopyAll);
            });
        }

        [MenuItem("BuildPipeline/（本地）构建 DLL")]
        public static void BuildDLLAndCopy()
        {

            AutoBuildUtils.BuildDLLAndCopy(null);
            LogMaster.BP("复制 DLL  Begin");
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

            LogMaster.BP("CompileDll DLL  Begin");
            //! build DLL
            CompileDllCommand.CompileDll(target);
            LogMaster.BP("CompileDll DLL  End");


            BuildAssetsCommand.CopyABAOTHotUpdateDlls(target);
            AssetDatabase.Refresh();
            LogMaster.BP("复制 DLL  End");
        }




        [MenuItem("BuildPipeline/（本地）构建 DLL + AssetBundle")]
        public static void BuildDLLAssetBundle()
        {
            AutoBuildUtils.BuildDLLAndCopy(() =>
            {
                AutoBuildUtils.BuildAssetBundle(null, YooAsset.Editor.ECopyBuildinFileOption.None); ;
            });
        }

        [MenuItem("BuildPipeline/（本地）构建热更包 DLL + AssetBundle + APP")]
        public static void BuildDLLAssetBundleAPP_Hot()
        {
            AutoBuildUtils.BuildDLLAndCopy(() =>
            {
                AutoBuildUtils.BuildAssetBundle(null, YooAsset.Editor.ECopyBuildinFileOption.None);
            });
        }





        [MenuItem("QuickHelper/打开配置表 data")]
        public static void OpenData()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../../data/excel/");
        }

        [MenuItem("QuickHelper/打开协议 Proto")]
        public static void OpenProto()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../../proto/");
        }

        [MenuItem("QuickHelper/Open Release")]
        public static void OpenRelease()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../Release/");
        }



        [MenuItem("QuickHelper/Open Bundles")]
        public static void OpenBundles()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../Bundles/");
        }

        [MenuItem("QuickHelper/Open Root Project")]
        public static void OpenRoot()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../../");
        }

        [MenuItem("QuickHelper/Open Tools")]
        public static void OpenTools()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../../Tools/");
        }

        [MenuItem("QuickHelper/Open Cache Bundles")]
        public static void OpenCacheBundles()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../Sandbox/");
        }


        [MenuItem("QuickHelper/Open HybridCLR Output")]
        public static void OpenHybridCLROutput()
        {
            Application.OpenURL("file://" + Application.dataPath + "/../HybridCLRData/HotUpdateDlls/");
        }


    }
}