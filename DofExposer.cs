using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000309 RID: 777
[ExecuteInEditMode]
public class DofExposer : SingletonComponent<DofExposer>
{
	// Token: 0x040017BB RID: 6075
	public PostProcessVolume PostVolume;

	// Token: 0x040017BC RID: 6076
	public bool DofEnabled;

	// Token: 0x040017BD RID: 6077
	public float FocalLength = 15.24f;

	// Token: 0x040017BE RID: 6078
	public float Blur = 2f;

	// Token: 0x040017BF RID: 6079
	public float FocalAperture = 13.16f;

	// Token: 0x040017C0 RID: 6080
	public bool debug;
}
