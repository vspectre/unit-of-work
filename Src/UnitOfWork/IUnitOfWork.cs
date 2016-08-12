using System;

namespace UnitOfWork
{
    public interface IUnitOfWork
    {
        void Commit();
        //event EventHandler Commiting;
    }
}