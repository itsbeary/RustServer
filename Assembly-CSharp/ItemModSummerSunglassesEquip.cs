using System;
using ConVar;

// Token: 0x02000606 RID: 1542
public class ItemModSummerSunglassesEquip : ItemMod
{
	// Token: 0x06002DDE RID: 11742 RVA: 0x0011432C File Offset: 0x0011252C
	public override void DoAction(Item item, BasePlayer player)
	{
		base.DoAction(item, player);
		if (player != null && !string.IsNullOrEmpty(this.AchivementName) && player.inventory.containerWear.FindItemByUID(item.uid) != null)
		{
			float time = Env.time;
			if (time < this.SunriseTime || time > this.SunsetTime)
			{
				player.GiveAchievement(this.AchivementName);
			}
		}
	}

	// Token: 0x04002598 RID: 9624
	public float SunsetTime;

	// Token: 0x04002599 RID: 9625
	public float SunriseTime;

	// Token: 0x0400259A RID: 9626
	public string AchivementName;
}
