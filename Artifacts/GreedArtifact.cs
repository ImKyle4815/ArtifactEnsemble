using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class GreedArtifact : NewArtifact<GreedArtifact>
    {

        public override string Name => "Artifact of Greed";
        public override string NameToken => "Greed";
        public override string Description => "Removes the one-scavenger-bag-per-stage limit";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.greed_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.greed_off, Color.gray);

        protected override void InitManager()
        {
            GreedArtifactManager.Init();
        }
    }

    public static class GreedArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return GreedArtifact.Instance.ArtifactDef; }
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
            ArtifactEnsemble.Logger.LogInfo("Artifact of Greed is now enabled.");
            On.EntityStates.ScavMonster.Death.OnPreDestroyBodyServer += resetScavDeath;
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            ArtifactEnsemble.Logger.LogInfo("Artifact of Greed is now disabled.");
            On.EntityStates.ScavMonster.Death.OnPreDestroyBodyServer -= resetScavDeath;
        }

        private static void resetScavDeath(On.EntityStates.ScavMonster.Death.orig_OnPreDestroyBodyServer orig, EntityStates.ScavMonster.Death self)
        {
            self.shouldDropPack = true;
            orig(self);
        }
    }
}
