namespace AVR
{
    public class Debug
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[AVR] " + message);
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning("[AVR] " + message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError("[AVR] " + message);
        }
    }
}