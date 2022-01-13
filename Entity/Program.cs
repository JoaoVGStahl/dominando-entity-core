using System;
using Entity.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEntityCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDelete();
            GapToEnsureCreated();
        }
        static ApplicatonContext Connection()
        {
            return new Entity.Data.ApplicatonContext();
        }
        static ApplicatonContextCidade ConnectionCidade()
        {
            return new Entity.Data.ApplicatonContextCidade();
        }

        static void EnsureCreatedAndDelete()
        {
            using var db = Connection();

            // ! Comando nunca devem ser usados em produção!
            db.Database.EnsureCreated();
            db.Database.EnsureDeleted();
        }
        static void GapToEnsureCreated()
        {
            using var db = Connection();
            using var dbCidade = ConnectionCidade();

            db.Database.EnsureCreated();

            dbCidade.Database.EnsureCreated();

            var dataCreator = dbCidade.GetService<IRelationalDatabaseCreator>();
            dataCreator.CreateTables();
        }
    }
}
