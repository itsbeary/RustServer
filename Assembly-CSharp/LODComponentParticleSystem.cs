using System;
using UnityEngine;

// Token: 0x0200053C RID: 1340
public abstract class LODComponentParticleSystem : LODComponent
{
	// Token: 0x04002264 RID: 8804
	[Tooltip("Automatically call Play() the particle system when it's shown via LOD")]
	public bool playOnShow = true;
}
