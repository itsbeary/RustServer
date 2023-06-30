using System;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class EmissionToggle : MonoBehaviour, IClientComponent
{
	// Token: 0x0400185E RID: 6238
	private Color emissionColor;

	// Token: 0x0400185F RID: 6239
	public Renderer[] targetRenderers;

	// Token: 0x04001860 RID: 6240
	public int materialIndex = -1;
}
