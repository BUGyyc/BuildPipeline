﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Pooling;
using UniFramework.Window;
using UniFramework.Machine;
using UniFramework.Singleton;
using YooAsset;

internal class FsmInitGame : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        UniSingleton.StartCoroutine(Prepare());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }


    /// <summary>
    /// 准备加载游戏
    /// </summary>
    /// <returns></returns>
    private IEnumerator Prepare()
    {
        yield return new WaitForSeconds(0.1f);

        // 初始化对象池系统
        UniPooling.Initalize();

        //? 前往大厅？？
        _machine.ChangeState<FsmSceneHome>();
    }
}