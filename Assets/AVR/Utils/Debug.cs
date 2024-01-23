namespace AVR {
    namespace Utils {
        public class Debug {
            public static string prefix = "[AVR] ";

            public static void Log(object message) {
                UnityEngine.Debug.Log(prefix + message);
            }

            public static void Log(object message, UnityEngine.Object context) {
                UnityEngine.Debug.Log(prefix + message, context);
            }

            public static void LogError(object message) {
                UnityEngine.Debug.LogError(prefix + message);
            }

            public static void LogError(object message, UnityEngine.Object context) {
                UnityEngine.Debug.LogError(prefix + message, context);
            }

            public static void LogWarning(object message) {
                UnityEngine.Debug.LogWarning(prefix + message);
            }

            public static void LogWarning(object message, UnityEngine.Object context) {
                UnityEngine.Debug.LogWarning(prefix + message, context);
            }
        }
    }
}