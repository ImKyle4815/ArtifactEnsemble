using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using R2API.Networking;
using System.Linq;
using System.Reflection;
using UnityEngine;
using RoR2;
using ArtifactEnsemble.Artifacts;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{
    //Lists Plugin MetaData
    [BepInPlugin("ImKyle4815.ArtifactEnsemble", "ArtifactEnsemble", "2.1.0")]
    //Declare submodule dependencies
    [BepInDependency("com.bepis.r2api")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(LanguageAPI), nameof(CommandHelper))]

    public class ArtifactEnsemble : BaseUnityPlugin
	{
        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }
        internal static Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong) DateTime.Now.Ticks);

        public void Awake()
        {
            //Init the logger for the mod
            ArtifactEnsemble.Logger = base.Logger;

            //Load the config
            ArtifactEnsembleConfig.Init(base.Config);

            //Tells R2API to scan the mod for console commands
            //CommandHelper.AddToConsoleWhenReady();

            //Initialize artifacts
            if (ArtifactEnsembleConfig.UseBazaarArtifact.Value) new BazaarArtifact();
            if (ArtifactEnsembleConfig.UseCorruptionArtifact.Value) new CorruptionArtifact();
            if (ArtifactEnsembleConfig.UseFortuneArtifact.Value) new FortuneArtifact();
            if (ArtifactEnsembleConfig.UseGreedArtifact.Value) new GreedArtifact();
            //if (ArtifactEnsembleConfig.UseBazaarArtifact.Value) new HasteArtifact();
            if (ArtifactEnsembleConfig.UseMountainArtifact.Value) new MountainArtifact();
            if (ArtifactEnsembleConfig.UseReanimationArtifact.Value) new ReanimationArtifact();
            if (ArtifactEnsembleConfig.UseTradeArtifact.Value) new TradeArtifact();

            //Disables the space bazaar's kickout feature
            On.EntityStates.NewtMonster.KickFromShop.OnEnter += (orig, self) => { };
        }

        private void ConfigInit()
        {

        }

        public static void TrySpawn(string path, Vector3 pos, Vector3 ang, DirectorPlacementRule.PlacementMode placeMode = DirectorPlacementRule.PlacementMode.Direct)
        {
            // Director stuff
            var directorPlacementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct
            };
            
            var spawnCard = Resources.Load<InteractableSpawnCard>(path);
            if (!spawnCard)
            {
                ArtifactEnsemble.Logger.LogWarning($"Spawner failed to load spawn card with path \"{path}\"");
                return;
            }
            
            spawnCard.skipSpawnWhenSacrificeArtifactEnabled = false;// Override since we want this to show here
            var spawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, Run.instance.runRNG);
            var spawnResult = spawnCard.DoSpawn(pos, Quaternion.identity, spawnRequest);
            var spawnedInstance = spawnResult.spawnedInstance;
            
            if (!spawnResult.success|| ReferenceEquals(spawnedInstance, null))
            {
                ArtifactEnsemble.Logger.LogWarning($"Spawning object with path \"{path}\" failed");
                return;
            }
            
            spawnedInstance.transform.eulerAngles = ang;
            
            if(NetworkServer.active)
                NetworkServer.Spawn(spawnedInstance);
        }
    }
}
