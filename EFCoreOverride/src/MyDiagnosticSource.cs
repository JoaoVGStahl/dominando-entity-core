using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace src
{
    // ! Responsável por manipular a informação do evento
    public class MyInterceptor : IObserver<KeyValuePair<string, object>>
    {
        private static readonly Regex _tableAliasRegex =
            new Regex(@"(?<tableAlias>FROM +(\[.*\]\.)?(\[.*\]) AS (\[.*\])(?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key == RelationalEventId.CommandExecuted.Name)
            {
                var command = ((CommandEventData)value.Value).Command;

                if (!command.CommandText.Contains("WITH (NOLOCK)"))
                {
                    Console.WriteLine($"\nAntes: {command.CommandText}");

                    command.CommandText = _tableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");

                    Console.WriteLine($"\nDepois: {command.CommandText}");
                    
                    /*
                    Antes: SELECT [d].[Id], [d].[Descricao]
                           FROM [Departamentos] AS [d]
                           WHERE [d].[Id] > 0

                    Depois: SELECT [d].[Id], [d].[Descricao]
                            FROM [Departamentos] AS [d] WITH (NOLOCK)
                            WHERE [d].[Id] > 0
                    */
                }
            }
        }
    }
    // ! Responsável por ouvir os eventos que serãp produzido pelo EFCore
    public class MyInterceptorListener : IObserver<DiagnosticListener>
    {
        private static readonly MyInterceptor _interceptor = new MyInterceptor();
        private static readonly Regex _tableAliasRegex =
            new Regex(@"(?<tableAlias>FROM +(\[.*\]\.)?(\[.*\]) AS (\[.*\])(?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase |
                RegexOptions.Compiled);
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(DiagnosticListener listerner)
        {
            if (listerner.Name == DbLoggerCategory.Name)
            {
                listerner.Subscribe(_interceptor);
            }
        }
    }
}