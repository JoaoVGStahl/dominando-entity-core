using System;
using System.Linq;
using Entity.domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Entity.Conversores
{
    public class ConversorCustomizado : ValueConverter<Status,string>
    {
        public ConversorCustomizado() : base(
            p => ConverterParaBanco(p),
            value => converterParaApp(value),
            new ConverterMappingHints(1)
            )
        {
            
        }

        static string ConverterParaBanco(Status status){
            return status.ToString()[0..1];
        }
        static Status converterParaApp(string value){
            
            var status = Enum
                .GetValues<Status>()
                .FirstOrDefault(p => p.ToString()[0..1]== value);
            
            return status;
        }
    }
}