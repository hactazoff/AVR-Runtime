
namespace AVR
{
    namespace Utils
    {
        class Save
        {
            public static string DefaultNamespace { get { return "AVR"; } }
            public static string DefaultSavePath
            {
                get
                {
                    var dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/." + DefaultNamespace.ToLower() + "/";
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    return dir;
                }
            }
        }
    }
}