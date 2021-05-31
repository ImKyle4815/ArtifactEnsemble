using System;
using System.Text.RegularExpressions;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Networking;
using UnityEngine;

namespace ArtifactEnsemble
{
    //Lists Plugin MetaData
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //R2API's command helper
    [R2APISubmoduleDependency(nameof(CommandHelper))]

    public class ArtifactEnsemble : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ImKyle4815";
        public const string PluginName = "ArtifactEnsemble";
        public const string PluginVersion = "1.0.0";

        ArtifactDef Rebirth = ScriptableObject.CreateInstance<ArtifactDef>();
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            //Tells R2API to scan the mod for console commands
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
            //Artifacts
            Rebirth.nameToken = "Artifact of Rebirth";
            Rebirth.descriptionToken = "Players respawn after 60 seconds";
            Rebirth.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.gray, Color.white, Color.white, Color.white);
            Rebirth.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.green, Color.gray, Color.gray, Color.gray);

            //Log that the mod is done loading
            Log.LogMessage("ImKyle4815's ArtifactEnsemble mod has finished loading.");
        }

        [ConCommand(commandName = "ae_toggle", flags = ConVarFlags.ExecuteOnServer, helpText = "Toggles Artifacts by name (partial names are sufficient)")]
        private static void AEToggleArtifact(ConCommandArgs args)
        {
            bool flag = !RunArtifactManager.instance;
            if (flag)
            {
                Log.LogInfo("You can only toggle artifacts while in a run.");
            }
            else
            {
                bool flag2 = GameNetworkManager.singleton.desiredHost.hostingParameters.listen && !SteamworksLobbyManager.ownsLobby;
                if (flag2)
                {
                    Log.LogInfo("You can only toggle artifacts if you're the lobby host.");
                }
                else
                {
                    bool flag3 = args.Count != 1;
                    if (flag3)
                    {
                        Log.LogInfo("Improper number of arguments provided. Use 'ae_toggle ARTIFACTNAME'");
                    }
                    else
                    {
                        ArtifactDef artifactDefFromString = StringToArtifactDef(args[0]);
                        bool flag4 = !artifactDefFromString;
                        if (flag4)
                        {
                            Log.LogInfo("No Artifact found under the given name.");
                        }
                        else
                        {
                            bool toggledState = !RunArtifactManager.instance.IsArtifactEnabled(artifactDefFromString);
                            RunArtifactManager.instance.SetArtifactEnabledServer(artifactDefFromString, toggledState);
                        }
                    }
                }
            }
        }

        private static ArtifactDef StringToArtifactDef(string partialName)
        {
            foreach (ArtifactDef artifactDef in ArtifactCatalog.artifactDefs)
            {
                bool flag = Regex.Replace(Language.GetString(artifactDef.nameToken), "[ '-]", string.Empty).ToLower().Contains(partialName.ToLower());
                if (flag)
                {
                    return artifactDef;
                }
            }
            return null;
        }
    }
}
