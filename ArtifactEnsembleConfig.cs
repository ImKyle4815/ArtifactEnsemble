using BepInEx.Configuration;

namespace ArtifactEnsemble
{
    public static class ArtifactEnsembleConfig
    {
        public static ConfigEntry<bool> UseBazaarArtifact { get; set; }
        public static ConfigEntry<bool> SpawnBazaarScrapper { get; set; }
        public static ConfigEntry<bool> SpawnBazaarCleansingPool { get; set; }
        public static ConfigEntry<bool> SpawnBazaarVoidChest { get; set; }

        public static ConfigEntry<bool> UseCorruptionArtifact { get; set; }
        public static ConfigEntry<bool> SpawnCorruptionLunarPod { get; set; }
        public static ConfigEntry<bool> SpawnCorruptionVoidChest { get; set; }

        public static ConfigEntry<bool> UseFortuneArtifact { get; set; }

        public static ConfigEntry<bool> UseGreedArtifact { get; set; }

        public static ConfigEntry<bool> UseMountainArtifact { get; set; }
        public static ConfigEntry<int> MountainCount { get; set; }

        public static ConfigEntry<bool> UseReanimationArtifact { get; set; }

        public static ConfigEntry<bool> UseTradeArtifact { get; set; }

        public static void Init(ConfigFile config)
        {
            UseBazaarArtifact = config.Bind<bool>("Artifact of Bazaar", "Enable artifact", true);
            SpawnBazaarScrapper = config.Bind<bool>("Artifact of Bazaar", "Spawn Scrapper", true);
            SpawnBazaarCleansingPool = config.Bind<bool>("Artifact of Bazaar", "Spawn Cleansing Pool", true);
            SpawnBazaarVoidChest = config.Bind<bool>("Artifact of Bazaar", "Spawn Void Chest", true);

            UseCorruptionArtifact = config.Bind<bool>("Artifact of Corruption", "Enable artifact", true);
            SpawnCorruptionLunarPod = config.Bind<bool>("Artifact of Corruption", "Spawn Lunar Pod", true);
            SpawnCorruptionVoidChest = config.Bind<bool>("Artifact of Corruption", "Spawn Void Chest", true);

            UseFortuneArtifact = config.Bind<bool>("Artifact of Fortune", "Enable artifact", true);

            UseGreedArtifact = config.Bind<bool>("Artifact of Greed", "Enable artifact", true);

            UseMountainArtifact = config.Bind<bool>("Artifact of Summit", "Enable artifact", true);
            MountainCount = config.Bind<int>("Artifact of Summit", "Mountain Shrine Count", 1);

            UseReanimationArtifact = config.Bind<bool>("Artifact of Reanimation", "Enable artifact", true);

            UseTradeArtifact = config.Bind<bool>("Artifact of Trade", "Enable artifact", true);

            ArtifactEnsemble.Logger.LogInfo("Loaded config for ImKyle4815's Artifact Ensemble");
        }
    }
}