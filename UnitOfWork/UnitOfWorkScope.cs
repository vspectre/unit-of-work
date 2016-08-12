using System;

namespace UnitOfWork
{
    public class UnitOfWorkScope<TUnitOfWork> : Disposable
        where TUnitOfWork : IUnitOfWork, new()
    {
        [ThreadStatic]
        private static ScopedUnitOfWork _scopedUnitOfWork;
        private bool _isRoot;
        private readonly UnitOfWorkScopeMode _mode;

        public TUnitOfWork UnitOfWork
        {
            get
            {
                return _scopedUnitOfWork.UnitOfWork;
            }
        }

        
        public UnitOfWorkScope(UnitOfWorkScopeMode mode)
        {
            _mode = mode;
            if (_scopedUnitOfWork == null)
            {
                _isRoot = true;
                _scopedUnitOfWork = new ScopedUnitOfWork(mode == UnitOfWorkScopeMode.Writing);
            }
        }

        public void Commit()
        {
            if (_mode != UnitOfWorkScopeMode.Writing)
                throw new InvalidOperationException(String.Format("Cannot commit for scopes in {0} mode.", UnitOfWorkScopeMode.Reading));

            if (!_isRoot)
                return;

            _scopedUnitOfWork.UnitOfWork.Commit();
        }

        /// <summary>
        /// Dispose implementation, checking post conditions for purpose and saving.
        /// </summary>
        /// <param name="disposing">Are we disposing?</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // We're disposing and SaveChanges wasn't called. That usually
                // means we're exiting the scope with an exception. Block saves
                // of the entire unit of work.
                //if (_mode == UnitOfWorkScopePurpose.Writing && !saveChangesCalled)
                //{
                //    scopedDbContext.BlockSave = true;
                //    // Don't throw here - it would mask original exception when exiting
                //    // a using block.
                //}

                if (_scopedUnitOfWork != null && _isRoot)
                {
                    //_scopedUnitOfWork.Dispose();
                    _scopedUnitOfWork = null;
                }
            }

            base.Dispose(disposing);
        }

        private class ScopedUnitOfWork : Disposable
        {
            public TUnitOfWork UnitOfWork { get; private set; }
            public bool ForWriting { get; private set; }

            public ScopedUnitOfWork(bool forWriting)
            {
                ForWriting = forWriting;
                UnitOfWork = new TUnitOfWork();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (UnitOfWork != null)
                    {
                        UnitOfWork.Commiting -= null;

                        var disposable = UnitOfWork as IDisposable;
                        if(disposable != null)
                            disposable.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }
    }
}