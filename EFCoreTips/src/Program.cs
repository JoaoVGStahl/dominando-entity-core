﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Domain;

namespace EFCore.Tips
{
    /*
    ? SingleOfDefault => Quando você precisar de 0 ou 1 elemento de sua consulta // ! Causa uma Exceção caso retorne mais de 1 elemento 
    ? FirstOrDefault => Quando você precisar apenas de 1 elemento de sua consulta
    */
    class Program
    {
        static void Main(string[] args)
        {
            //ToQueryString();
            //DebugView();
            //Clear();
            //ConsultaFiltrada();
            //SingleDefaultFirstDefault();
            //SemChavePrimeria();
            //ToView();
            //NaoUnicode();
            //OperadoresAgregacao();
            //OperadoresAgregacaoNoAgrupamento();
            ContadorDeEventos();
        }

        // dotnet counters monitor -p 8128 --counters Microsoft.EntityFrameworkCore
        static void ContadorDeEventos()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            Console.WriteLine($" PID: {System.Diagnostics.Process.GetCurrentProcess().Id}");

            while(Console.ReadKey().Key != ConsoleKey.Escape)
            {
                var departamento = new Departamento
                {
                    Descricao = $"Departamento Sem Colaborador"
                };

                db.Departamentos.Add(departamento);
                db.SaveChanges();

                _ = db.Departamentos.Find(1);
                _ = db.Departamentos.AsNoTracking().FirstOrDefault();
            }
            
        }
        static void OperadoresAgregacaoNoAgrupamento()
        {
            using var db = new ApplicationContext();

            var sql = db.Departamentos
                .GroupBy(p => p.Descricao)
                .Where(p => p.Count() > 1)
                .Select(p => new
                {
                    Descricao = p.Key,
                    Contador = p.Count()
                }).ToQueryString();

            Console.WriteLine(sql);
            /*
            SELECT [d].[descricao] AS [Descricao], COUNT(*) AS [Contador]
            FROM [departamentos] AS [d]
            GROUP BY [d].[descricao]
            HAVING COUNT(*) > 1
            */
        }
        static void OperadoresAgregacao()
        {
            using var db = new ApplicationContext();

            var sql = db.Departamentos.
                GroupBy(p => p.Descricao)
                .Select(p => new
                {
                    Descricao = p.Key,
                    Contador = p.Count(),
                    Media = p.Average(p => p.Id),
                    Max = p.Max(p => p.Id),
                    Sum = p.Sum(p => p.Id)
                }).ToQueryString();

            Console.WriteLine(sql);
            /*
            SELECT[d].[descricao] AS[Descricao], COUNT(*) AS[Contador], AVG(CAST([d].[id] AS float)) AS[Media], MAX([d].[id]) AS[Max], COALESCE(SUM([d].[id]), 0) AS[Sum]
            FROM[departamentos] AS[d]
            GROUP BY[d].[descricao]

            */

        }
        static void NaoUnicode()
        {
            using var db = new ApplicationContext();

            var sql = db.Database.GenerateCreateScript();

            Console.WriteLine(sql);
        }


        static void ToView()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Database.ExecuteSqlRaw(
                @"CREATE VIEW vw_departamento_relatorio AS
                SELECT
                    d.Descricao, count(c.Id) as Colaboradores
                FROM Departamentos d 
                LEFT JOIN Colaboradores c ON c.DepartamentoId=d.Id
                GROUP BY d.Descricao");

            var departamentos = Enumerable.Range(1, 10)
                .Select(p => new Departamento
                {
                    Descricao = $"Departamento {p}",
                    Colaboradores = Enumerable.Range(1, p)
                        .Select(c => new Colaborador
                        {
                            Nome = $"Colaborador {p}-{c}"
                        }).ToList()
                });

            var departamento = new Departamento
            {
                Descricao = $"Departamento Sem Colaborador"
            };

            db.Departamentos.Add(departamento);
            db.Departamentos.AddRange(departamentos);
            db.SaveChanges();

            var relatorio = db.DepartamentoRelatorio
                .Where(p => p.Colaboradores < 20)
                .OrderBy(p => p.Departamento)
                .ToList();

            foreach (var dep in relatorio)
            {
                Console.WriteLine($"{dep.Departamento} [ Colaboradores: {dep.Colaboradores}]");
            }
        }
        // ? Apenas consulta, não é possivel adição ou alteração dos dados
        static void SemChavePrimeria()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var UsuarioFuncoes = db.UsuarioFuncoes.Where(p => p.UsuarioId == Guid.NewGuid()).ToArray();
        }

        static void SingleDefaultFirstDefault()
        {
            using var db = new ApplicationContext();

            Console.WriteLine("SingleOrDefault:");

            _ = db.Departamentos.SingleOrDefault(p => p.Id > 2);

            // ? SELECT TOP(2) [d].[Id], [d].[Descricao] FROM [Departamentos] AS [d] WHERE [d].[Id] > 2

            Console.WriteLine("FirstOrDefault:");

            _ = db.Departamentos.FirstOrDefault(p => p.Id > 2);

            // ? SELECT TOP(1) [d].[Id], [d].[Descricao] FROM [Departamentos] AS [d] WHERE [d].[Id] > 2

        }
        // ? Adiciona ao EFCore 5
        static void ConsultaFiltrada()
        {
            using var db = new ApplicationContext();

            var sql = db
                .Departamentos
                .Include(p => p.Colaboradores.Where(c => c.Nome.Contains("Teste")))
                .ToQueryString();

            Console.WriteLine(sql);
        }
        static void Clear()
        {
            using var db = new ApplicationContext();

            db.Departamentos.Add(new Departamento { Descricao = "Teste DebugView" });

            db.ChangeTracker.Clear();
        }
        static void DebugView()
        {
            using var db = new ApplicationContext();

            db.Departamentos.Add(new Departamento { Descricao = "Teste DebugView" });

            var query = db.Departamentos.Where(p => p.Id > 2);

        }
        static void ToQueryString()
        {
            using var db = new ApplicationContext();

            db.Database.EnsureCreated();

            var query = db.Departamentos.Where(p => p.Id > 2);

            var sql = query.ToQueryString();

            Console.WriteLine(sql);
        }

    }
}
