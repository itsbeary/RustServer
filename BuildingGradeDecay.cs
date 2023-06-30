using System;
using ConVar;

// Token: 0x020003E7 RID: 999
public class BuildingGradeDecay : global::Decay
{
	// Token: 0x06002269 RID: 8809 RVA: 0x000DE84E File Offset: 0x000DCA4E
	public override float GetDecayDelay(BaseEntity entity)
	{
		return base.GetDecayDelay(this.decayGrade);
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000DE85C File Offset: 0x000DCA5C
	public override float GetDecayDuration(BaseEntity entity)
	{
		return base.GetDecayDuration(this.decayGrade);
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000DE86A File Offset: 0x000DCA6A
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}

	// Token: 0x04001A86 RID: 6790
	public BuildingGrade.Enum decayGrade;
}
