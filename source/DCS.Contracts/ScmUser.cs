namespace DCS.Contracts
{
    public class ScmUser : IScmUser
    {
        public ScmUser(string username, string email, string password)
        {
            _username = username;
            _email = email;
            _password = password;
        }

        private readonly string _username;
        private readonly string _email;
        private readonly string _password;

        public string Username
        {
            get { return _username; }
        }

        public string Email
        {
            get { return _email; }
        }

        public string Password
        {
            get { return _password; }
        }
    }
}