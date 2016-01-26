using System;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public class DelegatingDisposable : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _disposed;

        public DelegatingDisposable(Action disposeAction)
        {
            if (disposeAction == null) throw new ArgumentNullException("disposeAction");
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            lock (_disposeAction)
            {
                if (_disposed) return;
                _disposed = true;
                _disposeAction();
            }
        }
    }
}