using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.CardGames;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000039 RID: 57
public abstract class BaseCardGameEntity : global::BaseVehicle
{
	// Token: 0x06000241 RID: 577 RVA: 0x00027CD4 File Offset: 0x00025ED4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCardGameEntity.OnRpcMessage", 0))
		{
			if (rpc == 2395020190U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_MakeRandomMove ");
				}
				using (TimeWarning.New("RPC_Editor_MakeRandomMove", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2395020190U, "RPC_Editor_MakeRandomMove", this, player, 3f))
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
							this.RPC_Editor_MakeRandomMove(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Editor_MakeRandomMove");
					}
				}
				return true;
			}
			if (rpc == 1608700874U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_SpawnTestPlayer ");
				}
				using (TimeWarning.New("RPC_Editor_SpawnTestPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1608700874U, "RPC_Editor_SpawnTestPlayer", this, player, 3f))
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
							this.RPC_Editor_SpawnTestPlayer(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Editor_SpawnTestPlayer");
					}
				}
				return true;
			}
			if (rpc == 1499640189U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LeaveTable ");
				}
				using (TimeWarning.New("RPC_LeaveTable", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1499640189U, "RPC_LeaveTable", this, player, 3f))
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
							this.RPC_LeaveTable(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_LeaveTable");
					}
				}
				return true;
			}
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
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
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
			if (rpc == 2847205856U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Play ");
				}
				using (TimeWarning.New("RPC_Play", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2847205856U, "RPC_Play", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Play(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_Play");
					}
				}
				return true;
			}
			if (rpc == 2495306863U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PlayerInput ");
				}
				using (TimeWarning.New("RPC_PlayerInput", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2495306863U, "RPC_PlayerInput", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PlayerInput(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_PlayerInput");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000242 RID: 578 RVA: 0x00028540 File Offset: 0x00026740
	public int ScrapItemID
	{
		get
		{
			return this.scrapItemDef.itemid;
		}
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000243 RID: 579 RVA: 0x0002854D File Offset: 0x0002674D
	public CardGameController GameController
	{
		get
		{
			if (this._gameCont == null)
			{
				this._gameCont = this.GetGameController();
			}
			return this._gameCont;
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000244 RID: 580
	protected abstract float MaxStorageInteractionDist { get; }

	// Token: 0x06000245 RID: 581 RVA: 0x00028569 File Offset: 0x00026769
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			this.PotInstance.uid = info.msg.cardGame.potRef;
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00028598 File Offset: 0x00026798
	private CardGameController GetGameController()
	{
		BaseCardGameEntity.CardGameOption cardGameOption = this.gameOption;
		if (cardGameOption == BaseCardGameEntity.CardGameOption.TexasHoldEm)
		{
			return new TexasHoldEmController(this);
		}
		if (cardGameOption != BaseCardGameEntity.CardGameOption.Blackjack)
		{
			return new TexasHoldEmController(this);
		}
		return new BlackjackController(this);
	}

	// Token: 0x06000247 RID: 583 RVA: 0x000285C9 File Offset: 0x000267C9
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.GameController.Dispose();
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000248 RID: 584 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanSwapSeats
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000285DC File Offset: 0x000267DC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cardGame = Facepunch.Pool.Get<CardGame>();
		info.msg.cardGame.potRef = this.PotInstance.uid;
		if (!info.forDisk && this.storageLinked)
		{
			this.GameController.Save(info.msg.cardGame);
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00028644 File Offset: 0x00026844
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		int num = 0;
		int num2 = 0;
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardGamePlayerStorage cardGamePlayerStorage;
				if ((cardGamePlayerStorage = enumerator.Current as CardGamePlayerStorage) != null)
				{
					this.playerStoragePoints[num].storageInstance.Set(cardGamePlayerStorage);
					if (!cardGamePlayerStorage.inventory.IsEmpty())
					{
						num2++;
					}
					num++;
				}
			}
		}
		this.storageLinked = true;
		bool flag = true;
		StorageContainer pot = this.GetPot();
		if (pot == null)
		{
			flag = false;
		}
		else
		{
			int num3 = ((num2 > 0) ? num2 : this.playerStoragePoints.Length);
			int num4 = Mathf.CeilToInt((float)(pot.inventory.GetAmount(this.ScrapItemID, true) / num3));
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				CardGamePlayerStorage cardGamePlayerStorage2 = array[i].storageInstance.Get(base.isServer) as CardGamePlayerStorage;
				if (cardGamePlayerStorage2.IsValid() && (!cardGamePlayerStorage2.inventory.IsEmpty() || num2 == 0))
				{
					List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
					if (pot.inventory.Take(list, this.ScrapItemID, num4) > 0)
					{
						foreach (global::Item item in list)
						{
							if (!item.MoveToContainer(cardGamePlayerStorage2.inventory, -1, true, true, null, true))
							{
								item.Remove(0f);
							}
						}
					}
					Facepunch.Pool.FreeList<global::Item>(ref list);
				}
			}
		}
		if (flag)
		{
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].storageInstance.IsValid(base.isServer))
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning(base.GetType().Name + ": Card game storage didn't load in. Destroying the card game (and parent entity if there is one).");
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				parentEntity.Invoke(new Action(parentEntity.KillMessage), 0f);
				return;
			}
			base.Invoke(new Action(base.KillMessage), 0f);
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00028890 File Offset: 0x00026A90
	internal override void DoServerDestroy()
	{
		CardGameController gameController = this.GameController;
		if (gameController != null)
		{
			gameController.OnTableDestroyed();
		}
		BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayerStorage storage = array[i].GetStorage();
			if (storage != null)
			{
				storage.DropItems(null);
			}
		}
		StorageContainer pot = this.GetPot();
		if (pot != null)
		{
			pot.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600024C RID: 588 RVA: 0x000288FC File Offset: 0x00026AFC
	public override void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		if (!Rust.Application.isLoadingSave)
		{
			CardGamePlayerStorage playerStorage = this.GetPlayerStorage(player.userID);
			if (playerStorage != null)
			{
				global::Item slot = playerStorage.inventory.GetSlot(0);
				if (slot != null)
				{
					slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
				}
			}
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00028955 File Offset: 0x00026B55
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		this.GameController.LeaveTable(player.userID);
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00028970 File Offset: 0x00026B70
	public StorageContainer GetPot()
	{
		global::BaseEntity baseEntity = this.PotInstance.Get(true);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x000289A4 File Offset: 0x00026BA4
	public global::BasePlayer IDToPlayer(ulong id)
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null && mountPointInfo.mountable.GetMounted().userID == id)
			{
				return mountPointInfo.mountable.GetMounted();
			}
		}
		return null;
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00028A38 File Offset: 0x00026C38
	public virtual void PlayerStorageChanged()
	{
		this.GameController.PlayerStorageChanged();
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00028A45 File Offset: 0x00026C45
	public CardGamePlayerStorage GetPlayerStorage(int storageIndex)
	{
		return this.playerStoragePoints[storageIndex].GetStorage();
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00028A54 File Offset: 0x00026C54
	public CardGamePlayerStorage GetPlayerStorage(ulong playerID)
	{
		int mountPointIndex = this.GetMountPointIndex(playerID);
		if (mountPointIndex < 0)
		{
			return null;
		}
		return this.playerStoragePoints[mountPointIndex].GetStorage();
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00028A7C File Offset: 0x00026C7C
	public int GetMountPointIndex(ulong playerID)
	{
		int num = -1;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			BaseMountable mountable = this.mountPoints[i].mountable;
			if (mountable != null)
			{
				global::BasePlayer mounted = mountable.GetMounted();
				if (mounted != null && mounted.userID == playerID)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't find mount point for this player.");
		}
		return num;
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00028AF8 File Offset: 0x00026CF8
	public override void SpawnSubEntities()
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.potPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		StorageContainer storageContainer = baseEntity as StorageContainer;
		if (storageContainer != null)
		{
			storageContainer.SetParent(this, false, false);
			storageContainer.Spawn();
			this.PotInstance.Set(baseEntity);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Spawned prefab is not a StorageContainer as expected.");
		}
		foreach (BaseCardGameEntity.PlayerStorageInfo playerStorageInfo in this.playerStoragePoints)
		{
			baseEntity = GameManager.server.CreateEntity(this.playerStoragePrefab.resourcePath, playerStorageInfo.storagePos.localPosition, playerStorageInfo.storagePos.localRotation, true);
			CardGamePlayerStorage cardGamePlayerStorage = baseEntity as CardGamePlayerStorage;
			if (cardGamePlayerStorage != null)
			{
				cardGamePlayerStorage.SetCardTable(this);
				cardGamePlayerStorage.SetParent(this, false, false);
				cardGamePlayerStorage.Spawn();
				playerStorageInfo.storageInstance.Set(baseEntity);
				this.storageLinked = true;
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": Spawned prefab is not a CardTablePlayerStorage as expected.");
			}
		}
		base.SpawnSubEntities();
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00028C1D File Offset: 0x00026E1D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_PlayerInput(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.ReceivedInputFromPlayer(msg.player, msg.read.Int32(), true, msg.read.Int32());
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00028C47 File Offset: 0x00026E47
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LeaveTable(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.LeaveTable(msg.player.userID);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00028C60 File Offset: 0x00026E60
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GetPlayerStorage(player.userID).PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00028CA0 File Offset: 0x00026EA0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_SpawnTestPlayer(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		int num = this.GameController.MaxPlayersAtTable();
		if (this.GameController.NumPlayersAllowedToPlay(null) >= num || base.NumMounted() >= num)
		{
			return;
		}
		Debug.Log("Adding test NPC for card game");
		global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		global::BasePlayer basePlayer = (global::BasePlayer)baseEntity;
		this.AttemptMount(basePlayer, false);
		this.GameController.JoinTable(basePlayer);
		CardPlayerData cardPlayerData;
		if (this.GameController.TryGetCardPlayerData(basePlayer, out cardPlayerData))
		{
			int scrapAmount = cardPlayerData.GetScrapAmount();
			if (scrapAmount < 400)
			{
				StorageContainer storage = cardPlayerData.GetStorage();
				if (storage != null)
				{
					storage.inventory.AddItem(this.scrapItemDef, 400 - scrapAmount, 0UL, global::ItemContainer.LimitStack.Existing);
					return;
				}
				Debug.LogError("Couldn't get storage for NPC.");
				return;
			}
		}
		else
		{
			Debug.Log("Couldn't find player data for NPC. No scrap given.");
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00028D87 File Offset: 0x00026F87
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_MakeRandomMove(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		this.GameController.EditorMakeRandomMove();
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00028D9C File Offset: 0x00026F9C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_Play(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GameController.JoinTable(player);
		}
	}

	// Token: 0x04000227 RID: 551
	[Header("Card Game")]
	[SerializeField]
	private GameObjectRef uiPrefab;

	// Token: 0x04000228 RID: 552
	public ItemDefinition scrapItemDef;

	// Token: 0x04000229 RID: 553
	[SerializeField]
	private GameObjectRef potPrefab;

	// Token: 0x0400022A RID: 554
	public BaseCardGameEntity.PlayerStorageInfo[] playerStoragePoints;

	// Token: 0x0400022B RID: 555
	[SerializeField]
	private GameObjectRef playerStoragePrefab;

	// Token: 0x0400022C RID: 556
	private CardGameController _gameCont;

	// Token: 0x0400022D RID: 557
	public BaseCardGameEntity.CardGameOption gameOption;

	// Token: 0x0400022E RID: 558
	public EntityRef PotInstance;

	// Token: 0x0400022F RID: 559
	private bool storageLinked;

	// Token: 0x02000B7B RID: 2939
	[Serializable]
	public class PlayerStorageInfo
	{
		// Token: 0x06004D2B RID: 19755 RVA: 0x001A0304 File Offset: 0x0019E504
		public CardGamePlayerStorage GetStorage()
		{
			global::BaseEntity baseEntity = this.storageInstance.Get(true);
			if (baseEntity != null && baseEntity.IsValid())
			{
				return baseEntity as CardGamePlayerStorage;
			}
			return null;
		}

		// Token: 0x04003F93 RID: 16275
		public Transform storagePos;

		// Token: 0x04003F94 RID: 16276
		public EntityRef storageInstance;
	}

	// Token: 0x02000B7C RID: 2940
	public enum CardGameOption
	{
		// Token: 0x04003F96 RID: 16278
		TexasHoldEm,
		// Token: 0x04003F97 RID: 16279
		Blackjack
	}
}
