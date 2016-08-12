using System;

namespace UnitOfWork
{
    public interface IUnitOfWork
    {
        event EventHandler Commiting;

        void Commit();
    }
}