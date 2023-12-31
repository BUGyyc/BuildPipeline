﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Event;
using UniFramework.Machine;
using UniFramework.Singleton;

public class GameManager : SingletonInstance<GameManager>, ISingleton
{
	private bool _isRun = false;
	private EventGroup _eventGroup = new EventGroup();
	private StateMachine _machine;

	void ISingleton.OnCreate(object createParam)
	{
	}
	void ISingleton.OnDestroy()
	{
		_eventGroup.RemoveAllListener();
	}
	void ISingleton.OnUpdate()
	{
		if (_machine != null)
			_machine.Update();
	}

	public void Run()
	{
		if (_isRun == false)
		{
			_isRun = true;

			// 注册监听事件
			_eventGroup.AddListener<SceneEventDefine.ChangeToHomeScene>(OnHandleEventMessage);
			_eventGroup.AddListener<SceneEventDefine.ChangeToBattleScene>(OnHandleEventMessage);


			//! 框架中挺多这种状态机管理，但是其实内部都是一条链路的状态机，起始可以改成链式编程，一条链路表示流程操作
			Debug.Log("开启游戏流程...");

			Debug.Log("<color=yellow>-----------------------------------------------正式进入游戏流程--------------------------------------------- </color>");

			_machine = new StateMachine(this);
			_machine.AddNode<FsmInitGame>();
			_machine.AddNode<FsmSceneHome>();
			_machine.AddNode<FsmSceneBattle>();
			_machine.Run<FsmInitGame>();
		}
		else
		{
			Debug.LogWarning("补丁更新已经正在进行中!");
		}
	}

	/// <summary>
	/// 接收事件
	/// </summary>
	private void OnHandleEventMessage(IEventMessage message)
	{
		if(message is SceneEventDefine.ChangeToHomeScene)
		{
			_machine.ChangeState<FsmSceneHome>();
		}
		else if(message is SceneEventDefine.ChangeToBattleScene)
		{
			_machine.ChangeState<FsmSceneBattle>();
		}
	}
}