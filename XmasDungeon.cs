using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class XmasDungeon : HalloweenDungeon
{
	// Token: 0x0600183F RID: 6207 RVA: 0x000B5D2B File Offset: 0x000B3F2B
	public override float GetLifetime()
	{
		return XmasDungeon.xmaslifetime;
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x000B5D32 File Offset: 0x000B3F32
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.PlayerChecks), 1f, 1f);
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x000B5D58 File Offset: 0x000B3F58
	public void PlayerChecks()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			float num = Vector3.Distance(basePlayer.transform.position, base.transform.position);
			float num2 = Vector3.Distance(basePlayer.transform.position, proceduralDynamicDungeon.GetExitPortal(true).transform.position);
			if (num < XmasDungeon.playerdetectrange)
			{
				flag = true;
			}
			if (num2 < XmasDungeon.playerdetectrange * 2f)
			{
				flag2 = true;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, flag2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, flag, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved7, flag, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved8, flag2, false, true);
	}

	// Token: 0x0400110B RID: 4363
	public const BaseEntity.Flags HasPlayerOutside = BaseEntity.Flags.Reserved7;

	// Token: 0x0400110C RID: 4364
	public const BaseEntity.Flags HasPlayerInside = BaseEntity.Flags.Reserved8;

	// Token: 0x0400110D RID: 4365
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float xmaspopulation = 0f;

	// Token: 0x0400110E RID: 4366
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float xmaslifetime = 1200f;

	// Token: 0x0400110F RID: 4367
	[ServerVar(Help = "How far we detect players from our inside/outside", ShowInAdminUI = true)]
	public static float playerdetectrange = 30f;
}
