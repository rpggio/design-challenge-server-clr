using System;
using DCS.Core;

namespace DCS.ServerRuntime.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        private readonly Type _entityType;
        private readonly string _identifier;

        public static EntityNotFoundException For<T>(object identifier, string message = null)
        {
            return new EntityNotFoundException(typeof(T), identifier.ToStringOrNull(), message);
        }

        public EntityNotFoundException(Type entityType, string identifier)
        {
            _entityType = entityType;
            _identifier = identifier;
        }

        public EntityNotFoundException(Type entityType, string identifier, string message) : base(message)
        {
            _entityType = entityType;
            _identifier = identifier;
        }

        public EntityNotFoundException(Type entityType, string identifier, string message, Exception innerException)
            : base(message, innerException)
        {
            _entityType = entityType;
            _identifier = identifier;
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }
    }
}