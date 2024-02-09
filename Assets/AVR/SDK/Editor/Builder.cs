#if UNITY_EDITOR
using UnityEngine;
namespace AVR.SDK.Base
{
    public class Builder
    {
        public static SupportedPlatforms DetectBuildTarget()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => SupportedPlatforms.Windows,
                RuntimePlatform.OSXEditor => SupportedPlatforms.Mac,
                RuntimePlatform.LinuxEditor => SupportedPlatforms.Linux,
                _ => SupportedPlatforms.NoTarget,
            };
        }

        public static UnityEditor.BuildTarget ConvertToBuildTarget(SupportedPlatforms platform)
        {
            return platform switch
            {
                SupportedPlatforms.Windows => UnityEditor.BuildTarget.StandaloneWindows64,
                SupportedPlatforms.Mac => UnityEditor.BuildTarget.StandaloneOSX,
                SupportedPlatforms.Linux => UnityEditor.BuildTarget.StandaloneLinux64,
                _ => UnityEditor.BuildTarget.NoTarget,
            };
        }
    }
}
#endif