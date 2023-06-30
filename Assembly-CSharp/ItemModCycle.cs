using System;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
public class ItemModCycle : ItemMod
{
	// Token: 0x06002DA0 RID: 11680 RVA: 0x00113254 File Offset: 0x00111454
	public override void OnItemCreated(Item itemcreated)
	{
		float timeTaken = this.timerStart;
		itemcreated.onCycle += delegate(Item item, float delta)
		{
			if (this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			timeTaken += delta;
			if (timeTaken < this.timeBetweenCycles)
			{
				return;
			}
			timeTaken = 0f;
			if (!this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			this.CustomCycle(item, delta);
		};
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x0011328C File Offset: 0x0011148C
	private bool CanCycle(Item item)
	{
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CanDoAction(item, item.GetOwnerPlayer()))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x001132C4 File Offset: 0x001114C4
	public void CustomCycle(Item item, float delta)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DoAction(item, ownerPlayer);
		}
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x001132F7 File Offset: 0x001114F7
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null", base.gameObject);
		}
	}

	// Token: 0x0400254A RID: 9546
	public ItemMod[] actions;

	// Token: 0x0400254B RID: 9547
	public float timeBetweenCycles = 1f;

	// Token: 0x0400254C RID: 9548
	public float timerStart;

	// Token: 0x0400254D RID: 9549
	public bool onlyAdvanceTimerWhenPass;
}
