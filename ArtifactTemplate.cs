using RoR2;
using R2API;
using UnityEngine;
using BepInEx.Configuration;
using System;

namespace ArtifactEnsemble
{
    public abstract class ArtifactTemplate
    {
        public bool enabled = true;
        public ArtifactDef artifact;

        public void Init(string ArtifactName, string ArtifactDesc, byte[] SpriteOn, byte[] SpriteOff)
        {
            if (!enabled) return;
            string upperName = ArtifactName.ToUpper();

            LanguageAPI.Add("ARTIFACTENSEMBLE_" + upperName + "_NAME", "Artifact of " + ArtifactName);
            LanguageAPI.Add("ARTIFACTENSEMBLE_" + upperName + "_DESC", ArtifactDesc);

            artifact = ScriptableObject.CreateInstance<ArtifactDef>();
            artifact.cachedName = "ArtifactEnsemble" + ArtifactName;
            artifact.nameToken = "ARTIFACTENSEMBLE_" + upperName + "_NAME";
            artifact.descriptionToken = "ARTIFACTENSEMBLE_" + upperName + "_DESC";
			artifact.smallIconSelectedSprite = CreateSprite(SpriteOn, Color.gray);
			artifact.smallIconDeselectedSprite = CreateSprite(SpriteOff, Color.magenta);
			ContentAddition.AddArtifactDef(artifact);
        }

		public bool Enabled()
        {
			return RunArtifactManager.instance.IsArtifactEnabled(artifact.artifactIndex) && UnityEngine.Networking.NetworkServer.active;
		}

		public static Sprite CreateSprite(byte[] resourceBytes, Color fallbackColor)
		{
			Texture2D texture2D = new Texture2D(32, 32, TextureFormat.RGBA32, false);
			texture2D.filterMode = FilterMode.Bilinear;
			try
			{
				if (resourceBytes == null)
				{
					ArtifactTemplate.FillTexture(texture2D, fallbackColor);
				}
				else
				{
					texture2D.LoadImage(resourceBytes, false);
					texture2D.Apply();
				}
			}
			catch (Exception ex)
			{
				ArtifactEnsemble.Logger.LogError(ex.ToString());
				ArtifactEnsemble.Logger.LogInfo("Using fallback color.");
				ArtifactTemplate.FillTexture(texture2D, fallbackColor);
			}
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(31f, 31f));
		}
		public static Texture2D FillTexture(Texture2D tex, Color color)
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
	}
}