using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace User
    {
        public class UserManager : Base.Manager<User>
        {
            /**
             * Get a user by id or username and server address.
             */
            public static User GetUser(string search)
            {
                var up = UserPatern.Parser(search);
                if (up == null)
                    return null;
                User user = null;
                if (up.id != null)
                    user = GetUserById(up.id, up.server);
                if (up.username != null && user == null)
                    user = GetUserByUsername(up.username, up.server);
                return user;
            }

            /**
             * Get a user by id.
             */
            public static User GetUserById(string id, string server)
            {
                foreach (User user in Cache)
                    if (user.id == id && user.server == server)
                        return user;
                return null;
            }

            /**
             * Get a user by username.
             */
            public static User GetUserByUsername(string username, string server)
            {
                foreach (User user in Cache)
                    if (user.username == username && user.server == server)
                        return user;
                return null;
            }

            /**
             * Set/Update a user.
             */
            public static void SetUser(User user)
            {
                RemoveUser(user);
                Cache.Add(user);
            }

            /**
             * Remove a user.
             */
            public static void RemoveUser(User user)
            {
                if (GetUserById(user.id, user.server) != null)
                    Cache.Remove(user);
                if (GetUserByUsername(user.username, user.server) != null)
                    Cache.Remove(user);
            }

            /**
             * Get or fetch a user by id or username.
             */
            public async static UniTask<User> GetOrFetchUser(string search, string server)
            {
                User user = GetUserById(search, server) ?? GetUserByUsername(search, server);
                if (user != null)
                    return user;
                return await FetchUser(search, server);
            }

            /**
             * Fetch a user by id or username.
             */
            private async static UniTask<User> FetchUser(string search, string server_address)
            {
                User user = new() { server = server_address, id = search, username = search };
                return await user.Fetch();
            }
        }
    }
}

