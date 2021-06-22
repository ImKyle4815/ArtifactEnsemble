using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class RandomArtifact : NewArtifact<RandomArtifact>
    {

        public override string Name => "Artifact of Random";
        public override string NameToken => "RANDOM";
        public override string Description => "Randomizes the artifacts EVERY STAGE";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.test_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.test_off, Color.gray);

        protected override void InitManager()
        {
            RandomArtifactManager.Init();
        }
    }

    public static class RandomArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return RandomArtifact.Instance.ArtifactDef; }
        }

        public static void Init()
        {
            RunArtifactManager.onArtifactEnabledGlobal += OnArtifactEnabled;
            RunArtifactManager.onArtifactDisabledGlobal += OnArtifactDisabled;
        }

        private static void OnArtifactEnabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (!NetworkServer.active || artifactDef != myArtifact)
            {
                return;
            }

            // do things
            On.RoR2.Run.BeginStage += RandomizeArtifacts;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Random is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            On.RoR2.Run.BeginStage -= RandomizeArtifacts;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Random is now disabled.");
        }

        private static void RandomizeArtifacts(On.RoR2.Run.orig_BeginStage orig, RoR2.Run self)
        {
            foreach (ArtifactDef artifactDef in ArtifactCatalog.artifactDefs)
            {
                if (artifactDef.cachedName != "RANDOM")
                {
                    bool toggledState = UnityEngine.Random.Range(0f, 1f) > 0.5f;
                    ArtifactEnsemble.Logger.LogWarning(artifactDef.cachedName);
                    RunArtifactManager.instance.SetArtifactEnabledServer(artifactDef, toggledState);
                }
            }
            orig(self);
        }
    }
}
