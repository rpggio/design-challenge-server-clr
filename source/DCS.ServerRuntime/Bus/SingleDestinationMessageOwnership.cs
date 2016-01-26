using System;
using Rebus;

namespace DCS.ServerRuntime.Bus
{
    public class SingleDestinationMessageOwnership : IDetermineMessageOwnership
    {
        private readonly string _inputQueue;
        private readonly Predicate<Type> _shouldAccept;

        public SingleDestinationMessageOwnership(string inputQueue)
        {
            _inputQueue = inputQueue;
            _shouldAccept = delegate { return true; };
        }

        public SingleDestinationMessageOwnership(string inputQueue, Predicate<Type> shouldAccept)
        {
            _inputQueue = inputQueue;
            _shouldAccept = shouldAccept;
        }

        public string GetEndpointFor(Type messageType)
        {
            if (_shouldAccept(messageType))
            {
                return _inputQueue;
            }
            return null;
        }
    }
}