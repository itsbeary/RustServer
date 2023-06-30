using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008A RID: 138
public class ItemBasedFlowRestrictor : global::IOEntity, IContainerSounds
{
	// Token: 0x06000D0C RID: 3340 RVA: 0x000704E0 File Offset: 0x0006E6E0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ItemBasedFlowRestrictor.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00070648 File Offset: 0x0006E848
	public override void ResetIOState()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (this.inventory != null)
		{
			global::Item slot = this.inventory.GetSlot(0);
			if (slot != null)
			{
				slot.Drop(this.debugOrigin.transform.position + base.transform.forward * 0.5f, this.GetInheritedDropVelocity() + base.transform.forward * 2f, default(Quaternion));
			}
		}
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x000706D1 File Offset: 0x0006E8D1
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x000706EC File Offset: 0x0006E8EC
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(global::BaseEntity.Flags.On, this.IsPowered(), false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.IsOn() && !base.HasFlag(global::BaseEntity.Flags.Reserved1), false, true);
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x0007074C File Offset: 0x0006E94C
	public virtual bool HasPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return false;
		}
		global::Item slot = this.inventory.GetSlot(0);
		return slot != null && (this.passthroughItemConditionLossPerSec <= 0f || !slot.hasCondition || slot.conditionNormalized > 0f) && slot.info == this.passthroughItem;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x000707BC File Offset: 0x0006E9BC
	public virtual void TickPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		global::Item slot = this.inventory.GetSlot(0);
		if (slot != null && slot.hasCondition)
		{
			slot.LoseCondition(1f);
		}
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x0007080C File Offset: 0x0006EA0C
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.InvokeRandomized(new Action(this.TickPassthroughItem), 1f, 1f, 0.015f);
		base.ServerInit();
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0007085C File Offset: 0x0006EA5C
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x0007086C File Offset: 0x0006EA6C
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.passthroughItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00070908 File Offset: 0x0006EB08
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00070970 File Offset: 0x0006EB70
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000709DB File Offset: 0x0006EBDB
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		this.MarkDirty();
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000709F8 File Offset: 0x0006EBF8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
	}

	// Token: 0x04000857 RID: 2135
	public ItemDefinition passthroughItem;

	// Token: 0x04000858 RID: 2136
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x04000859 RID: 2137
	public int maxStackSize = 1;

	// Token: 0x0400085A RID: 2138
	public int numSlots;

	// Token: 0x0400085B RID: 2139
	public string lootPanelName = "generic";

	// Token: 0x0400085C RID: 2140
	public const global::BaseEntity.Flags HasPassthrough = global::BaseEntity.Flags.Reserved1;

	// Token: 0x0400085D RID: 2141
	public const global::BaseEntity.Flags Sparks = global::BaseEntity.Flags.Reserved2;

	// Token: 0x0400085E RID: 2142
	public float passthroughItemConditionLossPerSec = 1f;

	// Token: 0x0400085F RID: 2143
	public SoundDefinition openSound;

	// Token: 0x04000860 RID: 2144
	public SoundDefinition closeSound;

	// Token: 0x04000861 RID: 2145
	private global::ItemContainer inventory;
}
