using System;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class MeshPaintable : BaseMeshPaintable
{
	// Token: 0x040016C1 RID: 5825
	public string replacementTextureName = "_MainTex";

	// Token: 0x040016C2 RID: 5826
	public int textureWidth = 256;

	// Token: 0x040016C3 RID: 5827
	public int textureHeight = 256;

	// Token: 0x040016C4 RID: 5828
	public Color clearColor = Color.clear;

	// Token: 0x040016C5 RID: 5829
	public Texture2D targetTexture;

	// Token: 0x040016C6 RID: 5830
	public bool hasChanges;
}
