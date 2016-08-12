using Ploeh.AutoFixture;
using System;
using Xunit;

namespace UnitOfWork.UnitTests
{
    public class UnitOfWorkScopeTests
    {
        [Theory, AutoDomainData]
        public void UoWScopeIsDisposable(UnitOfWorkScope<TestUnitOfWork> sut)
        {
            Assert.IsAssignableFrom<IDisposable>(sut);
        }

        [Fact]
        public void WritingRootScopeCommitOnce()
        {
            using (var sut = new UnitOfWorkScope<TestUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                sut.Commit();
                Assert.Equal(1, sut.UnitOfWork.CommitCount);
            }
        }

        [Fact]
        public void ReadingScopeCommitThrows()
        {
            using (var sut = new UnitOfWorkScope<TestUnitOfWork>(UnitOfWorkScopeMode.Reading))
            {
                Assert.Throws<InvalidOperationException>(() => sut.Commit());
            }
        }

        [Fact]
        public void WritingChildScopeCommitZero()
        {
            using (var root = new UnitOfWorkScope<TestUnitOfWork>(UnitOfWorkScopeMode.Writing))
            using (var sut = new UnitOfWorkScope<TestUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                sut.Commit();
                Assert.Equal(0, sut.UnitOfWork.CommitCount);
            }
        }
    }
}
