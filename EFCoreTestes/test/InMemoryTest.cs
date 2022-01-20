using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Entities;
using Xunit;

namespace EFCore.Testes
{
    public class InMemoryTest
    {
        [Fact]
        public void DeveInserirUmDepartamento()
        {
            var departamento = new Departamento
            {
                Descricao = "Tecnologia",
                DataCadastro = DateTime.Now
            };
            //Setup
            var context = CreateContext();
            context.Departamentos.Add(departamento);

            //Action
            var inseridos = context.SaveChanges();

            //Assert
            Assert.Equal(1,inseridos);
        }
        [Fact]
        public void NaoImplementadoFuncoesDeDatasParaOInMemory()
        {
            var departamento = new Departamento
            {
                Descricao = "Tecnologia",
                DataCadastro = DateTime.Now
            };
            //Setup
            var context = CreateContext();
            context.Departamentos.Add(departamento);

            //Action
            var inseridos = context.SaveChanges();

            Action action = () => context.Departamentos.FirstOrDefault(p => EF.Functions.DateDiffDay(DateTime.Now, p.DataCadastro) > 0);

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        private ApplicationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase("InMemoryTest")
                .Options;

            return new ApplicationContext(options);
        }
    }
}
