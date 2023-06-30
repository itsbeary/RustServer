using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
public class OutlineManager : MonoBehaviour, IClientComponent
{
	// Token: 0x04002440 RID: 9280
	public static Material blurMat;

	// Token: 0x04002441 RID: 9281
	public List<OutlineObject> objectsToRender;

	// Token: 0x04002442 RID: 9282
	public float blurAmount = 2f;

	// Token: 0x04002443 RID: 9283
	public Material glowSolidMaterial;

	// Token: 0x04002444 RID: 9284
	public Material blendGlowMaterial;
}
