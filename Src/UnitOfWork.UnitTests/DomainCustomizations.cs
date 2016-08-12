using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace UnitOfWork.UnitTests
{
    public class DomainCustomizations : CompositeCustomization
    {
        public DomainCustomizations()
            : base(new AutoNSubstituteCustomization(),
                   new UnitofWorkFactoryCustomization())
        { }
    }
}