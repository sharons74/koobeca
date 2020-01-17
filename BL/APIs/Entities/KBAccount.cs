using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaSync.APIs.Koobeca
{
    public class KBAccount
    {
        public string oauth_token;
        public string oauth_secret;
        public User user;


        public class User
        {
            public string user_id;
            public string email;
        }
    }
}
