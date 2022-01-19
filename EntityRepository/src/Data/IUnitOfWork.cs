using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Data.Repositories;

namespace src.Data
{
    public interface IUnitOfWork : IDisposable
    {
        bool commit();
        IDepartamentoRepository DepartamentoRepository{get;}
    }
}