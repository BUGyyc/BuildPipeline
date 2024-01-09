
public static class VersionSettings
{
    public static readonly string PackageName = "UMI";

    public static readonly string AssetFolderName = "Asset";

    public static string CDN_URL = "";

#if sit
    public static readonly string branch_version = "sit";
#elif OPT
    public static readonly string branch_version = "opt";
#else
    public static readonly string branch_version = "dev";
#endif

    public static readonly string Remote_AB_Folder = "umi_{0}_" + branch_version;
    /// <summary>
    /// Editor 下是否使用 远端 CDN 服务器
    /// </summary>
    public const bool EditorRomoteServerURL = false;


    //public const bool HybridCLR_Enable = false;


#if UNITY_EDITOR



#else
    
    /// <summary>
    /// Release 下是否使用本地 CDN
    /// </summary>
    public const bool ReleaseLocalServerURL = true;

#endif


    public const string ProjectName = "BuildDemo";

}
