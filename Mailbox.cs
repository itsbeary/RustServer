using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000097 RID: 151
public class Mailbox : StorageContainer
{
	// Token: 0x06000DE2 RID: 3554 RVA: 0x00075944 File Offset: 0x00073B44
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Mailbox.OnRpcMessage", 0))
		{
			if (rpc == 131727457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Submit ");
				}
				using (TimeWarning.New("RPC_Submit", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Submit(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Submit");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x00075A68 File Offset: 0x00073C68
	public int mailInputSlot
	{
		get
		{
			return this.inventorySlots - 1;
		}
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00075A72 File Offset: 0x00073C72
	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00075A7A File Offset: 0x00073C7A
	public bool IsFull()
	{
		return this.shouldMarkAsFull && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00075A91 File Offset: 0x00073C91
	public void MarkFull(bool full)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull && full, false, true);
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00075AA8 File Offset: 0x00073CA8
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return base.PlayerOpenLoot(player, this.PlayerIsOwner(player) ? this.ownerPanel : panelToOpen, true);
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00075AC4 File Offset: 0x00073CC4
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (panelName == this.ownerPanel)
		{
			return this.PlayerIsOwner(player) && base.CanOpenLootPanel(player, panelName);
		}
		return this.HasFreeSpace() || !this.shouldMarkAsFull;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00075AFB File Offset: 0x00073CFB
	private bool HasFreeSpace()
	{
		return this.GetFreeSlot() != -1;
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00075B0C File Offset: 0x00073D0C
	private int GetFreeSlot()
	{
		for (int i = 0; i < this.mailInputSlot; i++)
		{
			if (base.inventory.GetSlot(i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00075B3B File Offset: 0x00073D3B
	public virtual bool MoveItemToStorage(Item item)
	{
		item.RemoveFromContainer();
		return item.MoveToContainer(base.inventory, -1, true, false, null, true);
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00075B5C File Offset: 0x00073D5C
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (this.autoSubmitWhenClosed)
		{
			this.SubmitInputItems(player);
		}
		if (this.IsFull())
		{
			Item slot = base.inventory.GetSlot(this.mailInputSlot);
			if (slot != null)
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
		base.PlayerStoppedLooting(player);
		if (this.PlayerIsOwner(player))
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00075BCC File Offset: 0x00073DCC
	[BaseEntity.RPC_Server]
	public void RPC_Submit(BaseEntity.RPCMessage msg)
	{
		if (this.IsFull())
		{
			return;
		}
		BasePlayer player = msg.player;
		this.SubmitInputItems(player);
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00075BF0 File Offset: 0x00073DF0
	public void SubmitInputItems(BasePlayer fromPlayer)
	{
		Item slot = base.inventory.GetSlot(this.mailInputSlot);
		if (this.IsFull())
		{
			return;
		}
		if (slot != null)
		{
			if (this.MoveItemToStorage(slot))
			{
				if (slot.position != this.mailInputSlot)
				{
					Effect.server.Run(this.mailDropSound.resourcePath, this.GetDropPosition(), default(Vector3), null, false);
					if (fromPlayer != null && !this.PlayerIsOwner(fromPlayer))
					{
						base.SetFlag(BaseEntity.Flags.On, true, false, true);
						return;
					}
				}
			}
			else
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00075C8C File Offset: 0x00073E8C
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		this.MarkFull(!this.HasFreeSpace());
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00075CA8 File Offset: 0x00073EA8
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		bool flag = this.PlayerIsOwner(player);
		if (!flag)
		{
			flag = item == base.inventory.GetSlot(this.mailInputSlot);
		}
		return flag && base.CanMoveFrom(player, item);
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x00075CE4 File Offset: 0x00073EE4
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (this.allowedItems == null || this.allowedItems.Length == 0)
		{
			return base.ItemFilter(item, targetSlot);
		}
		foreach (ItemDefinition itemDefinition in this.allowedItems)
		{
			if (item.info == itemDefinition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x00075D35 File Offset: 0x00073F35
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		if (player == null || this.PlayerIsOwner(player))
		{
			return -1;
		}
		return this.mailInputSlot;
	}

	// Token: 0x04000901 RID: 2305
	public string ownerPanel;

	// Token: 0x04000902 RID: 2306
	public GameObjectRef mailDropSound;

	// Token: 0x04000903 RID: 2307
	public ItemDefinition[] allowedItems;

	// Token: 0x04000904 RID: 2308
	public bool autoSubmitWhenClosed;

	// Token: 0x04000905 RID: 2309
	public bool shouldMarkAsFull;
}
