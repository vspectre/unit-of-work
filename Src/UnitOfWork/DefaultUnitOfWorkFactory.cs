using System;
using System.Reflection;

namespace UnitOfWork
{
    public class DefaultUnitOfWorkFactory : UnitOfWorkFactory
    {
        public override IUnitOfWork Create(Type unitOfWorkType)
        {
            return (IUnitOfWork)Activator.CreateInstance(unitOfWorkType);
        }
    }
}