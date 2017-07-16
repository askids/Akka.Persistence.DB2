## Akka.Persistence.DB2
Akka's persistance Journal and snapshot sore backed by DB2 database

**WARNING: Akka.Persistence.DB2 plugin is still in alpha and it's mechanics described bellow may be still subject to change**.

### Configuration

Both journal and snapshot store share the same configuration keys (however they resides in separate scopes, so they are definied distinctly for either journal or snapshot store):

Remember that connection string must be provided separately to Journal and Snapshot Store.

Unlike other persistent stores, its assumed that table/index creation will be done outside this package. DDL has been provided.

Keep the column names same. But you are free to change table/schema name.

```hocon
akka.persistence{
	journal {
		DB2 {
			# qualified type name of the SQL Server persistence journal actor
			class = "Akka.Persistence.SqlServer.Journal.DB2Journal, Akka.Persistence.DB2"

			# dispatcher used to drive journal actor
			plugin-dispatcher = "akka.actor.default-dispatcher"

			# connection string used for database access
			connection-string = ""

			# default SQL commands timeout
			connection-timeout = 30s

			# DB2 schema name to table corresponding with persistent journal
			schema-name = ABC

			# DB2 table corresponding with persistent journal
			table-name = EventJournal

			# should corresponding journal table be initialized automatically
			auto-initialize = off

			# timestamp provider used for generation of journal entries timestamps
			timestamp-provider = "Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common"

			# metadata table
			metadata-table-name = Metadata
		}
	}

	snapshot-store {
		DB2 {

			# qualified type name of the DB2 persistence journal actor
			class = "Akka.Persistence.SqlServer.Snapshot.DB2SnapshotStore, Akka.Persistence.DB2"

			# dispatcher used to drive journal actor
			plugin-dispatcher = ""akka.actor.default-dispatcher""

			# connection string used for database access
			connection-string = ""

			# default SQL commands timeout
			connection-timeout = 30s

			# DB2 schema name to table corresponding with persistent journal
			schema-name = ABC

			# DB2 table corresponding with persistent journal
			table-name = SnapshotStore

			# should corresponding journal table be initialized automatically
			auto-initialize = off
		}
	}
}
```
### Table Schema

DB2 persistence plugin will NOT define a default table schema used for journal, snapshot store and metadata table. It needs to be created manually.

```SQL
CREATE TABLE {your_journal_table_name} (
  Ordering BIGINT AS IDENTITY START WITH 1 NOT NULL,
  PersistenceID VARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp BIGINT NOT NULL,
  IsDeleted BIT NOT NULL,
  Manifest VARCHAR(500) NOT NULL,
  Payload VARBINARY(32704) NOT NULL,
  Tags VARCHAR(100) NULL
	CONSTRAINT PK_{your_journal_table_name} PRIMARY KEY (Ordering),
  CONSTRAINT QU_{your_journal_table_name} UNIQUE (PersistenceID, SequenceNr)
);

CREATE TABLE {your_snapshot_table_name} (
  PersistenceID VARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp DATETIME2 NOT NULL,
  Manifest VARCHAR(500) NOT NULL,
  Snapshot VARBINARY(32704) NOT NULL
  CONSTRAINT PK_{your_snapshot_table_name} PRIMARY KEY (PersistenceID, SequenceNr)
);

CREATE TABLE {your_metadata_table_name} (
  PersistenceID VARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  CONSTRAINT PK_{your_metadata_table_name} PRIMARY KEY (PersistenceID, SequenceNr)
);
```
Underneath Akka.Persistence.DB2 uses a raw ADO.NET commands. You may choose not to use a dedicated built in ones, but to create your own being better fit for your use case. To do so, you have to create your own versions of `IJournalQueryBuilder` and `IJournalQueryMapper` (for custom journals) or `ISnapshotQueryBuilder` and `ISnapshotQueryMapper` (for custom snapshot store) and then attach inside journal, just like in the example below:

```C#
class MyCustomSqlServerJournal: Akka.Persistence.SqlServer.Journal.SqlServerJournal
{
    public MyCustomSqlServerJournal() : base()
    {
        QueryBuilder = new MyCustomJournalQueryBuilder();
        QueryMapper = new MyCustomJournalQueryMapper();
    }
}
```

### Tests

The DB2 tests are packaged and run as part of the default "All" build task.

In order to run the tests, you must do the following things:

1. Download and install IBM DS Driver v10.5 FP7 from: http://www-01.ibm.com/support/docview.wss?uid=swg24041453
2. Higher version will also work.
3. Create the table and indexes using above DDL. Unlike other persistant store, its assumed that table creation is outside the scope of this package.
4. The default connection string uses the following credentials: "Data Source=DSNT;Database=DSNT;User Id=XXXXXXXX;Password=YYYYYYYYY;"
5. A custom app.config file can be used and needs to be placed in the same folder as the dll

