using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000095 RID: 149
public class LootableCorpse : BaseCorpse, LootPanel.IHasLootPanel
{
	// Token: 0x06000DAB RID: 3499 RVA: 0x00074288 File Offset: 0x00072488
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LootableCorpse.OnRpcMessage", 0))
		{
			if (rpc == 2278459738U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LootCorpse ");
				}
				using (TimeWarning.New("RPC_LootCorpse", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2278459738U, "RPC_LootCorpse", this, player, 3f))
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
							this.RPC_LootCorpse(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_LootCorpse");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000743F0 File Offset: 0x000725F0
	// (set) Token: 0x06000DAD RID: 3501 RVA: 0x00074409 File Offset: 0x00072609
	public virtual string playerName
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

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000DAE RID: 3502 RVA: 0x00074412 File Offset: 0x00072612
	// (set) Token: 0x06000DAF RID: 3503 RVA: 0x0007441A File Offset: 0x0007261A
	public virtual string streamerName { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x00074423 File Offset: 0x00072623
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x00074430 File Offset: 0x00072630
	public Translate.Phrase LootPanelName
	{
		get
		{
			return "N/A";
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x0007443C File Offset: 0x0007263C
	public override void ResetState()
	{
		this.firstLooted = false;
		base.ResetState();
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x0007444B File Offset: 0x0007264B
	// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x00074453 File Offset: 0x00072653
	public bool blockBagDrop { get; set; }

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0007445C File Offset: 0x0007265C
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00074464 File Offset: 0x00072664
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!this.blockBagDrop)
		{
			this.PreDropItems();
			this.DropItems();
		}
		this.blockBagDrop = false;
		if (this.containers != null)
		{
			global::ItemContainer[] array = this.containers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill();
			}
		}
		this.containers = null;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000744C0 File Offset: 0x000726C0
	public void TakeFrom(params global::ItemContainer[] source)
	{
		Assert.IsTrue(this.containers == null, "Initializing Twice");
		using (TimeWarning.New("Corpse.TakeFrom", 0))
		{
			this.containers = new global::ItemContainer[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				this.containers[i] = new global::ItemContainer();
				this.containers[i].ServerInitialize(null, source[i].capacity);
				this.containers[i].GiveUID();
				this.containers[i].entityOwner = this;
				foreach (global::Item item in source[i].itemList.ToArray())
				{
					if (!item.MoveToContainer(this.containers[i], -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			base.ResetRemovalTime();
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000745C0 File Offset: 0x000727C0
	public override bool CanRemove()
	{
		return !base.IsOpen();
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanLoot()
	{
		return true;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x000745CB File Offset: 0x000727CB
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (!this.firstLooted)
		{
			if (this.playerSteamID <= 10000000UL)
			{
				Analytics.Azure.OnFirstLooted(this, baseEntity);
			}
			this.firstLooted = true;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanLootContainer(global::ItemContainer c, int index)
	{
		return true;
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x000745F8 File Offset: 0x000727F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LootCorpse(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.CanLoot())
		{
			return;
		}
		if (this.containers == null)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			for (int i = 0; i < this.containers.Length; i++)
			{
				global::ItemContainer itemContainer = this.containers[i];
				if (this.CanLootContainer(itemContainer, i))
				{
					player.inventory.loot.AddContainer(itemContainer);
				}
			}
			player.inventory.loot.SendImmediate();
			base.ClientRPCPlayer(null, player, "RPC_ClientLootCorpse");
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000746A8 File Offset: 0x000728A8
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void PreDropItems()
	{
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x000746C4 File Offset: 0x000728C4
	public void DropItems()
	{
		if (Global.disableBagDropping)
		{
			return;
		}
		if (this.containers != null)
		{
			DroppedItemContainer droppedItemContainer = global::ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", base.transform.position, Quaternion.identity, this.containers);
			if (droppedItemContainer != null)
			{
				droppedItemContainer.playerName = this.playerName;
				droppedItemContainer.playerSteamID = this.playerSteamID;
			}
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00074724 File Offset: 0x00072924
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		info.msg.lootableCorpse.streamerName = this.streamerName;
		if (info.forDisk && this.containers != null)
		{
			info.msg.lootableCorpse.privateData = Facepunch.Pool.Get<ProtoBuf.LootableCorpse.Private>();
			info.msg.lootableCorpse.privateData.container = Facepunch.Pool.GetList<ProtoBuf.ItemContainer>();
			foreach (global::ItemContainer itemContainer in this.containers)
			{
				if (itemContainer != null)
				{
					ProtoBuf.ItemContainer itemContainer2 = itemContainer.Save();
					if (itemContainer2 != null)
					{
						info.msg.lootableCorpse.privateData.container.Add(itemContainer2);
					}
				}
			}
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0007480C File Offset: 0x00072A0C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.streamerName = info.msg.lootableCorpse.streamerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			if (info.fromDisk && info.msg.lootableCorpse.privateData != null && info.msg.lootableCorpse.privateData.container != null)
			{
				int count = info.msg.lootableCorpse.privateData.container.Count;
				this.containers = new global::ItemContainer[count];
				for (int i = 0; i < count; i++)
				{
					this.containers[i] = new global::ItemContainer();
					this.containers[i].ServerInitialize(null, info.msg.lootableCorpse.privateData.container[i].slots);
					this.containers[i].GiveUID();
					this.containers[i].entityOwner = this;
					this.containers[i].Load(info.msg.lootableCorpse.privateData.container[i]);
				}
			}
		}
	}

	// Token: 0x040008BD RID: 2237
	public string lootPanelName = "generic";

	// Token: 0x040008BE RID: 2238
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x040008BF RID: 2239
	[NonSerialized]
	public string _playerName;

	// Token: 0x040008C2 RID: 2242
	[NonSerialized]
	public global::ItemContainer[] containers;

	// Token: 0x040008C3 RID: 2243
	[NonSerialized]
	private bool firstLooted;
}
