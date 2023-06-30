using System;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class MeshPaintable3D : BaseMeshPaintable
{
	// Token: 0x040016C7 RID: 5831
	[ClientVar]
	public static float brushScale = 2f;

	// Token: 0x040016C8 RID: 5832
	[ClientVar]
	public static float uvBufferScale = 2f;

	// Token: 0x040016C9 RID: 5833
	public string replacementTextureName = "_MainTex";

	// Token: 0x040016CA RID: 5834
	public int textureWidth = 256;

	// Token: 0x040016CB RID: 5835
	public int textureHeight = 256;

	// Token: 0x040016CC RID: 5836
	public Camera cameraPreview;

	// Token: 0x040016CD RID: 5837
	public Camera camera3D;
}
