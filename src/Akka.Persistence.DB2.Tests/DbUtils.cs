﻿//-----------------------------------------------------------------------
// <copyright file="DbUtils.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using IBM.Data.DB2;

namespace Akka.Persistence.DB2.Tests
{
    public static class DbUtils
    {
        public static void Initialize()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            var connectionBuilder = new DB2ConnectionStringBuilder(connectionString);

            //connect to postgres database to create a new database
            var databaseName = connectionBuilder.DBName;
            connectionString = connectionBuilder.ToString();

            using (var conn = new DB2Connection(connectionString))
            {
                conn.Open();

                using (var cmd = new DB2Command())
                {
                    cmd.CommandText = string.Format(@"
                        IF db_id('{0}') IS NULL
                            BEGIN
                                CREATE DATABASE {0}
                            END
                            
                    ", databaseName);
                    cmd.Connection = conn;

                    var result = cmd.ExecuteScalar();
                }

                DropTables(conn, databaseName);
            }
        }

        public static void Clean()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            var connectionBuilder = new DB2ConnectionStringBuilder(connectionString);
            var databaseName = connectionBuilder.DBName;
            using (var conn = new DB2Connection(connectionString))
            {
                conn.Open();
                DropTables(conn, databaseName);
            }
        }

        private static void DropTables(DB2Connection conn, string databaseName)
        {
            using (var cmd = new DB2Command())
            {
                cmd.CommandText = string.Format(@"
                    USE {0};
                    IF EXISTS (SELECT * FROM SYSIBM.SYSTABLES WHERE SCHEMA = 'dbo' AND TABLE_NAME = 'EventJournal') BEGIN DROP TABLE dbo.EventJournal END;
                    IF EXISTS (SELECT * FROM SYSIBM.SYSTABLES WHERE SCHEMA = 'dbo' AND TABLE_NAME = 'Metadata') BEGIN DROP TABLE dbo.Metadata END;
                    IF EXISTS (SELECT * FROM SYSIBM.SYSTABLES WHERE SCHEMA = 'dbo' AND TABLE_NAME = 'SnapshotStore') BEGIN DROP TABLE dbo.SnapshotStore END;",
                databaseName);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }
    }
}