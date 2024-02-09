namespace  AVR.Utils
{
    public class Cache
    {
        public static string CachePath
        {
            get
            {
                var dir = AVR.Utils.Save.DefaultSavePath + "/cache/";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                return dir;
            }
        }
        
        // sha256 checksum
        public static string Checksum(string path)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var stream = System.IO.File.OpenRead(path);
            var hash = sha256.ComputeHash(stream);
            stream.Close();
            return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        
        public static bool ValidChecksum(string path, string hash) => Checksum(path) == hash;
    }
}