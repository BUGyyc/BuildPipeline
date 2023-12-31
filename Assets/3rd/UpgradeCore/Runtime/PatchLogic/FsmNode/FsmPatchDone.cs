﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Singleton;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmPatchDone : IStateNode
{
    void IStateNode.OnCreate(StateMachine machine)
    {
    }

    /// <summary>
    ///! 全部的下载完成，启动游戏步骤
    /// </summary>
    void IStateNode.OnEnter()
    {
        Debug.Log("热更完成-----------------------》 开始进入游戏");

        PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");

        if (PatchWindow.Instance != null)
        {
            GameObject.Destroy(PatchWindow.Instance.gameObject);
        }

        //// 创建游戏管理器
        //UniSingleton.CreateSingleton<GameManager>();

        ////! 游戏控制器的启动
        //// 开启游戏流程
        //GameManager.Instance.Run();

        PatchManager.UpgradeComplete();

    }

    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}