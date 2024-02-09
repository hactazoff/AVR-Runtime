namespace AVR.Utils
{
    public class Platform
    {
        public static string CurrentPlatform
        {
            get
            {
                switch (UnityEngine.Application.platform)
                {
                    case UnityEngine.RuntimePlatform.WindowsPlayer:
                    case UnityEngine.RuntimePlatform.WindowsEditor:
                        return "windows";
                    case UnityEngine.RuntimePlatform.OSXPlayer:
                    case UnityEngine.RuntimePlatform.OSXEditor:
                        return "osx";
                    case UnityEngine.RuntimePlatform.LinuxPlayer:
                        return "linux";
                    case UnityEngine.RuntimePlatform.Android:
                        return "android";
                    case UnityEngine.RuntimePlatform.IPhonePlayer:
                        return "ios";
                    default:
                        return "unknown";
                }
            }
        }
        
        public static string CurrentVersion => UnityEngine.Application.version;
        public static string CurrentEngine => "unity";
    }
}