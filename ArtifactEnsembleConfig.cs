using BepInEx.Configuration;

namespace ArtifactEnsemble
{
    public static class ArtifactEnsembleConfig
    {
        public static ConfigEntry<int> MountainCount { get; set; }

        public static void Init(ConfigFile config)
        {
            ArtifactEnsemble.Logger.LogInfo("Loaded config for ImKyle4815's Artifact Ensemble");
            MountainCount = config.Bind<int>("Artifact of the Summit", "Mountain Shrine Count", 1);
        }
    }
}