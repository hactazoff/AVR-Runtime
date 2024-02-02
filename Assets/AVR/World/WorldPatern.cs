using System.Text.RegularExpressions;

namespace AVR.Worlds
{
    public class WorldPatern
    {
        public string id;
        public string server;

        // <id>[@<address>]
        public static WorldPatern Parser(string str, string defaultServer = null)
        {
            if (Regex.Match(str, @"^w_[a-f0-9-]{36}$").Success)
                return new WorldPatern()
                {
                    id = str,
                    server = defaultServer
                };
            if (Regex.Match(str, @"^w_[a-f0-9-]{36}@.+").Success)
            {
                string[] split = str.Split('@');
                return new WorldPatern()
                {
                    id = split[0],
                    server = split[1]
                };
            }
            return null;
        }
    }
}