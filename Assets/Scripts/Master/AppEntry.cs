using System.Collections;

using UnityEngine;
//using UnityEngine.AddressableAssets;
using UniFramework.Event;
using UniFramework.Singleton;
using YooAsset;
//using Grpc.Core.Interceptors;

namespace UMI
{

    //class FixedGprc : Interceptor 
    //{

    //    public void test() {
    //    }
    //}


    /// <summary>
    /// 程序的启动入口
    /// </summary>
    public class AppEntry : MonoBehaviour
    {

        //FixedGprc a;
        //private UIUpdate mUIUpdate;
        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        private void Awake()
        {

            //a = new FixedGprc();
            //a.test();
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");
            Debug.Log("---------------------------   游戏启动   ---------------------------");

            DontDestroyOnLoad(this.gameObject);

            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;

#if !UNITY_EDITOR
            PlayMode = EPlayMode.OfflinePlayMode;
            Debug.Log($"纠正，资源系统运行模式：{PlayMode}");
#endif

        }

        void Start()
        {
            // 初始化事件系统
            UniEvent.Initialize();

            // 初始化单例系统
            UniSingleton.Initialize();

            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);

            // 创建补丁管理器
            UniSingleton.CreateSingleton<PatchManager>();

            // 开始补丁更新流程
            PatchManager.Instance.Run(PlayMode);

            
        }


        #region 

        ///// <summary>
        ///// 显示热更新的UI
        ///// </summary>
        //public void ShowHotUpdateUI()
        //{
        //    HotUpdateOverToStart();
        //}

        ///// <summary>
        ///// 热更新完成后加载正式的启动
        ///// </summary>
        //public void HotUpdateOverToStart()
        //{
        //    AOTLoad.LoadMetadataForAOTAssemblies(() =>
        //    {
        //        //str = string.Empty;
        //        Debug.Log("Start to load HotEntry");

        //        var handler = YooAssets.LoadAssetAsync<GameObject>("HotEntry");
        //        handler.Completed += (AssetOperationHandle handle) =>
        //        {
        //            Debug.Log("HotEntry Load Complete");
        //            if (handle.AssetObject == null)
        //                return;
        //            // 实例化对象
        //            var obj = handle.InstantiateSync();

        //            StartCoroutine(UnloadUpdateUI(obj));
        //        };
        //    });
        //}

        //private IEnumerator UnloadUpdateUI(GameObject obj)
        //{
        //    yield return new WaitForSeconds(1);
        //    //if (mUIUpdate != null)
        //    //{
        //    //    GameObject.DestroyImmediate(mUIUpdate.gameObject);
        //    //    mUIUpdate = null;
        //    //}

        //    if (obj != null)
        //    {
        //        GameObject.DestroyImmediate(obj);
        //        obj = null;
        //    }

        //    //Resources.UnloadUnusedAssets();
        //}

        #endregion
    }
}
