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
            if (this.Enabled())
            {
                ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
                ReanimateEachPlayer();
            }
        }

        private void ReanimatePlayersOnMithrix(On.EntityStates.Missions.BrotherEncounter.PreEncounter.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.PreEncounter self)
        {
            orig(self);
            if (this.Enabled())
            {
                ArtifactEnsemble.Logger.LogInfo("Artifact of Reanimation is now reviving players...");
                ReanimateEachPlayer();
            }
        }

        private void ReanimateEachPlayer()
        {
            if (RoR2Application.isInSinglePlayer || !NetworkServer.active) return;
            foreach (RoR2.PlayerCharacterMasterController player in RoR2.PlayerCharacterMasterController.instances)
            {
                if (player.master.IsDeadAndOutOfLivesServer())
                {
                    player.master.RespawnExtraLife();
                }
            }    
        }
    }
}
