//-----------------------------------------------------------------------
// <copyright file="InternalExtensions.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using IBM.Data.DB2;

namespace Akka.Persistence.SqlServer
{
    internal static class InternalExtensions
    {
        public static string QuoteSchemaAndTable(this string sqlQuery, string schemaName, string tableName)
        {
            var cb = new DB2CommandBuilder();
            return string.Format(sqlQuery, cb.QuoteIdentifier(schemaName), cb.QuoteIdentifier(tableName));
        }
    }
}