using System;

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
        */

        static void Main(string[] args)
        {
            
        }
    }
}
