using System.Collections.Generic;
using System.Linq;
using DCS.Contracts.Entities;
using Nancy.Security;

namespace DCS.WebServices.Api
{
    public class UserIdentity : IUserIdentity
    {
        private readonly UserEntity _user;

        public UserIdentity(UserEntity user)
        {
            _user = user;
        }

        public UserEntity User
        {
            get { return _user; }
        }

        public string UserName
        {
            get { return _user.Username; }
        }

        public IEnumerable<string> Claims {
            get
            {
                return _user.Claims ?? Enumerable.Empty<string>();
            }
        }
    }
}