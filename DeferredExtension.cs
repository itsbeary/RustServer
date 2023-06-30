using System;
using UnityEngine;

// Token: 0x0200071E RID: 1822
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	// Token: 0x040029CE RID: 10702
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	// Token: 0x040029CF RID: 10703
	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	// Token: 0x040029D0 RID: 10704
	public Texture2D blueNoise;

	// Token: 0x040029D1 RID: 10705
	public Texture preintegratedFGD_GGX;

	// Token: 0x040029D2 RID: 10706
	public Texture envBrdfLut;

	// Token: 0x040029D3 RID: 10707
	public float depthScale = 100f;

	// Token: 0x040029D4 RID: 10708
	public bool debug;

	// Token: 0x040029D5 RID: 10709
	public bool forceToCameraResolution;
}
