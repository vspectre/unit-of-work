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
        public void WritingRootScopeCommitOnce([Frozen]IUnitOfWork uow)
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
                sut.Commit();

            uow.Received(1).Commit();
        }

        [Fact, AutoDomainData]
        public void ReadingScopeCommitThrows()
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Reading))
                Assert.Throws<InvalidOperationException>(() => sut.Commit());
        }

        [Theory, AutoDomainData]
        public void WritingChildScopeCommitZero([Frozen]IUnitOfWork uow)
        {
            using (var root = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
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

        [Theory, AutoDomainData]
        public void DirectCommitCallThrows([Frozen] IUnitOfWork uow)
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                Assert.Throws<InvalidOperationException>(() => uow.Commit());
            }
        }

        [Fact, AutoDomainData]
        public void BlockCommitAfterDisposeThrows()
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                var child = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing);
                child.Dispose();

                Assert.Throws<InvalidOperationException>(() => sut.Commit());
            }
        }

        [Theory, AutoDomainData]
        public void NestedCommitsAllowed([Frozen]IUnitOfWork uow)
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                using (var child = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
                    child.Commit();
                
                sut.Commit();
            }

            uow.Received(1).Commit();
        }

        [Theory, AutoDomainData]
        public void ChildReadingScopeAllowsRootCommit([Frozen]IUnitOfWork uow)
        {
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Writing))
            {
                var child = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Reading);
                child.Dispose();

                sut.Commit();
            }

            uow.Received(1).Commit();
        }

        [Fact]
        public void UnitOfWorkFactoryPassedDoesNotChangeCurrentContext()
        {
            var factory = Substitute.For<UnitOfWorkFactory>();
            factory.Create<IUnitOfWork>().Returns(Substitute.For<IUnitOfWork>());
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Reading, factory))
            { }

            Assert.NotEqual(factory, UnitOfWorkFactory.Current);
        }

        [Fact]
        public void ScopeUsesFactoryPassed()
        {
            var factory = Substitute.For<UnitOfWorkFactory>();
            factory.Create<IUnitOfWork>().Returns(Substitute.For<IUnitOfWork>());
            using (var sut = new UnitOfWorkScope<IUnitOfWork>(UnitOfWorkScopeMode.Reading, factory))
            { }

            factory.Received().Create<IUnitOfWork>();
        }
    }
}
