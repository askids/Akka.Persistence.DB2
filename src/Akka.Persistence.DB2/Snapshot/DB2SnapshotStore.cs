//-----------------------------------------------------------------------
// <copyright file="DB2SnapshotStore.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Data.Common;
using IBM.Data.DB2;
using Akka.Configuration;
using Akka.Persistence.Sql.Common.Snapshot;

namespace Akka.Persistence.DB2.Snapshot
{
    public class DB2SnapshotStore : SqlSnapshotStore
    {
        protected readonly DB2Persistence Extension = DB2Persistence.Get(Context.System);
        public DB2SnapshotStore(Config config) : base(config)
        {
            var sqlConfig = config.WithFallback(Extension.DefaultSnapshotConfig);
            QueryExecutor = new DB2QueryExecutor(new QueryConfiguration(
                schemaName: config.GetString("schema-name"),
                snapshotTableName: config.GetString("table-name"),
                persistenceIdColumnName: "PersistenceId",
                sequenceNrColumnName: "SequenceNr",
                payloadColumnName: "Snapshot",
                manifestColumnName: "Manifest",
                timestampColumnName: "Timestamp",
                timeout: sqlConfig.GetTimeSpan("connection-timeout")),
                Context.System.Serialization);
        }

        protected override DbConnection CreateDbConnection(string connectionString) => new DB2Connection(connectionString);

        public override ISnapshotQueryExecutor QueryExecutor { get; }
    }
}