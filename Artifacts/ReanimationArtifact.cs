using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Utils;

namespace ArtifactEnsemble.Artifacts
{
    public class ReanimationArtifact : ArtifactTemplate
    {
        public ReanimationArtifact()
        {
            Init("Reanimation", "Reanimates dead players at the beginning of the teleporter event.", Properties.Resources.reanimation_on, Properties.Resources.reanimation_off);
            On.RoR2.TeleporterInteraction.OnInteractionBegin += ReanimatePlayersOnTP;
            On.EntityStates.Missions.BrotherEncounter.PreEncounter.OnEnter += ReanimatePlayersOnMithrix;
        }

        private void ReanimatePlayersOnTP(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            orig(self, activator);
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
            ReanimateEachPlayer();
        }

        private void ReanimatePlayersOnMithrix(On.EntityStates.Missions.BrotherEncounter.PreEncounter.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.PreEncounter self)
        {
            orig(self);
            ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
            ReanimateEachPlayer();
        }

        private void ReanimateEachPlayer()
        {
            bool flag = (!RoR2Application.isInSinglePlayer || NetworkServer.active);//&& ArtifactEnsemble.reanimationArtifact.ArtifactEnabled;
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
