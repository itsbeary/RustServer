using System;
using UnityEngine;

// Token: 0x02000354 RID: 852
public class ScaleParticleSystem : ScaleRenderer
{
	// Token: 0x06001F6B RID: 8043 RVA: 0x000D4C7C File Offset: 0x000D2E7C
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		this.startGravity = this.pSystem.gravityModifier;
		this.startSpeed = this.pSystem.startSpeed;
		this.startSize = this.pSystem.startSize;
		this.startLifeTime = this.pSystem.startLifetime;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000D4CD4 File Offset: 0x000D2ED4
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.pSystem.startSize = this.startSize * scale;
		this.pSystem.startLifetime = this.startLifeTime * scale;
		this.pSystem.startSpeed = this.startSpeed * scale;
		this.pSystem.gravityModifier = this.startGravity * scale;
	}

	// Token: 0x040018AF RID: 6319
	public ParticleSystem pSystem;

	// Token: 0x040018B0 RID: 6320
	public bool scaleGravity;

	// Token: 0x040018B1 RID: 6321
	[NonSerialized]
	private float startSize;

	// Token: 0x040018B2 RID: 6322
	[NonSerialized]
	private float startLifeTime;

	// Token: 0x040018B3 RID: 6323
	[NonSerialized]
	private float startSpeed;

	// Token: 0x040018B4 RID: 6324
	[NonSerialized]
	private float startGravity;
}
