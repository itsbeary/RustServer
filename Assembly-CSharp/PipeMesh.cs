using System;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class PipeMesh : MonoBehaviour
{
	// Token: 0x040011BB RID: 4539
	public float PipeRadius = 0.04f;

	// Token: 0x040011BC RID: 4540
	public Material PipeMaterial;

	// Token: 0x040011BD RID: 4541
	public float StraightLength = 0.3f;

	// Token: 0x040011BE RID: 4542
	public int PipeSubdivisions = 8;

	// Token: 0x040011BF RID: 4543
	public int BendTesselation = 6;

	// Token: 0x040011C0 RID: 4544
	public float RidgeHeight = 0.05f;

	// Token: 0x040011C1 RID: 4545
	public float UvScaleMultiplier = 2f;

	// Token: 0x040011C2 RID: 4546
	public float RidgeIncrements = 0.5f;

	// Token: 0x040011C3 RID: 4547
	public float RidgeLength = 0.05f;

	// Token: 0x040011C4 RID: 4548
	public Vector2 HorizontalUvRange = new Vector2(0f, 0.2f);
}
