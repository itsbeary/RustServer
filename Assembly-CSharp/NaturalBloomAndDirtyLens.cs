using System;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Natural Bloom and Dirty Lens")]
public class NaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x0400244E RID: 9294
	public Shader shader;

	// Token: 0x0400244F RID: 9295
	public Texture2D lensDirtTexture;

	// Token: 0x04002450 RID: 9296
	public float range = 10000f;

	// Token: 0x04002451 RID: 9297
	public float cutoff = 1f;

	// Token: 0x04002452 RID: 9298
	[Range(0f, 1f)]
	public float bloomIntensity = 0.05f;

	// Token: 0x04002453 RID: 9299
	[Range(0f, 1f)]
	public float lensDirtIntensity = 0.05f;

	// Token: 0x04002454 RID: 9300
	[Range(0f, 4f)]
	public float spread = 1f;

	// Token: 0x04002455 RID: 9301
	[Range(0f, 4f)]
	public int iterations = 1;

	// Token: 0x04002456 RID: 9302
	[Range(1f, 10f)]
	public int mips = 6;

	// Token: 0x04002457 RID: 9303
	public float[] mipWeights = new float[] { 0.5f, 0.6f, 0.6f, 0.45f, 0.35f, 0.23f };

	// Token: 0x04002458 RID: 9304
	public bool highPrecision;

	// Token: 0x04002459 RID: 9305
	public bool downscaleSource;

	// Token: 0x0400245A RID: 9306
	public bool debug;

	// Token: 0x0400245B RID: 9307
	public bool temporalFilter;

	// Token: 0x0400245C RID: 9308
	[Range(0.01f, 1f)]
	public float temporalFilterWeight = 0.75f;
}
