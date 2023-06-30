using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E4 RID: 228
public class TrainCarUnloadable : TrainCar
{
	// Token: 0x0600140D RID: 5133 RVA: 0x0009F6C0 File Offset: 0x0009D8C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainCarUnloadable.OnRpcMessage", 0))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0009F828 File Offset: 0x0009DA28
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old.HasFlag(global::BaseEntity.Flags.Reserved4) != next.HasFlag(global::BaseEntity.Flags.Reserved4) && this.fuelHatches != null)
		{
			this.fuelHatches.LinedUpStateChanged(base.LinedUpToUnload);
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0009F888 File Offset: 0x0009DA88
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		LootContainer lootContainer;
		if (child.TryGetComponent<LootContainer>(out lootContainer))
		{
			if (base.isServer)
			{
				lootContainer.inventory.SetLocked(!this.IsEmpty());
			}
			this.lootContainers.Add(new EntityRef<LootContainer>(lootContainer.net.ID));
		}
		if (base.isServer && child.prefabID == this.storagePrefab.GetEntity().prefabID)
		{
			StorageContainer storageContainer = (StorageContainer)child;
			this.storageInstance.Set(storageContainer);
			if (!Rust.Application.isLoadingSave)
			{
				this.FillWithLoot(storageContainer);
			}
		}
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0009F930 File Offset: 0x0009DB30
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null)
		{
			this.lootTypeIndex = info.msg.baseTrain.lootTypeIndex;
			if (base.isServer)
			{
				this.SetVisualOreLevel(info.msg.baseTrain.lootPercent);
			}
		}
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0009F985 File Offset: 0x0009DB85
	public bool IsEmpty()
	{
		return this.GetOrePercent() == 0f;
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x0009F994 File Offset: 0x0009DB94
	public bool TryGetLootType(out TrainWagonLootData.LootOption lootOption)
	{
		return TrainWagonLootData.instance.TryGetLootFromIndex(this.lootTypeIndex, out lootOption);
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x0009F9A7 File Offset: 0x0009DBA7
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && !this.IsEmpty();
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0009F9C0 File Offset: 0x0009DBC0
	public int GetFilledLootAmount()
	{
		TrainWagonLootData.LootOption lootOption;
		if (this.TryGetLootType(out lootOption))
		{
			return lootOption.maxLootAmount;
		}
		Debug.LogWarning(base.GetType().Name + ": Called GetFilledLootAmount without a lootTypeIndex set.");
		return 0;
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x0009F9FC File Offset: 0x0009DBFC
	public void SetVisualOreLevel(float percent)
	{
		if (this.orePlaneColliderDetailed == null)
		{
			return;
		}
		this._oreScale.y = Mathf.Clamp01(percent);
		this.orePlaneColliderDetailed.localScale = this._oreScale;
		if (base.isClient)
		{
			this.orePlaneVisuals.localScale = this._oreScale;
			this.orePlaneVisuals.gameObject.SetActive(percent > 0f);
		}
		if (base.isServer)
		{
			this.orePlaneColliderWorld.localScale = this._oreScale;
		}
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x0009FA84 File Offset: 0x0009DC84
	private void AnimateUnload(float startPercent)
	{
		this.prevAnimTime = UnityEngine.Time.time;
		this.animPercent = startPercent;
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Play();
		}
		base.InvokeRepeating(new Action(this.UnloadAnimTick), 0f, 0f);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0009FAE0 File Offset: 0x0009DCE0
	private void UnloadAnimTick()
	{
		this.animPercent -= (UnityEngine.Time.time - this.prevAnimTime) / 40f;
		this.SetVisualOreLevel(this.animPercent);
		this.prevAnimTime = UnityEngine.Time.time;
		if (this.animPercent <= 0f)
		{
			this.EndUnloadAnim();
		}
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x0009FB36 File Offset: 0x0009DD36
	private void EndUnloadAnim()
	{
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Stop();
		}
		base.CancelInvoke(new Action(this.UnloadAnimTick));
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0009FB6B File Offset: 0x0009DD6B
	public float GetOrePercent()
	{
		if (base.isServer)
		{
			return TrainWagonLootData.GetOrePercent(this.lootTypeIndex, this.GetStorageContainer());
		}
		return 0f;
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x0009FB8C File Offset: 0x0009DD8C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Facepunch.Pool.Get<BaseTrain>();
		info.msg.baseTrain.lootTypeIndex = this.lootTypeIndex;
		info.msg.baseTrain.lootPercent = this.GetOrePercent();
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0009FBDC File Offset: 0x0009DDDC
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null && !lootContainer.inventory.IsLocked())
				{
					lootContainer.DropItems(null);
				}
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0009FC68 File Offset: 0x0009DE68
	public bool IsLinedUpToUnload(BoxCollider unloaderBounds)
	{
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			if (unloaderBounds.bounds.Intersects(boxCollider.bounds))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0009FCA8 File Offset: 0x0009DEA8
	public void FillWithLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		TrainWagonLootData.LootOption lootOption = TrainWagonLootData.instance.GetLootOption(this.wagonType, out this.lootTypeIndex);
		int num = UnityEngine.Random.Range(lootOption.minLootAmount, lootOption.maxLootAmount);
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(lootOption.lootItem.itemid);
		sc.inventory.AddItem(itemDefinition, num, 0UL, global::ItemContainer.LimitStack.All);
		sc.inventory.SetLocked(true);
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0009FD2E File Offset: 0x0009DF2E
	public void EmptyOutLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0009FD54 File Offset: 0x0009DF54
	public void BeginUnloadAnimation()
	{
		float orePercent = this.GetOrePercent();
		this.AnimateUnload(orePercent);
		base.ClientRPC<float>(null, "RPC_AnimateUnload", orePercent);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0009FD7C File Offset: 0x0009DF7C
	public void EndEmptyProcess()
	{
		float orePercent = this.GetOrePercent();
		if (orePercent <= 0f)
		{
			this.lootTypeIndex = -1;
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null)
				{
					lootContainer.inventory.SetLocked(false);
				}
			}
		}
		this.SetVisualOreLevel(orePercent);
		base.ClientRPC<float>(null, "RPC_StopAnimateUnload", orePercent);
		this.decayingFor = 0f;
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0009FE2C File Offset: 0x0009E02C
	public StorageContainer GetStorageContainer()
	{
		StorageContainer storageContainer = this.storageInstance.Get(base.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0009FE56 File Offset: 0x0009E056
	protected override float GetDecayMinutes(bool hasPassengers)
	{
		if ((this.wagonType == TrainCarUnloadable.WagonType.Ore || this.wagonType == TrainCarUnloadable.WagonType.Fuel) && !hasPassengers && this.IsEmpty())
		{
			return TrainCarUnloadable.decayminutesafterunload;
		}
		return base.GetDecayMinutes(hasPassengers);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0009FE81 File Offset: 0x0009E081
	protected override bool CanDieFromDecayNow()
	{
		return this.IsEmpty() || base.CanDieFromDecayNow();
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0009FE94 File Offset: 0x0009E094
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			if (tier > 1)
			{
				this.FillWithLoot(storageContainer);
			}
			else
			{
				this.EmptyOutLoot(storageContainer);
			}
		}
		return true;
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0009FED0 File Offset: 0x0009E0D0
	public float MinDistToUnloadingArea(Vector3 point)
	{
		float num = float.MaxValue;
		point.y = 0f;
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			Vector3 vector = boxCollider.transform.position + boxCollider.transform.rotation * boxCollider.center;
			vector.y = 0f;
			float num2 = Vector3.Distance(point, vector);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0009FF50 File Offset: 0x0009E150
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			storageContainer.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x04000C7F RID: 3199
	[Header("Train Car Unloadable")]
	[SerializeField]
	private GameObjectRef storagePrefab;

	// Token: 0x04000C80 RID: 3200
	[SerializeField]
	private BoxCollider[] unloadingAreas;

	// Token: 0x04000C81 RID: 3201
	[SerializeField]
	private TrainCarFuelHatches fuelHatches;

	// Token: 0x04000C82 RID: 3202
	[SerializeField]
	private Transform orePlaneVisuals;

	// Token: 0x04000C83 RID: 3203
	[SerializeField]
	private Transform orePlaneColliderDetailed;

	// Token: 0x04000C84 RID: 3204
	[SerializeField]
	private Transform orePlaneColliderWorld;

	// Token: 0x04000C85 RID: 3205
	[SerializeField]
	[Range(0f, 1f)]
	public float vacuumStretchPercent = 0.5f;

	// Token: 0x04000C86 RID: 3206
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainer;

	// Token: 0x04000C87 RID: 3207
	[SerializeField]
	private ParticleSystem unloadingFX;

	// Token: 0x04000C88 RID: 3208
	public TrainCarUnloadable.WagonType wagonType;

	// Token: 0x04000C89 RID: 3209
	private int lootTypeIndex = -1;

	// Token: 0x04000C8A RID: 3210
	private List<EntityRef<LootContainer>> lootContainers = new List<EntityRef<LootContainer>>();

	// Token: 0x04000C8B RID: 3211
	private Vector3 _oreScale = Vector3.one;

	// Token: 0x04000C8C RID: 3212
	private float animPercent;

	// Token: 0x04000C8D RID: 3213
	private float prevAnimTime;

	// Token: 0x04000C8E RID: 3214
	[ServerVar(Help = "How long before an unloadable train car despawns afer being unloaded")]
	public static float decayminutesafterunload = 10f;

	// Token: 0x04000C8F RID: 3215
	private EntityRef<StorageContainer> storageInstance;

	// Token: 0x02000C1C RID: 3100
	public enum WagonType
	{
		// Token: 0x04004280 RID: 17024
		Ore,
		// Token: 0x04004281 RID: 17025
		Lootboxes,
		// Token: 0x04004282 RID: 17026
		Fuel
	}
}
