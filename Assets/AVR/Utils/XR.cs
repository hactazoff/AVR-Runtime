namespace AVR {
    namespace Utils {
        public class XR {
            /**
             * Check if VR is enabled
             */
            public static bool IsEnabled {
                get {
                    return UnityEngine.XR.XRSettings.enabled;
                }
            }

            /**
             * Check if VR is ready
             */
            public static bool IsReady {
                get {
                    return IsEnabled && UnityEngine.XR.XRSettings.loadedDeviceName != "None";
                }
            }
        }
    }
}