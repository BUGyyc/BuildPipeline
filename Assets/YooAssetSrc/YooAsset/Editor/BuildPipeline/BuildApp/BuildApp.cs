using UnityEngine;
using UnityEditor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;

namespace BuildPipelineCore
{
    public static class BuildApp
    {

        public const string APP_BUILD_OUTPUT_PATH = "../Release/";


        [MenuItem("BuildPipeline/Build Single App")]
        public static void BuildSingleApp()
        {
            var buildTask = new AutoBuildWindow();
            buildTask.Build();
        }


        [MenuItem("BuildPipeline/（本地）构建 BuildIn 包")]
        public static void BuildDLLAssetBundleAPP_BuildIn()
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

    }
}