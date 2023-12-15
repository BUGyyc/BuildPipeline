using System;
using System.Collections.Generic;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildPipelineCore
{

    /// <summary>
    /// 构建模式类型
    /// </summary>
    public enum BuildModeType
    {
        LOCAL,          // 内网测试包
        RELEASE,        // 正式包
        PROFILER,       // 调试表
    }

    /// <summary>
    /// 资源定位类型
    /// </summary>
    public enum AssetLocateType
    {
        LOCAL,          // 资源放到本地-包内
        REMOTE,         // 资源放到远端
    }

    /// <summary>
    /// 构建命令参数信息
    /// </summary>
    public class BuildCommandArgsInfo
    {
        // 通用
        public BuildModeType BuildMode;         // 构建模式
        public AssetLocateType AssetLocateType; // 资源定位类型
        public string Version;                  // 版本号
        public string CompanyName;              // 公司名称
        public string ProductName;              // 产品名称
        public string ApplicationIdentifier;    // 应用标识-包名

        public string OutProjectName;           // 生成的项目名称

        // android
        public string AndroidJdkPath;           // android AndroidJdkPath
        public string AndroidAndroidSdkRoot;    // android AndroidSdkRoot
        public string AndroidKeystoreName;      // android keystoreName
        public string AndroidKeyaliasName;      // android keyaliasName
        public string AndroidKeystorePass;      // android keystorePass
        public string AndroidKeyaliasPass;      // android keyaliasPass

        // ios
        public string IosTeamId;                // ios 证书团队uid

        // window
        public int WindowHeight;                // window 窗口高度
        public int WindowWidth;                 // window 窗口宽度
    }



    public abstract class AutoBuilderBase
    {
        private BuildCommandArgsInfo mArgsInfo;      // 命令参数信息



        protected BuildCommandArgsInfo ArgsInfo
        {
            get { return mArgsInfo; }
        }

        /// <summary>
        /// 获取构建目标类型
        /// </summary>
        /// <returns>构建类型</returns>
        public abstract BuildTarget GetBuildTarget();

        /// <summary>
        /// 获取构建选项
        /// </summary>
        /// <returns></returns>
        public abstract BuildPlayerOptions GetBuildPlayerOptions();

        /// <summary>
        /// 构建入口
        /// </summary>
        public void Build(bool localTest = false, Action buildAOTAction = null)
        {
            try
            {

                localTest = true;

                // 设置
                LogMaster.BP("Build SwitchPlatform Begin");
                SwitchPlatform(GetBuildTarget());
                LogMaster.BP("Build SwitchPlatform End");

                LogMaster.BP("Build InitCommandArgs Begin");
                InitCommandArgs(localTest);
                LogMaster.BP("Build InitCommandArgs End");

                LogMaster.BP("Build SetEditorPrefs Begin");
                SetEditorPrefs();
                LogMaster.BP("Build SetEditorPrefs End");

                LogMaster.BP("Build SetPlayerSettings Begin");
                SetPlayerSettings();
                LogMaster.BP("Build SetPlayerSettings End");

                var opt = GetBuildPlayerOptions();
                var buildReport = BuildPipeline.BuildPlayer(opt);

                if (buildReport.summary.result == BuildResult.Succeeded)
                {
                    LogMaster.BP("打包成功。path: " + buildReport.summary.outputPath);

                    if (buildAOTAction != null) 
                    {
                        buildAOTAction.Invoke();
                    }


                    BuildApp.OpenRelease();
                }
                else if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogMaster.BP("  Error 打包失败。");
                    LogMaster.BP("Error  fail reason, total Error ;  " + buildReport.summary.totalErrors);
                }

                LogMaster.BP(" 耗时：  " + buildReport.summary.totalTime);
            }
            catch (Exception e)
            {
                LogMaster.BP("Error  打包失败。");
                LogMaster.BP(e.ToString());

                EditorApplication.Exit(1);
            }
        }


        /// <summary>
        /// 初始化命令参数
        /// </summary>
        /// <returns></returns>
        private void InitCommandArgs(bool localTest)
        {

            mArgsInfo = new BuildCommandArgsInfo();



            // 通用
            string buildMode = GetCommandArg("BuildMode", "local");
            if (buildMode.ToLower().Equals("release"))
            {
                mArgsInfo.BuildMode = BuildModeType.RELEASE;
            }
            else if (buildMode.ToLower().Equals("profiler"))
            {
                mArgsInfo.BuildMode = BuildModeType.PROFILER;
            }
            else
            {
                mArgsInfo.BuildMode = BuildModeType.LOCAL;
            }

            string assetLocateType = GetCommandArg("AssetLocateType", "local");
            if (assetLocateType.ToLower().Equals("remote"))
            {
                mArgsInfo.AssetLocateType = AssetLocateType.REMOTE;
            }
            else
            {
                mArgsInfo.AssetLocateType = AssetLocateType.LOCAL;
            }

            mArgsInfo.Version = GetCommandArg("Version");
            mArgsInfo.ProductName = GetCommandArg("ProductName");
            mArgsInfo.CompanyName = GetCommandArg("CompanyName");
            mArgsInfo.ApplicationIdentifier = GetCommandArg("ApplicationIdentifier");
            mArgsInfo.OutProjectName = GetCommandArg("OutProjectName");

            // android
            mArgsInfo.AndroidJdkPath = GetCommandArg("AndroidJdkPath");
            mArgsInfo.AndroidAndroidSdkRoot = GetCommandArg("AndroidAndroidSdkRoot");
            mArgsInfo.AndroidKeystoreName = GetCommandArg("AndroidKeystoreName");
            mArgsInfo.AndroidKeyaliasName = GetCommandArg("AndroidKeyaliasName");
            mArgsInfo.AndroidKeystorePass = GetCommandArg("AndroidKeystorePass");
            mArgsInfo.AndroidKeyaliasPass = GetCommandArg("AndroidKeyaliasPass");

            // ios
            mArgsInfo.IosTeamId = GetCommandArg("IosTeamId");

            // window
            int outHeight;
            Int32.TryParse(GetCommandArg("WindowHeight"), out outHeight);
            int outWidth;
            Int32.TryParse(GetCommandArg("WindowWidth"), out outWidth);
            mArgsInfo.WindowHeight = outHeight;
            mArgsInfo.WindowWidth = outWidth;


            if (localTest)
            {
                LogMaster.BP("本地 Local 模式打包");
                mArgsInfo.BuildMode = BuildModeType.LOCAL;
                mArgsInfo.AssetLocateType = AssetLocateType.LOCAL;
                mArgsInfo.CompanyName = "com.bwoil";
                mArgsInfo.ProductName = "UMI";
                mArgsInfo.ApplicationIdentifier = "com.bwoil.UMI";
                mArgsInfo.AndroidKeystoreName = "AndroidKeys/UMI.keystore";
                mArgsInfo.AndroidKeyaliasName = "UmiKey";
                mArgsInfo.AndroidKeystorePass = "123456";
                mArgsInfo.AndroidKeyaliasPass = "123456";


                mArgsInfo.WindowHeight = 600;
                mArgsInfo.WindowWidth = 900;


                mArgsInfo.IosTeamId = "LL5QD7M4MY";

                LogMaster.BP("本地 Local 模式打包 ， 重新设置 mArgsInfo");
            }

        }

        /// <summary>
        /// 设置 EditorPrefs
        /// </summary>
        private void SetEditorPrefs()
        {
            OnSetEditorPrefs();
        }

        /// <summary>
        /// 自定义设置 EditorPrefs
        /// </summary>
        protected virtual void OnSetEditorPrefs()
        {

        }

        /// <summary>
        /// 设置 PlayerSettings
        /// </summary>
        private void SetPlayerSettings()
        {
            if (mArgsInfo.BuildMode == BuildModeType.RELEASE)
            {
                // release关掉debug和warning日志
                CloseTraceLogType();
            }

            if (mArgsInfo.BuildMode == BuildModeType.PROFILER)
            {
                PlayerSettings.enableInternalProfiler = true;
            }
            else
            {
                PlayerSettings.enableInternalProfiler = false;
            }

            ScriptingImplementation scripType = ScriptingImplementation.IL2CPP;
            //#if UNITY_STANDALONE_WIN
            //            scripType = ScriptingImplementation.Mono2x;
            //            LogMaster.BP("强行设置 Mono2x");
            //#endif

            PlayerSettings.productName = mArgsInfo.ProductName;
            PlayerSettings.bundleVersion = mArgsInfo.Version;
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, scripType);
            PlayerSettings.SetIncrementalIl2CppBuild(EditorUserBuildSettings.selectedBuildTargetGroup, true);

            OnSetPlayerSettings();
        }

        /// <summary>
        /// 自定义设置 PlayerSettings
        /// </summary>
        protected virtual void OnSetPlayerSettings()
        {

        }

        /// <summary>
        /// 关闭日志跟踪
        /// </summary>
        private void CloseTraceLogType()
        {
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        }

        /// <summary>
        /// 获取命令参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="defaultValue">默认参数</param>
        /// <returns>参数值</returns>
        private string GetCommandArg(string paramName, string defaultValue = "")
        {
            foreach (var command in Environment.GetCommandLineArgs())
            {
                if (command.StartsWith(paramName))
                {
                    string s = command.Split('=')[1];
                    s = s.Replace("\"", "");

                    LogMaster.BP($"GetCommandArg: {paramName}=${s}");
                    return s;
                }
            }

            LogMaster.BP($"GetCommandArg: {paramName}=${defaultValue}");
            return defaultValue;
        }

        /// <summary>
        /// 获取构建的场景
        /// </summary>
        /// <returns>场景集合</returns>
        protected string[] GetBuildScenes()
        {
            List<string> listScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    LogMaster.BP("Build Scene:" + scene.path);
                    listScenes.Add(scene.path);
                }
            }

            return listScenes.ToArray();
            //return MainSceneList.ToArray();
        }

        /// <summary>
        /// 切换平台
        /// </summary>
        /// <param name="target"></param>
        public static void SwitchPlatform(BuildTarget target)
        {
            LogMaster.BP($"SwitchPlatform: {target}");

            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                if (target == BuildTarget.iOS)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                }
                if (target == BuildTarget.Android)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }
            }
        }
    }

}