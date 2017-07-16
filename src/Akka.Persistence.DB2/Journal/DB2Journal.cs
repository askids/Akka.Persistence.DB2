//-----------------------------------------------------------------------
// <copyright file="DB2SJournal.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Data.Common;
using IBM.Data.DB2;
using Akka.Configuration;
using Akka.Persistence.Sql.Common.Journal;

namespace Akka.Persistence.DB2.Journal
{
    public class DB2Journal : SqlJournal
    {
        public static readonly DB2Persistence Extension = DB2Persistence.Get(Context.System);
        public DB2Journal(Config journalConfig) : base(journalConfig)
        {
            var config = journalConfig.WithFallback(Extension.DefaultJournalConfig);
            QueryExecutor = new DB2QueryExecutor(new QueryConfiguration(
                schemaName: config.GetString("schema-name"),
                journalEventsTableName: config.GetString("table-name"),
                metaTableName: config.GetString("metadata-table-name"),
                persistenceIdColumnName: "PersistenceId",
                sequenceNrColumnName: "SequenceNr",
                payloadColumnName: "Payload",
                manifestColumnName: "Manifest",
                timestampColumnName: "Timestamp",
                isDeletedColumnName: "IsDeleted",
                tagsColumnName: "Tags",
                orderingColumnName: "Ordering",
                timeout: config.GetTimeSpan("connection-timeout")),
                    Context.System.Serialization,
                    GetTimestampProvider(config.GetString("timestamp-provider")));
        }

        protected override DbConnection CreateDbConnection(string connectionString) => new DB2Connection(connectionString);

        protected override string JournalConfigPath => DB2JournalSettings.ConfigPath;

        public override IJournalQueryExecutor QueryExecutor { get; }
    }
}