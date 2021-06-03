using System.Text.RegularExpressions;
using RoR2;
using RoR2.Networking;
using R2API.Utils;

namespace ArtifactEnsemble
{
    [R2APISubmoduleDependency(nameof(CommandHelper))]
    public class ArtifactToggler
    {
        public void Awake()
        {
            //Tells R2API to scan the mod for console commands
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
        }

        [ConCommand(commandName = "ae_toggle", flags = ConVarFlags.ExecuteOnServer, helpText = "Toggles Artifacts by name (partial names are sufficient)")]
        private static void AEToggleArtifact(ConCommandArgs args)
        {
            bool flag = !RunArtifactManager.instance;
            if (flag)
            {
                ArtifactEnsemble.Logger.LogMessage("You can only toggle artifacts while in a run.");
            }
            else
            {
                bool flag2 = GameNetworkManager.singleton.desiredHost.hostingParameters.listen && !SteamworksLobbyManager.ownsLobby;
                if (flag2)
                {
                    ArtifactEnsemble.Logger.LogMessage("You can only toggle artifacts if you're the lobby host.");
                }
                else
                {
                    bool flag3 = args.Count != 1;
                    if (flag3)
                    {
                        ArtifactEnsemble.Logger.LogMessage("Improper number of arguments provided. Use 'ae_toggle ARTIFACTNAME'");
                    }
                    else
                    {
                        ArtifactDef artifactDefFromString = StringToArtifactDef(args[0]);
                        bool flag4 = !artifactDefFromString;
                        if (flag4)
                        {
                            ArtifactEnsemble.Logger.LogMessage("No Artifact found under the given name.");
                        }
                        else
                        {
                            string artifactName = artifactDefFromString.nameToken;
                            bool toggledState = !RunArtifactManager.instance.IsArtifactEnabled(artifactDefFromString);
                            RunArtifactManager.instance.SetArtifactEnabledServer(artifactDefFromString, toggledState);
                            if (toggledState)
                            {
                                ArtifactEnsemble.Logger.LogMessage(artifactName + " is now enabled.");
                            }
                            else
                            {
                                ArtifactEnsemble.Logger.LogMessage(artifactName + " is now disabled.");
                            }
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
