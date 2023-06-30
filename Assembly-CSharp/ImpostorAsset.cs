using System;
using UnityEngine;

// Token: 0x0200072B RID: 1835
public class ImpostorAsset : ScriptableObject
{
	// Token: 0x06003328 RID: 13096 RVA: 0x00139BBC File Offset: 0x00137DBC
	public Texture2D FindTexture(string name)
	{
		foreach (ImpostorAsset.TextureEntry textureEntry in this.textures)
		{
			if (textureEntry.name == name)
			{
				return textureEntry.texture;
			}
		}
		return null;
	}

	// Token: 0x040029FB RID: 10747
	public ImpostorAsset.TextureEntry[] textures;

	// Token: 0x040029FC RID: 10748
	public Vector2 size;

	// Token: 0x040029FD RID: 10749
	public Vector2 pivot;

	// Token: 0x040029FE RID: 10750
	public Mesh mesh;

	// Token: 0x02000E50 RID: 3664
	[Serializable]
	public class TextureEntry
	{
		// Token: 0x06005282 RID: 21122 RVA: 0x001B056B File Offset: 0x001AE76B
		public TextureEntry(string name, Texture2D texture)
		{
			this.name = name;
			this.texture = texture;
		}

		// Token: 0x04004B6B RID: 19307
		public string name;

		// Token: 0x04004B6C RID: 19308
		public Texture2D texture;
	}
}
