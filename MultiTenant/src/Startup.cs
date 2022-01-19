using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using src.Data;
using src.Data.Interceptors;
using src.Data.ModelFactory;
using src.Domain;
using src.Extensions;
using src.Middlewares;
using src.Provider;

namespace EFCore.MultTenant
{
    /* 
    ? Repository Pattern faz mediação entre o dominio e a camada de mapeamento e dado usando uma interface
    ? basicamente é um objeto que faz o isolamento das entidades do dominio de seu codigo que faz acesso á dados
    ? Prós    => Um unico ponto de acesso á dados / Encapsulamento da logica de acesso á dados / SPOF (Ponto unico de falha)
    ? Contras => Maior Complexidade
    ? Devo realmente adotar essa complexidade?

    ? UnitOfWork Definido como uma unica transação que pode envolver multiplas operações
    ? Mantém os objetos afetados na memoria e posteriormente submete ao banco de dados
    */
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<TenantData>();


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EFCore.MultTenant", Version = "v1" });
            });

            

            /* Estratégia 1 -- Identificador na Tabela
            services.AddScoped<StrategySchemaInterceptor>();
            services.AddDbContext<ApplicationContext>(p => p
                .UseSqlServer("Data Source=DESKTOP-LD0IN04\\DELLSERVER;Initial Catalog=EFCoreWebAPI;User Id=sa;Password=@jr120401;Pooling=True;Application Name=EFCore")
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging());
            */

            /* Estratégia 2 -- Schema
            services.AddDbContext<ApplicationContext>((provider, options) =>
            {
                 ? SELECT [p].[Id], [p].[Name], [p].[TenantId] FROM [tenant-1].[People] AS [p]

                options
                     .UseSqlServer("Data Source=DESKTOP-LD0IN04\\DELLSERVER;Initial Catalog=EFCoreWebAPI;User Id=sa;Password=@jr120401;Pooling=True;Application Name=EFCore")
                     .LogTo(Console.WriteLine)
                     .ReplaceService<IModelCacheKeyFactory, StrategySchemaModelCacheKey>()
                     .EnableSensitiveDataLogging();

                 var interceptor = provider.GetRequiredService<StrategySchemaInterceptor>();

                options.AddInterceptors(interceptor); 
            });
            */

            // Estratégia 3 - Banco de Dados

            services.AddHttpContextAccessor();

             
            services.AddScoped<ApplicationContext>(provider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
                var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
                var tenentId = httpContext?.GetTenantId();

                //var connectionString = Configuration.GetConnectionString(tenentId);
                var connectionString = Configuration.GetConnectionString("custom").Replace("_DATABSE_", tenentId);
                optionsBuilder
                    .UseSqlServer(connectionString)
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();

                return new ApplicationContext(optionsBuilder.Options);
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFCore.MultTenant v1"));
            }

            //DataBaseInitialize(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMiddleware<TenantMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void DataBaseInitialize(IApplicationBuilder app)
        {
            using var db = app.ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ApplicationContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (int i = 1; i <= 5; i++)
            {
                db.People.Add(new Person { Name = $"Person {i}" });
                db.Products.Add(new Product { Description = $"Product {i}" });
            }

            db.SaveChanges();
        }
    }
}
