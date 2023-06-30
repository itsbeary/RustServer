using System;

// Token: 0x02000548 RID: 1352
public class ParticleCollisionLOD : LODComponentParticleSystem
{
	// Token: 0x0400227F RID: 8831
	[Horizontal(1, 0)]
	public ParticleCollisionLOD.State[] States;

	// Token: 0x02000D5A RID: 3418
	public enum QualityLevel
	{
		// Token: 0x040047A1 RID: 18337
		Disabled = -1,
		// Token: 0x040047A2 RID: 18338
		HighQuality,
		// Token: 0x040047A3 RID: 18339
		MediumQuality,
		// Token: 0x040047A4 RID: 18340
		LowQuality
	}

	// Token: 0x02000D5B RID: 3419
	[Serializable]
	public class State
	{
		// Token: 0x040047A5 RID: 18341
		public float distance;

		// Token: 0x040047A6 RID: 18342
		public ParticleCollisionLOD.QualityLevel quality = ParticleCollisionLOD.QualityLevel.Disabled;
	}
}
