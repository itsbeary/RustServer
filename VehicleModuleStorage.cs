using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EB RID: 235
public class VehicleModuleStorage : VehicleModuleSeating
{
	// Token: 0x060014AE RID: 5294 RVA: 0x000A2BF8 File Offset: 0x000A0DF8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleStorage.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
			if (rpc == 425471188U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TryOpenWithKeycode ");
				}
				using (TimeWarning.New("RPC_TryOpenWithKeycode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(425471188U, "RPC_TryOpenWithKeycode", this, player, 3f))
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
							this.RPC_TryOpenWithKeycode(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_TryOpenWithKeycode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x000A2EF8 File Offset: 0x000A10F8
	public IItemContainerEntity GetContainer()
	{
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as IItemContainerEntity;
		}
		return null;
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x000A2F30 File Offset: 0x000A1130
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.storageUnitInstance.uid = info.msg.simpleUID.uid;
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000A2F54 File Offset: 0x000A1154
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave && this.storage.storageUnitPoint.gameObject.activeSelf)
		{
			this.CreateStorageEntity();
		}
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x000A2F80 File Offset: 0x000A1180
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			global::ItemContainer inventory = container.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000A1F32 File Offset: 0x000A0132
	private void OnItemAddedRemoved(global::Item item, bool add)
	{
		global::Item associatedItemInstance = this.AssociatedItemInstance;
		if (associatedItemInstance == null)
		{
			return;
		}
		associatedItemInstance.LockUnlock(!this.CanBeMovedNowOnVehicle());
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000A2FCC File Offset: 0x000A11CC
	public override void NonUserSpawn()
	{
		Rust.Modular.EngineStorage engineStorage = this.GetContainer() as Rust.Modular.EngineStorage;
		if (engineStorage != null)
		{
			engineStorage.NonUserSpawn();
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x000A2FF4 File Offset: 0x000A11F4
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			IItemContainerEntity container = this.GetContainer();
			if (!container.IsUnityNull<IItemContainerEntity>())
			{
				container.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x000A3024 File Offset: 0x000A1224
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Facepunch.Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.storageUnitInstance.uid;
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000A3058 File Offset: 0x000A1258
	public void CreateStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		if (!this.storageUnitInstance.IsValid(base.isServer))
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.storage.storageUnitPrefab.resourcePath, this.storage.storageUnitPoint.localPosition, this.storage.storageUnitPoint.localRotation, true);
			this.storageUnitInstance.Set(baseEntity);
			baseEntity.SetParent(this, false, false);
			baseEntity.Spawn();
			global::ItemContainer inventory = this.GetContainer().inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000A3110 File Offset: 0x000A1310
	public void DestroyStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			BaseCombatEntity baseCombatEntity;
			if ((baseCombatEntity = baseEntity as BaseCombatEntity) != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x000A3162 File Offset: 0x000A1362
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		this.TryOpen(msg.player);
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x000A3174 File Offset: 0x000A1374
	private bool TryOpen(global::BasePlayer player)
	{
		if (!player.IsValid() || !this.CanBeLooted(player))
		{
			return false;
		}
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			container.PlayerOpenLoot(player, "", true);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": No container component found.");
		}
		return true;
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x000A31D0 File Offset: 0x000A13D0
	protected override bool CanBeMovedNowOnVehicle()
	{
		IItemContainerEntity container = this.GetContainer();
		return container.IsUnityNull<IItemContainerEntity>() || container.inventory.IsEmpty();
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000A31FC File Offset: 0x000A13FC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TryOpenWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		if (!base.IsOnACar)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string text = msg.read.String(256);
		if (base.Car.CarLock.TryOpenWithCode(player, text))
		{
			this.TryOpen(player);
			return;
		}
		base.Car.ClientRPC(null, "CodeEntryFailed");
	}

	// Token: 0x04000D0D RID: 3341
	[SerializeField]
	private VehicleModuleStorage.Storage storage;

	// Token: 0x04000D0E RID: 3342
	private EntityRef storageUnitInstance;

	// Token: 0x02000C22 RID: 3106
	[Serializable]
	public class Storage
	{
		// Token: 0x040042A4 RID: 17060
		public GameObjectRef storageUnitPrefab;

		// Token: 0x040042A5 RID: 17061
		public Transform storageUnitPoint;
	}
}
