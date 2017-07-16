//-----------------------------------------------------------------------
// <copyright file="Extension.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Sql.Common;

namespace Akka.Persistence.DB2
{
    public class DB2JournalSettings : JournalSettings
    {
        public const string ConfigPath = "akka.persistence.journal.DB2";

        public DB2JournalSettings(Config config) : base(config)
        {
        }
    }

    public class DB2SnapshotSettings : SnapshotStoreSettings
    {
        public const string ConfigPath = "akka.persistence.snapshot-store.DB2";

        public DB2SnapshotSettings(Config config) : base(config)
        {
        }
    }

    /// <summary>
    /// An actor system extension initializing support for DB2 persistence layer.
    /// </summary>
    public class DB2Persistence : IExtension
    {
        /// <summary>
        /// Returns a default configuration for akka persistence SQLite-based journals and snapshot stores.
        /// </summary>
        /// <returns></returns>
        public static Config DefaultConfiguration()
        {
            return ConfigurationFactory.FromResource<DB2Persistence>("Akka.Persistence.DB2.DB2.conf");
        }

        public static DB2Persistence Get(ActorSystem system)
        {
            return system.WithExtension<DB2Persistence, DB2PersistenceProvider>();
        }

        /// <summary>
        /// Journal-related settings loaded from HOCON configuration.
        /// </summary>
        public readonly Config DefaultJournalConfig;

        /// <summary>
        /// Snapshot store related settings loaded from HOCON configuration.
        /// </summary>
        public readonly Config DefaultSnapshotConfig;

        public DB2Persistence(ExtendedActorSystem system)
        {
            var defaultConfig = DefaultConfiguration();
            system.Settings.InjectTopLevelFallback(defaultConfig);

            DefaultJournalConfig = defaultConfig.GetConfig(DB2JournalSettings.ConfigPath);
            DefaultSnapshotConfig = defaultConfig.GetConfig(DB2SnapshotSettings.ConfigPath);
        }
    }

    /// <summary>
    /// Singleton class used to setup SQL Server backend for akka persistence plugin.
    /// </summary>
    public class DB2PersistenceProvider : ExtensionIdProvider<DB2Persistence>
    {
        public override DB2Persistence CreateExtension(ExtendedActorSystem system)
        {
            return new DB2Persistence(system);
        }
    }
}