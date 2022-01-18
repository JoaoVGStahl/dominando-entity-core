using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Entity.Funcoes
{
    public static class MinhasFuncoes
    {
        // ? DataAnnotation
        [DbFunction(name: "LEFT", IsBuiltIn = true)]
        public static string Left(string value, int quantidade)
        {
            throw new NotImplementedException();
        }
        public static string LetrasMaiusculas(string value)
        {
            throw new NotImplementedException();
        }
        public static int DateDiff(string indentificador, DateTime inicial, DateTime final)
        {
            throw new NotImplementedException();
        }


        public static void RegistrarFuncoes(ModelBuilder modelBuilder)
        {
            var funcoes = typeof(MinhasFuncoes).GetMethods().Where(p => Attribute.IsDefined(p, typeof(DbFunctionAttribute)));

            foreach (var funcao in funcoes)
            {
                modelBuilder.HasDbFunction(funcao);
            }
        }
    }
}