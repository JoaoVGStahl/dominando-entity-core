using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace src
{
    public class MySqlServerQuerySqlGenerator : SqlServerQuerySqlGenerator
    {
        
        public MySqlServerQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) : base(dependencies)
        {

        }

        protected override Expression VisitTable(TableExpression tableExpression)
        {
            var table = base.VisitTable(tableExpression);
            Sql.Append(" WITH (NOLOCK)");

            return table;
        }
    }

    public class MySqlServerQueryGeneratorFactory : SqlServerQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        public MySqlServerQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies) : base(dependencies)
        {
            _dependencies = dependencies;
        }

        public override QuerySqlGenerator Create()
        {
            return new MySqlServerQuerySqlGenerator(_dependencies);
        }
    }
}