using System;
using System.Linq;
using Entity.Data;
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
            HealtCheckDataBase();
        }
        static ApplicatonContext ContextInstance()
        {
            return new Entity.Data.ApplicatonContext();
        }
        static ApplicatonContextCidade ContextInstanceCidade()
        {
            return new Entity.Data.ApplicatonContextCidade();
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
