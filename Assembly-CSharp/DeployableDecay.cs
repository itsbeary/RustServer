using System;
using ConVar;

// Token: 0x020003EA RID: 1002
public class DeployableDecay : global::Decay
{
	// Token: 0x0600228B RID: 8843 RVA: 0x000DF1F6 File Offset: 0x000DD3F6
	public override float GetDecayDelay(BaseEntity entity)
	{
		return this.decayDelay * 60f * 60f;
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000DF20A File Offset: 0x000DD40A
	public override float GetDecayDuration(BaseEntity entity)
	{
		return this.decayDuration * 60f * 60f;
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000DE86A File Offset: 0x000DCA6A
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}

	// Token: 0x04001A92 RID: 6802
	public float decayDelay = 8f;

	// Token: 0x04001A93 RID: 6803
	public float decayDuration = 8f;
}
