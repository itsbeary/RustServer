using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F4 RID: 244
public class WorldItem : global::BaseEntity
{
	// Token: 0x0600155C RID: 5468 RVA: 0x000A8CCC File Offset: 0x000A6ECC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WorldItem.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778075470U, "Pickup", this, player, 3f))
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
							this.Pickup(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000A8E34 File Offset: 0x000A7034
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000A8E56 File Offset: 0x000A7056
	private void DoItemNetworking()
	{
		if (this._isInvokingSendItemUpdate)
		{
			return;
		}
		this._isInvokingSendItemUpdate = true;
		base.Invoke(new Action(this.SendItemUpdate), 0.1f);
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x000A8E80 File Offset: 0x000A7080
	private void SendItemUpdate()
	{
		this._isInvokingSendItemUpdate = false;
		if (this.item == null)
		{
			return;
		}
		using (UpdateItem updateItem = Facepunch.Pool.Get<UpdateItem>())
		{
			updateItem.item = this.item.Save(false, false);
			base.ClientRPC<UpdateItem>(null, "UpdateItem", updateItem);
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x000A8EE0 File Offset: 0x000A70E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (!this.allowPickup)
		{
			return;
		}
		base.ClientRPC(null, "PickupSound");
		global::Item item = this.item;
		Analytics.Azure.OnItemPickup(msg.player, this);
		this.RemoveItem();
		msg.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		msg.player.SignalBroadcast(global::BaseEntity.Signal.Gesture, "pickup_item", null);
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000A8F54 File Offset: 0x000A7154
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.item == null)
		{
			return;
		}
		bool forDisk = info.forDisk;
		info.msg.worldItem = Facepunch.Pool.Get<ProtoBuf.WorldItem>();
		info.msg.worldItem.item = this.item.Save(forDisk, false);
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000A8FA5 File Offset: 0x000A71A5
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.DestroyItem();
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000A8FB3 File Offset: 0x000A71B3
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, this.parentBone, false, false);
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x000A8FC4 File Offset: 0x000A71C4
	public override global::Item GetItem()
	{
		return this.item;
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x000A8FCC File Offset: 0x000A71CC
	public void InitializeItem(global::Item in_item)
	{
		if (this.item != null)
		{
			this.RemoveItem();
		}
		this.item = in_item;
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty += this.OnItemDirty;
		base.name = this.item.info.shortname + " (world)";
		this.item.SetWorldEntity(this);
		this.OnItemDirty(this.item);
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x000A9047 File Offset: 0x000A7247
	public void RemoveItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item = null;
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x000A9071 File Offset: 0x000A7271
	public void DestroyItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item.Remove(0f);
		this.item = null;
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x000A90AB File Offset: 0x000A72AB
	protected virtual void OnItemDirty(global::Item in_item)
	{
		Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
		this.DoItemNetworking();
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000A90E0 File Offset: 0x000A72E0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem == null)
		{
			return;
		}
		if (info.msg.worldItem.item == null)
		{
			return;
		}
		global::Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
		if (item != null)
		{
			this.InitializeItem(item);
		}
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x0600156A RID: 5482 RVA: 0x000A9141 File Offset: 0x000A7341
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			if (this.item != null)
			{
				return this.item.Traits;
			}
			return base.Traits;
		}
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000A9160 File Offset: 0x000A7360
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (this.eatSeconds <= 0f)
		{
			return;
		}
		this.eatSeconds -= timeSpent;
		baseNpc.AddCalories(this.caloriesPerSecond * timeSpent);
		if (this.eatSeconds < 0f)
		{
			this.DestroyItem();
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000A91B4 File Offset: 0x000A73B4
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				string text = "{1}[{0}] {2}";
				Networkable net = this.net;
				this._name = string.Format(text, (net != null) ? net.ID : default(NetworkableId), base.ShortPrefabName, this.IsUnityNull<global::WorldItem>() ? "NULL" : base.name);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x04000D8E RID: 3470
	private bool _isInvokingSendItemUpdate;

	// Token: 0x04000D8F RID: 3471
	[Header("WorldItem")]
	public bool allowPickup = true;

	// Token: 0x04000D90 RID: 3472
	[NonSerialized]
	public global::Item item;

	// Token: 0x04000D91 RID: 3473
	protected float eatSeconds = 10f;

	// Token: 0x04000D92 RID: 3474
	protected float caloriesPerSecond = 1f;
}
