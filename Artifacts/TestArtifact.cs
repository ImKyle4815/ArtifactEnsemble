using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class TestArtifact : NewArtifact<TestArtifact>
    {

        public override string Name => "Artifact of Test";
        public override string NameToken => "TEST";
        public override string Description => "Just for testing purposes. Just logs a message, I guess.";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.test_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.test_off, Color.gray);

        protected override void InitManager()
        {
            TestArtifactManager.Init();
        }
    }

    public static class TestArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return TestArtifact.Instance.ArtifactDef; }
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
            Run.onRunStartGlobal += alertArtifactTest;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Test is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            Run.onRunStartGlobal -= alertArtifactTest;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Test is now disabled.");
        }

        private static void alertArtifactTest(Run run)
        {
            ArtifactEnsemble.Logger.LogWarning("Artifact of Test is enabled!!!");
        }
    }
}
