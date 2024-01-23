using System.Text.RegularExpressions;

namespace AVR
{
    namespace Instance
    {
        public class InstancePatern
        {
            public string id;
            public string name;
            public string server;

            // <id|name>@<address>
            public static InstancePatern Parser(string str, string defaultServer = null)
            {
                if (Regex.Match(str, @"^i_[a-f0-9-]{36}$").Success) 
                    return new InstancePatern() {
                        id = str,
                        name = null,
                        server = defaultServer
                    };
                if (Regex.Match(str, @"^i_[a-f0-9-]{36}@.+").Success) {
                    string[] split = str.Split('@');
                    return new InstancePatern() {
                        id = split[0],
                        name = null,
                        server = split[1]
                    };
                }
                if (Regex.Match(str, @"^[a-z0-9-_.]{3,8}$").Success) 
                    return new InstancePatern() {
                        id = null,
                        name = str,
                        server = defaultServer
                    };
                if (Regex.Match(str, @"^[a-z0-9-_.]{3,8}@.+").Success) {
                    string[] split = str.Split('@');
                    return new InstancePatern() {
                        id = null,
                        name = split[0],
                        server = split[1]
                    };
                }
                return null;
            }
        }
    }
}