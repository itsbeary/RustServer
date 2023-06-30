using System;
using UnityEngine;

// Token: 0x0200023B RID: 571
[CreateAssetMenu(menuName = "Rust/Reverb Settings")]
public class ReverbSettings : ScriptableObject
{
	// Token: 0x04001483 RID: 5251
	[Range(-10000f, 0f)]
	public int room;

	// Token: 0x04001484 RID: 5252
	[Range(-10000f, 0f)]
	public int roomHF;

	// Token: 0x04001485 RID: 5253
	[Range(-10000f, 0f)]
	public int roomLF;

	// Token: 0x04001486 RID: 5254
	[Range(0.1f, 20f)]
	public float decayTime;

	// Token: 0x04001487 RID: 5255
	[Range(0.1f, 2f)]
	public float decayHFRatio;

	// Token: 0x04001488 RID: 5256
	[Range(-10000f, 1000f)]
	public int reflections;

	// Token: 0x04001489 RID: 5257
	[Range(0f, 0.3f)]
	public float reflectionsDelay;

	// Token: 0x0400148A RID: 5258
	[Range(-10000f, 2000f)]
	public int reverb;

	// Token: 0x0400148B RID: 5259
	[Range(0f, 0.1f)]
	public float reverbDelay;

	// Token: 0x0400148C RID: 5260
	[Range(1000f, 20000f)]
	public float HFReference;

	// Token: 0x0400148D RID: 5261
	[Range(20f, 1000f)]
	public float LFReference;

	// Token: 0x0400148E RID: 5262
	[Range(0f, 100f)]
	public float diffusion;

	// Token: 0x0400148F RID: 5263
	[Range(0f, 100f)]
	public float density;
}
