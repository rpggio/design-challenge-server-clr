using System.Data;
using DCS.Contracts.Entities;
using DCS.ServerRuntime.Framework;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Bootstrap
{
    public class InitializeDatabaseOperation : IOperation
    {
        private readonly IDbConnection _db;

        public InitializeDatabaseOperation(IDbConnection db)
        {
            _db = db;
        }

        public void Execute()
        {
            _db.CreateTable<UserEntity>();
            _db.CreateTable<RepositoryEntity>();
            _db.CreateTable<UserChallengeEntity>();
            _db.CreateTable<CommitEntity>();
            _db.CreateTable<ContactEntity>();
        }
    }
}
