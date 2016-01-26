using System;
using System.Data;
using System.Linq;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.ServerRuntime.Services;
using Nancy.Security;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Entities
{
    public class Users : EntitiesBase<UserEntity, Guid>
    {
        private readonly AppSettings _appSettings;

        public Users(IDbConnection db, AppSettings appSettings) : base(db)
        {
            _appSettings = appSettings;
        }

        public UserEntity Get(IUserAware userAware)
        {
            return Get(userAware.UserId);
        }

        public UserEntity Get(string username)
        {
            if (_appSettings.Env.IsDcsDbSqlite)
            {
                // meh
                return Db.LoadSelect<UserEntity>(s => s.Where("Username LIKE {0}", username)).SingleOrDefault();
            }
            return Db.LoadSelect<UserEntity>(u => u.Username == username).SingleOrDefault();
        }

        public UserEntity ValidateUsernameOrEmail(string usernameOrEmail, string password)
        {
            return Db.LoadSelect<UserEntity>(u => 
                (u.Email == usernameOrEmail || u.Username == usernameOrEmail) && u.Password == password)
                .SingleOrDefault();
        }
    }
}