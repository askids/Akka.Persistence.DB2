//-----------------------------------------------------------------------
// <copyright file="DB2AllPersistenceIdsSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration;
using Akka.Persistence.Query.Sql;
using Akka.Persistence.Sql.TestKit;
using Akka.Util.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.DB2.Tests.Query
{
    [Collection("DB2Spec")]
    public class DB2AllPersistenceIdsSpec : AllPersistenceIdsSpec
    {
        public static Config Config => ConfigurationFactory.ParseString($@"
            akka.loglevel = INFO
            akka.test.single-expect-default = 10s
            akka.persistence.journal.plugin = ""akka.persistence.journal.DB2""
            akka.persistence.journal.DB2 {{
                class = ""Akka.Persistence.DB2.Journal.DB2Journal, Akka.Persistence.DB2""
                plugin-dispatcher = ""akka.actor.default-dispatcher""
                table-name = EventJournal
                schema-name = dbo
                auto-initialize = on
                connection-string-name = ""TestDb""
                refresh-interval = 1s
            }}")
            .WithFallback(SqlReadJournal.DefaultConfiguration());

        public DB2AllPersistenceIdsSpec(ITestOutputHelper output) : base(Config, output)
        {
            DbUtils.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DbUtils.Clean();
        }
    }
}