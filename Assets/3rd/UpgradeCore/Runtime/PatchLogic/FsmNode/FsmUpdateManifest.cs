﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Singleton;
using YooAsset;

/// <summary>
/// 更新资源清单
/// </summary>
public class FsmUpdateManifest : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
		UniSingleton.StartCoroutine(UpdateManifest());
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}
	

	/// <summary>
	/// ! 更新资源清单文件
	/// </summary>
	/// <returns></returns>
	private IEnumerator UpdateManifest()
	{
		yield return new WaitForSecondsRealtime(0.5f);

		bool savePackageVersion = true;
		var package = YooAssets.GetPackage(VersionSettings.PackageName);
		var operation = package.UpdatePackageManifestAsync(PatchManager.Instance.PackageVersion, savePackageVersion);
		yield return operation;

		if(operation.Status == EOperationStatus.Succeed)
		{
			//! 开始启动下载器
			_machine.ChangeState<FsmCreateDownloader>();
		}
		else
		{
			Debug.LogWarning(operation.Error);
			PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
		}
	}
}