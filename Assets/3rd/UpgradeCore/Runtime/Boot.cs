﻿//using UnityEngine;
//using UniFramework.Event;
//using UniFramework.Singleton;
//using YooAsset;


//namespace Yoo
//{
//    public class Boot : MonoBehaviour
//    {
//        /// <summary>
//        /// 资源系统运行模式
//        /// </summary>
//        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

//        void Awake()
//        {
//            Debug.Log($"资源系统运行模式：{PlayMode}");
//            Application.targetFrameRate = 60;
//            Application.runInBackground = true;

//#if !UNITY_EDITOR
//            PlayMode = EPlayMode.HostPlayMode;
//            Debug.Log($"纠正，资源系统运行模式：{PlayMode}");
//#endif
//        }
//        void Start()
//        {
//            // 初始化事件系统
//            UniEvent.Initialize();

//            // 初始化单例系统
//            UniSingleton.Initialize();

//            // 初始化资源系统
//            YooAssets.Initialize();
//            YooAssets.SetOperationSystemMaxTimeSlice(30);

//            // 创建补丁管理器
//            UniSingleton.CreateSingleton<PatchManager>();

//            // 开始补丁更新流程
//            PatchManager.Instance.Run(PlayMode);
//        }
//    }
//}