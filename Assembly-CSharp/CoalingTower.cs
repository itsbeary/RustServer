using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x02000020 RID: 32
public class CoalingTower : global::IOEntity, INotifyEntityTrigger
{
	// Token: 0x06000096 RID: 150 RVA: 0x00004DF0 File Offset: 0x00002FF0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.coalingTower = Facepunch.Pool.Get<ProtoBuf.CoalingTower>();
		info.msg.coalingTower.lootTypeIndex = this.LootTypeIndex;
		info.msg.coalingTower.oreStorageID = this.oreStorageInstance.uid;
		info.msg.coalingTower.fuelStorageID = this.fuelStorageInstance.uid;
		info.msg.coalingTower.activeUnloadableID = this.activeTrainCarRef.uid;
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00004E80 File Offset: 0x00003080
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00004EE0 File Offset: 0x000030E0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == this.oreStoragePrefab.GetEntity().prefabID)
			{
				this.oreStorageInstance.Set((OreHopper)child);
				return;
			}
			if (child.prefabID == this.fuelStoragePrefab.GetEntity().prefabID)
			{
				this.fuelStorageInstance.Set((PercentFullStorageContainer)child);
			}
		}
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00004F4F File Offset: 0x0000314F
	public void OnEmpty()
	{
		this.ClearActiveTrainCar();
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00004F58 File Offset: 0x00003158
	public void OnEntityEnter(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			return;
		}
		if (ent.isClient)
		{
			return;
		}
		TrainCar trainCar = ent as TrainCar;
		if (trainCar != null)
		{
			this.SetActiveTrainCar(trainCar);
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00004F90 File Offset: 0x00003190
	public void OnEntityLeave(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			return;
		}
		if (ent.isClient)
		{
			return;
		}
		global::BaseEntity baseEntity = ent.parentEntity.Get(base.isServer);
		TrainCar trainCar = this.activeTrainCarRef.Get(true);
		if (trainCar == ent && trainCar != baseEntity)
		{
			this.ClearActiveTrainCar();
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00004FE8 File Offset: 0x000031E8
	private void SetActiveTrainCar(TrainCar trainCar)
	{
		if (this.GetActiveTrainCar() == trainCar)
		{
			return;
		}
		this.activeTrainCarRef.Set(trainCar);
		TrainCarUnloadable trainCarUnloadable;
		if ((trainCarUnloadable = trainCar as TrainCarUnloadable) != null)
		{
			this.activeUnloadableRef.Set(trainCarUnloadable);
		}
		else
		{
			this.activeUnloadableRef.Set(null);
		}
		bool flag = this.activeUnloadableRef.IsValid(true);
		this.CheckWagonLinedUp(false);
		if (flag)
		{
			base.InvokeRandomized(new Action(this.CheckWagonLinedUp), 0.15f, 0.15f, 0.015f);
		}
		else
		{
			base.CancelInvoke(new Action(this.CheckWagonLinedUp));
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00005085 File Offset: 0x00003285
	private void ClearActiveTrainCar()
	{
		this.SetActiveTrainCar(null);
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000508E File Offset: 0x0000328E
	private void CheckWagonLinedUp()
	{
		this.CheckWagonLinedUp(true);
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00005098 File Offset: 0x00003298
	private void CheckWagonLinedUp(bool networkUpdate)
	{
		bool flag = false;
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable != null)
		{
			flag = activeUnloadable.IsLinedUpToUnload(this.unloadingBounds);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, flag, false, networkUpdate);
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000050D4 File Offset: 0x000032D4
	private bool TryUnloadActiveWagon(out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		TrainCarUnloadable.WagonType wagonType = activeUnloadable.wagonType;
		if (!this.CanUnloadNow(out attemptStatus))
		{
			return false;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.WagonBeginUnloadAnim), this.vacuumStartDelay);
		return true;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00005130 File Offset: 0x00003330
	private void WagonBeginUnloadAnim()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		TrainWagonLootData.LootOption lootOption;
		if (!activeUnloadable.TryGetLootType(out lootOption))
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		int num;
		TrainWagonLootData.instance.TryGetIndexFromLoot(lootOption, out num);
		this.LootTypeIndex.Value = num;
		this.tcUnloadingNow = activeUnloadable;
		this.tcUnloadingNow.BeginUnloadAnimation();
		float num2 = 4f;
		base.InvokeRepeating(new Action(this.EmptyTenPercent), 0f, num2);
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000051C0 File Offset: 0x000033C0
	private void EmptyTenPercent()
	{
		if (!this.IsPowered())
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.GenericError);
			return;
		}
		if (!this.HasUnloadableLinedUp)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (this.tcUnloadingNow == null || activeUnloadable != this.tcUnloadingNow)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		StorageContainer storageContainer = this.tcUnloadingNow.GetStorageContainer();
		TrainWagonLootData.LootOption lootOption;
		if (storageContainer.inventory == null || !TrainWagonLootData.instance.TryGetLootFromIndex(this.LootTypeIndex, out lootOption))
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		bool flag = this.tcUnloadingNow.wagonType != TrainCarUnloadable.WagonType.Fuel;
		global::ItemContainer itemContainer = null;
		PercentFullStorageContainer percentFullStorageContainer = (flag ? this.GetOreStorage() : this.GetFuelStorage());
		if (percentFullStorageContainer != null)
		{
			itemContainer = percentFullStorageContainer.inventory;
		}
		if (itemContainer == null)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.GenericError);
			return;
		}
		global::ItemContainer inventory = storageContainer.inventory;
		global::ItemContainer itemContainer2 = itemContainer;
		int num = Mathf.RoundToInt((float)lootOption.maxLootAmount / 10f);
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		int num2 = inventory.Take(list, lootOption.lootItem.itemid, num);
		bool flag2 = true;
		if (num2 > 0)
		{
			foreach (global::Item item in list)
			{
				if (this.tcUnloadingNow.wagonType == TrainCarUnloadable.WagonType.Lootboxes)
				{
					item.Remove(0f);
				}
				else
				{
					bool flag3 = item.MoveToContainer(itemContainer2, -1, true, false, null, true);
					if (flag2 && !flag3)
					{
						item.MoveToContainer(inventory, -1, true, false, null, true);
						flag2 = false;
						break;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		float orePercent = this.tcUnloadingNow.GetOrePercent();
		if (orePercent == 0f)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoError);
			return;
		}
		if (!flag2)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.OutputIsFull);
			return;
		}
		if (flag)
		{
			this.tcUnloadingNow.SetVisualOreLevel(orePercent);
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x000053A0 File Offset: 0x000035A0
	private void EndEmptyProcess(global::CoalingTower.ActionAttemptStatus status)
	{
		base.CancelInvoke(new Action(this.EmptyTenPercent));
		base.CancelInvoke(new Action(this.WagonBeginUnloadAnim));
		if (this.tcUnloadingNow != null)
		{
			this.tcUnloadingNow.EndEmptyProcess();
			this.tcUnloadingNow = null;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (status != global::CoalingTower.ActionAttemptStatus.NoError)
		{
			base.ClientRPC<byte, bool>(null, "ActionFailed", (byte)status, false);
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005418 File Offset: 0x00003618
	private bool TryShuntTrain(bool next, out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		if (!this.IsPowered() || base.HasFlag(global::BaseEntity.Flags.Reserved3) || base.HasFlag(global::BaseEntity.Flags.Reserved4))
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.GenericError;
			return false;
		}
		TrainCar activeTrainCar = this.GetActiveTrainCar();
		if (activeTrainCar == null)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		Vector3 unloadingPos = this.UnloadingPos;
		unloadingPos.y = 0f;
		TrainCar trainCar;
		if (activeTrainCar is TrainCarUnloadable && !this.HasUnloadableLinedUp)
		{
			Vector3 position = activeTrainCar.transform.position;
			Vector3 vector = unloadingPos - position;
			if (Vector3.Dot(base.transform.forward, vector) >= 0f == next)
			{
				trainCar = activeTrainCar;
				goto IL_BA;
			}
		}
		if (!activeTrainCar.TryGetTrainCar(next, base.transform.forward, out trainCar))
		{
			attemptStatus = (next ? global::CoalingTower.ActionAttemptStatus.NoNextTrainCar : global::CoalingTower.ActionAttemptStatus.NoPrevTrainCar);
			return false;
		}
		IL_BA:
		Vector3 position2 = trainCar.transform.position;
		position2.y = 0f;
		Vector3 vector2 = unloadingPos - position2;
		float magnitude = vector2.magnitude;
		return activeTrainCar.completeTrain.TryShuntCarTo(vector2, magnitude, trainCar, new Action<global::CoalingTower.ActionAttemptStatus>(this.ShuntEnded), out attemptStatus);
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00005526 File Offset: 0x00003726
	private void ShuntEnded(global::CoalingTower.ActionAttemptStatus status)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, true);
		if (status != global::CoalingTower.ActionAttemptStatus.NoError)
		{
			base.ClientRPC(null, "IssueDuringShunt");
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005554 File Offset: 0x00003754
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Unload(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (!this.TryUnloadActiveWagon(out actionAttemptStatus) && msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005590 File Offset: 0x00003790
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Next(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (this.TryShuntTrain(true, out actionAttemptStatus))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
			return;
		}
		if (msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x000055DC File Offset: 0x000037DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Prev(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (this.TryShuntTrain(false, out actionAttemptStatus))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, true, false, true);
			return;
		}
		if (msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060000A9 RID: 169 RVA: 0x00005626 File Offset: 0x00003826
	private bool HasTrainCar
	{
		get
		{
			return this.activeTrainCarRef.IsValid(base.isServer);
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060000AA RID: 170 RVA: 0x00005639 File Offset: 0x00003839
	private bool HasUnloadable
	{
		get
		{
			return this.activeUnloadableRef.IsValid(base.isServer);
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060000AB RID: 171 RVA: 0x0000564C File Offset: 0x0000384C
	private bool HasUnloadableLinedUp
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060000AC RID: 172 RVA: 0x00005659 File Offset: 0x00003859
	// (set) Token: 0x060000AD RID: 173 RVA: 0x00005661 File Offset: 0x00003861
	public Vector3 UnloadingPos { get; private set; }

	// Token: 0x060000AE RID: 174 RVA: 0x0000566C File Offset: 0x0000386C
	public override void InitShared()
	{
		base.InitShared();
		this.LootTypeIndex = new NetworkedProperty<int>(this);
		this.UnloadingPos = this.unloadingBounds.transform.position + this.unloadingBounds.transform.rotation * this.unloadingBounds.center;
		global::CoalingTower.unloadersInWorld.Add(this);
	}

	// Token: 0x060000AF RID: 175 RVA: 0x000056D1 File Offset: 0x000038D1
	public override void DestroyShared()
	{
		base.DestroyShared();
		global::CoalingTower.unloadersInWorld.Remove(this);
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x000056E8 File Offset: 0x000038E8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.coalingTower != null)
		{
			this.LootTypeIndex.Value = info.msg.coalingTower.lootTypeIndex;
			this.oreStorageInstance.uid = info.msg.coalingTower.oreStorageID;
			this.fuelStorageInstance.uid = info.msg.coalingTower.fuelStorageID;
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x0000575C File Offset: 0x0000395C
	public static bool IsUnderAnUnloader(TrainCar trainCar, out bool isLinedUp, out Vector3 unloaderPos)
	{
		foreach (global::CoalingTower coalingTower in global::CoalingTower.unloadersInWorld)
		{
			if (coalingTower.TrainCarIsUnder(trainCar, out isLinedUp))
			{
				unloaderPos = coalingTower.UnloadingPos;
				return true;
			}
		}
		isLinedUp = false;
		unloaderPos = Vector3.zero;
		return false;
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x000057D4 File Offset: 0x000039D4
	public bool TrainCarIsUnder(TrainCar trainCar, out bool isLinedUp)
	{
		isLinedUp = false;
		if (!trainCar.IsValid())
		{
			return false;
		}
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable != null && activeUnloadable.EqualNetID(trainCar))
		{
			isLinedUp = this.HasUnloadableLinedUp;
			return true;
		}
		return false;
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00005814 File Offset: 0x00003A14
	private OreHopper GetOreStorage()
	{
		OreHopper oreHopper = this.oreStorageInstance.Get(base.isServer);
		if (oreHopper.IsValid())
		{
			return oreHopper;
		}
		return null;
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005840 File Offset: 0x00003A40
	private PercentFullStorageContainer GetFuelStorage()
	{
		PercentFullStorageContainer percentFullStorageContainer = this.fuelStorageInstance.Get(base.isServer);
		if (percentFullStorageContainer.IsValid())
		{
			return percentFullStorageContainer;
		}
		return null;
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0000586C File Offset: 0x00003A6C
	private TrainCar GetActiveTrainCar()
	{
		TrainCar trainCar = this.activeTrainCarRef.Get(base.isServer);
		if (trainCar.IsValid())
		{
			return trainCar;
		}
		return null;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005898 File Offset: 0x00003A98
	private TrainCarUnloadable GetActiveUnloadable()
	{
		TrainCarUnloadable trainCarUnloadable = this.activeUnloadableRef.Get(base.isServer);
		if (trainCarUnloadable.IsValid())
		{
			return trainCarUnloadable;
		}
		return null;
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000058C4 File Offset: 0x00003AC4
	private bool OutputBinIsFull()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			return false;
		}
		TrainCarUnloadable.WagonType wagonType = activeUnloadable.wagonType;
		if (wagonType == TrainCarUnloadable.WagonType.Lootboxes)
		{
			return false;
		}
		if (wagonType != TrainCarUnloadable.WagonType.Fuel)
		{
			OreHopper oreStorage = this.GetOreStorage();
			return oreStorage != null && oreStorage.IsFull();
		}
		PercentFullStorageContainer fuelStorage = this.GetFuelStorage();
		return fuelStorage != null && fuelStorage.IsFull();
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005928 File Offset: 0x00003B28
	private bool WagonIsEmpty()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		return !(activeUnloadable != null) || activeUnloadable.GetOrePercent() == 0f;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005954 File Offset: 0x00003B54
	private bool CanUnloadNow(out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		if (!this.HasUnloadableLinedUp)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (this.OutputBinIsFull())
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.OutputIsFull;
			return false;
		}
		attemptStatus = global::CoalingTower.ActionAttemptStatus.NoError;
		return this.IsPowered();
	}

	// Token: 0x060000BA RID: 186 RVA: 0x0000597C File Offset: 0x00003B7C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CoalingTower.OnRpcMessage", 0))
		{
			if (rpc == 3071873383U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Next ");
				}
				using (TimeWarning.New("RPC_Next", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3071873383U, "RPC_Next", this, player, 3f))
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
							this.RPC_Next(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Next");
					}
				}
				return true;
			}
			if (rpc == 3656312045U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Prev ");
				}
				using (TimeWarning.New("RPC_Prev", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3656312045U, "RPC_Prev", this, player, 3f))
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
							this.RPC_Prev(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Prev");
					}
				}
				return true;
			}
			if (rpc == 998476828U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Unload ");
				}
				using (TimeWarning.New("RPC_Unload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(998476828U, "RPC_Unload", this, player, 3f))
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
							this.RPC_Unload(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Unload");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000081 RID: 129
	private TrainCarUnloadable tcUnloadingNow;

	// Token: 0x04000082 RID: 130
	[Header("Coaling Tower")]
	[SerializeField]
	private BoxCollider unloadingBounds;

	// Token: 0x04000083 RID: 131
	[SerializeField]
	private GameObjectRef oreStoragePrefab;

	// Token: 0x04000084 RID: 132
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04000085 RID: 133
	[SerializeField]
	private MeshRenderer[] signalLightsExterior;

	// Token: 0x04000086 RID: 134
	[SerializeField]
	private MeshRenderer[] signalLightsInterior;

	// Token: 0x04000087 RID: 135
	[ColorUsage(false, true)]
	public Color greenLightOnColour;

	// Token: 0x04000088 RID: 136
	[ColorUsage(false, true)]
	public Color yellowLightOnColour;

	// Token: 0x04000089 RID: 137
	[SerializeField]
	private Animator vacuumAnimator;

	// Token: 0x0400008A RID: 138
	[SerializeField]
	private float vacuumStartDelay = 2f;

	// Token: 0x0400008B RID: 139
	[FormerlySerializedAs("unloadingFXContainer")]
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerOre;

	// Token: 0x0400008C RID: 140
	[SerializeField]
	private ParticleSystem[] unloadingFXMain;

	// Token: 0x0400008D RID: 141
	[SerializeField]
	private ParticleSystem[] unloadingFXDust;

	// Token: 0x0400008E RID: 142
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerFuel;

	// Token: 0x0400008F RID: 143
	[Header("Coaling Tower Text")]
	[SerializeField]
	private TokenisedPhrase noTraincar;

	// Token: 0x04000090 RID: 144
	[SerializeField]
	private TokenisedPhrase noNextTraincar;

	// Token: 0x04000091 RID: 145
	[SerializeField]
	private TokenisedPhrase noPrevTraincar;

	// Token: 0x04000092 RID: 146
	[SerializeField]
	private TokenisedPhrase trainIsMoving;

	// Token: 0x04000093 RID: 147
	[SerializeField]
	private TokenisedPhrase outputIsFull;

	// Token: 0x04000094 RID: 148
	[SerializeField]
	private TokenisedPhrase trainHasThrottle;

	// Token: 0x04000095 RID: 149
	[Header("Coaling Tower Audio")]
	[SerializeField]
	private GameObject buttonSoundPos;

	// Token: 0x04000096 RID: 150
	[SerializeField]
	private SoundDefinition buttonPressSound;

	// Token: 0x04000097 RID: 151
	[SerializeField]
	private SoundDefinition buttonReleaseSound;

	// Token: 0x04000098 RID: 152
	[SerializeField]
	private SoundDefinition failedActionSound;

	// Token: 0x04000099 RID: 153
	[SerializeField]
	private SoundDefinition failedShuntAlarmSound;

	// Token: 0x0400009A RID: 154
	[SerializeField]
	private SoundDefinition armMovementLower;

	// Token: 0x0400009B RID: 155
	[SerializeField]
	private SoundDefinition armMovementRaise;

	// Token: 0x0400009C RID: 156
	[SerializeField]
	private SoundDefinition suctionAirStart;

	// Token: 0x0400009D RID: 157
	[SerializeField]
	private SoundDefinition suctionAirStop;

	// Token: 0x0400009E RID: 158
	[SerializeField]
	private SoundDefinition suctionAirLoop;

	// Token: 0x0400009F RID: 159
	[SerializeField]
	private SoundDefinition suctionOreStart;

	// Token: 0x040000A0 RID: 160
	[SerializeField]
	private SoundDefinition suctionOreLoop;

	// Token: 0x040000A1 RID: 161
	[SerializeField]
	private SoundDefinition suctionOreStop;

	// Token: 0x040000A2 RID: 162
	[SerializeField]
	private SoundDefinition suctionOreInteriorLoop;

	// Token: 0x040000A3 RID: 163
	[SerializeField]
	private SoundDefinition oreBinLoop;

	// Token: 0x040000A4 RID: 164
	[SerializeField]
	private SoundDefinition suctionFluidStart;

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private SoundDefinition suctionFluidLoop;

	// Token: 0x040000A6 RID: 166
	[SerializeField]
	private SoundDefinition suctionFluidStop;

	// Token: 0x040000A7 RID: 167
	[SerializeField]
	private SoundDefinition suctionFluidInteriorLoop;

	// Token: 0x040000A8 RID: 168
	[SerializeField]
	private SoundDefinition fluidTankLoop;

	// Token: 0x040000A9 RID: 169
	[SerializeField]
	private GameObject interiorPipeSoundLocation;

	// Token: 0x040000AA RID: 170
	[SerializeField]
	private GameObject armMovementSoundLocation;

	// Token: 0x040000AB RID: 171
	[SerializeField]
	private GameObject armSuctionSoundLocation;

	// Token: 0x040000AC RID: 172
	[SerializeField]
	private GameObject oreBinSoundLocation;

	// Token: 0x040000AD RID: 173
	[SerializeField]
	private GameObject fluidTankSoundLocation;

	// Token: 0x040000AE RID: 174
	private NetworkedProperty<int> LootTypeIndex;

	// Token: 0x040000AF RID: 175
	private EntityRef<TrainCar> activeTrainCarRef;

	// Token: 0x040000B0 RID: 176
	private EntityRef<TrainCarUnloadable> activeUnloadableRef;

	// Token: 0x040000B1 RID: 177
	private const global::BaseEntity.Flags LinedUpFlag = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040000B2 RID: 178
	private const global::BaseEntity.Flags HasUnloadableFlag = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040000B3 RID: 179
	private const global::BaseEntity.Flags UnloadingInProgressFlag = global::BaseEntity.Flags.Busy;

	// Token: 0x040000B4 RID: 180
	private const global::BaseEntity.Flags MoveToNextInProgressFlag = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040000B5 RID: 181
	private const global::BaseEntity.Flags MoveToPrevInProgressFlag = global::BaseEntity.Flags.Reserved4;

	// Token: 0x040000B6 RID: 182
	private EntityRef<OreHopper> oreStorageInstance;

	// Token: 0x040000B7 RID: 183
	private EntityRef<PercentFullStorageContainer> fuelStorageInstance;

	// Token: 0x040000B8 RID: 184
	public const float TIME_TO_EMPTY = 40f;

	// Token: 0x040000BA RID: 186
	private static List<global::CoalingTower> unloadersInWorld = new List<global::CoalingTower>();

	// Token: 0x040000BB RID: 187
	private Sound armMovementLoopSound;

	// Token: 0x040000BC RID: 188
	private Sound suctionAirLoopSound;

	// Token: 0x040000BD RID: 189
	private Sound suctionMaterialLoopSound;

	// Token: 0x040000BE RID: 190
	private Sound interiorPipeLoopSound;

	// Token: 0x040000BF RID: 191
	private Sound unloadDestinationSound;

	// Token: 0x02000B5C RID: 2908
	public enum ActionAttemptStatus
	{
		// Token: 0x04003F4D RID: 16205
		NoError,
		// Token: 0x04003F4E RID: 16206
		GenericError,
		// Token: 0x04003F4F RID: 16207
		NoTrainCar,
		// Token: 0x04003F50 RID: 16208
		NoNextTrainCar,
		// Token: 0x04003F51 RID: 16209
		NoPrevTrainCar,
		// Token: 0x04003F52 RID: 16210
		TrainIsMoving,
		// Token: 0x04003F53 RID: 16211
		OutputIsFull,
		// Token: 0x04003F54 RID: 16212
		AlreadyShunting,
		// Token: 0x04003F55 RID: 16213
		TrainHasThrottle
	}
}
