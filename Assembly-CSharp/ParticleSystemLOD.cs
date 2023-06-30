using System;
using UnityEngine;

// Token: 0x0200054A RID: 1354
public class ParticleSystemLOD : LODComponentParticleSystem
{
	// Token: 0x04002281 RID: 8833
	[Horizontal(1, 0)]
	public ParticleSystemLOD.State[] States;

	// Token: 0x02000D5C RID: 3420
	[Serializable]
	public class State
	{
		// Token: 0x040047A7 RID: 18343
		public float distance;

		// Token: 0x040047A8 RID: 18344
		[Range(0f, 1f)]
		public float emission;
	}
}
