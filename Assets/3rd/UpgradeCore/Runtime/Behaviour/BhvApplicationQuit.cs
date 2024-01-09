using System;
using UnityEngine;
using YooAsset;
using static UnityEngine.Application;

public class BhvApplicationQuit : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Application.lowMemory += OnLowMemory;

    }

    void OnLowMemory()
    {
        // 在内存不足时执行处理逻辑
        // 例如释放资源或者进行内存优化
        Debug.Log("Low memory 内存不足 ,需要释放内存，进行强制清理");


        YooAssets.ForceUnloadUnusedAB();
    }
    private void OnApplicationQuit()
    {
        YooAssets.Destroy();
    }
}