using System;
using System.Collections.Generic;

namespace AVR
{
public static class UserManager
{
    class ShortUserMe
    {
        public string server;
        public string id;
    }


    public delegate void OnCacheUpdate();
    public static event OnCacheUpdate onCacheUpdate;

    public static List<AVR.User> users = new();
    private static ShortUserMe _userMe = new();

    // userMe
    public static AVR.UserMe UserMe
    {
        get => GetUser(_userMe.id, _userMe.server) as AVR.UserMe;
        set
        {
            if (value == null)
            {
                
                _userMe = null;
                return;
            }

            SetUser(value);
            _userMe = new ShortUserMe() { server = value.server, id = value.id };
        }
    }

    // get user by id
    public static AVR.User GetUser(string id, string server)
    {
        foreach (AVR.User user in users)
            if ((user.id == id || user.username == id) && user.server == server)
                return user;
        return null;
    }

    // set user
    public static void SetUser(AVR.User user)
    {
        AVR.User existingUser = GetUser(user.id, user.server);
        if (existingUser != null)
            users.Remove(existingUser);
        users.Add(user);
        onCacheUpdate?.Invoke();
    }

    // remove user
    public static void RemoveUser(AVR.User user)
    {
        if (user != null)
            users.Remove(user);
        onCacheUpdate?.Invoke();
    }
}
}