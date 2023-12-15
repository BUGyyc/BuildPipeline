using BuildPipelineCore;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildPipelineCore
{
    /// <summary>
    /// window 平台自动打包
    /// </summary>
    public class AutoBuildWindow : AutoBuilderBase
    {
        public override BuildTarget GetBuildTarget()
        {
            return BuildTarget.StandaloneWindows64;
        }

        public override BuildPlayerOptions GetBuildPlayerOptions()
        {
            BuildOptions options = BuildOptions.None;
            if (ArgsInfo.BuildMode == BuildModeType.PROFILER)
            {
                options |= BuildOptions.Development;
                options |= BuildOptions.ConnectWithProfiler;
                options |= BuildOptions.AllowDebugging;
            }

#if UNITY_EDITOR_WIN

            if (ArgsInfo.OutProjectName.Length == 0)
            {
                ArgsInfo.OutProjectName = Application.dataPath + "/" + BuildApp.APP_BUILD_OUTPUT_PATH;
            }
#endif


            BuildPlayerOptions playerOpts = new BuildPlayerOptions();
            playerOpts.scenes = GetBuildScenes();
            playerOpts.locationPathName = ArgsInfo.OutProjectName + "/exe/umi.exe";
            playerOpts.target = BuildTarget.StandaloneWindows64;
            playerOpts.options = options;

            return playerOpts;
        }

        protected override void OnSetPlayerSettings()
        {
            PlayerSettings.resizableWindow = false;
            PlayerSettings.runInBackground = true;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = ArgsInfo.WindowWidth;
            PlayerSettings.defaultScreenHeight = ArgsInfo.WindowHeight;
        }
    }
}