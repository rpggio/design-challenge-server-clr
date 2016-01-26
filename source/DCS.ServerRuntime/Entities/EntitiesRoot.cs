using System.Data;

namespace DCS.ServerRuntime.Entities
{
    /// <summary>
    ///     Starting point for accessing model.
    /// </summary>
    public class EntitiesRoot
    {
        private readonly IDbConnection _db;
        private readonly Users _users;
        private readonly Repositories _repositories;

        public EntitiesRoot(IDbConnection db, Users users, Repositories repositories)
        {
            _db = db;
            _users = users;
            _repositories = repositories;
        }

        public Users Users
        {
            get { return _users; }
        }

        public Repositories Repositories
        {
            get { return _repositories; }
        }

    }
}