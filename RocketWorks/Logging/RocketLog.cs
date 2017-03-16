class RocketLog
{
    public static void Log(string s, object context = null)
    {
        string c = (context == null) ? "RocketLog" : context.GetType().Name;
        Log("[" + c + "] " + s);
    }

    public static void LogFormat(string s, object context = null, params object[] items)
    {
        string c = (context == null) ? "RocketLog" : context.GetType().Name;
        Log("[" + c + "] " + string.Format(s, items));
    }

    private static void Log(string s)
    {
#if UNITY_5 || UNITY_EDITOR
        UnityEngine.Debug.Log(s);
#endif
        System.Console.WriteLine(s);
    }
}
