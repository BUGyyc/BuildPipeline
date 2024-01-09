namespace YooAsset
{
    /// <summary>
    /// ! 解压清单文件？？？？
    /// </summary>
    internal class UnpackBuildinManifestOperation : AsyncOperationBase
    {
        private enum ESteps
        {
            None,

            /// <summary>
            /// ？解压清单 Hash 文件
            /// </summary>
            UnpackManifestHashFile,

            /// <summary>
            /// ? 解压清单文件
            /// </summary>
            UnpackManifestFile,
            Done,
        }

        private readonly string _buildinPackageName;
        private readonly string _buildinPackageVersion;
        private UnityWebFileRequester _downloader1;
        private UnityWebFileRequester _downloader2;
        private ESteps _steps = ESteps.None;

        public UnpackBuildinManifestOperation(
            string buildinPackageName,
            string buildinPackageVersion
        )
        {
            _buildinPackageName = buildinPackageName;
            _buildinPackageVersion = buildinPackageVersion;
        }

        internal override void Start()
        {
            _steps = ESteps.UnpackManifestHashFile;
        }

        internal override void Update()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.UnpackManifestHashFile)
            {
                if (_downloader1 == null)
                {
                    //! 这是一个沙盒路径
                    //- ：  D:/gitee/LSGameCore/Unity/Sandbox/ManifestFiles/PackageManifest_YooPreview_2023-09-12-1059.hash
                    string savePath = PersistentTools.GetCachePackageHashFilePath(
                        _buildinPackageName,
                        _buildinPackageVersion
                    );
                    //！ 保存的是 Package_preview.version
                    //- PackageManifest_YooPreview_2023-09-12-1059.hash
                    string fileName = YooAssetSettingsData.GetPackageHashFileName(
                        _buildinPackageName,
                        _buildinPackageVersion
                    );
                    //!  这个 StreamAsset 文件路径
                    //- D:/gitee/LSGameCore/Unity/Assets/StreamingAssets/BuildinFiles/PackageManifest_YooPreview_2023-09-12-1059.hash
                    string filePath = PersistentTools.MakeStreamingLoadPath(fileName);

                    //- file:///D:/gitee/LSGameCore/Unity/Assets/StreamingAssets/BuildinFiles/PackageManifest_YooPreview_2023-09-12-1059.hash
                    string url = PersistentTools.ConvertToWWWPath(filePath);

                    LogMaster.S(
                        "UnpackManifestHashFile",
                        $"\n savePath:   {savePath}\n  fileName: {fileName}\n  filePath:	{filePath}\n  url:	{url}"
                    );

                    _downloader1 = new UnityWebFileRequester();
                    _downloader1.SendRequest(url, savePath);
                }

                if (_downloader1.IsDone() == false)
                    return;

                if (_downloader1.HasError())
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = _downloader1.GetError();
                }
                else
                {
                    _steps = ESteps.UnpackManifestFile;
                }

                _downloader1.Dispose();
            }

            if (_steps == ESteps.UnpackManifestFile)
            {
                if (_downloader2 == null)
                {
                    string savePath = PersistentTools.GetCacheManifestFilePath(
                        _buildinPackageName,
                        _buildinPackageVersion
                    );
                    string fileName = YooAssetSettingsData.GetManifestBinaryFileName(
                        _buildinPackageName,
                        _buildinPackageVersion
                    );
                    string filePath = PersistentTools.MakeStreamingLoadPath(fileName);
                    string url = PersistentTools.ConvertToWWWPath(filePath);

                    LogMaster.S(
                        "UnpackManifestFile",
                        $"\n savePath:   {savePath}\n  fileName: {fileName}\n  filePath:	{filePath}\n  url:	{url}"
                    );

                    _downloader2 = new UnityWebFileRequester();
                    _downloader2.SendRequest(url, savePath);
                }

                if (_downloader2.IsDone() == false)
                    return;

                if (_downloader2.HasError())
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = _downloader2.GetError();
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Succeed;
                }

                _downloader2.Dispose();
            }
        }
    }
}
