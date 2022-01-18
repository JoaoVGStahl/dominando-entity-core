using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Entity.Configurations;
using Entity.domain;
using Entity.Funcoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Entity.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("EntityLog.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Conversor> Conversores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Ator> Atores { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Instrutor> Instrutores { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Atributo> Atributos { get; set; }
        public DbSet<Funcao> Funcoes { get; set; }
        public DbSet<Livro> Livros { get; set; }

        public DbSet<Dictionary<string, object>> Configuracoes => Set<Dictionary<string, object>>("Configurações");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER;Initial Catalog=PontoSys-02;User Id=sa;Password=@jr120401;Pooling=True;Application Name=EFCore";
            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .AddInterceptors(new Interceptadores.InterceptadorDeComandos())
                .AddInterceptors(new Interceptadores.InterceptadorDeConexao())
                .AddInterceptors(new Interceptadores.InterceptadorPersistencia());
            /*
            .UseSqlServer(strConnection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .UseSqlServer(strConnection)

                .UseSqlServer(
                    strConnection,
                        o => o
                            .MaxBatchSize(100) // ! MaxBatchSize é Recomendavel utilizar em redes instaveis, para evitar varias conexões com o banco
                            .CommandTimeout(30)
                            .EnableRetryOnFailure(5,TimeSpan.FromSeconds(10), null)) // ! Tentará 5 vezes durante com delay de 10 segundos
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

            .LogTo(Console.WriteLine,
            new[]{ CoreEventId.ContextInitialized,
            RelationalEventId.CommandExecuted},
            LogLevel.Information,
            DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine);

            .LogTo(_writer.WriteLine, LogLevel.Information);
            .EnableDetailedErrors(); // ! Usado apenas em dubug, pois gera uma sobrecarga!
             */

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ! Filtro Global
            // * modelBuilder.Entity<Departamento>().HasQueryFilter( p => !p.Excluido);

            // ? SQL_Latin1_General => é o designador de agrupamento => Regras basicas de agrupamento
            // ? CP1 => Windows 1252 
            // ? CI => não ira diferenciar maiscula de minuscula / CS é o inverso ( Joao => joao)
            // ? AI => irá ignorar acentuação / CS => irá validar acentuação ( João => Joao)

            /*
             ! Configuração a nivel global!
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

             ! Configuração Propriedade!
            modelBuilder.Entity<Departamento>().Property(p => p.Descricao).UseCollation("SQL_Latin1_General_CP1_CS_AS");

             ? Por padrão é criado em int porem suporta apenas long, decimal, byte => limitação do banco de dados
             ? basicamento é um Identity, porem é mais flexivel
            modelBuilder
                .HasSequence<int>("MinhaSequencia", "sequencias")
                .StartsAt(1)
                .IncrementsBy(2)
                .HasMin(1)
                .HasMax(10)
                .IsCyclic();

            modelBuilder.Entity<Departamento>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");
            */

            /*
             ? Criação de Indices e Indices Compostos
            modelBuilder
                .Entity<Departamento>()
                .HasIndex(p => new {p.Descricao, p.Ativo})
                .HasDatabaseName("idx_meu_indice_composto")
                .HasFilter("Descricao IS NOT NULL")
                 ? Definir Fator de Preenchimento usado para ajustar o armazentamento e o desempennho dos indices / Irá deixar 20% da folha para beneficio proprio
                .HasFillFactor(80) 
                .IsUnique(); // ! Evita que o indice seja duplicado

            */

            /*
             ? Propagação de dados => Utilizado para informações que não serão alteradas com frequência
            modelBuilder
                .Entity<Estado>()
                .HasData(new[]{
                    new Estado { Id = 1, Nome = "São Paulo"},
                    new Estado {Id= 2, Nome = "Rio De Janeiro"}
                });

            */

            /*
            modelBuilder.HasDefaultSchema("cadastros");

            modelBuilder.Entity<Estado>().ToTable("Estados", "SegundaEsquema");
            */

            /*
             ? Todos os conversores de valores do EFCore => Microsoft.EntityFrameworkCore.Storage.ValueConversion

            var conversao = new ValueConverter<Versao,string>(p => p.ToString(), p =>(Versao)Enum.Parse(typeof(Versao), p));

            var conversao1 = new EnumToStringConverter<Versao>(); 

            modelBuilder.Entity<Conversor>()
                .Property(p => p.Versao)
                .HasConversion(conversao1);
                .HasConversion(conversao);
                .HasConversion<string>(); 
                .HasConversion(p => p.ToString(), p =>(Versao)Enum.Parse(typeof(Versao), p));  ? Salva no banco como String, porem ao ler converte para o obj Versao que é um Enum

            modelBuilder.Entity<Conversor>()
                .Property( p => p.Status)
                .HasConversion(new Conversores.ConversorCustomizado());

            modelBuilder.Entity<Departamento>()
                .Property<DateTime>("UltimaAtualizacao");
            */
            // ? Ativar uma configuração de Entidade
            modelBuilder.ApplyConfiguration(new ClienteConfiguration());

            // ? Ativar todas as configurações de entidade
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Configurações", b =>
            {
                b.Property<int>("Id");

                b.Property<string>("Chave")
                    .HasColumnType("VARCHAR(40)")
                    .IsRequired();

                b.Property<string>("Valor")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();
            });

            // ? Propriedade de sombra são utilizadas quando não queremo expor um campo em nossas Views
            modelBuilder
                .Entity<Funcao>(confi =>
                {
                    confi.Property<string>("PropriedadeSombra")
                        .HasColumnType("VARCHAR(100)")
                        .HasDefaultValueSql("'TESTE'");
                });

            // ? Utilizando Atributos
            // MinhaFuncoes.RegistrarFuncoes(modelBuilder);

            // ! Fluent API
            modelBuilder
                .HasDbFunction(_minhaFuncao)
                .HasName("LEFT")
                .IsBuiltIn();

            modelBuilder
                .HasDbFunction(_letrasMaiusculas)
                .HasName("ConverterParaLetrasMaiusculas")
                .HasSchema("dbo");

            modelBuilder
                .HasDbFunction(_dateDiff)
                .HasName("DATEDIFF")
                .HasTranslation(p =>
                {
                    var argumentos = p.ToList();
                    var constante = (SqlConstantExpression)argumentos[0];
                    argumentos[0] = new SqlFragmentExpression(constante.Value.ToString());

                    return new SqlFunctionExpression("DATEDIFF", argumentos, false, new[] { false, false, false }, typeof(int), null);
                })
                .IsBuiltIn();

        }
        private static MethodInfo _minhaFuncao = typeof(MinhasFuncoes).GetRuntimeMethod("Left", new[] { typeof(string), typeof(int) });

        private static MethodInfo _letrasMaiusculas = typeof(MinhasFuncoes).GetRuntimeMethod(nameof(MinhasFuncoes.LetrasMaiusculas), new[] { typeof(string) });
        private static MethodInfo _dateDiff = typeof(MinhasFuncoes).GetRuntimeMethod(nameof(MinhasFuncoes.DateDiff), new[] { typeof(string), typeof(DateTime), typeof(DateTime) });

        // ? Flush StreamWriter
        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }

        /*
        [DbFunction(name: "LEFT", IsBuiltIn = true)]
        public static string Left(string value, int quantidade)
        {
            throw new NotImplementedException();
        }
        */
    }
}