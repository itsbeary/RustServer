using System;
using UnityEngine;

// Token: 0x020005ED RID: 1517
public class ItemModConsumeChance : ItemModConsume
{
	// Token: 0x06002D8B RID: 11659 RVA: 0x00112BA8 File Offset: 0x00110DA8
	private bool GetChance()
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(Time.frameCount);
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.chanceForSecondaryConsume;
		UnityEngine.Random.state = state;
		return flag;
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x00112BE5 File Offset: 0x00110DE5
	public override ItemModConsumable GetConsumable()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumable;
		}
		return base.GetConsumable();
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x00112BFC File Offset: 0x00110DFC
	public override GameObjectRef GetConsumeEffect()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumeEffect;
		}
		return base.GetConsumeEffect();
	}

	// Token: 0x04002535 RID: 9525
	public float chanceForSecondaryConsume = 0.5f;

	// Token: 0x04002536 RID: 9526
	public GameObjectRef secondaryConsumeEffect;

	// Token: 0x04002537 RID: 9527
	public ItemModConsumable secondaryConsumable;
}
