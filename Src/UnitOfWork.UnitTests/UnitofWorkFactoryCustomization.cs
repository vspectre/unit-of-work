using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.UnitTests
{
    public class UnitofWorkFactoryCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var unitOfWork = fixture.Freeze<IUnitOfWork>();
            
            var uowFactory = Substitute.ForPartsOf<UnitOfWorkFactory>();
            uowFactory.Create<IUnitOfWork>().Returns(unitOfWork);
            UnitOfWorkFactory.Current = uowFactory;
        }
    }
}
