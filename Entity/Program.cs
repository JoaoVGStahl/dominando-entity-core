using System;
using System.Linq;
using Entity.Data;
using Entity.domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace DominandoEntityCore
{
    // ? Collections é a forma como os caracteres são codigificados e interpretados na base de dados / Como meus dados são ordenados e comparados.
    // ? SQL server NÃO é Case Sensitive
    // ? PostGree é Case Sensitive
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

            //DivisaoConsulta();

            //CriarStoredProcedures();

            //InserirViaStoredProcedures();

            //ConsultaViaStoredProcedures();

            //ConsultaDepartamentos();

            //DadosSensiveis();

            //HabilitandoBatchSize();

            //CommandTimeout();

            //ExecutarEstrategiaResiliencia();

            //Collection();

            PropagarDados();
        }

        static void PropagarDados(){
            using var db =new  ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void Collection(){
            using var db =new  ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void ExecutarEstrategiaResiliencia()
        {
            // ! Não salvar dados duplicados no banco
            
            using var db = new ApplicationContext();

            var strategy = db.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var transaction = db.Database.BeginTransaction();

                db.Departamentos.Add(new Departamento { Descricao = "Departamento Transaction" });

                db.SaveChanges();

                transaction.Commit();
            });
        }

        static void CommandTimeout()
        {
            using var db = new ApplicationContext();

            db.Database.SetCommandTimeout(10);

            db.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:07'; SELECT 1");
        }

        static void HabilitandoBatchSize()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (int i = 0; i < 50; i++)
            {
                db.Departamentos.Add(
                    new Departamento
                    {
                        Descricao = "Departamento" + i
                    }
                );
            }
            db.SaveChanges();
        }
        static void DadosSensiveis()
        {
            using var db = new ApplicationContext();

            var descricao = "Departamento";

            var departamentos = db.Departamentos.Where(p => p.Descricao == descricao).ToArray();
        }
        static void ConsultaDepartamentos()
        {
            using var db = new ApplicationContext();

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToArray();
        }

        static void ConsultaViaStoredProcedures()
        {
            using var db = new ApplicationContext();

            var dep = new SqlParameter("@Dep", "Departamento");

            var departamentos = db.Departamentos.FromSqlRaw("execute GetDepartamentos @Dep", dep).ToList();

            // * var departamentos = db.Departamentos.FromSqlRaw("execute GetDepartamentos @p0", "D").ToList();

            // * var departamentos = db.Departamentos.FromSqlInterpolated($"execute GetDepartamentos {dep}").ToList();

            // * var departamentos = db.Departamentos.FromSqlRaw("execute GetDepartamentos {0}", "D").ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine(departamento.Descricao);
            }
        }
        static void CriarConsultaViaStoredProcedures()
        {

            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE GetDepartamentos
                 @Descricao VARCHAR(50)
            AS 
            BEGIN
                SELECT * FROM Departamentos Where Descricao Like @Descricao + '%'
            END";

            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }

        static void InserirViaStoredProcedures()
        {
            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw("execute CriarDepartamento @p0, @p1", "Departamento Via Procedure", true);
        }
        static void CriarStoredProcedures()
        {

            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE CriarDepartamento
                 @Descricao VARCHAR(50),
                @Ativo bit
            AS 
            BEGIN
                INSERT INTO
                    Departamentos(Descricao,Ativo,Excluido)
                VALUES (@Descricao,@Ativo,0)
            END";




            using var db = new ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }
        static void DivisaoConsulta()
        {

            // ! SplitQuery Implementada no EF Core 5
            // ! Utilizado quando se realiza uma constula com muitos dados no banco.
            // ! Cuidado com explosão de plano carteziano!
            using var db = new ApplicationContext();

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
            using var db = new ApplicationContext();
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
            using var db = new ApplicationContext();
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
            using var db = new ApplicationContext();
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
            using var db = new ApplicationContext();
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
            using var db = new ApplicationContext();
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
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }
        static void FiltroGlobal()
        {
            using var db = new ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }

        static void Setup(ApplicationContext db)
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
