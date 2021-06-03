using System;
using R2API;
using RoR2;
using UnityEngine;

namespace ArtifactEnsemble
{
	public abstract class NewArtifact<T> where T : NewArtifact<T>
	{
		public static T Instance { get; private set; }

		public abstract string Name { get; }

		public abstract string NameToken { get; }
		
		public abstract string Description { get; }

		public virtual UnlockableDef UnlockableDef { get; private set; }

		public abstract Sprite IconSelectedSprite { get; }

		public abstract Sprite IconDeselectedSprite { get; }

		public virtual GameObject PickupModelPrefab { get; }

		public ArtifactDef ArtifactDef { get; protected set; }

		public bool ArtifactEnabled
		{
			get
			{
				return RunArtifactManager.instance.IsArtifactEnabled(this.ArtifactDef);
			}
		}

		public NewArtifact()
		{
			if (NewArtifact<T>.Instance != null)
			{
				throw new InvalidOperationException("Same artifact cannot be created more than once. Use the already existing Instance.");
			}
			NewArtifact<T>.Instance = (T)((object)this);
			this.InitManager();
			this.InitArtifact();
		}

		protected abstract void InitManager();

		protected void InitArtifact()
		{
			LanguageAPI.Add("ARTIFACT_" + this.NameToken + "_NAME", this.Name);
			LanguageAPI.Add("ARTIFACT_" + this.NameToken + "_DESCRIPTION", this.Description);
			this.ArtifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
			this.ArtifactDef.cachedName = "ARTIFACT_" + this.NameToken;
			this.ArtifactDef.nameToken = "ARTIFACT_" + this.NameToken + "_NAME";
			this.ArtifactDef.descriptionToken = "ARTIFACT_" + this.NameToken + "_DESCRIPTION";
			if (this.UnlockableDef)
			{
				this.ArtifactDef.unlockableDef = this.UnlockableDef;
			}
			this.ArtifactDef.smallIconSelectedSprite = this.IconSelectedSprite;
			this.ArtifactDef.smallIconDeselectedSprite = this.IconDeselectedSprite;
			if (this.PickupModelPrefab)
			{
				this.ArtifactDef.pickupModelPrefab = this.PickupModelPrefab;
			}
			ArtifactAPI.Add(this.ArtifactDef);
			ArtifactEnsemble.Logger.LogInfo("Initialized Artifact: " + this.Name);
		}

		public static Sprite CreateSprite(byte[] resourceBytes, Color fallbackColor)
		{
			Texture2D texture2D = new Texture2D(32, 32, TextureFormat.RGBA32, false);
			texture2D.filterMode = FilterMode.Bilinear;
			try
			{
				if (resourceBytes == null)
				{
					NewArtifact<T>.FillTexture(texture2D, fallbackColor);
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
				NewArtifact<T>.FillTexture(texture2D, fallbackColor);
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
