using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008E RID: 142
public class KeyLock : BaseLock
{
	// Token: 0x06000D3D RID: 3389 RVA: 0x00071664 File Offset: 0x0006F864
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("KeyLock.OnRpcMessage", 0))
		{
			if (rpc == 4135414453U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_CreateKey ");
				}
				using (TimeWarning.New("RPC_CreateKey", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4135414453U, "RPC_CreateKey", this, player, 3f))
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
							this.RPC_CreateKey(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_CreateKey");
					}
				}
				return true;
			}
			if (rpc == 954115386U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lock ");
				}
				using (TimeWarning.New("RPC_Lock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(954115386U, "RPC_Lock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Lock(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Lock");
					}
				}
				return true;
			}
			if (rpc == 1663222372U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Unlock ");
				}
				using (TimeWarning.New("RPC_Unlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1663222372U, "RPC_Unlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Unlock(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Unlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00071AC0 File Offset: 0x0006FCC0
	public override bool HasLockPermission(global::BasePlayer player)
	{
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		foreach (global::Item item in player.inventory.FindItemIDs(this.keyItemType.itemid))
		{
			if (this.CanKeyUnlockUs(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00071B48 File Offset: 0x0006FD48
	private bool CanKeyUnlockUs(global::Item key)
	{
		return key.instanceData != null && key.instanceData.dataInt == this.keyCode;
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00071B6A File Offset: 0x0006FD6A
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			this.keyCode = info.msg.keyLock.code;
		}
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00071B96 File Offset: 0x0006FD96
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.OwnerID == 0UL && base.GetParentEntity())
		{
			base.OwnerID = base.GetParentEntity().OwnerID;
		}
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00071BC4 File Offset: 0x0006FDC4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.keyLock = Facepunch.Pool.Get<ProtoBuf.KeyLock>();
			info.msg.keyLock.code = this.keyCode;
		}
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00071BFB File Offset: 0x0006FDFB
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.keyCode = UnityEngine.Random.Range(1, 100000);
		this.Lock(deployedBy);
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00071C1E File Offset: 0x0006FE1E
	public override bool OnTryToOpen(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00071C1E File Offset: 0x0006FE1E
	public override bool OnTryToClose(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00071C34 File Offset: 0x0006FE34
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Unlock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(rpc.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00071C6E File Offset: 0x0006FE6E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Lock(global::BaseEntity.RPCMessage rpc)
	{
		this.Lock(rpc.player);
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00071C7C File Offset: 0x0006FE7C
	private void Lock(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(player))
		{
			return;
		}
		this.LockLock(player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00071CB4 File Offset: 0x0006FEB4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_CreateKey(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked() && !this.HasLockPermission(rpc.player))
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.keyItemType.itemid);
		if (itemDefinition == null)
		{
			Debug.LogWarning("RPC_CreateKey: Itemdef is missing! " + this.keyItemType);
			return;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(itemDefinition);
		if (rpc.player.inventory.crafting.CanCraft(itemBlueprint, 1, false))
		{
			ProtoBuf.Item.InstanceData instanceData = Facepunch.Pool.Get<ProtoBuf.Item.InstanceData>();
			instanceData.dataInt = this.keyCode;
			rpc.player.inventory.crafting.CraftItem(itemBlueprint, rpc.player, instanceData, 1, 0, null, false);
			if (!this.firstKeyCreated)
			{
				this.LockLock(rpc.player);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.firstKeyCreated = true;
			}
			return;
		}
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00071D8E File Offset: 0x0006FF8E
	public void LockLock(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		if (player.IsValid())
		{
			player.GiveAchievement("LOCK_LOCK");
		}
	}

	// Token: 0x0400087C RID: 2172
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	// Token: 0x0400087D RID: 2173
	private int keyCode;

	// Token: 0x0400087E RID: 2174
	private bool firstKeyCreated;
}
