using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000041 RID: 65
public class BaseLock : BaseEntity
{
	// Token: 0x0600040D RID: 1037 RVA: 0x00032B70 File Offset: 0x00030D70
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLock.OnRpcMessage", 0))
		{
			if (rpc == 3572556655U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeLock ");
				}
				using (TimeWarning.New("RPC_TakeLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3572556655U, "RPC_TakeLock", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RPC_TakeLock(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_TakeLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00032CD8 File Offset: 0x00030ED8
	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return this.OnTryToOpen(player);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00032CE1 File Offset: 0x00030EE1
	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00032CEC File Offset: 0x00030EEC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeLock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		Analytics.Azure.OnEntityPickedUp(rpc.player, this);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00032D46 File Offset: 0x00030F46
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x0400031C RID: 796
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;
}
