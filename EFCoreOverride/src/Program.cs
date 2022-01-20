using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src.Data;

namespace EFCore.Override
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ApplicationContext();

            db.Database.EnsureCreated();

            var sql = db.Departamentos.Where(p => p.Id > 0).ToQueryString();
            
            Console.WriteLine(sql);
        }
    }
}
