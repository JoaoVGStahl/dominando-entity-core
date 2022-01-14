using System;
using System.Linq;
using Entity.Data;
using Entity.domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace DominandoEntityCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //FiltroGlobal();

            //IgnoreFiltroGlobal();

            //ConsultaProjetada();

            //ConsultaParametrizada();

            //ConsultaInterpolada();

            //ConsultaComTag();

            //Consulta1NN1();

            DivisaoConsulta();
        }

        static void DivisaoConsulta(){

            // ! Evitar Explosão Carteziana Implementada no EF Core 5
            using var db = new ApplicatonContext();

            Setup(db);

            var departamentos = db.Departamentos.Include(p => p.Funcionarios).Where(p => p.Id < 3)
            //.AsSplitQuery()
            //.AsSingleQuery()
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }
        }

        static void Consulta1NN1()
        {
            //  * Consultas Segura
            using var db = new ApplicatonContext();
            Setup(db);

            // * N-1
            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"\tNome: {funcionario.Nome} / Descricao Dep : {funcionario.Departamento.Descricao}");
            }

            // * 1-N
            var departamentos = db.Departamentos.Include(p => p.Funcionarios).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }
        }
        static void ConsultaComTag()
        {
            //  * Consultas Segura
            using var db = new ApplicatonContext();
            Setup(db);

            var departamentos = db.Departamentos.TagWith("Consulta Departamentos No Banco! - João Girardi").ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaInterpolada()
        {
            //  * Consultas Segura
            using var db = new ApplicatonContext();
            Setup(db);

            var id = 1;

            var departamentos = db.Departamentos.FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id >{id}")
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaParametrizada()
        {
            //  * Consultas Segura
            using var db = new ApplicatonContext();
            Setup(db);

            var id = new SqlParameter
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };

            var departamentos = db.Departamentos.FromSqlRaw("SELECT * FROM Departamentos WHERE Id >{0}", id)
            .Where(p => !p.Excluido).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaProjetada()
        {
            // ! Busca no banco apenas os campos que serão utilizados
            // ! Consultas projetadas é um boa práicas
            using var db = new ApplicatonContext();
            Setup(db);

            var departamentos = db.Departamentos
            .Where(p => p.Id > 0)
            .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }
        static void IgnoreFiltroGlobal()
        {
            using var db = new ApplicatonContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }
        static void FiltroGlobal()
        {
            using var db = new ApplicatonContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }

        static void Setup(ApplicatonContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Funcionario>
                        {
                            new Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Funcionario>
                        {
                            new Funcionario
                            {
                                Nome = "Bruno Brito",
                                CPF = "88888888811",
                                RG= "3100062"
                            },
                            new Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG= "1100062"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

    }
}
