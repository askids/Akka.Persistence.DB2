//-----------------------------------------------------------------------
// <copyright file="DB2SnapshotStoreSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using Akka.Configuration;
using Akka.Persistence.TestKit.Snapshot;
using Akka.TestKit;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.DB2.Tests
{
    [Collection("DB2Spec")]
    public class DB2SnapshotStoreSpec : SnapshotStoreSpec
    {
        private static readonly Config SpecConfig;

        static DB2SnapshotStoreSpec()
        {
            var specString = @"
                        akka.persistence {
                            publish-plugin-commands = on
                            snapshot-store {
                                plugin = ""akka.persistence.snapshot-store.DB2""
                                DB2 {
                                    class = ""Akka.Persistence.DB2.Snapshot.DB2SnapshotStore, Akka.Persistence.DB2""
                                    plugin-dispatcher = ""akka.actor.default-dispatcher""
                                    table-name = SnapshotStore
                                    schema-name = dbo
                                    auto-initialize = on
                                    connection-string-name = ""TestDb""
                                }
                            }
                        }";

            SpecConfig = ConfigurationFactory.ParseString(specString);


            //need to make sure db is created before the tests start
            DbUtils.Initialize();
        }

        public DB2SnapshotStoreSpec(ITestOutputHelper output)
            : base(SpecConfig, "DB2SnapshotStoreSpec", output)
        {
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DbUtils.Clean();
        }
    }
}