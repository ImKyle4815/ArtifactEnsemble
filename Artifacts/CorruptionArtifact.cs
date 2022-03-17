using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

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
            if (this.Enabled() && NetworkServer.active && self.waveIndex > -1)
            {
                ArtifactEnsemble.Logger.LogInfo("Spawning wave bonuses");
                foreach (RoR2.PlayerCharacterMasterController player in RoR2.PlayerCharacterMasterController.instances)
                {
                    if (player.master.IsDeadAndOutOfLivesServer()) continue;
                    int waveIndex = self.waveIndex;
                    if (ArtifactEnsembleConfig.SpawnCorruptionLunarPod.Value && waveIndex > 0 && waveIndex % 2 == 0)
                        spawnInteractableNearPoint(player.body.footPosition, Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscLunarChest"));
                    if (ArtifactEnsembleConfig.SpawnCorruptionVoidChest.Value && waveIndex > 0 && waveIndex % 5 == 0)
                        spawnInteractableNearPoint(player.body.footPosition, Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscVoidChest"));
                }
            }
            orig(self);
        }

        private void spawnInteractableNearPoint(Vector3 spawnOrigin, SpawnCard spawnCard, float minDist = -10f, float maxDist = 10f)
        {
            // Director stuff
            DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
            directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
            // Actual spawning
            Vector3 spawnPosition = spawnOrigin;
            Vector3 raycastOrigin = spawnPosition + new Vector3(ArtifactEnsemble.rng.RangeFloat(minDist, maxDist), 3f, ArtifactEnsemble.rng.RangeFloat(minDist, maxDist));
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(raycastOrigin, Vector3.down), out raycastHit, float.MaxValue, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                spawnPosition = new Vector3?(raycastHit.point) ?? spawnPosition;
            }
            GameObject spawnedInstance = spawnCard.DoSpawn(
                spawnPosition,
                Quaternion.identity,
                new DirectorSpawnRequest(spawnCard, directorPlacementRule, Run.instance.runRNG)
            ).spawnedInstance;
            NetworkServer.Spawn(spawnedInstance);
        }
    }
}
