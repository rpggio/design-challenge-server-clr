using System;
using System.Collections.Generic;
using System.Data;
using DCS.Contracts;
using DCS.Contracts.Entities;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Entities
{
    public interface IEntityStore
    {
    }

    public class EntityStore<TEntity, TId> : EntitiesBase<TEntity, TId> where TEntity : IEntity<TId>
    {
        public EntityStore(IDbConnection db) : base(db)
        {
        }
    }

    public abstract class EntitiesBase<TEntity, TId> : IEntityStore where TEntity : IEntity<TId>
    {
        private readonly IDbConnection _db;

        protected IDbConnection Db
        {
            get { return _db; }
        }

        protected EntitiesBase(IDbConnection db)
        {
            _db = db;
        }

        public TEntity Get(TId id)
        {
            return _db.LoadSingleById<TEntity>(id);
        }

        public void Save(TEntity entity)
        {
            _db.Save(entity, true);
        }

        public List<TEntity> GetAll()
        {
            return _db.Select<TEntity>();
        }

        //public void Insert(TEntity entity)
        //{
        //    _db.Insert(entity);
        //}

        public void Update(TEntity entity, Func<SqlExpression<TEntity>, SqlExpression<TEntity>> foo)
        {
            _db.UpdateOnly(entity, foo);
        }
    }
}