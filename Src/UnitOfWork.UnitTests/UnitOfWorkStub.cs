using System;

namespace UnitOfWork.UnitTests
{
    public class TestUnitOfWork : IUnitOfWork
    {
        private int _commitCount = 0;

        public int CommitCount { get { return _commitCount; } }

        public void Commit()
        {
            _commitCount++;
        }
    }
}