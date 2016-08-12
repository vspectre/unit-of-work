using System;

namespace UnitOfWork
{
    public abstract class Disposable : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposed = true;
            }
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}