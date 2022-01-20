using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using src.Data;
using src.Data.Repositories;
using src.Domain;

namespace EFCore.UowRepository.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentoController : ControllerBase
    {

        private readonly ILogger<DepartamentoController> _logger;
        //private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUnitOfWork _uow;

        public DepartamentoController(ILogger<DepartamentoController> logger, IDepartamentoRepository departamentoRepository, IUnitOfWork uow)
        {
            _logger = logger;
            //_departamentoRepository = departamentoRepository;
            _uow = uow;

        }

        //Departamento/id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(int id /* [FromServices] IDepartamentoRepository repository */)
        {
            var departamento = await _uow.DepartamentoRepository.GetByIdAsync(id);

            if (departamento != null) return Ok(departamento);

            return NotFound();
        }
        [HttpPost()]
        public IActionResult CreateDepartamento(Departamento departamento)
        {

            //_departamentoRepository.Add(departamento);

            _uow.DepartamentoRepository.Add(departamento);

            //var  saved = _departamentoRepository.Save();

            var saved = _uow.commit();

            if (saved) return Ok(departamento);

            return BadRequest(departamento);
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var departamento = await _uow.DepartamentoRepository.GetByIdAsync(id);
            _uow.DepartamentoRepository.Remove(departamento);
            var saved = _uow.commit();
            if (saved) return Ok(departamento);

            return BadRequest(departamento); 
        }
    }
}


