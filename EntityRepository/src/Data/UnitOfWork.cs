using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Data.Repositories;

namespace src.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private  IDepartamentoRepository _departamentoRepository;
        public  IDepartamentoRepository DepartamentoRepository
        {
            get => _departamentoRepository ?? (_departamentoRepository = new DepartamentoRepository(_context));
        }

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
        }

        public bool commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}