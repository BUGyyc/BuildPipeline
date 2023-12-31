﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
    /// <summary>
    /// ! AssetBundle 的核心构建逻辑
    /// </summary>
    public class AssetBundleBuilder
    {
        private readonly BuildContext _buildContext = new BuildContext();

        /// <summary>
        /// 开始构建
        /// </summary>
        public BuildResult Run(BuildParameters buildParameters)
        {
            // 清空旧数据
            _buildContext.ClearAllContext();

            // 检测构建参数是否为空
            if (buildParameters == null)
                throw new Exception($"{nameof(buildParameters)} is null !");

            // 检测可编程构建管线参数
            if (buildParameters.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            {
                if (buildParameters.SBPParameters == null)
                    throw new Exception($"{nameof(BuildParameters.SBPParameters)} is null !");

                if (buildParameters.BuildMode == EBuildMode.DryRunBuild)
                    throw new Exception(
                        $"{nameof(EBuildPipeline.ScriptableBuildPipeline)} not support {nameof(EBuildMode.DryRunBuild)} build mode !"
                    );

                if (buildParameters.BuildMode == EBuildMode.ForceRebuild)
                    throw new Exception(
                        $"{nameof(EBuildPipeline.ScriptableBuildPipeline)} not support {nameof(EBuildMode.ForceRebuild)} build mode !"
                    );
            }

            // 构建参数
            var buildParametersContext = new BuildParametersContext(buildParameters);
            _buildContext.SetContextObject(buildParametersContext);

            // 创建构建节点
            List<IBuildTask> pipeline;
            //! 这里的内置构建管线和可编程构建管线的区别，需要查找相关资料，确认一下
            //?? 构建步骤如下实例的顺序
            if (buildParameters.BuildPipeline == EBuildPipeline.BuiltinBuildPipeline)
            {
                pipeline = new List<IBuildTask>
                {
                    new TaskPrepare(), //前期准备工作
                    new TaskGetBuildMap(), //获取构建列表
                    new TaskBuilding(), //开始执行构建
                    new TaskCopyRawFile(), //拷贝原生文件
                    new TaskVerifyBuildResult(), //验证构建结果
                    new TaskEncryption(), //加密资源文件
                    new TaskUpdateBundleInfo(), //更新资源包信息
                    new TaskCreateManifest(), //创建清单文件
                    new TaskCreateReport(), //创建报告文件
                    new TaskCreatePackage(), //制作包裹
                    new TaskCopyBuildinFiles(), //拷贝内置文件
                };
            }
            else if (buildParameters.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            {
                pipeline = new List<IBuildTask>
                {
                    new TaskPrepare(), //前期准备工作
                    new TaskGetBuildMap(), //获取构建列表
                    new TaskBuilding_SBP(), //开始执行构建
                    new TaskCopyRawFile(), //拷贝原生文件
                    new TaskVerifyBuildResult_SBP(), //验证构建结果
                    new TaskEncryption(), //加密资源文件
                    new TaskUpdateBundleInfo(), //更新补丁信息
                    new TaskCreateManifest(), //创建清单文件
                    new TaskCreateReport(), //创建报告文件
                    new TaskCreatePackage(), //制作补丁包
                    new TaskCopyBuildinFiles(), //拷贝内置文件
                };
            }
            else
            {
                throw new NotImplementedException();
            }

            // 初始化日志
            BuildLogger.InitLogger(buildParameters.EnableLog);

            // 执行构建流程
            var buildResult = BuildRunner.Run(pipeline, _buildContext);
            if (buildResult.Success)
            {
                buildResult.OutputPackageDirectory =
                    buildParametersContext.GetPackageOutputDirectory();
                BuildLogger.Log($"{buildParameters.BuildMode} pipeline build succeed !");

                //！写入最新版本 hash-code
                //var versionCode = buildParametersContext.Parameters.PackageVersion;

                //VersionCfg cubeAsset = AssetDatabase.LoadAssetAtPath<VersionCfg>(@"Assets/YooAsset/blueCube.asset");

                //var cfg = SettingLoader.LoadSettingData<VersionCfg>();

                //cfg.VersionCode = versionCode;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //LogMaster.I(" build version code  " + versionCode);
            }
            else
            {
                BuildLogger.Warning($"{buildParameters.BuildMode} pipeline build failed !");
                BuildLogger.Error($"Build task failed : {buildResult.FailedTask}");
                BuildLogger.Error(buildResult.ErrorInfo);
            }

            return buildResult;
        }
    }
}
