using System;
using Rust;

// Token: 0x020001FA RID: 506
public class TunnelDweller : HumanNPC
{
	// Token: 0x06001A87 RID: 6791 RVA: 0x000BF2C0 File Offset: 0x000BD4C0
	protected override string OverrideCorpseName()
	{
		return "Tunnel Dweller";
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x000BF2C8 File Offset: 0x000BD4C8
	protected override void OnKilledByPlayer(BasePlayer p)
	{
		base.OnKilledByPlayer(p);
		TrainEngine trainEngine;
		if (GameInfo.HasAchievements && p.GetParentEntity() != null && (trainEngine = p.GetParentEntity() as TrainEngine) != null && trainEngine.CurThrottleSetting != TrainEngine.EngineSpeeds.Zero && trainEngine.IsMovingOrOn)
		{
			p.stats.Add("dweller_kills_while_moving", 1, Stats.All);
			p.stats.Save(true);
		}
	}

	// Token: 0x040012D4 RID: 4820
	private const string DWELLER_KILL_STAT = "dweller_kills_while_moving";
}
