# BuildPipeline
 BuildPipeline


## YooAsset

### 判断是否断点下载

```
	bool breakDownload = bundleInfo.Bundle.FileSize >= BreakpointResumeFileSize;
    DownloaderBase newDownloader = new FileDownloader(bundleInfo, breakDownload);
    newDownloader.SendRequest(failedTryAgain, timeout);

```


### 如何删除 沙盒内资源


先查找未使用的 GUID List

```
		/// <summary>
		/// 获取未被使用的缓存文件
		/// </summary>
		public static List<string> GetUnusedCacheGUIDs(ResourcePackage package)
		{
			var cache = GetOrCreateCache(package.PackageName);
			var keys = cache.GetAllKeys();
			List<string> result = new List<string>(keys.Count);
			foreach (var cacheGUID in keys)
			{
				if (package.IsIncludeBundleFile(cacheGUID) == false)
				{
					result.Add(cacheGUID);
				}
			}
			return result;
		}

```

通过未使用的 GUID List  删除沙盒内的无用的文件

```

			if (_steps == ESteps.GetUnusedCacheFiles)
			{
				_unusedCacheGUIDs = CacheSystem.GetUnusedCacheGUIDs(_package);
				_unusedFileTotalCount = _unusedCacheGUIDs.Count;
				YooLogger.Log($"Found unused cache file count : {_unusedFileTotalCount}");
				_steps = ESteps.ClearUnusedCacheFiles;
			}

```



遍历旧的寻址路径是否在新的 清单文件中存在。如果不存在，就是需要删除的