using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System;
using Xunit;
using NSubstitute;
using System.Linq;

namespace UnitOfWork.UnitTests
{
    public class UnitOfWorkScopeTests
    {
        [Theory, AutoDomainData]
        public void UoWScopeIsDisposable(UnitOfWorkScope<IUnitOfWork> sut)
        {
            Assert.IsAssignableFrom<IDisposable>(sut);
        }

        [Theory, AutoDomainData]
        public void WritingRootScopeCommitOnce([Frozen]IUnitOfWork uow, UnitOfWorkScope<IUnitOfWork> sut)
        {
            sut.Commit();
            uow.Received(1).Commit();
        }

        [Theory, AutoDomainData]
        public void ReadingScopeCommitThrows(Fixture fixture)
        {
            fixture.Inject(UnitOfWorkScopeMode.Reading);
            using (var sut = fixture.Create<UnitOfWorkScope<IUnitOfWork>>())
                Assert.Throws<InvalidOperationException>(() => sut.Commit());
        }

        [Theory, AutoDomainData]
        public void WritingChildScopeCommitZero([Frozen]IUnitOfWork uow, Fixture fixture)
        {
            fixture.Inject(UnitOfWorkScopeMode.Writing);
            using (var root = fixture.Create<UnitOfWorkScope<IUnitOfWork>>())
            using (var sut = fixture.Create<UnitOfWorkScope<IUnitOfWork>>())
            {
                sut.Commit();
            }

            uow.DidNotReceive().Commit();
        }

        [Fact]
        public void WritingChildReadingRootScopeThrows()
        {
            using (var root = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Reading))
            {
                Assert.Throws<InvalidOperationException>(() => new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing));
            }
        }
    }
}
