using System.Text.RegularExpressions;

namespace AVR {
    namespace Users {
        public class UserPatern {
            public string id = null;
            public string username = null;
            public string server = null;

            public static UserPatern Parser(string str, string defaultServer = null) {
                if (Regex.Match(str, @"^u_[a-f0-9-]{36}$").Success) 
                    return new UserPatern() {
                        id = str,
                        username = null,
                        server = defaultServer
                    };
                if (Regex.Match(str, @"^u_[a-f0-9-]{36}@.+").Success) {
                    string[] split = str.Split('@');
                    return new UserPatern() {
                        id = split[0],
                        username = null,
                        server = split[1]
                    };
                }
                if (Regex.Match(str, @"^[A-Za-z][A-Za-z0-9_]{3,16}$").Success) 
                    return new UserPatern() {
                        id = null,
                        username = str,
                        server = defaultServer
                    };
                if (Regex.Match(str, @"^[A-Za-z][A-Za-z0-9_]{3,16}@.+").Success) {
                    string[] split = str.Split('@');
                    return new UserPatern() {
                        id = null,
                        username = split[0],
                        server = split[1]
                    };
                }
                return null;
            }
        }
    }
}