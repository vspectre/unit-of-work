using System;

namespace UnitOfWork
{
    public class UnitOfWorkScope<TUnitOfWork> : Disposable
        where TUnitOfWork : IUnitOfWork
    {
        [ThreadStatic]
        private static ScopedUnitOfWork _scopedUnitOfWork;
        private bool _isRoot;
        private bool _commitCalled;
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
            else if (mode == UnitOfWorkScopeMode.Writing && !_scopedUnitOfWork.ForWriting)
                throw new InvalidOperationException("Cannot open a child scope for writing when the root scope is in reading mode.");
        }

        public void Commit()
        {
            if (_mode != UnitOfWorkScopeMode.Writing)
                throw new InvalidOperationException("Cannot commit for scopes in reading mode.");

            if (_scopedUnitOfWork.BlockCommit)
                throw new InvalidOperationException("Commiting is blocked for this scope. An enclosed scope was disposed without calling Commit.");

            _commitCalled = true;

            if (!_isRoot)
                return;

            _scopedUnitOfWork.AllowCommit = true;
            _scopedUnitOfWork.UnitOfWork.Commit();
            _scopedUnitOfWork.AllowCommit = false;
        }

        /// <summary>
        /// Dispose implementation, checking post conditions for purpose and saving.
        /// </summary>
        /// <param name="disposing">Are we disposing?</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // We're disposing and Commit wasn't called. That usually
                // means we're exiting the scope with an exception. Block commits
                // of the entire unit of work.
                if (_mode == UnitOfWorkScopeMode.Writing && !_commitCalled)
                {
                    _scopedUnitOfWork.BlockCommit = true;
                    // Don't throw here - it would mask original exception when exiting
                    // a using block.
                }

                if (_scopedUnitOfWork != null && _isRoot)
                {
                    _scopedUnitOfWork.Dispose();
                    _scopedUnitOfWork = null;
                }
            }

            base.Dispose(disposing);
        }

        private class ScopedUnitOfWork : Disposable
        {
            public TUnitOfWork UnitOfWork { get; private set; }
            public bool ForWriting { get; private set; }
            public bool AllowCommit { get; internal set; }
            public bool BlockCommit { get; internal set; }

            public ScopedUnitOfWork(bool forWriting)
            {
                ForWriting = forWriting;
                UnitOfWork = UnitOfWorkFactory.Current.Create<TUnitOfWork>();
                UnitOfWork.Commiting += GuardAgainstDirectCommits;
            }

            void GuardAgainstDirectCommits(object sender, EventArgs e)
            {
                if (!AllowCommit)
                {
                    throw new InvalidOperationException("Don't call Commit directly on unit-of-work owned by a UnitOfWorkScope. Use UnitOfWorkScope.Commit() instead.");
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (UnitOfWork != null)
                    {
                        UnitOfWork.Commiting -= GuardAgainstDirectCommits;

                        var disposable = UnitOfWork as IDisposable;
                        if(disposable != null)
                            disposable.Dispose();

                        UnitOfWork = default(TUnitOfWork);
                    }
                }
                base.Dispose(disposing);
            }
        }
    }
}