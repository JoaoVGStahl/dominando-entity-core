using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Entity.Data;
using Entity.domain;
using Entity.Funcoes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace DominandoEntityCore
{
    /*
     ? Collections é a forma como os caracteres são codigificados e interpretados na base de dados / Como meus dados são ordenados e comparados.
     ? SQL server NÃO é Case Sensitive
     ? PostGree é Case Sensitive
     ? Esquema é a forma de organizar seu modelo de dados lá em seu banco
     ? Transaction  => Atomicidade / Consistência / Isolamento / Durabilidade
     ? Atomicidade  => Ou Faz tudo ou não faz nada
     ? Consistência => Garantir que os dados estejam consistente antes e depois de uma transação
     ? Isolamento   => Um transação ainda em andamento ocorre isoladamente das outras operações
     ? Durabilidade => Faz com que os dados sejam gravados mesmo após uma reinicialização.
     ? UDF (User Definid Function) => São funções definidas pelo usuário
     ? AsNoTracking vs Tracking
     ? Resolução de Identidade
     ? Migrations => Versionamento de Modelo de Dados
     ? Dependencias Migrations => Microsoft.EntityFrameworkCore.Design | Microsoft.EntityFrameworkCore.Tools | EF Cli
    */
    class Program
    {
        static void Main(string[] args)
        {
            //Setup();
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
            //PacotesDePropriedades();
            //Atributos();
            //FuncoesDeDatas();
            //FuncaoLike();
            //FuncaoDataLength();
            //FuncaoProperty();
            //FuncaoCollate();
            //TesteInterceptor();
            //TesteInterceptacaoSavingChanges();
            //ComportamentoPadrao();
            //GerenciandoTransactionManualmente();
            //SalvarPontoTransacao();
            //TransactionScope();
            //FuncaoLeft();
            //FuncaoDefinidaPeloUsuario();
            //DateDIFF();
            //ConsultaRastreada();
            //ConsultaNaoRastreada();
            //ConsultaComResolucaoIdentidade();
            //ConsultaCustomizada();
            //ConsultaProjetadaERastrada();
            //Inserir_200_Departamentos_Com_1MB();
            //ConsultaProjetada2();
        }
        static void ConsultaProjetada2()
        {
            using var db = new ApplicationContext();
            // ! 360 MB / 6s 790md
            // ! var departamentos = db.Departamentos.ToArray();
            // * 55 MB / 3s 165ms
            var departamentos = db.Departamentos.Select(p => p.Descricao).ToArray();
            var memoria = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024) + " MB";
            Console.WriteLine(memoria);
        }
        static void Inserir_200_Departamentos_Com_1MB()
        {
            // ? 36S 616ms
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            var random = new Random();
            db.Departamentos.AddRange(Enumerable.Range(1, 200).Select(p => new Departamento
            {
                Descricao = "Departamento Teste",
                Image = getBytes()
            }));
            db.SaveChanges();
            byte[] getBytes()
            {
                var buffer = new byte[1024 * 1024];
                random.NextBytes(buffer);
                return buffer;
            }
        }
        static void ConsultasProjetadas()
        {
            // ? Alterando o comportamento enquanto o contexto existir
            using var db = new ApplicationContext();
            var departamentos = db.Departamentos.Include(p => p.Funcionarios).Select(p => new
            {
                Departamento = p,
                TotalFuncionario = p.Funcionarios.Count()
            }).ToList();
            departamentos[0].Departamento.Descricao = "Departamento Teste Atualizado";
            db.SaveChanges();
        }
        static void ConsultaProjetadaERastrada()
        {
            // ? Alterando o comportamento enquanto o contexto existir
            using var db = new ApplicationContext();
            var departamentos = db.Departamentos.Include(p => p.Funcionarios).Select(p => new
            {
                Departamento = p,
                TotalFuncionario = p.Funcionarios.Count()
            }).ToList();
            departamentos[0].Departamento.Descricao = "Departamento Teste Atualizado";
            db.SaveChanges();
        }
        static void ConsultaCustomizada()
        {
            // ? Alterando o comportamento enquanto o contexto existir
            using var db = new ApplicationContext();
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }
        static void ConsultaComResolucaoIdentidade()
        {
            // ? Agora o EFCore foi até o banco, e criou apenas 1 Instância de Departamento para cada Funcionário
            using var db = new ApplicationContext();
            var funcionarios = db.Funcionarios.AsNoTrackingWithIdentityResolution().Include(p => p.Departamento).ToList();
        }
        static void ConsultaNaoRastreada()
        {
            // ? Como a cosulta não foi rastreada, o EFCore criou 100 instâncias para cada objeto funcionário.
            using var db = new ApplicationContext();
            var funcionarios = db.Funcionarios.AsNoTracking().Include(p => p.Departamento).ToList();
        }
        static void ConsultaRastreada()
        {
            // ? Como a cosulta foi rastreada, o EFCore recuperou a instancia do departamento e reutilizou para o funcionário.
            using var db = new ApplicationContext();
            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }
        static void Setup()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Departamentos.Add(new Departamento
            {
                Descricao = "Departamento Teste",
                Ativo = true,
                Funcionarios = Enumerable.Range(1, 100).Select(p => new Funcionario
                {
                    CPF = p.ToString().PadLeft(11, '0'),
                    Nome = $"Funcionando {p}",
                    RG = p.ToString()
                }).ToList()
            });
            db.SaveChanges();
        }
        static void DateDIFF()
        {
            CadastrarLivro();
            using var db = new ApplicationContext();
            /*
            var resultado = db
                .Livros
                .Select(p => EF.Functions.DateDiffDay(p.CadastradoEm, DateTime.Now));
            */
            var resultado = db
                .Livros
                .Select(p => MinhasFuncoes.DateDiff("DAY", p.CadastradoEm, DateTime.Now));
            foreach (var diff in resultado)
            {
                Console.WriteLine(diff);
            }
        }
        static void FuncaoDefinidaPeloUsuario()
        {
            CadastrarLivro();
            using var db = new ApplicationContext();
            db.Database.ExecuteSqlRaw(@"
                CREATE FUNCTION ConverterParaLetrasMaiusculas(@dados VARCHAR(100))
                RETURNS VARCHAR(100)
                BEGIN
                    RETURN UPPER(@dados)
                END");
            var resultado = db.Livros.Select(p => MinhasFuncoes.LetrasMaiusculas(p.Titulo));
            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }
        static void FuncaoLeft()
        {
            CadastrarLivro();
            using var db = new ApplicationContext();
            var result = db.Livros.Select(p => MinhasFuncoes.Left(p.Titulo, 10));
            foreach (var parteTitulo in result)
            {
                Console.WriteLine(parteTitulo);
            }
        }
        static void TransactionScope()
        {
            CadastrarLivro();
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                ConsultarAtualizar();
                CadastraLivroEnterprise();
                CadastrarLivroDominandoEFCore();
                scope.Complete();
            }
        }
        static void ConsultarAtualizar()
        {
            using (var db = new ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Rafael Almeida";
                db.SaveChanges();
            }
        }
        static void CadastraLivroEnterprise()
        {
            using (var db = new ApplicationContext())
            {
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "ASP.NET Core Enterprise Applications",
                        Autor = "Eduardo Pires"
                    });
                db.SaveChanges();
            }
        }
        static void CadastrarLivroDominandoEFCore()
        {
            using (var db = new ApplicationContext())
            {
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    });
                db.SaveChanges();
            }
        }
        static void SalvarPontoTransacao()
        {
            CadastrarLivro();
            using (var db = new ApplicationContext())
            {
                using var transacao = db.Database.BeginTransaction();
                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "João Girardi";
                    db.SaveChanges();
                    transacao.CreateSavepoint("desfazer_apenas_insercao");
                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "ASP.NET Core Enterprise Applications",
                            Autor = "Eduardo Pires"
                        });
                    db.SaveChanges();
                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Rafael Almeida".PadLeft(100, '*')
                        });
                    db.SaveChanges();
                    transacao.Commit();
                }
                catch (DbUpdateException e)
                {
                    transacao.RollbackToSavepoint("desfazer_apenas_insercao");
                    if (e.Entries.Count(p => p.State == EntityState.Added) == e.Entries.Count)
                    {
                        transacao.Commit();
                    }
                }
            }
        }
        static void GerenciandoTransactionManualmente()
        {
            CadastrarLivro();
            using (var db = new ApplicationContext())
            {
                var transaction = db.Database.BeginTransaction();
                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Robert Cecil Martin";
                    db.SaveChanges();
                    db.Livros.Add(new Livro
                    {
                        Titulo = "Arquitetura Limpa",
                        Autor = "Robert Cecil Martin".PadLeft(100, '*')
                    });
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                }
            }
        }
        static void ComportamentoPadrao()
        {
            CadastrarLivro();
            using (var db = new ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Robert Cecil Martin";
                db.Livros.Add(new Livro
                {
                    Titulo = "Arquitetura Limpa",
                    Autor = "Robert Cecil Martin"
                });
                db.SaveChanges();
            }
        }
        static void CadastrarLivro()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Livros.Add(new Livro
                {
                    Titulo = "Código Limpo",
                    Autor = "Robert",
                    CadastradoEm = DateTime.Now.AddDays(7)
                });
                db.SaveChanges();
            }
        }
        static void TesteInterceptacaoSavingChanges()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Funcoes.Add(new Funcao
                {
                    Descricao1 = "Teste Interceptor"
                });
                db.SaveChanges();
            }
        }
        static void TesteInterceptor()
        {
            using (var db = new ApplicationContext())
            {
                var consulta = db.Funcoes.TagWith("Use NOLOCK").FirstOrDefault();
                Console.WriteLine($"Consulta: {consulta?.Descricao1}");
            }
        }
        // ? Realizar consultas com Collates especificos => Por ex: Descricao1 é utilizado CaseSensitive, ja Descricao2 não!
        static void FuncaoCollate()
        {
            using (var db = new ApplicationContext())
            {
                var consulta1 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CS_AS") == "tela");
                var consulta2 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CI_AS") == "Tela");
                Console.WriteLine($"Consulta1: {consulta1?.Descricao1}");
                Console.WriteLine($"Consulta2: {consulta2?.Descricao1}");
            }
            /*
                Consulta1: 
                Consulta2: Tela
            */
        }
        static void FuncaoProperty()
        {
            ApagarCriarBanco();
            using (var db = new ApplicationContext())
            {
                // ? Nunca Utilizar o AsNoTracking
                var resultado = db
                    .Funcoes
                    //.AsNoTracking()
                    .FirstOrDefault(p => EF.Property<string>(p, "PropriedadeSombra") == "Teste");
                var propriedadeSombra = db
                    .Entry(resultado)
                    .Property<string>("PropriedadeSombra")
                    .CurrentValue;
                Console.WriteLine("Resultado:");
                Console.WriteLine(propriedadeSombra);
            }
        }
        static void FuncaoDataLength()
        {
            using (var db = new ApplicationContext())
            {
                var resultado = db
                    .Funcoes
                    .AsNoTracking()
                    .Select(p => new
                    {
                        TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
                        TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
                        TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
                        Total1 = p.Descricao1.Length,
                        Total2 = p.Descricao2.Length
                    })
                    .FirstOrDefault();
                Console.WriteLine("Resultado:");
                Console.WriteLine(resultado);
            }
            // * SELECT TOP(1) DATALENGTH([f].[Data1]) AS [TotalBytesCampoData], DATALENGTH([f].[Descricao1]) AS [TotalBytes1], DATALENGTH([f].[Descricao2]) AS [TotalBytes2], CAST(LEN([f].[Descricao1]) AS int) AS [Total1], CAST(LEN([f].[Descricao2]) AS int) AS [Total2] FROM [Funcoes] AS [f]
            /*
            ?   Formato NVARCHAR reserva 1 Byte a mais por conta de Linguas que ocupam 2 Bytes por caracteres.
             * Resultado:
             * { TotalBytesCampoData = 8, TotalBytes1 = 12, TotalBytes2 = 6, Total1 = 6, Total2 = 6 } 
             */
        }
        static void FuncaoLike()
        {
            using (var db = new ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);
                var dados = db
                    .Funcoes
                    .AsNoTracking()
                    //.Where(p => EF.Functions.Like(p.Descricao1, "Bo%"))
                    .Where(p => EF.Functions.Like(p.Descricao1, "B[ao]%"))
                    .Select(p => p.Descricao1)
                    .ToArray();
                Console.WriteLine("Resultado:");
                foreach (var descricao in dados)
                {
                    Console.WriteLine(descricao);
                }
            }
            // * SELECT [f].[Descricao1] FROM [Funcoes] AS [f] WHERE [f].[Descricao1] LIKE N'B[ao]%'
        }
        static void FuncoesDeDatas()
        {
            ApagarCriarBanco();
            using (var db = new ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);
                var dados = db.Funcoes.AsNoTracking().Select(p =>
                   new
                   {
                       Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
                       Meses = EF.Functions.DateDiffMonth(DateTime.Now, p.Data1),
                       Data = EF.Functions.DateFromParts(2021, 1, 2),
                       DataValida = EF.Functions.IsDate(p.Data2),
                   });
                foreach (var f in dados)
                {
                    Console.WriteLine(f);
                }
            }
            // * SELECT DATEDIFF(DAY, GETDATE(), [f].[Data1]) AS [Dias], DATEDIFF(MONTH, GETDATE(), [f].[Data1]) AS [Meses], DATEFROMPARTS(2021, 1, 2) AS [Data], CAST(ISDATE([f].[Data2]) AS bit) AS [DataValida] FROM [Funcoes] AS [f]
        }
        static void ApagarCriarBanco()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Funcoes.AddRange(
                new Funcao
                {
                    Data1 = DateTime.Now.AddDays(2),
                    Data2 = "2022-01-01",
                    Descricao1 = "Bala 1",
                    Descricao2 = "Bala 1"
                },
                new Funcao
                {
                    Data1 = DateTime.Now.AddDays(1),
                    Data2 = "XX22-01-01",
                    Descricao1 = "Bola 2",
                    Descricao2 = "Bola 2"
                },
                new Funcao
                {
                    Data1 = DateTime.Now.AddDays(1),
                    Data2 = "XX22-01-01",
                    Descricao1 = "Tela",
                    Descricao2 = "Tela"
                }
            );
            db.SaveChanges();
        }
        static void Atributos()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);
                db.Atributos.Add(new Atributo
                {
                    Descricao = "Exemplo",
                    Observacao = "Observacao"
                });
                db.SaveChanges();
            }
        }
        static void PacotesDePropriedades()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                var configuracao = new Dictionary<string, object>
                {
                    ["Chave"] = "SenhaBancoDados",
                    ["Valor"] = Guid.NewGuid().ToString()
                };
                db.Configuracoes.Add(configuracao);
                db.SaveChanges();
                var configuracoes = db.Configuracoes
                    .AsNoTracking()
                    .Where(p => p["Chave"].Equals("SenhaBancoDados"))
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
        static void CampoDeApoio()
        {
            using (var db = new ApplicationContext())
            {
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
        static void Relacionamento1ParaMuitos()
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                var estado = new Estado
                {
                    Nome = "Sao Paulo",
                    Governador = new Governador { Nome = "Joao Girardi" }
                };
                estado.Cidades.Add(new Cidade { Nome = "Bebedouro" });
                db.Estados.Add(estado);
                db.SaveChanges();
            }
            using (var db = new ApplicationContext())
            {
                var estados = db.Estados.ToList();
                estados[0].Cidades.Add(new Cidade { Nome = "Barretos" });
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
            estados.ForEach(est =>
            {
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
