using System.Data;
using DCS.Contracts.Entities;
using log4net;
using ServiceStack.OrmLite;

namespace DCS.Console.Commands
{
    public class CreateSchemaCommand : ConsoleCommandBase
    {
        private readonly IDbConnection _db;
        private readonly ILog _log;

        public CreateSchemaCommand(IDbConnection db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public override bool Execute()
        {
            _log.InfoFormat("Creating DCS database schema");

            var entityTypes = new[]
            {
                typeof(UserEntity),
                typeof(RepositoryEntity),
                typeof(CommitEntity),
                typeof(UserChallengeEntity),
                typeof(ContactEntity)
            };

            foreach (var type in entityTypes)
            {
                _db.CreateTableIfNotExists(type);
            }
            return true;
        }
    }
}