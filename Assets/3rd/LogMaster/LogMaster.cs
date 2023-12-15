
public static class LogMaster
{
    //public static int LogMode = 




    public static void BP(string str)
    {
        UnityEngine.Debug.Log($"<color=yellow>[BuildPipeline]</color> {str}");
    }

    public static void Y(string str)
    {
        UnityEngine.Debug.Log($"<color=yellow>[YooAsset]</color> {str}");
    }
    /// <summary>
    /// ! NET 相关的 log
    /// </summary>
    /// <param name="str"></param>
    public static void N(params string[] args)
    {
        //    System.Diagnostics.Debug.Log($"<color=red> {string.Join(",", args)}   </color>");
    }

    public static void L(params string[] str)
    {
        int tickValue = 0;
        UnityEngine.Debug.LogFormat(
            $"<color=yellow> [Tick:{tickValue}] {string.Join(",", str)}   </color>"
        );
    }

    //[Conditional("DEBUG")]
    public static void Log(string args)
    {
        UnityEngine.Debug.Log($"<color=yellow> {args}   </color>");
    }

    public static void Log(string str1, string str2)
    {
        UnityEngine.Debug.Log($"<color=yellow> {str1}   </color>" + $" {str2}");
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void I(params string[] args)
    {
        UnityEngine.Debug.LogFormat($"<color=yellow> {string.Join(",", args)}   </color>");
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void S(string str1, string str2)
    {
        UnityEngine.Debug.LogFormat($"<color=yellow>{str1}</color>  {str2}");
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void A(string str1)
    {
        UnityEngine.Debug.Log($"{str1}");
    }

    public static void E(params string[] str)
    {
        UnityEngine.Debug.LogErrorFormat($"<color=red>{string.Join(",", str)}   </color>");
    }
}
