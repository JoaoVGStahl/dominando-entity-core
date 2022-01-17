using System;
using System.Collections.Generic;
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

    // ? Esquema é a forma de organizar seu modelo de dados lá em seu banco
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

            //PropagarDados();

            //Esquema();

            //ConversorValor();

            //ConversorCustomizado();

            //PropriedadesDeSombra();

            //TrabalhandoComPropriedadesDeSombra();

            //TiposPropriedades();

            //Relacionamento1Para1();

            //Relacionamento1ParaMuitos();

            //RelacionamentoMuitosParaMuitos();

            //CampoDeApoio();

            //ExemploTPH();

            //PacotesDePropriesdades();

            Atributos();

        }

        static void Atributos(){
            using (var db = new ApplicationContext()){

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                db.Atributos.Add( new Atributo{
                    Descricao = "Exemplo",
                    Observacao = "Observacao"
                });

                db.SaveChanges();
            }
        }

        static void PacotesDePropriesdades(){
            using (var db = new ApplicationContext()){
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var configuracao = new Dictionary<string, object>{
                    ["Chave"] = "SenhaBancoDados",
                    ["Valor"] = Guid.NewGuid().ToString()
                };
                db.Configuracoes.Add(configuracao);

                db.SaveChanges();

                var configuracoes = db.Configuracoes
                    .AsNoTracking()
                    .Where( p => p["Chave"].Equals("SenhaBancoDados"))
                    .ToArray();

                foreach (var dic in configuracoes)
                {
                    Console.WriteLine($"Chave: {dic["Chave"]} - Valor: {dic["Valor"]}");
                }
                
            }
        }

        static void ExemploTPH()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var pessoa = new Pessoa { Nome = "Jose Silva" };

                var instrutor = new Instrutor { Nome = "Rafael Almeida", Tecnologia = ".NET EFCore", Desde = DateTime.Now };

                var aluno = new Aluno { Nome = "Joao Girardi", Idade = 20, DataContrato = DateTime.Now.AddDays(-1) };

                db.AddRange(pessoa, instrutor, aluno);
                db.SaveChanges();

                var pessoas = db.Pessoas.AsNoTracking().ToArray();
                var instrutores = db.Instrutores.AsNoTracking().ToArray();
                // * var alunos = db.Alunos.AsNoTracking().ToArray();
                var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

                Console.WriteLine("Pessoas **************");
                foreach (var p in pessoas)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}");
                }

                Console.WriteLine("Instrutores **************");
                foreach (var p in instrutores)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Tecnologia: {p.Tecnologia}, Desde: {p.Desde}");
                }

                Console.WriteLine("Alunos **************");
                foreach (var p in alunos)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Idade: {p.Idade}, Data do Contrato: {p.DataContrato}");
                }
            }
        }

        static void CampoDeApoio(){
            using ( var db = new ApplicationContext()){
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var documento = new Documento();

                documento.SetCPF("12345678900");

                db.Documentos.Add(documento);
                db.SaveChanges();

                foreach (var doc in db.Documentos.AsNoTracking())
                {
                    Console.WriteLine($"CPF => {doc.GetCPF()}");
                }
            }
        }
        static void RelacionamentoMuitosParaMuitos()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var ator1 = new Ator { Nome = "Joao" };
                var ator2 = new Ator { Nome = "Jose" };
                var ator3 = new Ator { Nome = "Juca" };

                var filme1 = new Filme { Descricao = "Velozes e Furiosos" };
                var filme2 = new Filme { Descricao = "Lentos e Tranquilos" };
                var filme3 = new Filme { Descricao = "Nem um, nem outro" };

                ator1.Filmes.Add(filme1);
                ator1.Filmes.Add(filme2);

                ator2.Filmes.Add(filme1);

                filme3.Atores.Add(ator1);
                filme3.Atores.Add(ator2);
                filme3.Atores.Add(ator3);

                db.AddRange(ator1, ator2, filme3);

                db.SaveChanges();

                foreach (var ator in db.Atores.Include(e => e.Filmes))
                {
                    Console.WriteLine($"Ator: {ator.Nome}");

                    foreach (var filme in ator.Filmes)
                    {
                        Console.WriteLine($"\tFilme: {filme.Descricao}");
                    }
                }
            }
        }

        static void Relacionamento1ParaMuitos(){
            using (var db = new ApplicationContext()){
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var estado = new Estado{
                    Nome = "Sao Paulo", Governador = new Governador{ Nome = "Joao Girardi"}
                };

                estado.Cidades.Add( new Cidade { Nome = "Bebedouro"});

                db.Estados.Add(estado);

                db.SaveChanges();
            }

            using (var db = new ApplicationContext()){
                var estados = db.Estados.ToList();

                estados[0].Cidades.Add(new Cidade {Nome = "Barretos"});

                db.SaveChanges();

                foreach (var est in db.Estados.Include(p => p.Cidades).AsNoTracking())
                {
                    Console.WriteLine($"Estado: {est.Nome}. Governador: {est.Governador.Nome}");
                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\t Cidade: {cidade.Nome}");
                    }
                }
            }
        }

        static void Relacionamento1Para1()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "São Paulo",
                Governador = new Governador { Nome = "Joao Girardi" }
            };

            db.Estados.Add(estado);
            db.SaveChanges();

            var estados = db.Estados.AsNoTracking().ToList();

            estados.ForEach(est => {
                Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");
            });

        }

        static void TiposPropriedades()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente
            {
                Nome = "Joao Girardi",
                Telefone = "(00)12345-7890",
                Endereco = new Endereco { Bairro = "Centro", Cidade = "Sao Paulo" }
            };
            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

                Console.WriteLine(json);
            });
        }

        static void TrabalhandoComPropriedadesDeSombra()
        {
            using var db = new ApplicationContext();

            /*

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento
            {
                Descricao = "Departamento Sombra"
            };

            db.Departamentos.Add(departamento);

            db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            db.SaveChanges();
            */

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now).ToArray();
        }


        static void PropriedadesDeSombra()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void ConversorCustomizado()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Conversores.Add(
                new Conversor
                {
                    Status = Status.Devolvido,
                }
            );
            db.SaveChanges();

            var conversorEmAnalise = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Analise);

            var conversorDevolvido = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Devolvido);

        }

        static void ConversorValor() => Esquema();

        static void Esquema()
        {
            using var db = new ApplicationContext();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void PropagarDados()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void Collection()
        {
            using var db = new ApplicationContext();

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
