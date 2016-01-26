using System;

namespace DCS.Contracts.Entities
{
    public class InvalidEntityException : Exception
    {
        public InvalidEntityException()
        {
        }

        public InvalidEntityException(string message) : base(message)
        {
        }

        public InvalidEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}