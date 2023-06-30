using System;
using UnityEngine;

// Token: 0x020005D9 RID: 1497
public class ItemMod : MonoBehaviour
{
	// Token: 0x06002D42 RID: 11586 RVA: 0x00112080 File Offset: 0x00110280
	public virtual void ModInit()
	{
		this.siblingMods = base.GetComponents<ItemMod>();
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemCreated(Item item)
	{
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnVirginItem(Item item)
	{
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerCommand(Item item, string command, BasePlayer player)
	{
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnRemove(Item item)
	{
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnParentChanged(Item item)
	{
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAttacked(Item item, HitInfo info)
	{
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnChanged(Item item)
	{
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x00112090 File Offset: 0x00110290
	public virtual bool CanDoAction(Item item, BasePlayer player)
	{
		ItemMod[] array = this.siblingMods;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Passes(item))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool Passes(Item item)
	{
		return true;
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnRemovedFromWorld(Item item)
	{
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnMovedToWorld(Item item)
	{
	}

	// Token: 0x0400250B RID: 9483
	protected ItemMod[] siblingMods;
}
