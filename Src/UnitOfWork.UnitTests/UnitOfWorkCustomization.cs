using Ploeh.AutoFixture;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.UnitTests
{
    public class UnitOfWorkCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<IUnitOfWork>(() =>
            {
                var u = Substitute.For<IUnitOfWork>();
                u.When(x => x.Commit())
                 .Do(x => u.Commiting += Raise.EventWith(new EventArgs()));

                return u;
            });
        }
    }
}
