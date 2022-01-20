using System;
using Microsoft.EntityFrameworkCore;
using src.Data;

namespace EFCore
{
    class Program
    {
        /* 
        * Comandos Utilizados:
        mkdir EntityMigrationsModules
        cd .\EntityMigrationsModules\
        dotnet new sln -n EFMigrations
        dotnet new sln -n EFMigrations
        dotnet new console -n EFCore -o src -f net5.0
        dotnet sln EFMigrations.sln add .\src\EFCore.csproj
        dotnet tool update --global dotnet-ef --version 5.0.3
        dotnet add .\src\EFCore.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 5.0.3
        dotnet add .\src\EFCore.csproj package Microsoft.EntityFrameworkCore.Design --version 5.0.3
        dotnet add .\src\EFCore.csproj package Microsoft.EntityFrameworkCore.Tools --version 5.0.3
        code .
        dotnet restore
        dotnet build 

        dotnet ef migrations add PrimeiraMigracao -p  .\src\EFCore.csproj 
        dotnet ef migrations script PrimeiraMigracao -p  .\src\EFCore.csproj -o novoDiretorio
        dotnet ef migrations add PrimeiraMigracao -p  .\src\EFCore.csproj -o novoDiretorio -i

        dotnet ef database update -p .\src\EFCore.csproj -v
        dotnet ef migrations add AdicionarTelefone -p .\src\EFCore.csproj 

        dotnet ef database update PrimeiraMigracao -p .\src\EFCore.csproj -v

        dotnet ef migrations remove -p .\src\EFCore.csproj

        dotnet ef migrations list -p .\src\EFCore.csproj

        ? Multi-Tenant => Uma unica aplicação para diversos clientes, muito utlizado em Cloud-Computing
        ? Vantagens => Manutenção / Custo 
        ? Desvatagens => Segurança / Customização

        ? Single-Tenant -> Isolar tanto a aplicação quanto o banco de dados 
        ? Vantagens => Fácil customização / Segurança
        ? Desvatagens => Manutenção

        ? Estratégias:
        * Mais indicada
        ? Banco de dados => Uma unica aplicação que consegua acessar vários bancos de dados respectivos aos clientes
        ? Schema => Uma unica aplicação e banco de dados, porem com esquemas separados
        * Mais facil de ser aplicado e Manutenido
        ? Identificador na tabela => Uma unica aplicação, banco e esquema porem todas as tabelas tem um campo adicional para identifcar quem é o dono daquele registro

        */

        static void Main(string[] args)
        {
            VerificarMigracoes();
        }

        static void VerificarMigracoes()
        {
            var db = new ApplicationContext();
            // ! Muito Cuidado ao utilizar em produção!
            // ! db.Database.Migrate();

            var migracoes = db.Database.GetPendingMigrations();

            foreach (var migracao in migracoes)
            {
                Console.WriteLine(migracao);
            }
        }
    }
}
