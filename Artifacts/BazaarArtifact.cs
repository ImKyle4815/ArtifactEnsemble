using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble.Artifacts
{

    public class BazaarArtifact : ArtifactTemplate
    {
        public BazaarArtifact()
        {
            Init("Bazaar", "Adds a cleansing pool and a scrapper to the space bazaar.", Properties.Resources.bazaar_on, Properties.Resources.bazaar_off);
            On.RoR2.BazaarController.Awake += SpawnBazaarExtras;
        }

        private void SpawnBazaarExtras(On.RoR2.BazaarController.orig_Awake orig, BazaarController self)
        {
            orig(self);
            if (this.Enabled())
            {
                ArtifactEnsemble.Logger.LogInfo("Spawning cleansing pool and scrapper.");
                // Director stuff
                DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
                directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
                // Actual spawning
                // Cleansing Pool
                SpawnCard spawnCardCleansingPool = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineCleanse");
                GameObject spawnedInstanceCleansingPool = spawnCardCleansingPool.DoSpawn(new Vector3(-65.7f, -23.5f, -18.9f), Quaternion.identity, new DirectorSpawnRequest(spawnCardCleansingPool, directorPlacementRule, Run.instance.runRNG)).spawnedInstance;
                NetworkServer.Spawn(spawnedInstanceCleansingPool);
                // Scrapper
                SpawnCard spawnCardScrapper = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscScrapper");
                GameObject spawnedInstanceScrapper = spawnCardScrapper.DoSpawn(new Vector3(-82.1f, -23.7f, -5.2f), Quaternion.identity, new DirectorSpawnRequest(spawnCardScrapper, directorPlacementRule, Run.instance.runRNG)).spawnedInstance;
                spawnedInstanceScrapper.transform.eulerAngles = new Vector3(0f, 72.6f, 0f);
                NetworkServer.Spawn(spawnedInstanceScrapper);
            }
        }
    }
}
