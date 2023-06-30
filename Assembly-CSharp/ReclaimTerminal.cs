using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B9 RID: 185
public class ReclaimTerminal : StorageContainer
{
	// Token: 0x060010A4 RID: 4260 RVA: 0x000895CC File Offset: 0x000877CC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReclaimTerminal.OnRpcMessage", 0))
		{
			if (rpc == 2609933020U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ReloadLoot ");
				}
				using (TimeWarning.New("RPC_ReloadLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2609933020U, "RPC_ReloadLoot", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2609933020U, "RPC_ReloadLoot", this, player, 3f))
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
							this.RPC_ReloadLoot(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ReloadLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x0008978C File Offset: 0x0008798C
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000897A8 File Offset: 0x000879A8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_ReloadLoot(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		if (player.inventory.loot.entitySource != this)
		{
			return;
		}
		this.LoadReclaimLoot(player);
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000897F4 File Offset: 0x000879F4
	public void LoadReclaimLoot(global::BasePlayer player)
	{
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		List<global::ReclaimManager.PlayerReclaimEntry> list = Facepunch.Pool.GetList<global::ReclaimManager.PlayerReclaimEntry>();
		global::ReclaimManager.instance.GetReclaimsForPlayer(player.userID, ref list);
		this.itemCount = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				this.itemCount++;
			}
		}
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in list)
		{
			for (int j = playerReclaimEntry.inventory.itemList.Count - 1; j >= 0; j--)
			{
				global::Item item = playerReclaimEntry.inventory.itemList[j];
				this.itemCount++;
				item.MoveToContainer(base.inventory, -1, true, false, null, true);
			}
		}
		Facepunch.Pool.FreeList<global::ReclaimManager.PlayerReclaimEntry>(ref list);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000898FC File Offset: 0x00087AFC
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (global::ReclaimManager.instance == null)
		{
			return false;
		}
		this.LoadReclaimLoot(player);
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00089920 File Offset: 0x00087B20
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		global::ReclaimManager.instance.DoCleanup();
		if (base.inventory.itemList.Count > 0)
		{
			global::ReclaimManager.instance.AddPlayerReclaim(player.userID, base.inventory.itemList, 0UL, "", -1);
			player.ShowToast(GameTip.Styles.Blue_Long, global::ReclaimTerminal.DespawnToast, Array.Empty<string>());
		}
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00089994 File Offset: 0x00087B94
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.reclaimTerminal = Facepunch.Pool.Get<ProtoBuf.ReclaimTerminal>();
			info.msg.reclaimTerminal.itemCount = this.itemCount;
		}
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x000899CB File Offset: 0x00087BCB
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.reclaimTerminal != null)
		{
			this.itemCount = info.msg.reclaimTerminal.itemCount;
		}
	}

	// Token: 0x04000A98 RID: 2712
	public int itemCount;

	// Token: 0x04000A99 RID: 2713
	public static readonly Translate.Phrase DespawnToast = new Translate.Phrase("softcore.reclaimdespawn", "Items remaining in the reclaim terminal will despawn in two hours.");
}
