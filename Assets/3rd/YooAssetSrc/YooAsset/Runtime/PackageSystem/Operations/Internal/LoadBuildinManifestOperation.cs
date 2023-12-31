﻿
namespace YooAsset
{
	/// <summary>
	/// ! Load 本地 （StreamAsset）路径下的清单文件
	/// </summary>
	internal class LoadBuildinManifestOperation : AsyncOperationBase
	{
		private enum ESteps
		{
			None,
			LoadBuildinManifest,
			CheckDeserializeManifest,
			Done,
		}

		private readonly string _buildinPackageName;
		private readonly string _buildinPackageVersion;
		private UnityWebDataRequester _downloader;
		private DeserializeManifestOperation _deserializer;
		private ESteps _steps = ESteps.None;

		/// <summary>
		/// 加载的清单实例
		/// </summary>
		public PackageManifest Manifest { private set; get; }


		public LoadBuildinManifestOperation(string buildinPackageName, string buildinPackageVersion)
		{
			_buildinPackageName = buildinPackageName;
			_buildinPackageVersion = buildinPackageVersion;
		}
		internal override void Start()
		{
			_steps = ESteps.LoadBuildinManifest;
		}
		internal override void Update()
		{
			if (_steps == ESteps.None || _steps == ESteps.Done)
				return;

			if (_steps == ESteps.LoadBuildinManifest)
			{
				if (_downloader == null)
				{
					//!获取清单文件完整名称
					string fileName = YooAssetSettingsData.GetManifestBinaryFileName(_buildinPackageName, _buildinPackageVersion);
					string filePath = PersistentTools.MakeStreamingLoadPath(fileName);
					string url = PersistentTools.ConvertToWWWPath(filePath);
					_downloader = new UnityWebDataRequester();
					_downloader.SendRequest(url);
					LogMaster.Log("[LoadBuildinManifest]",$"LoadBuildinManifest  \n fileName:{fileName}  url:{url}  ");
				}

				if (_downloader.IsDone() == false)
					return;

				if (_downloader.HasError())
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = _downloader.GetError();
				}
				else
				{
					LogMaster.Log("[LoadBuildinManifest]",$" get bytes data   ");
					byte[] bytesData = _downloader.GetData();
					//！反序列化清单文件
					_deserializer = new DeserializeManifestOperation(bytesData);
					OperationSystem.StartOperation(_deserializer);
					_steps = ESteps.CheckDeserializeManifest;
				}

				_downloader.Dispose();
			}

			if (_steps == ESteps.CheckDeserializeManifest)
			{
				Progress = _deserializer.Progress;
				if (_deserializer.IsDone == false)
					return;

				if (_deserializer.Status == EOperationStatus.Succeed)
				{
					Manifest = _deserializer.Manifest;
					_steps = ESteps.Done;
					Status = EOperationStatus.Succeed;
				}
				else
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = _deserializer.Error;
				}
			}
		}
	}
}