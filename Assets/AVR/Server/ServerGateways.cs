namespace AVR
{
    namespace Server
    {
        [System.Serializable]
        public class ServerGateways
        {
            public string http;
            public string ws;
            public string proxy;

            /**
             * Combine the basepath with the path, adding a slash if necessary.
             */
            private static string Combine(string basepath, string path)
            {
                if (!basepath.EndsWith("/") && !path.StartsWith("/"))
                    return basepath + "/" + path;
                else if (basepath.EndsWith("/") && path.StartsWith("/"))
                    return basepath + path.Substring(1);
                else return basepath + path; 
            }

            /**
             * Combine the basepath with the http path, adding a slash if necessary.
             */
            public string CombineHTTP(string pathname) => Combine(http, pathname);

            /**
             * Combine the basepath with the ws path, adding a slash if necessary.
             */
            public string CombineWS(string pathname) => Combine(ws, pathname);

            /**
             * Combine the basepath with the proxy path, adding a slash if necessary.
             */
            public string CombineProxy(string pathname) => Combine(proxy, pathname);
        }
    }
}