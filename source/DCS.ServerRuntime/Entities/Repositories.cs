using System;
using System.Data;
using System.Linq;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using log4net;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Entities
{
    public class Repositories : EntitiesBase<RepositoryEntity, Guid>
    {
        private readonly ILog _log;

        public Repositories(IDbConnection db, ILog log) : base(db)
        {
            _log = log;
        }

        public RepositoryEntity Get(string name)
        {
            name = name.Replace(".git", "");
            return Db.LoadSelect<RepositoryEntity>(r => r.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Try to lock repository.
        /// </summary>
        /// <returns>Disposable for unlocking, if lock was obtained. Null, if lock could not be obtained.</returns>
        public IDisposable TryLock(RepositoryEntity repository, int timeoutSeconds = 120)
        {
            if (repository.LockedAt.HasValue
                && (DateTimeOffset.UtcNow - repository.LockedAt.Value).TotalSeconds < timeoutSeconds)
            {
                // still locked
                _log.DebugFormat("Waiting to acquire lock on {0}", repository.Name);
                return null;
            }

            repository.LockedAt = DateTime.UtcNow;
            Db.Save(repository);
            return new DelegatingDisposable(() =>
            {
                repository.LockedAt = null;
                Db.Save(repository);
            });
        }
    }
}