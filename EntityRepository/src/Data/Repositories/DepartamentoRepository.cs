using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Data.Repositories.Base;
using src.Domain;

namespace src.Data.Repositories
{
    public class DepartamentoRepository : GenericRepository<Departamento>, IDepartamentoRepository
    {
        //  private readonly ApplicationContext _context;
        // private readonly DbSet<Departamento> _dbset;

        public DepartamentoRepository(ApplicationContext context) : base(context)
        {
            // _context = context;
            // _dbset = context.Set<Departamento>();
        }

        /*
        public void Add(Departamento departamento)
        {
            _dbset.Add(departamento);
        }

        public async Task<Departamento> GetByIdAsync(int id)
        {
            return await _dbset.Include(p => p.Colaboradores).FirstOrDefaultAsync( p => p.Id == id);
        }

        
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
        */
    }
}