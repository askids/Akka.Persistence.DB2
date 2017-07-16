//-----------------------------------------------------------------------
// <copyright file="DB2Executor.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Data.Common;
using IBM.Data.DB2;
using Akka.Persistence.Sql.Common.Journal;

namespace Akka.Persistence.DB2.Journal
{
    public class DB2QueryExecutor : AbstractQueryExecutor
    {
        public DB2QueryExecutor(QueryConfiguration configuration, Akka.Serialization.Serialization serialization, ITimestampProvider timestampProvider)
            : base(configuration, serialization, timestampProvider)
        {
            ByTagSql = $@"
            SELECT 
            e.{Configuration.PersistenceIdColumnName} as PersistenceId, 
            e.{Configuration.SequenceNrColumnName} as SequenceNr, 
            e.{Configuration.TimestampColumnName} as Timestamp, 
            e.{Configuration.IsDeletedColumnName} as IsDeleted, 
            e.{Configuration.ManifestColumnName} as Manifest, 
            e.{Configuration.PayloadColumnName} as Payload,
            e.{Configuration.OrderingColumnName} as Ordering
            FROM {Configuration.FullJournalTableName} e
            WHERE e.{Configuration.OrderingColumnName} > @Ordering AND e.{Configuration.TagsColumnName} LIKE @Tag
            ORDER BY {Configuration.OrderingColumnName} ASC
            FETCH FIRST (@Take) ROWS ONLY
            ";

            CreateEventsJournalSql = $@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{configuration.SchemaName}' AND TABLE_NAME = '{configuration.JournalEventsTableName}')
            BEGIN
                CREATE TABLE {configuration.FullJournalTableName} (
                    {configuration.OrderingColumnName} BIGINT IDENTITY(1,1) NOT NULL,
	                  {configuration.PersistenceIdColumnName} NVARCHAR(255) NOT NULL,
	                  {configuration.SequenceNrColumnName} BIGINT NOT NULL,
                    {configuration.TimestampColumnName} BIGINT NOT NULL,
                    {configuration.IsDeletedColumnName} BIT NOT NULL,
                    {configuration.ManifestColumnName} NVARCHAR(500) NOT NULL,
	                  {configuration.PayloadColumnName} VARBINARY(MAX) NOT NULL,
                    {configuration.TagsColumnName} NVARCHAR(100) NULL,
                    CONSTRAINT PK_{configuration.JournalEventsTableName} PRIMARY KEY ({configuration.OrderingColumnName}),
                    CONSTRAINT UQ_{configuration.JournalEventsTableName} UNIQUE ({configuration.PersistenceIdColumnName}, {configuration.SequenceNrColumnName})
                );
                CREATE INDEX IX_{configuration.JournalEventsTableName}_{configuration.SequenceNrColumnName} ON {configuration.FullJournalTableName}({configuration.SequenceNrColumnName});
                CREATE INDEX IX_{configuration.JournalEventsTableName}_{configuration.TimestampColumnName} ON {configuration.FullJournalTableName}({configuration.TimestampColumnName});
            END
            ";
            CreateMetaTableSql = $@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{configuration.SchemaName}' AND TABLE_NAME = '{configuration.MetaTableName}')
            BEGIN
                CREATE TABLE {configuration.FullMetaTableName} (
	                {configuration.PersistenceIdColumnName} NVARCHAR(255) NOT NULL,
	                {configuration.SequenceNrColumnName} BIGINT NOT NULL,
                    CONSTRAINT PK_{configuration.MetaTableName} PRIMARY KEY ({configuration.PersistenceIdColumnName}, {configuration.SequenceNrColumnName})
                );
            END
            ";
        }

        protected override DbCommand CreateCommand(DbConnection connection) => new DB2Command { Connection = (DB2Connection)connection };

        protected override string ByTagSql { get; }
        protected override string CreateEventsJournalSql { get; }
        protected override string CreateMetaTableSql { get; }
    }
}
