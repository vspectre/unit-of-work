using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public abstract class UnitOfWorkFactory
    {
        private static UnitOfWorkFactory _current;
        
        public static UnitOfWorkFactory Current
        {
            get { return UnitOfWorkFactory._current; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                UnitOfWorkFactory._current = value;
            }
        }


        static UnitOfWorkFactory()
        {
            UnitOfWorkFactory._current = new DefaultUnitOfWorkFactory();
        }

        public static void ResetToDefault()
        {
            UnitOfWorkFactory._current = new DefaultUnitOfWorkFactory();
        }

        public virtual TUnitOfWork Create<TUnitOfWork>() where TUnitOfWork : IUnitOfWork
        {
            return (TUnitOfWork)Create(typeof(TUnitOfWork));
        }

        public abstract IUnitOfWork Create(Type unitOfWorkType);
    }
}
