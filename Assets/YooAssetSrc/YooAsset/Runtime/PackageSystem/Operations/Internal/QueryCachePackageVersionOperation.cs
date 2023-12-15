using System.IO;

namespace YooAsset
{
    /// <summary>
    /// ！ 查询缓存中 Package 的版本
    /// </summary>
    internal class QueryCachePackageVersionOperation : AsyncOperationBase
    {
        private enum ESteps
        {
            None,

            /// <summary>
            /// ! 加载缓存中 Package 版本文件
            /// </summary>
            LoadCachePackageVersionFile,
            Done,
        }

        private readonly string _packageName;
        private ESteps _steps = ESteps.None;

        /// <summary>
        /// 包裹版本
        /// </summary>
        public string PackageVersion { private set; get; }

        public QueryCachePackageVersionOperation(string packageName)
        {
            _packageName = packageName;
        }

        internal override void Start()
        {
            _steps = ESteps.LoadCachePackageVersionFile;
        }

        internal override void Update()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.LoadCachePackageVersionFile)
            {
                //- 执行查询
                string filePath = PersistentTools.GetCachePackageVersionFilePath(_packageName);
                if (File.Exists(filePath) == false)
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Cache package version file not found : {filePath}";

                    LogMaster.S(
                        "[YooAssetPipeline]",
                        "[QueryCachePackageVersion] SandBox 没有指定文件   error:    "
                            + Error
                            + "               _packageName:"
                            + _packageName
                    );

                    return;
                }

                PackageVersion = FileUtility.ReadAllText(filePath);

                LogMaster.S(
                    "[QueryCachePackageVersion]",
                    $" 找到本地文本，  path:{filePath} version:{PackageVersion}"
                );

                if (string.IsNullOrEmpty(PackageVersion))
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Cache package version file content is empty !";
                    LogMaster.E("[QueryCachePackageVersion] error:" + Error);
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Succeed;
                }
            }
        }
    }
}
