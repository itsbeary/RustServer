using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000049 RID: 73
public class BaseRidableAnimal : global::BaseVehicle
{
	// Token: 0x0600073E RID: 1854 RVA: 0x0004AEEC File Offset: 0x000490EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseRidableAnimal.OnRpcMessage", 0))
		{
			if (rpc == 2333451803U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Claim ");
				}
				using (TimeWarning.New("RPC_Claim", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2333451803U, "RPC_Claim", this, player, 3f))
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
							this.RPC_Claim(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Claim");
					}
				}
				return true;
			}
			if (rpc == 3653170552U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lead ");
				}
				using (TimeWarning.New("RPC_Lead", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3653170552U, "RPC_Lead", this, player, 3f))
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
							this.RPC_Lead(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Lead");
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
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0000564C File Offset: 0x0000384C
	public bool IsForSale()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0004B348 File Offset: 0x00049548
	public void ContainerServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0004B368 File Offset: 0x00049568
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.onlyAllowedItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.onDirty += this.OnInventoryDirty;
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0004B434 File Offset: 0x00049634
	public void SaveContainer(global::BaseNetworkable.SaveInfo info)
	{
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryDirty()
	{
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0004B492 File Offset: 0x00049692
	public bool ItemFilter(global::Item item, int targetSlot)
	{
		return this.CanAnimalAcceptItem(item, targetSlot);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanAnimalAcceptItem(global::Item item, int targetSlot)
	{
		return true;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0004B49C File Offset: 0x0004969C
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
		if (!this.CanOpenStorage(player))
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0004B539 File Offset: 0x00049739
	public virtual bool CanOpenStorage(global::BasePlayer player)
	{
		return !base.HasFlag(global::BaseEntity.Flags.On) || this.PlayerIsMounted(player);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0004B554 File Offset: 0x00049754
	public void LoadContainer(global::BaseNetworkable.LoadInfo info)
	{
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0004B5C0 File Offset: 0x000497C0
	public float GetBreathingDelay()
	{
		switch (this.currentRunState)
		{
		default:
			return -1f;
		case BaseRidableAnimal.RunState.walk:
			return 8f;
		case BaseRidableAnimal.RunState.run:
			return 5f;
		case BaseRidableAnimal.RunState.sprint:
			return 2.5f;
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0004B603 File Offset: 0x00049803
	public bool IsLeading()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0004B610 File Offset: 0x00049810
	public static float UnitsToKPH(float unitsPerSecond)
	{
		return unitsPerSecond * 60f * 60f / 1000f;
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x0600074F RID: 1871 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0004B628 File Offset: 0x00049828
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = BaseRidableAnimal.framebudgetms / 1000f;
		while (BaseRidableAnimal._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BaseRidableAnimal baseRidableAnimal = BaseRidableAnimal._processQueue.Dequeue();
			if (baseRidableAnimal != null)
			{
				baseRidableAnimal.BudgetedUpdate();
				baseRidableAnimal.inQueue = false;
			}
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0004B682 File Offset: 0x00049882
	public void SetLeading(global::BaseEntity newLeadTarget)
	{
		this.leadTarget = newLeadTarget;
		base.SetFlag(global::BaseEntity.Flags.Reserved7, this.leadTarget != null, false, true);
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0004B6A4 File Offset: 0x000498A4
	public override float GetNetworkTime()
	{
		return this.lastMovementUpdateTime;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0004B6AC File Offset: 0x000498AC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.SaveContainer(info);
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0004B6BC File Offset: 0x000498BC
	private void OnPhysicsNeighbourChanged()
	{
		base.Invoke(new Action(this.DelayedDropToGround), UnityEngine.Time.fixedDeltaTime);
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0004B6D5 File Offset: 0x000498D5
	public void DelayedDropToGround()
	{
		this.DropToGround(base.transform.position, true);
		this.UpdateGroundNormal(true);
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0004B6F1 File Offset: 0x000498F1
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.LoadContainer(info);
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasValidSaddle()
	{
		return true;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasSeatAvailable()
	{
		return true;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x0004B701 File Offset: 0x00049901
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (this.IsForSale())
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void LeadingChanged()
	{
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0004B714 File Offset: 0x00049914
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Claim(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.IsForSale())
		{
			return;
		}
		int num = msg.read.Int32();
		global::Item item = this.GetPurchaseToken(player, num);
		if (item == null)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.OnClaimedWithToken(item);
		item.UseItem(1);
		Analytics.Server.VehiclePurchased(base.ShortPrefabName);
		Analytics.Azure.OnVehiclePurchased(msg.player, this);
		this.AttemptMount(player, false);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0004B790 File Offset: 0x00049990
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Lead(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (this.AnyMounted())
		{
			return;
		}
		if (this.IsForSale())
		{
			return;
		}
		bool flag = this.IsLeading();
		bool flag2 = msg.read.Bit();
		if (flag == flag2)
		{
			return;
		}
		this.SetLeading(flag2 ? player : null);
		this.LeadingChanged();
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnClaimedWithToken(global::Item tokenItem)
	{
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0004B7E9 File Offset: 0x000499E9
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.SetFlag(global::BaseEntity.Flags.On, true, true, true);
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x0004B7FD File Offset: 0x000499FD
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		if (base.NumMounted() == 0)
		{
			base.SetFlag(global::BaseEntity.Flags.On, false, true, true);
		}
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0004B81C File Offset: 0x00049A1C
	public void SetDecayActive(bool isActive)
	{
		if (isActive)
		{
			base.InvokeRandomized(new Action(this.AnimalDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
			return;
		}
		base.CancelInvoke(new Action(this.AnimalDecay));
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x0004B86A File Offset: 0x00049A6A
	public float TimeUntilNextDecay()
	{
		return this.nextDecayTime - UnityEngine.Time.time;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0004B878 File Offset: 0x00049A78
	public void AddDecayDelay(float amount)
	{
		if (this.nextDecayTime < UnityEngine.Time.time)
		{
			this.nextDecayTime = UnityEngine.Time.time + 5f;
		}
		this.nextDecayTime += amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Add Decay Delay ! amount is ",
				amount,
				"time until next decay : ",
				this.nextDecayTime - UnityEngine.Time.time
			}));
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0004B8F6 File Offset: 0x00049AF6
	public override void Hurt(HitInfo info)
	{
		if (this.IsForSale())
		{
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0004B908 File Offset: 0x00049B08
	public void AnimalDecay()
	{
		if (base.healthFraction == 0f || base.IsDestroyed)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastInputTime + 600f)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEatTime + 600f)
		{
			return;
		}
		if (this.IsForSale())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextDecayTime)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("Skipping animal decay due to hitching");
			}
			return;
		}
		float num = 1f / BaseRidableAnimal.decayminutes;
		float num2 = ((!this.IsOutside()) ? 1f : 0.5f);
		base.Hurt(this.MaxHealth() * num * num2, DamageType.Decay, this, false);
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0004B9B1 File Offset: 0x00049BB1
	public void UseStamina(float amount)
	{
		if (this.onIdealTerrain)
		{
			amount *= 0.5f;
		}
		this.staminaSeconds -= amount;
		if (this.staminaSeconds <= 0f)
		{
			this.staminaSeconds = 0f;
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x0004B9EA File Offset: 0x00049BEA
	public bool CanInitiateSprint()
	{
		return this.staminaSeconds > 4f;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0004B9F9 File Offset: 0x00049BF9
	public bool CanSprint()
	{
		return this.staminaSeconds > 0f;
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0004BA08 File Offset: 0x00049C08
	public void ReplenishStamina(float amount)
	{
		float num = 1f + Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
		amount *= num;
		amount = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaSeconds, amount);
		float num2 = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaCoreLossRatio * amount, amount * this.staminaCoreLossRatio);
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds - num2, 0f, this.maxStaminaSeconds);
		this.staminaSeconds = Mathf.Clamp(this.staminaSeconds + num2 / this.staminaCoreLossRatio, 0f, this.currentMaxStaminaSeconds);
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float ReplenishRatio()
	{
		return 1f;
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0004BAB4 File Offset: 0x00049CB4
	public void ReplenishStaminaCore(float calories, float hydration)
	{
		float num = calories * this.calorieToStaminaRatio;
		float num2 = hydration * this.hydrationToStaminaRatio;
		float num3 = this.ReplenishRatio();
		num2 = Mathf.Min(this.maxStaminaCoreFromWater - this.currentMaxStaminaSeconds, num2);
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		float num4 = num + num2 * num3;
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds + num4, 0f, this.maxStaminaSeconds);
		this.staminaSeconds = Mathf.Clamp(this.staminaSeconds + num4, 0f, this.currentMaxStaminaSeconds);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x0004BB3C File Offset: 0x00049D3C
	public void UpdateStamina(float delta)
	{
		if (this.currentRunState == BaseRidableAnimal.RunState.sprint)
		{
			this.UseStamina(delta);
			return;
		}
		if (this.currentRunState == BaseRidableAnimal.RunState.run)
		{
			this.ReplenishStamina(this.staminaReplenishRatioMoving * delta);
			return;
		}
		this.ReplenishStamina(this.staminaReplenishRatioStanding * delta);
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0004BB75 File Offset: 0x00049D75
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		this.RiderInput(inputState, player);
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0004BB8C File Offset: 0x00049D8C
	public void DismountHeavyPlayers()
	{
		if (this.AnyMounted())
		{
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (!(mountPointInfo.mountable == null))
				{
					global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (!(mounted == null) && this.IsPlayerTooHeavy(mounted))
					{
						mountPointInfo.mountable.DismountAllPlayers();
					}
				}
			}
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0004BC1C File Offset: 0x00049E1C
	public BaseMountable GetSaddle()
	{
		if (!this.saddleRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.saddleRef.Get(base.isServer).GetComponent<BaseMountable>();
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x0004BC4C File Offset: 0x00049E4C
	public void BudgetedUpdate()
	{
		this.DismountHeavyPlayers();
		this.UpdateOnIdealTerrain();
		this.UpdateStamina(UnityEngine.Time.fixedDeltaTime);
		if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
		{
			this.EatNearbyFood();
		}
		if (this.lastMovementUpdateTime == -1f)
		{
			this.lastMovementUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		}
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastMovementUpdateTime;
		this.UpdateMovement(num);
		this.lastMovementUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.UpdateDung(num);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0004BCBD File Offset: 0x00049EBD
	public void ApplyDungCalories(float calories)
	{
		this.pendingDungCalories += calories;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0004BCD0 File Offset: 0x00049ED0
	private void UpdateDung(float delta)
	{
		if (this.Dung == null)
		{
			return;
		}
		if (Mathf.Approximately(BaseRidableAnimal.dungTimeScale, 0f))
		{
			return;
		}
		float num = Mathf.Min(this.pendingDungCalories * delta, this.CaloriesToDigestPerHour / 3600f * delta) * this.DungProducedPerCalorie;
		this.dungProduction += num;
		this.pendingDungCalories -= num;
		if (this.dungProduction >= 1f)
		{
			this.DoDung();
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0004BD50 File Offset: 0x00049F50
	private void DoDung()
	{
		this.dungProduction -= 1f;
		ItemManager.Create(this.Dung, 1, 0UL).Drop(base.transform.position + -base.transform.forward + Vector3.up * 1.1f + UnityEngine.Random.insideUnitSphere * 0.1f, -base.transform.forward, default(Quaternion));
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0004BDE4 File Offset: 0x00049FE4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.timeAlive += UnityEngine.Time.fixedDeltaTime;
		if (!this.inQueue)
		{
			BaseRidableAnimal._processQueue.Enqueue(this);
			this.inQueue = true;
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0004BE18 File Offset: 0x0004A018
	public float StaminaCoreFraction()
	{
		return Mathf.InverseLerp(0f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0004BE30 File Offset: 0x0004A030
	public void DoEatEvent()
	{
		base.ClientRPC(null, "Eat");
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0004BE40 File Offset: 0x0004A040
	public void ReplenishFromFood(ItemModConsumable consumable)
	{
		if (consumable)
		{
			base.ClientRPC(null, "Eat");
			this.lastEatTime = UnityEngine.Time.time;
			float ifType = consumable.GetIfType(MetabolismAttribute.Type.Calories);
			float ifType2 = consumable.GetIfType(MetabolismAttribute.Type.Hydration);
			float num = consumable.GetIfType(MetabolismAttribute.Type.Health) + consumable.GetIfType(MetabolismAttribute.Type.HealthOverTime);
			this.ApplyDungCalories(ifType);
			this.ReplenishStaminaCore(ifType, ifType2);
			this.Heal(num * 4f);
		}
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x0004BEA8 File Offset: 0x0004A0A8
	public virtual void EatNearbyFood()
	{
		if (UnityEngine.Time.time < this.nextEatTime)
		{
			return;
		}
		float num = this.StaminaCoreFraction();
		this.nextEatTime = UnityEngine.Time.time + UnityEngine.Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, num) * 4f;
		if (num >= 1f)
		{
			return;
		}
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position + base.transform.forward * 1.5f, 2f, list, 67109377, QueryTriggerInteraction.Collide);
		list.Sort((global::BaseEntity a, global::BaseEntity b) => (b is DroppedItem).CompareTo(a is DroppedItem));
		foreach (global::BaseEntity baseEntity in list)
		{
			if (!baseEntity.isClient)
			{
				DroppedItem droppedItem = baseEntity as DroppedItem;
				if (droppedItem && droppedItem.item != null && droppedItem.item.info.category == ItemCategory.Food)
				{
					ItemModConsumable component = droppedItem.item.info.GetComponent<ItemModConsumable>();
					if (component)
					{
						this.ReplenishFromFood(component);
						droppedItem.item.UseItem(1);
						if (droppedItem.item.amount <= 0)
						{
							droppedItem.Kill(global::BaseNetworkable.DestroyMode.None);
							break;
						}
						break;
					}
				}
				CollectibleEntity collectibleEntity = baseEntity as CollectibleEntity;
				if (collectibleEntity && collectibleEntity.IsFood(false))
				{
					collectibleEntity.DoPickup(null, false);
					break;
				}
				global::GrowableEntity growableEntity = baseEntity as global::GrowableEntity;
				if (growableEntity && growableEntity.CanPick())
				{
					growableEntity.PickFruit(null, false);
					break;
				}
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0004C07C File Offset: 0x0004A27C
	public void SwitchMoveState(BaseRidableAnimal.RunState newState)
	{
		if (newState == this.currentRunState)
		{
			return;
		}
		this.currentRunState = newState;
		this.timeInMoveState = 0f;
		base.SetFlag(global::BaseEntity.Flags.Reserved8, this.currentRunState == BaseRidableAnimal.RunState.sprint, false, false);
		this.MarkObstacleDistanceDirty();
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0004C0B8 File Offset: 0x0004A2B8
	public void UpdateOnIdealTerrain()
	{
		if (UnityEngine.Time.time < this.nextIdealTerrainCheckTime)
		{
			return;
		}
		this.nextIdealTerrainCheckTime = UnityEngine.Time.time + UnityEngine.Random.Range(1f, 2f);
		this.onIdealTerrain = false;
		if (TerrainMeta.TopologyMap != null && (TerrainMeta.TopologyMap.GetTopology(base.transform.position) & 526336) != 0)
		{
			this.onIdealTerrain = true;
		}
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0004C128 File Offset: 0x0004A328
	public float MoveStateToVelocity(BaseRidableAnimal.RunState stateToCheck)
	{
		float num;
		switch (stateToCheck)
		{
		default:
			num = 0f;
			break;
		case BaseRidableAnimal.RunState.walk:
			num = this.GetWalkSpeed();
			break;
		case BaseRidableAnimal.RunState.run:
			num = this.GetTrotSpeed();
			break;
		case BaseRidableAnimal.RunState.sprint:
			num = this.GetRunSpeed();
			break;
		}
		return num;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0004C175 File Offset: 0x0004A375
	public float GetDesiredVelocity()
	{
		return this.MoveStateToVelocity(this.currentRunState);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x0004C183 File Offset: 0x0004A383
	public BaseRidableAnimal.RunState StateFromSpeed(float speedToUse)
	{
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.stopped))
		{
			return BaseRidableAnimal.RunState.stopped;
		}
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.walk))
		{
			return BaseRidableAnimal.RunState.walk;
		}
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.run))
		{
			return BaseRidableAnimal.RunState.run;
		}
		return BaseRidableAnimal.RunState.sprint;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0004C1AC File Offset: 0x0004A3AC
	public void ModifyRunState(int dir)
	{
		if ((this.currentRunState == BaseRidableAnimal.RunState.stopped && dir < 0) || (this.currentRunState == BaseRidableAnimal.RunState.sprint && dir > 0))
		{
			return;
		}
		BaseRidableAnimal.RunState runState = this.currentRunState + dir;
		this.SwitchMoveState(runState);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0004C1E4 File Offset: 0x0004A3E4
	public bool CanStand()
	{
		return this.nextStandTime <= UnityEngine.Time.time && !(this.mountPoints[0].mountable == null) && this.IsStandCollisionClear();
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0004C218 File Offset: 0x0004A418
	public virtual bool IsStandCollisionClear()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(this.mountPoints[0].mountable.eyePositionOverride.transform.position - base.transform.forward * 1f, 2f, list, 2162689, QueryTriggerInteraction.Collide);
		int num = ((list.Count > 0) ? 1 : 0);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return num == 0;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0004C28C File Offset: 0x0004A48C
	public void DoDebugMovement()
	{
		if (this.aiInputState == null)
		{
			this.aiInputState = new InputState();
		}
		if (!this.debugMovement)
		{
			this.aiInputState.current.buttons &= -3;
			this.aiInputState.current.buttons &= -9;
			this.aiInputState.current.buttons &= -129;
			return;
		}
		this.aiInputState.current.buttons |= 2;
		this.aiInputState.current.buttons |= 8;
		this.aiInputState.current.buttons |= 128;
		this.RiderInput(this.aiInputState, null);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0004C35C File Offset: 0x0004A55C
	public virtual void RiderInput(InputState inputState, global::BasePlayer player)
	{
		float num = UnityEngine.Time.time - this.lastInputTime;
		this.lastInputTime = UnityEngine.Time.time;
		num = Mathf.Clamp(num, 0f, 1f);
		Vector3 zero = Vector3.zero;
		this.timeInMoveState += num;
		if (inputState != null)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				this.lastForwardPressedTime = UnityEngine.Time.time;
				this.forwardHeldSeconds += num;
			}
			else
			{
				this.forwardHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				this.lastBackwardPressedTime = UnityEngine.Time.time;
				this.backwardHeldSeconds += num;
			}
			else
			{
				this.backwardHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.SPRINT))
			{
				this.lastSprintPressedTime = UnityEngine.Time.time;
				this.sprintHeldSeconds += num;
			}
			else
			{
				this.sprintHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.DUCK) && this.CanStand() && (this.currentRunState == BaseRidableAnimal.RunState.stopped || (this.currentRunState == BaseRidableAnimal.RunState.walk && this.currentSpeed < 1f)))
			{
				base.ClientRPC(null, "Stand");
				this.nextStandTime = UnityEngine.Time.time + 3f;
				this.currentSpeed = 0f;
			}
			if (UnityEngine.Time.time < this.nextStandTime)
			{
				this.forwardHeldSeconds = 0f;
				this.backwardHeldSeconds = 0f;
			}
			if (this.forwardHeldSeconds > 0f)
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.walk);
				}
				else if (this.currentRunState == BaseRidableAnimal.RunState.walk)
				{
					if (this.sprintHeldSeconds > 0f)
					{
						this.SwitchMoveState(BaseRidableAnimal.RunState.run);
					}
				}
				else if (this.currentRunState == BaseRidableAnimal.RunState.run && this.sprintHeldSeconds > 1f && this.CanInitiateSprint())
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.sprint);
				}
			}
			else if (this.backwardHeldSeconds > 1f)
			{
				this.ModifyRunState(-1);
				this.backwardHeldSeconds = 0.1f;
			}
			else if (this.backwardHeldSeconds == 0f && this.forwardHeldSeconds == 0f && this.timeInMoveState > 1f && this.currentRunState != BaseRidableAnimal.RunState.stopped)
			{
				this.ModifyRunState(-1);
			}
			if (this.currentRunState == BaseRidableAnimal.RunState.sprint && (!this.CanSprint() || UnityEngine.Time.time - this.lastSprintPressedTime > 5f))
			{
				this.ModifyRunState(-1);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = 1f;
				return;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = -1f;
				return;
			}
			this.desiredRotation = 0f;
		}
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0004C5FD File Offset: 0x0004A7FD
	public override float MaxVelocity()
	{
		return this.maxSpeed * 1.5f;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0004C60B File Offset: 0x0004A80B
	private float NormalizeAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0004C620 File Offset: 0x0004A820
	public void UpdateGroundNormal(bool force = false)
	{
		if (UnityEngine.Time.time >= this.nextGroundNormalUpdateTime || force)
		{
			this.nextGroundNormalUpdateTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.2f, 0.3f);
			this.targetUp = this.averagedUp;
			Transform[] array = this.groundSampleOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector;
				Vector3 vector2;
				if (TransformUtil.GetGroundInfo(array[i].position + Vector3.up * 2f, out vector, out vector2, 4f, 295763969, null))
				{
					this.targetUp += vector2;
				}
				else
				{
					this.targetUp += Vector3.up;
				}
			}
			this.targetUp /= (float)(this.groundSampleOffsets.Length + 1);
		}
		this.averagedUp = Vector3.Lerp(this.averagedUp, this.targetUp, UnityEngine.Time.deltaTime * 2f);
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0004C71F File Offset: 0x0004A91F
	public void MarkObstacleDistanceDirty()
	{
		this.nextObstacleCheckTime = 0f;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0004C72C File Offset: 0x0004A92C
	public float GetObstacleDistance()
	{
		if (UnityEngine.Time.time >= this.nextObstacleCheckTime)
		{
			float desiredVelocity = this.GetDesiredVelocity();
			if (this.currentSpeed > 0f || desiredVelocity > 0f)
			{
				this.cachedObstacleDistance = this.ObstacleDistanceCheck(Mathf.Max(desiredVelocity, 2f));
			}
			this.nextObstacleCheckTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.25f, 0.35f);
		}
		return this.cachedObstacleDistance;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0004C79C File Offset: 0x0004A99C
	public float ObstacleDistanceCheck(float speed = 10f)
	{
		Vector3 position = base.transform.position;
		float num = (float)Mathf.Max(2, Mathf.Min((int)speed, 10));
		float num2 = 0.5f;
		int num3 = Mathf.CeilToInt(num / num2);
		float num4 = 0f;
		Vector3 vector = QuaternionEx.LookRotationForcedUp(base.transform.forward, Vector3.up) * Vector3.forward;
		Vector3 vector2 = this.movementLOSOrigin.transform.position;
		vector2.y = base.transform.position.y;
		Vector3 up = base.transform.up;
		for (int i = 0; i < num3; i++)
		{
			float num5 = num2;
			bool flag = false;
			float num6 = 0f;
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.up;
			Vector3 vector5 = vector2;
			Vector3 vector6 = vector5 + Vector3.up * (this.maxStepHeight + this.obstacleDetectionRadius);
			Vector3 vector7 = vector5 + vector * num5;
			float num7 = this.maxStepDownHeight + this.obstacleDetectionRadius;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.SphereCast(vector6, this.obstacleDetectionRadius, vector, out raycastHit, num5, 1486954753))
			{
				num6 = raycastHit.distance;
				vector3 = raycastHit.point;
				vector4 = raycastHit.normal;
				flag = true;
			}
			if (!flag)
			{
				if (!TransformUtil.GetGroundInfo(vector7 + Vector3.up * 2f, out vector3, out vector4, 2f + num7, 295763969, null))
				{
					return num4;
				}
				num6 = Vector3.Distance(vector5, vector3);
				if (WaterLevel.Test(vector3 + Vector3.one * this.maxWaterDepth, true, true, this))
				{
					vector4 = -base.transform.forward;
					return num4;
				}
				flag = true;
			}
			if (flag)
			{
				float num8 = Vector3.Angle(up, vector4);
				float num9 = Vector3.Angle(vector4, Vector3.up);
				if (num8 > this.maxWallClimbSlope || num9 > this.maxWallClimbSlope)
				{
					Vector3 vector8 = vector4;
					float num10 = vector3.y;
					int num11 = 1;
					for (int j = 0; j < this.normalOffsets.Length; j++)
					{
						Vector3 vector9 = vector7 + this.normalOffsets[j].x * base.transform.right;
						float num12 = this.maxStepHeight * 2.5f;
						Vector3 vector10;
						Vector3 vector11;
						if (TransformUtil.GetGroundInfo(vector9 + Vector3.up * num12 + this.normalOffsets[j].z * base.transform.forward, out vector10, out vector11, num7 + num12, 295763969, null))
						{
							num11++;
							vector8 += vector11;
							num10 += vector10.y;
						}
					}
					num10 /= (float)num11;
					vector8.Normalize();
					float num13 = Vector3.Angle(up, vector8);
					num9 = Vector3.Angle(vector8, Vector3.up);
					if (num13 > this.maxWallClimbSlope || num9 > this.maxWallClimbSlope || Mathf.Abs(num10 - vector7.y) > this.maxStepHeight)
					{
						return num4;
					}
				}
			}
			num4 += num6;
			vector = QuaternionEx.LookRotationForcedUp(base.transform.forward, vector4) * Vector3.forward;
			vector2 = vector3;
		}
		return num4;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void MarkDistanceTravelled(float amount)
	{
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0004CADC File Offset: 0x0004ACDC
	public void UpdateMovement(float delta)
	{
		float num = this.WaterFactor();
		if (num > 1f && !base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		if (this.desiredRotation != 0f)
		{
			this.MarkObstacleDistanceDirty();
		}
		if (num >= 0.3f && this.currentRunState > BaseRidableAnimal.RunState.run)
		{
			this.currentRunState = BaseRidableAnimal.RunState.run;
		}
		else if (num >= 0.45f && this.currentRunState > BaseRidableAnimal.RunState.walk)
		{
			this.currentRunState = BaseRidableAnimal.RunState.walk;
		}
		if (UnityEngine.Time.time - this.lastInputTime > 3f && !this.IsLeading())
		{
			this.currentRunState = BaseRidableAnimal.RunState.stopped;
			this.desiredRotation = 0f;
		}
		if ((base.HasDriver() && this.IsLeading()) || this.leadTarget == null)
		{
			this.SetLeading(null);
		}
		if (this.IsLeading())
		{
			Vector3 position = this.leadTarget.transform.position;
			Vector3 vector = Vector3Ex.Direction2D(base.transform.position + base.transform.right * 1f, base.transform.position);
			Vector3 vector2 = Vector3Ex.Direction2D(base.transform.position + base.transform.forward * 0.01f, base.transform.position);
			Vector3 vector3 = Vector3Ex.Direction2D(position, base.transform.position);
			float num2 = Vector3.Dot(vector, vector3);
			float num3 = Vector3.Dot(vector2, vector3);
			bool flag = Vector3Ex.Distance2D(position, base.transform.position) > 2.5f;
			bool flag2 = Vector3Ex.Distance2D(position, base.transform.position) > 10f;
			if (flag || num3 < 0.95f)
			{
				float num4 = Mathf.InverseLerp(0f, 1f, num2);
				float num5 = 1f - Mathf.InverseLerp(-1f, 0f, num2);
				this.desiredRotation = 0f;
				this.desiredRotation += num4 * 1f;
				this.desiredRotation += num5 * -1f;
				if (Mathf.Abs(this.desiredRotation) < 0.001f)
				{
					this.desiredRotation = 0f;
				}
				if (flag)
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.walk);
				}
				else
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
				}
			}
			else
			{
				this.desiredRotation = 0f;
				this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
			}
			if (flag2)
			{
				this.SetLeading(null);
				this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
			}
		}
		float obstacleDistance = this.GetObstacleDistance();
		BaseRidableAnimal.RunState runState = this.StateFromSpeed(obstacleDistance * this.GetRunSpeed());
		if (runState < this.currentRunState)
		{
			this.SwitchMoveState(runState);
		}
		float desiredVelocity = this.GetDesiredVelocity();
		Vector3 vector4 = Vector3.forward * Mathf.Sign(desiredVelocity);
		float num6 = Mathf.InverseLerp(0.85f, 1f, obstacleDistance);
		float num7 = Mathf.InverseLerp(1.25f, 10f, obstacleDistance);
		float num8 = 1f - Mathf.InverseLerp(20f, 45f, Vector3.Angle(Vector3.up, this.averagedUp));
		num7 = num6 * 0.1f + num7 * 0.9f;
		float num9 = Mathf.Min(Mathf.Clamp01(Mathf.Min(num8 + 0.2f, num7)) * this.GetRunSpeed(), desiredVelocity);
		float num10 = ((num9 < this.currentSpeed) ? 3f : 1f);
		if (Mathf.Abs(this.currentSpeed) < 2f && desiredVelocity == 0f)
		{
			this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, delta * 3f);
		}
		else
		{
			this.currentSpeed = Mathf.Lerp(this.currentSpeed, num9, delta * num10);
		}
		if (num7 == 0f)
		{
			this.currentSpeed = 0f;
		}
		float num11 = 1f - Mathf.InverseLerp(2f, 7f, this.currentSpeed);
		num11 = (num11 + 1f) / 2f;
		if (this.desiredRotation != 0f)
		{
			Vector3 position2 = this.animalFront.transform.position;
			Quaternion rotation = base.transform.rotation;
			base.transform.Rotate(Vector3.up, this.desiredRotation * delta * this.turnSpeed * num11);
			if (!this.IsLeading() && global::Vis.AnyColliders(this.animalFront.transform.position, this.obstacleDetectionRadius * 0.25f, 1503731969, QueryTriggerInteraction.Ignore))
			{
				base.transform.rotation = rotation;
			}
		}
		Vector3 vector5 = base.transform.TransformDirection(vector4);
		Vector3 normalized = vector5.normalized;
		float num12 = this.currentSpeed * delta;
		Vector3 vector6 = base.transform.position + normalized * num12 * Mathf.Sign(this.currentSpeed);
		this.currentVelocity = vector5 * this.currentSpeed;
		this.UpdateGroundNormal(false);
		if (this.currentSpeed > 0f || this.timeAlive < 2f || this.dropUntilTime > 0f)
		{
			base.transform.position + base.transform.InverseTransformPoint(this.animalFront.transform.position).y * base.transform.up;
			RaycastHit raycastHit;
			bool flag3 = UnityEngine.Physics.SphereCast(this.animalFront.transform.position, this.obstacleDetectionRadius, normalized, out raycastHit, num12, 1503731969);
			bool flag4 = UnityEngine.Physics.SphereCast(base.transform.position + base.transform.InverseTransformPoint(this.animalFront.transform.position).y * base.transform.up, this.obstacleDetectionRadius, normalized, out raycastHit, num12, 1503731969);
			if (!global::Vis.AnyColliders(this.animalFront.transform.position + normalized * num12, this.obstacleDetectionRadius, 1503731969, QueryTriggerInteraction.Ignore) && !flag3 && !flag4)
			{
				if (this.DropToGround(vector6 + Vector3.up * this.maxStepHeight, false))
				{
					this.MarkDistanceTravelled(num12);
					return;
				}
				this.currentSpeed = 0f;
				return;
			}
			else
			{
				this.currentSpeed = 0f;
			}
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0004D10C File Offset: 0x0004B30C
	public bool DropToGround(Vector3 targetPos, bool force = false)
	{
		float num = (force ? 10000f : (this.maxStepHeight + this.maxStepDownHeight));
		Vector3 vector;
		Vector3 vector2;
		if (!TransformUtil.GetGroundInfo(targetPos, out vector, out vector2, num, 295763969, null))
		{
			return false;
		}
		if (UnityEngine.Physics.CheckSphere(vector + Vector3.up * 1f, 0.2f, 295763969))
		{
			return false;
		}
		base.transform.position = vector;
		Vector3 eulerAngles = QuaternionEx.LookRotationForcedUp(base.transform.forward, this.averagedUp).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, -10f, 10f);
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		return true;
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0004D20C File Offset: 0x0004B40C
	public virtual void DoNetworkUpdate()
	{
		bool flag = false || this.prevStamina != this.staminaSeconds || this.prevMaxStamina != this.currentMaxStaminaSeconds || this.prevRunState != (int)this.currentRunState || this.prevMaxSpeed != this.GetRunSpeed();
		this.prevStamina = this.staminaSeconds;
		this.prevMaxStamina = this.currentMaxStaminaSeconds;
		this.prevRunState = (int)this.currentRunState;
		this.prevMaxSpeed = this.GetRunSpeed();
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0004D2AB File Offset: 0x0004B4AB
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0004D2BC File Offset: 0x0004B4BC
	public override void ServerInit()
	{
		this.ContainerServerInit();
		base.ServerInit();
		base.InvokeRepeating(new Action(this.DoNetworkUpdate), UnityEngine.Random.Range(0f, 0.2f), 0.333f);
		this.SetDecayActive(true);
		if (this.debugMovement)
		{
			base.InvokeRandomized(new Action(this.DoDebugMovement), 0f, 0.1f, 0.1f);
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0004D32C File Offset: 0x0004B52C
	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			this.SetupCorpse(baseCorpse);
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(base.KillMessage), 0.5f);
		base.OnKilled(hitInfo);
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0004D398 File Offset: 0x0004B598
	public virtual void SetupCorpse(BaseCorpse corpse)
	{
		corpse.flags = this.flags;
		global::LootableCorpse component = corpse.GetComponent<global::LootableCorpse>();
		if (component)
		{
			component.TakeFrom(new global::ItemContainer[] { this.inventory });
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0004D3D5 File Offset: 0x0004B5D5
	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0004D3DD File Offset: 0x0004B5DD
	public void UpdateDropToGroundForDuration(float duration)
	{
		this.dropUntilTime = duration;
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0004D3EB File Offset: 0x0004B5EB
	public override void InitShared()
	{
		base.InitShared();
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0004D3F3 File Offset: 0x0004B5F3
	public bool PlayerHasToken(global::BasePlayer player, int tokenItemID)
	{
		return this.GetPurchaseToken(player, tokenItemID) != null;
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0004D400 File Offset: 0x0004B600
	public global::Item GetPurchaseToken(global::BasePlayer player, int tokenItemID)
	{
		return player.inventory.FindItemID(tokenItemID);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0004D40E File Offset: 0x0004B60E
	public virtual float GetWalkSpeed()
	{
		return this.walkSpeed;
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0004D416 File Offset: 0x0004B616
	public virtual float GetTrotSpeed()
	{
		return this.trotSpeed;
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0004D420 File Offset: 0x0004B620
	public virtual float GetRunSpeed()
	{
		if (base.isServer)
		{
			float num = this.runSpeed;
			float num2 = Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds) * this.staminaCoreSpeedBonus;
			float num3 = (this.onIdealTerrain ? this.roadSpeedBonus : 0f);
			return this.runSpeed + num2 + num3;
		}
		return this.runSpeed;
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0004D488 File Offset: 0x0004B688
	public bool IsPlayerTooHeavy(global::BasePlayer player)
	{
		return player.Weight >= 10f;
	}

	// Token: 0x040004B3 RID: 1203
	public ItemDefinition onlyAllowedItem;

	// Token: 0x040004B4 RID: 1204
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x040004B5 RID: 1205
	public int maxStackSize = 1;

	// Token: 0x040004B6 RID: 1206
	public int numSlots;

	// Token: 0x040004B7 RID: 1207
	public string lootPanelName = "generic";

	// Token: 0x040004B8 RID: 1208
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x040004B9 RID: 1209
	public bool isLootable = true;

	// Token: 0x040004BA RID: 1210
	public global::ItemContainer inventory;

	// Token: 0x040004BB RID: 1211
	public const global::BaseEntity.Flags Flag_ForSale = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040004BC RID: 1212
	public Translate.Phrase SingleHorseTitle = new Translate.Phrase("purchase_single_horse", "Purchase Single Saddle");

	// Token: 0x040004BD RID: 1213
	public Translate.Phrase SingleHorseDescription = new Translate.Phrase("purchase_single_horse_desc", "A single saddle for one player.");

	// Token: 0x040004BE RID: 1214
	public Translate.Phrase DoubleHorseTitle = new Translate.Phrase("purchase_double_horse", "Purchase Double Saddle");

	// Token: 0x040004BF RID: 1215
	public Translate.Phrase DoubleHorseDescription = new Translate.Phrase("purchase_double_horse_desc", "A double saddle for two players.");

	// Token: 0x040004C0 RID: 1216
	private Vector3 lastMoveDirection;

	// Token: 0x040004C1 RID: 1217
	public GameObjectRef saddlePrefab;

	// Token: 0x040004C2 RID: 1218
	public EntityRef saddleRef;

	// Token: 0x040004C3 RID: 1219
	public Transform movementLOSOrigin;

	// Token: 0x040004C4 RID: 1220
	public SoundPlayer sprintSounds;

	// Token: 0x040004C5 RID: 1221
	public SoundPlayer largeWhinny;

	// Token: 0x040004C6 RID: 1222
	public const global::BaseEntity.Flags Flag_Lead = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040004C7 RID: 1223
	public const global::BaseEntity.Flags Flag_HasRider = global::BaseEntity.Flags.On;

	// Token: 0x040004C8 RID: 1224
	[Header("Purchase")]
	public List<BaseRidableAnimal.PurchaseOption> PurchaseOptions;

	// Token: 0x040004C9 RID: 1225
	public ItemDefinition purchaseToken;

	// Token: 0x040004CA RID: 1226
	public GameObjectRef eatEffect;

	// Token: 0x040004CB RID: 1227
	public GameObjectRef CorpsePrefab;

	// Token: 0x040004CC RID: 1228
	[Header("Obstacles")]
	public Transform animalFront;

	// Token: 0x040004CD RID: 1229
	public float obstacleDetectionRadius = 0.25f;

	// Token: 0x040004CE RID: 1230
	public float maxWaterDepth = 1.5f;

	// Token: 0x040004CF RID: 1231
	public float roadSpeedBonus = 2f;

	// Token: 0x040004D0 RID: 1232
	public float maxWallClimbSlope = 53f;

	// Token: 0x040004D1 RID: 1233
	public float maxStepHeight = 1f;

	// Token: 0x040004D2 RID: 1234
	public float maxStepDownHeight = 1.35f;

	// Token: 0x040004D3 RID: 1235
	[Header("Movement")]
	public BaseRidableAnimal.RunState currentRunState = BaseRidableAnimal.RunState.stopped;

	// Token: 0x040004D4 RID: 1236
	public float walkSpeed = 2f;

	// Token: 0x040004D5 RID: 1237
	public float trotSpeed = 7f;

	// Token: 0x040004D6 RID: 1238
	public float runSpeed = 14f;

	// Token: 0x040004D7 RID: 1239
	public float turnSpeed = 30f;

	// Token: 0x040004D8 RID: 1240
	public float maxSpeed = 5f;

	// Token: 0x040004D9 RID: 1241
	public Transform[] groundSampleOffsets;

	// Token: 0x040004DA RID: 1242
	[Header("Dung")]
	public ItemDefinition Dung;

	// Token: 0x040004DB RID: 1243
	public float CaloriesToDigestPerHour = 100f;

	// Token: 0x040004DC RID: 1244
	public float DungProducedPerCalorie = 0.001f;

	// Token: 0x040004DD RID: 1245
	private float pendingDungCalories;

	// Token: 0x040004DE RID: 1246
	private float dungProduction;

	// Token: 0x040004DF RID: 1247
	protected float prevStamina;

	// Token: 0x040004E0 RID: 1248
	protected float prevMaxStamina;

	// Token: 0x040004E1 RID: 1249
	protected int prevRunState;

	// Token: 0x040004E2 RID: 1250
	protected float prevMaxSpeed;

	// Token: 0x040004E3 RID: 1251
	[Header("Stamina")]
	public float staminaSeconds = 10f;

	// Token: 0x040004E4 RID: 1252
	public float currentMaxStaminaSeconds = 10f;

	// Token: 0x040004E5 RID: 1253
	public float maxStaminaSeconds = 20f;

	// Token: 0x040004E6 RID: 1254
	public float staminaCoreLossRatio = 0.1f;

	// Token: 0x040004E7 RID: 1255
	public float staminaCoreSpeedBonus = 3f;

	// Token: 0x040004E8 RID: 1256
	public float staminaReplenishRatioMoving = 0.5f;

	// Token: 0x040004E9 RID: 1257
	public float staminaReplenishRatioStanding = 1f;

	// Token: 0x040004EA RID: 1258
	public float calorieToStaminaRatio = 0.1f;

	// Token: 0x040004EB RID: 1259
	public float hydrationToStaminaRatio = 0.5f;

	// Token: 0x040004EC RID: 1260
	public float maxStaminaCoreFromWater = 0.5f;

	// Token: 0x040004ED RID: 1261
	public bool debugMovement = true;

	// Token: 0x040004EE RID: 1262
	private const float normalOffsetDist = 0.15f;

	// Token: 0x040004EF RID: 1263
	private Vector3[] normalOffsets = new Vector3[]
	{
		new Vector3(0.15f, 0f, 0f),
		new Vector3(-0.15f, 0f, 0f),
		new Vector3(0f, 0f, 0.15f),
		new Vector3(0f, 0f, 0.3f),
		new Vector3(0f, 0f, 0.6f),
		new Vector3(0.15f, 0f, 0.3f),
		new Vector3(-0.15f, 0f, 0.3f)
	};

	// Token: 0x040004F0 RID: 1264
	[ServerVar(Help = "How long before a horse dies unattended")]
	public static float decayminutes = 180f;

	// Token: 0x040004F1 RID: 1265
	public float currentSpeed;

	// Token: 0x040004F2 RID: 1266
	public float desiredRotation;

	// Token: 0x040004F3 RID: 1267
	public float animalPitchClamp = 90f;

	// Token: 0x040004F4 RID: 1268
	public float animalRollClamp;

	// Token: 0x040004F5 RID: 1269
	public static Queue<BaseRidableAnimal> _processQueue = new Queue<BaseRidableAnimal>();

	// Token: 0x040004F6 RID: 1270
	[ServerVar]
	[Help("How many miliseconds to budget for processing ridable animals per frame")]
	public static float framebudgetms = 1f;

	// Token: 0x040004F7 RID: 1271
	[ServerVar]
	[Help("Scale all ridable animal dung production rates by this value. 0 will disable dung production.")]
	public static float dungTimeScale = 1f;

	// Token: 0x040004F8 RID: 1272
	private global::BaseEntity leadTarget;

	// Token: 0x040004F9 RID: 1273
	private float nextDecayTime;

	// Token: 0x040004FA RID: 1274
	private float lastMovementUpdateTime = -1f;

	// Token: 0x040004FB RID: 1275
	private bool inQueue;

	// Token: 0x040004FC RID: 1276
	protected float nextEatTime;

	// Token: 0x040004FD RID: 1277
	private float lastEatTime = float.NegativeInfinity;

	// Token: 0x040004FE RID: 1278
	private float lastInputTime;

	// Token: 0x040004FF RID: 1279
	private float forwardHeldSeconds;

	// Token: 0x04000500 RID: 1280
	private float backwardHeldSeconds;

	// Token: 0x04000501 RID: 1281
	private float sprintHeldSeconds;

	// Token: 0x04000502 RID: 1282
	private float lastSprintPressedTime;

	// Token: 0x04000503 RID: 1283
	private float lastForwardPressedTime;

	// Token: 0x04000504 RID: 1284
	private float lastBackwardPressedTime;

	// Token: 0x04000505 RID: 1285
	private float timeInMoveState;

	// Token: 0x04000506 RID: 1286
	protected bool onIdealTerrain;

	// Token: 0x04000507 RID: 1287
	private float nextIdealTerrainCheckTime;

	// Token: 0x04000508 RID: 1288
	private float nextStandTime;

	// Token: 0x04000509 RID: 1289
	private InputState aiInputState;

	// Token: 0x0400050A RID: 1290
	private Vector3 currentVelocity;

	// Token: 0x0400050B RID: 1291
	private Vector3 averagedUp = Vector3.up;

	// Token: 0x0400050C RID: 1292
	private float nextGroundNormalUpdateTime;

	// Token: 0x0400050D RID: 1293
	private Vector3 targetUp = Vector3.up;

	// Token: 0x0400050E RID: 1294
	private float nextObstacleCheckTime;

	// Token: 0x0400050F RID: 1295
	private float cachedObstacleDistance = float.PositiveInfinity;

	// Token: 0x04000510 RID: 1296
	private const int maxObstacleCheckSpeed = 10;

	// Token: 0x04000511 RID: 1297
	private float timeAlive;

	// Token: 0x04000512 RID: 1298
	private TimeUntil dropUntilTime;

	// Token: 0x02000BC2 RID: 3010
	[Serializable]
	public struct PurchaseOption
	{
		// Token: 0x0400414F RID: 16719
		public ItemDefinition TokenItem;

		// Token: 0x04004150 RID: 16720
		public Translate.Phrase Title;

		// Token: 0x04004151 RID: 16721
		public Translate.Phrase Description;

		// Token: 0x04004152 RID: 16722
		public Sprite Icon;

		// Token: 0x04004153 RID: 16723
		public int order;
	}

	// Token: 0x02000BC3 RID: 3011
	public enum RunState
	{
		// Token: 0x04004155 RID: 16725
		stopped = 1,
		// Token: 0x04004156 RID: 16726
		walk,
		// Token: 0x04004157 RID: 16727
		run,
		// Token: 0x04004158 RID: 16728
		sprint,
		// Token: 0x04004159 RID: 16729
		LAST
	}
}
