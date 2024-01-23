namespace AVR
{
    namespace User {
        [System.Serializable]
        public class UserMe: User {
            public string[] friends;
            public string status;
            public string token;
        }
    }
}