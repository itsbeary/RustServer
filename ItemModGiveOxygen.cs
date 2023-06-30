using System;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
public class ItemModGiveOxygen : ItemMod, IAirSupply
{
	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x06002DB5 RID: 11701 RVA: 0x00113669 File Offset: 0x00111869
	public ItemModGiveOxygen.AirSupplyType AirType
	{
		get
		{
			return this.airType;
		}
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x00113671 File Offset: 0x00111871
	public float GetAirTimeRemaining()
	{
		return this.timeRemaining;
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x0011367C File Offset: 0x0011187C
	public override void ModInit()
	{
		base.ModInit();
		this.cycleTime = 1f;
		ItemMod[] siblingMods = this.siblingMods;
		for (int i = 0; i < siblingMods.Length; i++)
		{
			ItemModCycle itemModCycle;
			if ((itemModCycle = siblingMods[i] as ItemModCycle) != null)
			{
				this.cycleTime = itemModCycle.timeBetweenCycles;
			}
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x001136C8 File Offset: 0x001118C8
	public override void DoAction(Item item, BasePlayer player)
	{
		if (!item.hasCondition)
		{
			return;
		}
		if (item.conditionNormalized == 0f)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		float num = Mathf.Clamp01(0.525f);
		if (player.AirFactor() > num)
		{
			return;
		}
		if (item.parent == null)
		{
			return;
		}
		if (item.parent != player.inventory.containerWear)
		{
			return;
		}
		Effect.server.Run((!this.inhaled) ? this.inhaleEffect.resourcePath : this.exhaleEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		this.inhaled = !this.inhaled;
		if (!this.inhaled && WaterLevel.GetWaterDepth(player.eyes.position, true, true, player) > 3f)
		{
			Effect.server.Run(this.bubblesEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		}
		item.LoseCondition((float)this.amountToConsume);
		player.metabolism.oxygen.Add(1f);
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x001137DE File Offset: 0x001119DE
	public override void OnChanged(Item item)
	{
		if (item.hasCondition)
		{
			this.timeRemaining = item.condition * ((float)this.amountToConsume / this.cycleTime);
			return;
		}
		this.timeRemaining = 0f;
	}

	// Token: 0x04002560 RID: 9568
	public ItemModGiveOxygen.AirSupplyType airType = ItemModGiveOxygen.AirSupplyType.ScubaTank;

	// Token: 0x04002561 RID: 9569
	public int amountToConsume = 1;

	// Token: 0x04002562 RID: 9570
	public GameObjectRef inhaleEffect;

	// Token: 0x04002563 RID: 9571
	public GameObjectRef exhaleEffect;

	// Token: 0x04002564 RID: 9572
	public GameObjectRef bubblesEffect;

	// Token: 0x04002565 RID: 9573
	private float timeRemaining;

	// Token: 0x04002566 RID: 9574
	private float cycleTime;

	// Token: 0x04002567 RID: 9575
	private bool inhaled;

	// Token: 0x02000D9E RID: 3486
	public enum AirSupplyType
	{
		// Token: 0x040048AB RID: 18603
		Lungs,
		// Token: 0x040048AC RID: 18604
		ScubaTank,
		// Token: 0x040048AD RID: 18605
		Submarine
	}
}
