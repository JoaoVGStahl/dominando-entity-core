using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Entities;
using Xunit;

namespace EFCore.Testes
{
    public class SQLiteTest
    {
        [Theory]
        [InlineData("Tecnologia")]
        [InlineData("Financeiro")]
        [InlineData("Departamento Pessoal")]
        
        public void DeveInserirEConsultarUmDepartamento(string descricao)
        {
            var departamento = new Departamento
            {
                Descricao = descricao,
                DataCadastro = DateTime.Now
            };
            //Setup
            var context = CreateContext();
            context.Database.EnsureCreated();
            context.Departamentos.Add(departamento);

            //Action
            var inseridos = context.SaveChanges();
            departamento = context.Departamentos.FirstOrDefault(p => p.Descricao == descricao);
            //Assert
            Assert.Equal(1, inseridos);
            Assert.Equal(descricao, departamento.Descricao);
        }
        private ApplicationContext CreateContext()
        {
            var conexao = new SqliteConnection("DataSource=:memory:");
            conexao.Open();
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                //.UseSqlite("DataSource=:memory:")
                .UseSqlite(conexao)
                .Options;

            return new ApplicationContext(options);
        }
    }
}
