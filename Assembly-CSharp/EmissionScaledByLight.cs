using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class EmissionScaledByLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04001858 RID: 6232
	private Color emissionColor;

	// Token: 0x04001859 RID: 6233
	public Renderer[] targetRenderers;

	// Token: 0x0400185A RID: 6234
	public int materialIndex = -1;

	// Token: 0x0400185B RID: 6235
	private static MaterialPropertyBlock block;

	// Token: 0x0400185C RID: 6236
	public Light lightToFollow;

	// Token: 0x0400185D RID: 6237
	public float maxEmissionValue = 3f;
}
