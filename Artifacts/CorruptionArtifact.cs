using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static ArtifactEnsemble.ArtifactEnsemble;

namespace ArtifactEnsemble.Artifacts
{
    public class CorruptionArtifact : ArtifactTemplate
    {
        public CorruptionArtifact()
        {
            Init("Corruption", "Spawns a Lunar Pod after waves 2, 4, 8, and 10, and a Void Chest every 5 waves for each player. (Simulacrum only)", Properties.Resources.eye_on, Properties.Resources.eye_off);
            On.RoR2.InfiniteTowerRun.AdvanceWave += SpawnWaveBonuses;
        }
        
        private void SpawnWaveBonuses(On.RoR2.InfiniteTowerRun.orig_AdvanceWave orig, InfiniteTowerRun self)
        {
            var waveIndex = self.waveIndex;
            if (this.Enabled() && NetworkServer.active && waveIndex is > -1 and > 0)
            {
                ArtifactEnsemble.Logger.LogInfo("Spawning wave bonuses");
                foreach (var player in RoR2.PlayerCharacterMasterController.instances)
                {
                    if (player.master.IsDeadAndOutOfLivesServer()) continue;
                    if (ArtifactEnsembleConfig.SpawnCorruptionLunarPod.Value && waveIndex % 2 == 0)
                        TrySpawn("SpawnCards/InteractableSpawnCard/iscLunarChest",player.body.footPosition, Vector3.zero, DirectorPlacementRule.PlacementMode.Approximate, true);
                    if (ArtifactEnsembleConfig.SpawnCorruptionVoidChest.Value && waveIndex % 5 == 0)
                        TrySpawn("SpawnCards/InteractableSpawnCard/iscVoidChest",player.body.footPosition, Vector3.zero, DirectorPlacementRule.PlacementMode.Approximate, true);
                }
            }
            orig(self);
        }
    }
}
