using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace ArtifactEnsemble
{

    public class MountainArtifact : NewArtifact<MountainArtifact>
    {

        public override string Name => "Artifact of the Summit";
        public override string NameToken => "SUMMIT";
        public override string Description => "Automatically activates one mountain shrine for every completed loop, plus one.";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.mountain_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.mountain_off, Color.gray);

        protected override void InitManager()
        {
            MountainArtifactManager.Init();
        }
    }

    public static class MountainArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return MountainArtifact.Instance.ArtifactDef; }
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
            On.RoR2.TeleporterInteraction.OnInteractionBegin += activateMountainShrines;
            ArtifactEnsemble.Logger.LogInfo("Artifact of the Summit is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= activateMountainShrines;
            ArtifactEnsemble.Logger.LogInfo("Artifact of the Summit is now disabled.");
        }

        private static void activateMountainShrines(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, RoR2.TeleporterInteraction self, Interactor activator)
        {
            int numberOfMountainShrines = (Run.instance.stageClearCount / Run.stagesPerLoop) + 1;
            for (int i = 0; i < numberOfMountainShrines * ArtifactEnsembleConfig.MountainCount.Value; i ++)
            {
                self.AddShrineStack();
            }
            orig(self, activator);
        }
    }
}
