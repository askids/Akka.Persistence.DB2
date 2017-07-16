//-----------------------------------------------------------------------
// <copyright file="DB2JournalSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2017-2017 NA <http://www.NA.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.TestKit.Journal;
using Akka.TestKit;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.DB2.Tests
{
    [Collection("DB2Spec")]
    public class DB2JournalSpec : JournalSpec
    {
        private static readonly Config SpecConfig;

        static DB2JournalSpec()
        {
            var specString = @"
                    akka.persistence {
                        publish-plugin-commands = on
                        journal {
                            plugin = ""akka.persistence.journal.DB2""
                            DB2 {
                                class = ""Akka.Persistence.DB2.Journal.DB2Journal, Akka.Persistence.DB2""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = EventJournal
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

        public DB2JournalSpec(ITestOutputHelper output)
            : base(SpecConfig, "DB2JournalSpec", output)
        {
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DbUtils.Clean();
        }

        [Fact]
        public void Journal_should_not_reset_HighestSequenceNr_after_journal_cleanup()
        {
            TestProbe _receiverProbe = CreateTestProbe();
            Journal.Tell(new ReplayMessages(0, long.MaxValue, long.MaxValue, Pid, _receiverProbe.Ref));
            for (int i = 1; i <= 5; i++) _receiverProbe.ExpectMsg<ReplayedMessage>(m => IsReplayedMessage(m, i));
            _receiverProbe.ExpectMsg<RecoverySuccess>(m => m.HighestSequenceNr == 5L);

            Journal.Tell(new DeleteMessagesTo(Pid, long.MaxValue, _receiverProbe.Ref));
            _receiverProbe.ExpectMsg<DeleteMessagesSuccess>(m => m.ToSequenceNr == long.MaxValue);

            Journal.Tell(new ReplayMessages(0, long.MaxValue, long.MaxValue, Pid, _receiverProbe.Ref));
            _receiverProbe.ExpectMsg<RecoverySuccess>(m => m.HighestSequenceNr == 5L);
        }
    }
}