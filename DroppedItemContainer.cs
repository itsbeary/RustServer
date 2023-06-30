using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006D RID: 109
public class DroppedItemContainer : BaseCombatEntity, LootPanel.IHasLootPanel, IContainerSounds, ILootableEntity
{
	// Token: 0x06000AA9 RID: 2729 RVA: 0x0006168C File Offset: 0x0005F88C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0))
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

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000AAA RID: 2730 RVA: 0x000617F4 File Offset: 0x0005F9F4
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000AAB RID: 2731 RVA: 0x00061801 File Offset: 0x0005FA01
	// (set) Token: 0x06000AAC RID: 2732 RVA: 0x0006181A File Offset: 0x0005FA1A
	public string playerName
	{
		get
		{
			return NameHelper.Get(this.playerSteamID, this._playerName, base.isClient);
		}
		set
		{
			this._playerName = value;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000AAD RID: 2733 RVA: 0x00061823 File Offset: 0x0005FA23
	// (set) Token: 0x06000AAE RID: 2734 RVA: 0x0006182B File Offset: 0x0005FA2B
	public ulong LastLootedBy { get; set; }

	// Token: 0x06000AAF RID: 2735 RVA: 0x00061834 File Offset: 0x0005FA34
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && (!this.onlyOwnerLoot || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0006186D File Offset: 0x0005FA6D
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0006187B File Offset: 0x0005FA7B
	public void RemoveMe()
	{
		if (base.IsOpen())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00061894 File Offset: 0x0005FA94
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			base.Invoke(new Action(this.RemoveMe), dur);
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x000618DC File Offset: 0x0005FADC
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.CalculateRemovalTime());
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000618EC File Offset: 0x0005FAEC
	public float CalculateRemovalTime()
	{
		if (!this.ItemBasedDespawn)
		{
			return ConVar.Server.itemdespawn * 16f * ConVar.Server.itemdespawn_container_scale;
		}
		float num = ConVar.Server.itemdespawn_quick;
		if (this.inventory != null)
		{
			foreach (global::Item item in this.inventory.itemList)
			{
				num = Mathf.Max(num, item.GetDespawnDuration());
			}
		}
		return num;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00061974 File Offset: 0x0005FB74
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00061998 File Offset: 0x0005FB98
	public void TakeFrom(global::ItemContainer[] source, float destroyPercent = 0f)
	{
		Assert.IsTrue(this.inventory == null, "Initializing Twice");
		using (TimeWarning.New("DroppedItemContainer.TakeFrom", 0))
		{
			int num = 0;
			foreach (global::ItemContainer itemContainer in source)
			{
				num += itemContainer.itemList.Count;
			}
			this.inventory = new global::ItemContainer();
			this.inventory.ServerInitialize(null, Mathf.Min(num, this.maxItemCount));
			this.inventory.GiveUID();
			this.inventory.entityOwner = this;
			this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
			List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
			for (int i = 0; i < source.Length; i++)
			{
				global::Item[] array = source[i].itemList.ToArray();
				int j = 0;
				while (j < array.Length)
				{
					global::Item item = array[j];
					if (destroyPercent <= 0f)
					{
						goto IL_F9;
					}
					if (item.amount != 1)
					{
						item.amount = Mathf.CeilToInt((float)item.amount * (1f - destroyPercent));
						goto IL_F9;
					}
					list.Add(item);
					IL_125:
					j++;
					continue;
					IL_F9:
					if (!item.MoveToContainer(this.inventory, -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
						goto IL_125;
					}
					goto IL_125;
				}
			}
			if (list.Count > 0)
			{
				int num2 = Mathf.FloorToInt((float)list.Count * destroyPercent);
				int num3 = Mathf.Max(0, list.Count - num2);
				list.Shuffle((uint)UnityEngine.Random.Range(0, int.MaxValue));
				for (int k = 0; k < num3; k++)
				{
					global::Item item2 = list[k];
					if (!item2.MoveToContainer(this.inventory, -1, true, false, null, true))
					{
						item2.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			Facepunch.Pool.FreeList<global::Item>(ref list);
			this.ResetRemovalTime();
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00061BA0 File Offset: 0x0005FDA0
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

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00061C2C File Offset: 0x0005FE2C
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		this.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00061C7F File Offset: 0x0005FE7F
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.ServerInitialize(null, 0);
		this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00061CBC File Offset: 0x0005FEBC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00061D60 File Offset: 0x0005FF60
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}

	// Token: 0x040006F1 RID: 1777
	public string lootPanelName = "generic";

	// Token: 0x040006F2 RID: 1778
	public int maxItemCount = 36;

	// Token: 0x040006F3 RID: 1779
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x040006F4 RID: 1780
	[NonSerialized]
	public string _playerName;

	// Token: 0x040006F5 RID: 1781
	public bool ItemBasedDespawn;

	// Token: 0x040006F6 RID: 1782
	public bool onlyOwnerLoot;

	// Token: 0x040006F7 RID: 1783
	public SoundDefinition openSound;

	// Token: 0x040006F8 RID: 1784
	public SoundDefinition closeSound;

	// Token: 0x040006FA RID: 1786
	public global::ItemContainer inventory;
}
