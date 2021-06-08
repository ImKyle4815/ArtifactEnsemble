using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class BazaarArtifact : NewArtifact<BazaarArtifact>
    {

        public override string Name => "Artifact of Bazaar";
        public override string NameToken => "BAZAAR";
        public override string Description => "Adds a cleansing pool and a scrapper to the space bazaar";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.bazaar_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.bazaar_off, Color.gray);

        protected override void InitManager()
        {
            BazaarArtifactManager.Init();
        }
    }

    public static class BazaarArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return BazaarArtifact.Instance.ArtifactDef; }
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
            On.RoR2.BazaarController.Awake += SpawnBazaarExtras;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Bazaar is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            On.RoR2.BazaarController.Awake -= SpawnBazaarExtras;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Bazaar is now disabled.");
        }

        private static void SpawnBazaarExtras(On.RoR2.BazaarController.orig_Awake orig, BazaarController self)
        {
            orig(self);
            if (NetworkServer.active)
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
