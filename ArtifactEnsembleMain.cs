using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Networking;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{
    //Lists Plugin MetaData
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //R2API's command helper
    [R2APISubmoduleDependency(nameof(CommandHelper))]
    //Temporary
    [R2APISubmoduleDependency(nameof(LoadoutAPI))]

    public class ArtifactEnsembleMain : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ImKyle4815";
        public const string PluginName = "ArtifactEnsemble";
        public const string PluginVersion = "1.0.0";

        ArtifactDef Fortune = ScriptableObject.CreateInstance<ArtifactDef>();
        ArtifactDef Reanimation = ScriptableObject.CreateInstance<ArtifactDef>();
        ArtifactDef Trade = ScriptableObject.CreateInstance<ArtifactDef>();
        //public static List<ArtifactDef> artifactDefs;
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            
            //Tells R2API to scan the mod for console commands
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
            
            //Artifact definitions
            AENewArtifact(Fortune, "Fortune", "As the run begins, your lunar coins will reset to (nearly) max", Properties.Resources.reanimation_off, Properties.Resources.reanimation_on);
            AENewArtifact(Reanimation, "Reanimation", "Dead players respawn at the beginning of the teleporter event", Properties.Resources.reanimation_off, Properties.Resources.reanimation_on);
            AENewArtifact(Trade, "Trade", "Starts the run in the Space Bazaar", Properties.Resources.reanimation_off, Properties.Resources.reanimation_on);

            //Add all artifacts
            ArtifactCatalog.getAdditionalEntries += (list) => list.AddRange(new List<ArtifactDef> { Fortune, Reanimation, Trade });

            //Artifact hooks
            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);
                AEAddLunarCoins(Fortune);
            };
            On.RoR2.TeleporterInteraction.OnInteractionBegin += (orig, self, activator) =>
            {
                AERespawnPlayers(Reanimation);
                orig(self, activator);
            };
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                AEStartInBazaar(Trade);
            };
            //RoR2.KickFromShop.OnEnter += (orig, self) =>


            //Log that the mod is done loading
            Log.LogInfo("ImKyle4815's ArtifactEnsemble mod has finished loading.");
        }

        private static void AENewArtifact(ArtifactDef artifact, string name, string desc, byte[] offSprite, byte[] onSprite)
        {
            artifact.nameToken = "Artifact of " + name;
            artifact.descriptionToken = desc;
            artifact.smallIconDeselectedSprite = CreateSprite(offSprite, Color.gray);
            artifact.smallIconSelectedSprite = CreateSprite(onSprite, Color.magenta);
            //artifactDefs.Add(artifact);
        }
        private static Sprite CreateSprite(byte[] resourceBytes, Color fallbackColor)
        {
            Texture2D texture2D = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            texture2D.filterMode = FilterMode.Bilinear;
            try
            {
                if (resourceBytes == null)
                {
                    FillTexture(texture2D, fallbackColor);
                }
                else
                {
                    texture2D.LoadImage(resourceBytes, false);
                    texture2D.Apply();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString() + "\nUsing fallback color.");
                FillTexture(texture2D, fallbackColor);
            }
            return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(31f, 31f));
        }
        private static Texture2D FillTexture(Texture2D tex, Color color)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
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

        private static void AEAddLunarCoins(ArtifactDef Fortune)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(Fortune.artifactIndex))
            {
                foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
                {
                    int num = 2000000000 - (int)networkUser.netLunarCoins;
                    if (num > 0)
                    {
                        networkUser.AwardLunarCoins((uint)num);
                    }
                    else if (num < 0)
                    {
                        num *= -1;
                        networkUser.DeductLunarCoins((uint)num);
                    }
                }
            }
        }

        private static void AERespawnPlayers(ArtifactDef Reanimation)
        {
            bool flag = (!RoR2Application.isInSinglePlayer || NetworkServer.active) && RunArtifactManager.instance.IsArtifactEnabled(Reanimation.artifactIndex);
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
    
        private static void AEStartInBazaar(ArtifactDef Trade)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(Trade.artifactIndex))
            {
                Log.LogInfo("Starting in the Bazaar");
                SceneField sceneField = new SceneField("bazaar");
                NetworkManager.singleton.ServerChangeScene(sceneField);
            }
        }









        /*private static Sprite CreateSprite(byte[] resourceBytes, Color fallbackColor)
        {
            var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            try
            {
                if (resourceBytes == null)
                {
                    FillTexture(tex, fallbackColor);
                }
                else
                {
                    tex.LoadImage(resourceBytes, false);
                    tex.Apply();
                    CleanAlpha(tex);
                }
            }
            catch (Exception e)
            {
                Log.LogDebug(e.ToString());
                FillTexture(tex, fallbackColor);
            }
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(31, 31));
        }
        private static Texture2D FillTexture(Texture2D tex, Color color)
        {
            var pixels = tex.GetPixels();
            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }
        private static Texture2D CleanAlpha(Texture2D tex)
        {
            var pixels = tex.GetPixels();
            for (var i = 0; i < pixels.Length; ++i)
            {
                if (pixels[i].a < 0.05f)
                {
                    pixels[i] = Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }
        private static void RegisterLanguageToken(string token, string text)
        {
            LanguageAPI.Add(token, text);
        }
        private void initTextures()
        {
            //ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }*/
    }
}
