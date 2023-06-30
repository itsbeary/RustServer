using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000448 RID: 1096
public class PlayerBelt
{
	// Token: 0x17000300 RID: 768
	// (get) Token: 0x060024C2 RID: 9410 RVA: 0x000E9888 File Offset: 0x000E7A88
	public static int MaxBeltSlots
	{
		get
		{
			return 6;
		}
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000E988B File Offset: 0x000E7A8B
	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000E989C File Offset: 0x000E7A9C
	public void DropActive(Vector3 position, Vector3 velocity)
	{
		Item activeItem = this.player.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		using (TimeWarning.New("PlayerBelt.DropActive", 0))
		{
			DroppedItem droppedItem = activeItem.Drop(position, velocity, default(Quaternion)) as DroppedItem;
			if (droppedItem != null)
			{
				droppedItem.DropReason = DroppedItem.DropReasonEnum.Death;
				droppedItem.DroppedBy = this.player.userID;
				Analytics.Azure.OnItemDropped(this.player, droppedItem, DroppedItem.DropReasonEnum.Death);
			}
			this.player.svActiveItemID = default(ItemId);
			this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000E9944 File Offset: 0x000E7B44
	public Item GetItemInSlot(int slot)
	{
		if (this.player == null)
		{
			return null;
		}
		if (this.player.inventory == null)
		{
			return null;
		}
		if (this.player.inventory.containerBelt == null)
		{
			return null;
		}
		return this.player.inventory.containerBelt.GetSlot(slot);
	}

	// Token: 0x04001CCE RID: 7374
	public static int SelectedSlot = -1;

	// Token: 0x04001CCF RID: 7375
	protected BasePlayer player;
}
