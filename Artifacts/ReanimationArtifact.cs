using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Utils;

namespace ArtifactEnsemble
{

    public class ReanimationArtifact : NewArtifact<ReanimationArtifact>
    {

        public override string Name => "Artifact of Reanimation";
        public override string NameToken => "REANIMATION";
        public override string Description => "Reanimates dead players at the beginning of the teleporter event";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.reanimation_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.reanimation_off, Color.gray);

        protected override void InitManager()
        {
            ReanimationArtifactManager.Init();
        }
    }

    public static class ReanimationArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return ReanimationArtifact.Instance.ArtifactDef; }
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
            On.RoR2.TeleporterInteraction.OnInteractionBegin += ReanimatePlayersOnTP;
            On.EntityStates.Missions.BrotherEncounter.PreEncounter.OnEnter += ReanimatePlayersOnMithrix;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= ReanimatePlayersOnTP;
            On.EntityStates.Missions.BrotherEncounter.PreEncounter.OnEnter -= ReanimatePlayersOnMithrix;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now disabled.");
        }

        private static void ReanimatePlayersOnTP(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            orig(self, activator);
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
            ReanimateEachPlayer();
        }

        private static void ReanimatePlayersOnMithrix(On.EntityStates.Missions.BrotherEncounter.PreEncounter.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.PreEncounter self)
        {
            orig(self);
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
            ReanimateEachPlayer();
        }

        private static void ReanimateEachPlayer()
        {
            bool flag = (!RoR2Application.isInSinglePlayer || NetworkServer.active) && ArtifactEnsemble.reanimationArtifact.ArtifactEnabled;
            if (flag)
            {
                foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
                {
                    bool isActiveAndEnabled = networkUser.isActiveAndEnabled;
                    if (isActiveAndEnabled)
                    {
                        bool flag2 = !networkUser.master.GetBody() || networkUser.master.IsDeadAndOutOfLivesServer() || !networkUser.master.GetBody().healthComponent.alive;
                        if (flag2)
                        {
                            Vector3 fieldValue = Reflection.GetFieldValue<Vector3>(networkUser.master, "deathFootPosition");
                            Quaternion rotation = networkUser.master.transform.rotation;
                            networkUser.master.Respawn(fieldValue, rotation);
                        }
                    }
                }
            }
        }
    }
}
