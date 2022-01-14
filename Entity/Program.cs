using System;
using System.Linq;
using Entity.Data;
using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEntityCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDelete();
            //GapToEnsureCreated();
            //HealtCheckDataBase();
            // Warmup
            /*
            new ApplicatonContext().Departamentos.AsNoTracking().Any();
            GerenciarEstadoConexão(false);
            GerenciarEstadoConexão(true);
            */

            //SqlInjection();

            //MigracoesPendentes();

            //AplicarMigracaoEmTempoDeExecucao();

            //TodasMigracoes();

            //MigracoesAplicadas();

            //GerarScriptBancoDeDados();

            //CarregandoAdiantado();

            //CarregandoExplicito();

            CarregandoLento();

        }
        static ApplicatonContext ContextInstance()
        {
            return new Entity.Data.ApplicatonContext();
        }
        static ApplicatonContextCidade ContextInstanceCidade()
        {
            return new Entity.Data.ApplicatonContextCidade();
        }

        static void CarregandoLento()
        {
            // ! Cudiado ao utilizar LazyLoad em estruturas de repetições!
            // ! Adicionar virtual nas propriedades que serão utilizadas o LazyLoad
            // ! 1º Forma => add virtual nas propriedades

            using var db = ContextInstance();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db.Departamentos.ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionários: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum Funcionário encontrado!");
                }
            }
        }
        static void CarregandoExplicito()
        {

            using var db = ContextInstance();
            SetupTiposCarregamentos(db);

            var departamentos = db.Departamentos.ToList();

            foreach (var departamento in departamentos)
            {
                if (departamento.Id == 2)
                {

                    //db.Entry(departamento).Collection(p => p.Funcionarios).Load();

                    db.Entry(departamento).Collection(p => p.Funcionarios).Query().Where(p => p.Id > 2).ToList();
                }
                Console.WriteLine("--------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionários: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum Funcionário encontrado!");
                }
            }
        }
        static void CarregandoAdiantado()
        {
            // ! Recomendável utilizar apenas em tabelas com poucos campos

            using var db = ContextInstance();
            SetupTiposCarregamentos(db);

            var departamentos = db.Departamentos.Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionários: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum Funcionário encontrado!");
                }
            }
        }
        static void SetupTiposCarregamentos(ApplicatonContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Entity.domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Funcionario>
                        {
                            new Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        }
                    },
                    new Entity.domain.Departamento
                    {
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
        static void GerarScriptBancoDeDados()
        {

            using var db = ContextInstance();

            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }
        static void MigracoesAplicadas()
        {
            using var db = ContextInstance();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }

        }
        static void TodasMigracoes()
        {
            using var db = ContextInstance();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }

        }
        static void AplicarMigracaoEmTempoDeExecucao()
        {
            using var db = ContextInstance();

            // ! Atuliza o banco para a ultima migração, EVITAR AO MÁXIMO realizar em produção!
            db.Database.Migrate();
        }

        static void MigracoesPendentes()
        {
            using var db = ContextInstance();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void ExecuteSQL()
        {
            using var db = ContextInstance();

            // 1ª Opçãp
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // 2ª Opção
            // SqlRaw, recebe variasves e as converte em parâmentro para evitar Sql Injection
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //3ª Opção
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }

        static void SqlInjection()
        {
            using var db = ContextInstance();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Entity.domain.Departamento
                {
                    Descricao = "Departamento 01"
                },
                new Entity.domain.Departamento
                {
                    Descricao = "Departamento 02"
                }
            );
            db.SaveChanges();

            var descricao = "Tester ' or 1='1";

            // * Seguro

            //db.Database.ExecuteSqlRaw("update departamentos set descricao='Departamento Alterado' where descricao={0}", descricao);

            // ! Inseguro!
            db.Database.ExecuteSqlRaw($"update departamentos set descricao='Ataque Sql Injection' where descricao='{descricao}'");

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                System.Console.WriteLine($"Id: {departamento.Id}, Descricao: {departamento.Descricao}");
            }
        }

        static int _count;
        static void GerenciarEstadoConexão(bool gerenciarEstadoConexao)
        {
            _count = 0;
            using var db = ContextInstance();

            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;
            if (gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }
            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";
            Console.WriteLine(mensagem);

        }
        static void HealtCheckDataBase()
        {
            using var db = ContextInstance();
            var Connect = db.Database.CanConnect();

            if (Connect)
            {
                Console.WriteLine("Conectado!");
            }
            else
            {
                Console.WriteLine("Falha na conexão!");
            }

            /*
            try
            {
                // ! Option 1
                var connection = db.Database.GetDbConnection();
                connection.Open();

                // ! Option 2
                db.Departamentos.Any();

                Console.WriteLine("Conectado!");
            }
            catch (System.Exception)
            {
                Console.WriteLine("Falha na conexão!");
            }
            */
        }
        static void EnsureCreatedAndDelete()
        {
            using var db = ContextInstance();

            // ! Comando nunca devem ser usados em produção!
            db.Database.EnsureCreated();
            db.Database.EnsureDeleted();
        }
        static void GapToEnsureCreated()
        {
            using var db = ContextInstance();
            using var dbCidade = ContextInstanceCidade();

            db.Database.EnsureCreated();

            dbCidade.Database.EnsureCreated();

            var dataCreator = dbCidade.GetService<IRelationalDatabaseCreator>();
            dataCreator.CreateTables();
        }
    }
}
