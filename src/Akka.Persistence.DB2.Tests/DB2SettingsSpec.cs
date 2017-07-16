//-----------------------------------------------------------------------
// <copyright file="DB2SettingsSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Dispatch;
using FluentAssertions;
using Xunit;

namespace Akka.Persistence.DB2.Tests
{
    public class DB2ConfigSpec : Akka.TestKit.Xunit2.TestKit
    {
        [Fact]
        public void Should_DB2_journal_has_default_config()
        {
            DB2Persistence.Get(Sys);
            var config = Sys.Settings.Config.GetConfig("akka.persistence.journal.DB2");

            config.Should().NotBeNull();
            config.GetString("class").Should().Be("Akka.Persistence.DB2.Journal.DB2Journal, Akka.Persistence.DB2");
            config.GetString("plugin-dispatcher").Should().Be(Dispatchers.DefaultDispatcherId);
            config.GetString("connection-string").Should().BeEmpty();
            config.GetString("connection-string-name").Should().BeNullOrEmpty();
            config.GetTimeSpan("connection-timeout").Should().Be(30.Seconds());
            config.GetString("schema-name").Should().Be("dbo");
            config.GetString("table-name").Should().Be("EventJournal");
            config.GetString("metadata-table-name").Should().Be("Metadata");
            config.GetBoolean("auto-initialize").Should().BeFalse();
            config.GetString("timestamp-provider").Should().Be("Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common");
        }

        [Fact]
        public void Should_DB2_snapshot_has_default_config()
        {
            DB2Persistence.Get(Sys);
            var config = Sys.Settings.Config.GetConfig("akka.persistence.snapshot-store.DB2");

            config.Should().NotBeNull();
            config.GetString("class").Should().Be("Akka.Persistence.DB2.Snapshot.DB2SnapshotStore, Akka.Persistence.DB2");
            config.GetString("plugin-dispatcher").Should().Be(Dispatchers.DefaultDispatcherId);
            config.GetString("connection-string").Should().BeEmpty();
            config.GetString("connection-string-name").Should().BeNullOrEmpty();
            config.GetTimeSpan("connection-timeout").Should().Be(30.Seconds());
            config.GetString("schema-name").Should().Be("dbo");
            config.GetString("table-name").Should().Be("SnapshotStore");
            config.GetBoolean("auto-initialize").Should().BeFalse();
        }
    }
}
