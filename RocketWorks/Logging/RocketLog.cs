class RocketLog
{
    public static void Log(string s, object context = null)
    {
        Log("[" + context.GetType() + "] " + s);
    }

    public static void LogFormat(string s, object context = null, params object[] items)
    {
        string c = (context == null) ? "RocketLog" : context.GetType().ToString();
        Log("[" + c + "] " + string.Format(s, items));
    }

    private static void Log(string s)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        UnityEngine.Debug.Log(s);
#endif
        System.Console.WriteLine(s);
    }
}
