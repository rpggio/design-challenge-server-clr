using System;
using System.Collections.Generic;

namespace DCS.ServerRuntime.Services.GitblitApi
{
    /// <summary>
    /// https://github.com/gitblit/gitblit/blob/3e0c6ca8a65bd4b076cac1451c9cdfde4be1d4b8/src/main/java/com/gitblit/models/UserModel.java
    /// </summary>
    public class GitblitUser
    {
        public string accountType { get; set; }
        public String username{ get; set; }
        public String password{ get; set; }
        public String displayName{ get; set; }
        public String emailAddress{ get; set; }
        public bool canAdmin{ get; set; }
        public bool canFork { get; set; }
        public bool canCreate { get; set; }
        public bool disabled { get; set; }

        public List<string> repositories { get; private set; } 
        public Dictionary<string, string> permissions { get; private set; }

        public GitblitUser()
        {
            repositories = new List<string>();
            permissions = new Dictionary<string, string>();
        }
    }

    public class UserPreferences
    {
        private String locale;
    }
}