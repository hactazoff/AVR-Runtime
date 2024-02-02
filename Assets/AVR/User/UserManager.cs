using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Users
    {
        public class UserManager : Base.Manager<User>
        {
            public delegate void OnUserUpdateEvent(User user);
            public static event OnUserUpdateEvent OnUserUpdate;
            public static void OnUpdate(User user) => OnUserUpdate?.Invoke(user);

            public delegate void OnUserRemoveEvent(User user);
            public static event OnUserRemoveEvent OnUserRemove;
            public static void OnRemove(User user) => OnUserRemove?.Invoke(user);

            public delegate void OnUserAddEvent(User user);
            public static event OnUserAddEvent OnUserAdd;
            public static void OnAdd(User user) => OnUserAdd?.Invoke(user);


            /**
             * Get a user by id or username and server address.
             */
            public static User GetUser(string search)
            {
                var up = UserPatern.Parser(search);
                if (up == null)
                    return null;
                foreach (var user in Cache)
                    if (user.id == up.id && user.server == up.server)
                        return user;
                    else if (user.username == up.username && user.server == up.server)
                        return user;
                return null;
            }

            /**
             * Set/Update a user.
             */
            public static void SetUser(User user)
            {
                if (GetUser(user.id) != null)
                {
                    Cache.Add(user);
                    OnUpdate(user);
                }
                else
                {
                    Cache.Add(user);
                    OnAdd(user);
                }
            }

            /**
             * Remove a user.
             */
            public static void RemoveUser(User user)
            {
                Cache.Remove(user);
                OnRemove(user);
            }

            /**
             * Get or fetch a user by id or username.
             */
            public async static UniTask<User> GetOrFetchUser(string search)
            {
                var user = GetUser(search);
                if (user != null)
                    return user;
                return await FetchUser(search);
            }

            /**
             * Fetch a user by id or username.
             */
            private async static UniTask<User> FetchUser(string search)
            {
                var up = UserPatern.Parser(search);
                var user = new User() { server = up.server, id = up.id, username = up.username };
                return await user.Fetch();
            }
        }
    }
}

