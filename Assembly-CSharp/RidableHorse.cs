using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C5 RID: 197
public class RidableHorse : BaseRidableAnimal
{
	// Token: 0x0600118A RID: 4490 RVA: 0x0008EF3C File Offset: 0x0008D13C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RidableHorse.OnRpcMessage", 0))
		{
			if (rpc == 1765203204U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ReqSwapSaddleType ");
				}
				using (TimeWarning.New("RPC_ReqSwapSaddleType", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1765203204U, "RPC_ReqSwapSaddleType", this, player, 3f))
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
							this.RPC_ReqSwapSaddleType(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ReqSwapSaddleType");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0008F0A4 File Offset: 0x0008D2A4
	public int GetStorageSlotCount()
	{
		return this.numStorageSlots;
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600118C RID: 4492 RVA: 0x0008F0AC File Offset: 0x0008D2AC
	public override float RealisticMass
	{
		get
		{
			return 550f;
		}
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0008F0B4 File Offset: 0x0008D2B4
	public void ApplyBreed(int index)
	{
		if (this.currentBreed == index)
		{
			return;
		}
		if (index >= this.breeds.Length || index < 0)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"ApplyBreed issue! index is ",
				index,
				" breed length is : ",
				this.breeds.Length
			}));
			return;
		}
		this.ApplyBreedInternal(this.breeds[index]);
		this.currentBreed = index;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0008F129 File Offset: 0x0008D329
	protected void ApplyBreedInternal(HorseBreed breed)
	{
		if (base.isServer)
		{
			base.SetMaxHealth(this.StartHealth() * breed.maxHealth);
			base.health = this.MaxHealth();
		}
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0008F152 File Offset: 0x0008D352
	public HorseBreed GetBreed()
	{
		if (this.currentBreed == -1 || this.currentBreed >= this.breeds.Length)
		{
			return null;
		}
		return this.breeds[this.currentBreed];
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0008F17C File Offset: 0x0008D37C
	public override float GetTrotSpeed()
	{
		float num = this.equipmentSpeedMod / (base.GetRunSpeed() * this.GetBreed().maxSpeed);
		return base.GetTrotSpeed() * this.GetBreed().maxSpeed * (1f + num);
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0008F1C0 File Offset: 0x0008D3C0
	public override float GetRunSpeed()
	{
		float runSpeed = base.GetRunSpeed();
		HorseBreed breed = this.GetBreed();
		return runSpeed * breed.maxSpeed + this.equipmentSpeedMod;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0008F1E8 File Offset: 0x0008D3E8
	public override void OnInventoryFirstCreated(global::ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		this.SpawnWildSaddle();
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0008F1F7 File Offset: 0x0008D3F7
	private void SpawnWildSaddle()
	{
		this.SetSeatCount(1);
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0008F200 File Offset: 0x0008D400
	public void SetForSale()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		this.SetSeatCount(0);
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0008F218 File Offset: 0x0008D418
	public override bool IsStandCollisionClear()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		bool flag = false;
		if (this.HasSingleSaddle())
		{
			global::Vis.Colliders<Collider>(this.mountPoints[0].mountable.eyePositionOverride.transform.position - base.transform.forward * 1f, 2f, list, 2162689, QueryTriggerInteraction.Collide);
			flag = list.Count > 0;
		}
		else if (this.HasDoubleSaddle())
		{
			global::Vis.Colliders<Collider>(this.mountPoints[1].mountable.eyePositionOverride.transform.position - base.transform.forward * 1f, 2f, list, 2162689, QueryTriggerInteraction.Collide);
			flag = list.Count > 0;
			if (!flag)
			{
				global::Vis.Colliders<Collider>(this.mountPoints[2].mountable.eyePositionOverride.transform.position - base.transform.forward * 1f, 2f, list, 2162689, QueryTriggerInteraction.Collide);
				flag = list.Count > 0;
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return !flag;
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0008F352 File Offset: 0x0008D552
	public override bool IsPlayerSeatSwapValid(global::BasePlayer player, int fromIndex, int toIndex)
	{
		return this.HasSaddle() && !this.HasSingleSaddle() && (!this.HasDoubleSaddle() || toIndex != 0);
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0008F376 File Offset: 0x0008D576
	public override int NumSwappableSeats()
	{
		return this.mountPoints.Count;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x0008F384 File Offset: 0x0008D584
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (base.IsForSale())
		{
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable baseMountable;
		if (this.HasSingleSaddle())
		{
			baseMountable = this.mountPoints[0].mountable;
		}
		else
		{
			if (!this.HasDoubleSaddle())
			{
				return;
			}
			baseMountable = (base.HasDriver() ? this.mountPoints[2].mountable : this.mountPoints[1].mountable);
		}
		if (baseMountable != null)
		{
			baseMountable.AttemptMount(player, doMountChecks);
		}
		if (this.PlayerIsMounted(player))
		{
			this.PlayerMounted(player, baseMountable);
		}
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0008F41C File Offset: 0x0008D61C
	public override void SetupCorpse(BaseCorpse corpse)
	{
		base.SetupCorpse(corpse);
		HorseCorpse component = corpse.GetComponent<HorseCorpse>();
		if (component)
		{
			component.breedIndex = this.currentBreed;
			return;
		}
		Debug.Log("no horse corpse");
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0008F456 File Offset: 0x0008D656
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.riderProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0008F476 File Offset: 0x0008D676
	public override void OnKilled(HitInfo hitInfo = null)
	{
		this.TryLeaveHitch();
		base.OnKilled(hitInfo);
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x0008F485 File Offset: 0x0008D685
	public void SetBreed(int index)
	{
		this.ApplyBreed(index);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0008F495 File Offset: 0x0008D695
	public override void LeadingChanged()
	{
		if (!base.IsLeading())
		{
			this.TryHitch();
		}
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x0008F4A8 File Offset: 0x0008D6A8
	public override void ServerInit()
	{
		this.SetBreed(UnityEngine.Random.Range(0, this.breeds.Length));
		this.baseHorseProtection = this.baseProtection;
		this.riderProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection.Add(this.baseHorseProtection, 1f);
		base.ServerInit();
		this.EquipmentUpdate();
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x0600119F RID: 4511 RVA: 0x000349F2 File Offset: 0x00032BF2
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0008F50D File Offset: 0x0008D70D
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.InvokeRepeating(new Action(this.RecordDistance), this.distanceRecordingSpacing, this.distanceRecordingSpacing);
		this.TryLeaveHitch();
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0008F53B File Offset: 0x0008D73B
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.CancelInvoke(new Action(this.RecordDistance));
		if (base.NumMounted() == 0)
		{
			this.TryHitch();
		}
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0008F565 File Offset: 0x0008D765
	public bool IsHitched()
	{
		return this.currentHitch != null;
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0008F573 File Offset: 0x0008D773
	public void SetHitch(HitchTrough Hitch)
	{
		this.currentHitch = Hitch;
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.currentHitch != null, false, true);
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float ReplenishRatio()
	{
		return 1f;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0008F598 File Offset: 0x0008D798
	public override void EatNearbyFood()
	{
		if (UnityEngine.Time.time < this.nextEatTime)
		{
			return;
		}
		if (base.StaminaCoreFraction() >= 1f && base.healthFraction >= 1f)
		{
			return;
		}
		if (this.IsHitched())
		{
			global::Item foodItem = this.currentHitch.GetFoodItem();
			if (foodItem != null && foodItem.amount > 0)
			{
				ItemModConsumable component = foodItem.info.GetComponent<ItemModConsumable>();
				if (component)
				{
					float num = component.GetIfType(MetabolismAttribute.Type.Calories) * this.currentHitch.caloriesToDecaySeconds;
					base.AddDecayDelay(num);
					base.ReplenishFromFood(component);
					foodItem.UseItem(1);
					this.nextEatTime = UnityEngine.Time.time + UnityEngine.Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, base.StaminaCoreFraction()) * 4f;
					return;
				}
			}
		}
		base.EatNearbyFood();
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0008F66C File Offset: 0x0008D86C
	public void TryLeaveHitch()
	{
		if (this.currentHitch)
		{
			this.currentHitch.Unhitch(this);
		}
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0008F688 File Offset: 0x0008D888
	public void TryHitch()
	{
		List<HitchTrough> list = Facepunch.Pool.GetList<HitchTrough>();
		global::Vis.Entities<HitchTrough>(base.transform.position, 2.5f, list, 256, QueryTriggerInteraction.Ignore);
		foreach (HitchTrough hitchTrough in list)
		{
			if (Vector3.Dot(Vector3Ex.Direction2D(hitchTrough.transform.position, base.transform.position), base.transform.forward) >= 0.4f && !hitchTrough.isClient && hitchTrough.HasSpace() && hitchTrough.ValidHitchPosition(base.transform.position) && hitchTrough.AttemptToHitch(this, null))
			{
				break;
			}
		}
		Facepunch.Pool.FreeList<HitchTrough>(ref list);
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0008F75C File Offset: 0x0008D95C
	public void RecordDistance()
	{
		global::BasePlayer driver = base.GetDriver();
		if (driver == null)
		{
			this.tempDistanceTravelled = 0f;
			return;
		}
		this.kmDistance += this.tempDistanceTravelled / 1000f;
		if (this.kmDistance >= 1f)
		{
			driver.stats.Add(this.distanceStatName + "_km", 1, (global::Stats)5);
			this.kmDistance -= 1f;
		}
		driver.stats.Add(this.distanceStatName, Mathf.FloorToInt(this.tempDistanceTravelled), global::Stats.Steam);
		driver.stats.Save(false);
		this.totalDistance += this.tempDistanceTravelled;
		this.tempDistanceTravelled = 0f;
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0008F821 File Offset: 0x0008DA21
	public override void MarkDistanceTravelled(float amount)
	{
		this.tempDistanceTravelled += amount;
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0008F834 File Offset: 0x0008DA34
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Facepunch.Pool.Get<ProtoBuf.Horse>();
		info.msg.horse.staminaSeconds = this.staminaSeconds;
		info.msg.horse.currentMaxStaminaSeconds = this.currentMaxStaminaSeconds;
		info.msg.horse.breedIndex = this.currentBreed;
		info.msg.horse.numStorageSlots = this.numStorageSlots;
		if (!info.forDisk)
		{
			info.msg.horse.runState = (int)this.currentRunState;
			info.msg.horse.maxSpeed = this.GetRunSpeed();
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0008F8E4 File Offset: 0x0008DAE4
	public override void OnClaimedWithToken(global::Item tokenItem)
	{
		base.OnClaimedWithToken(tokenItem);
		this.SetSeatCount(this.GetSaddleItemSeatCount(tokenItem));
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0008F8FA File Offset: 0x0008DAFA
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0008F904 File Offset: 0x0008DB04
	public override void OnInventoryDirty()
	{
		this.EquipmentUpdate();
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0008F90C File Offset: 0x0008DB0C
	public override bool CanAnimalAcceptItem(global::Item item, int targetSlot)
	{
		ItemModAnimalEquipment component = item.info.GetComponent<ItemModAnimalEquipment>();
		if (base.IsForSale() && this.ItemIsSaddle(item) && targetSlot >= 0 && targetSlot < this.numEquipmentSlots)
		{
			return false;
		}
		if (targetSlot >= 0 && targetSlot < this.numEquipmentSlots && !component)
		{
			return false;
		}
		if (this.ItemIsSaddle(item) && this.HasSaddle())
		{
			return false;
		}
		if (targetSlot < this.numEquipmentSlots)
		{
			if (component.slot == ItemModAnimalEquipment.SlotType.Basic)
			{
				return true;
			}
			for (int i = 0; i < this.numEquipmentSlots; i++)
			{
				global::Item slot = this.inventory.GetSlot(i);
				if (slot != null)
				{
					ItemModAnimalEquipment component2 = slot.info.GetComponent<ItemModAnimalEquipment>();
					if (!(component2 == null) && component2.slot == component.slot)
					{
						Debug.Log(string.Concat(new object[]
						{
							"rejecting because slot same, found : ",
							(int)component2.slot,
							" new : ",
							(int)component.slot
						}));
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0008FA08 File Offset: 0x0008DC08
	public int GetStorageStartIndex()
	{
		return this.numEquipmentSlots;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0008FA10 File Offset: 0x0008DC10
	public void EquipmentUpdate()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, false);
		this.riderProtection.Clear();
		this.baseProtection.Clear();
		this.equipmentSpeedMod = 0f;
		this.numStorageSlots = 0;
		for (int i = 0; i < this.numEquipmentSlots; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModAnimalEquipment component = slot.info.GetComponent<ItemModAnimalEquipment>();
				if (component)
				{
					base.SetFlag(component.WearableFlag, true, false, false);
					if (component.hideHair)
					{
						base.SetFlag(global::BaseEntity.Flags.Reserved4, true, false, true);
					}
					if (component.riderProtection)
					{
						this.riderProtection.Add(component.riderProtection, 1f);
					}
					if (component.animalProtection)
					{
						this.baseProtection.Add(component.animalProtection, 1f);
					}
					this.equipmentSpeedMod += component.speedModifier;
					this.numStorageSlots += component.additionalInventorySlots;
				}
			}
		}
		for (int j = this.GetStorageStartIndex(); j < this.inventory.capacity; j++)
		{
			if (j >= this.GetStorageStartIndex() + this.numStorageSlots)
			{
				global::Item slot2 = this.inventory.GetSlot(j);
				if (slot2 != null)
				{
					slot2.RemoveFromContainer();
					slot2.Drop(base.transform.position + Vector3.up + UnityEngine.Random.insideUnitSphere * 0.25f, Vector3.zero, default(Quaternion));
				}
			}
		}
		this.inventory.capacity = this.GetStorageStartIndex() + this.numStorageSlots;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x0008FBE4 File Offset: 0x0008DDE4
	private void SetSeatCount(int count)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved9, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, false);
		if (count == 1)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved9, true, false, false);
		}
		else if (count == 2)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved10, true, false, false);
		}
		this.UpdateMountFlags();
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x0008FC3C File Offset: 0x0008DE3C
	public override void DoNetworkUpdate()
	{
		bool flag = false || this.prevStamina != this.staminaSeconds || this.prevMaxStamina != this.currentMaxStaminaSeconds || this.prevBreed != this.currentBreed || this.prevSlots != this.numStorageSlots || this.prevRunState != (int)this.currentRunState || this.prevMaxSpeed != this.GetRunSpeed();
		this.prevStamina = this.staminaSeconds;
		this.prevMaxStamina = this.currentMaxStaminaSeconds;
		this.prevRunState = (int)this.currentRunState;
		this.prevMaxSpeed = this.GetRunSpeed();
		this.prevBreed = this.currentBreed;
		this.prevSlots = this.numStorageSlots;
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x0008FD20 File Offset: 0x0008DF20
	public int GetSaddleItemSeatCount(global::Item item)
	{
		if (!this.ItemIsSaddle(item))
		{
			return 0;
		}
		ItemModAnimalEquipment component = item.info.GetComponent<ItemModAnimalEquipment>();
		if (component.slot == ItemModAnimalEquipment.SlotType.Saddle)
		{
			return 1;
		}
		if (component.slot == ItemModAnimalEquipment.SlotType.SaddleDouble)
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0008FD5B File Offset: 0x0008DF5B
	public bool HasSaddle()
	{
		return this.HasSingleSaddle() || this.HasDoubleSaddle();
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0008FD6D File Offset: 0x0008DF6D
	public bool HasSingleSaddle()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved9);
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x0008FD7A File Offset: 0x0008DF7A
	public bool HasDoubleSaddle()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved10);
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0008FD88 File Offset: 0x0008DF88
	private bool ItemIsSaddle(global::Item item)
	{
		if (item == null)
		{
			return false;
		}
		ItemModAnimalEquipment component = item.info.GetComponent<ItemModAnimalEquipment>();
		return !(component == null) && (component.slot == ItemModAnimalEquipment.SlotType.Saddle || component.slot == ItemModAnimalEquipment.SlotType.SaddleDouble);
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0008FDC8 File Offset: 0x0008DFC8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			this.staminaSeconds = info.msg.horse.staminaSeconds;
			this.currentMaxStaminaSeconds = info.msg.horse.currentMaxStaminaSeconds;
			this.numStorageSlots = info.msg.horse.numStorageSlots;
			this.ApplyBreed(info.msg.horse.breedIndex);
		}
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x0008FE41 File Offset: 0x0008E041
	public override bool HasValidSaddle()
	{
		return this.HasSaddle();
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0008FE49 File Offset: 0x0008E049
	public override bool HasSeatAvailable()
	{
		return this.HasValidSaddle() && !base.HasFlag(global::BaseEntity.Flags.Reserved11);
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0008FE65 File Offset: 0x0008E065
	public int GetSeatCapacity()
	{
		if (this.HasDoubleSaddle())
		{
			return 2;
		}
		if (this.HasSingleSaddle())
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x0008FE7C File Offset: 0x0008E07C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_ReqSwapSaddleType(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (base.IsForSale())
		{
			return;
		}
		if (!this.HasSaddle())
		{
			return;
		}
		if (this.AnyMounted())
		{
			return;
		}
		int num = msg.read.Int32();
		global::Item purchaseToken = base.GetPurchaseToken(player, num);
		if (purchaseToken == null)
		{
			return;
		}
		ItemDefinition itemDefinition = (this.HasSingleSaddle() ? this.PurchaseOptions[0].TokenItem : this.PurchaseOptions[1].TokenItem);
		this.OnClaimedWithToken(purchaseToken);
		purchaseToken.UseItem(1);
		global::Item item = ItemManager.Create(itemDefinition, 1, 0UL);
		player.GiveItem(item, global::BaseEntity.GiveItemReason.Generic);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0008FF20 File Offset: 0x0008E120
	public override int MaxMounted()
	{
		return this.GetSeatCapacity();
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0008FF28 File Offset: 0x0008E128
	[ServerVar]
	public static void setHorseBreed(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		int @int = arg.GetInt(0, 0);
		List<RidableHorse> list = Facepunch.Pool.GetList<RidableHorse>();
		global::Vis.Entities<RidableHorse>(basePlayer.eyes.position, basePlayer.eyes.position + basePlayer.eyes.HeadForward() * 5f, 0f, list, -1, QueryTriggerInteraction.Collide);
		foreach (RidableHorse ridableHorse in list)
		{
			ridableHorse.SetBreed(@int);
		}
		Facepunch.Pool.FreeList<RidableHorse>(ref list);
	}

	// Token: 0x04000AE5 RID: 2789
	public Translate.Phrase SwapToSingleTitle;

	// Token: 0x04000AE6 RID: 2790
	public Translate.Phrase SwapToSingleDescription;

	// Token: 0x04000AE7 RID: 2791
	public Sprite SwapToSingleIcon;

	// Token: 0x04000AE8 RID: 2792
	public Translate.Phrase SwapToDoubleTitle;

	// Token: 0x04000AE9 RID: 2793
	public Translate.Phrase SwapToDoubleDescription;

	// Token: 0x04000AEA RID: 2794
	public Sprite SwapToDoubleIcon;

	// Token: 0x04000AEB RID: 2795
	public ItemDefinition WildSaddleItem;

	// Token: 0x04000AEC RID: 2796
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	// Token: 0x04000AED RID: 2797
	public string distanceStatName = "";

	// Token: 0x04000AEE RID: 2798
	public HorseBreed[] breeds;

	// Token: 0x04000AEF RID: 2799
	public SkinnedMeshRenderer[] bodyRenderers;

	// Token: 0x04000AF0 RID: 2800
	public SkinnedMeshRenderer[] hairRenderers;

	// Token: 0x04000AF1 RID: 2801
	private int currentBreed = -1;

	// Token: 0x04000AF2 RID: 2802
	private ProtectionProperties riderProtection;

	// Token: 0x04000AF3 RID: 2803
	private ProtectionProperties baseHorseProtection;

	// Token: 0x04000AF4 RID: 2804
	public const global::BaseEntity.Flags Flag_HideHair = global::BaseEntity.Flags.Reserved4;

	// Token: 0x04000AF5 RID: 2805
	public const global::BaseEntity.Flags Flag_WoodArmor = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000AF6 RID: 2806
	public const global::BaseEntity.Flags Flag_RoadsignArmor = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000AF7 RID: 2807
	public const global::BaseEntity.Flags Flag_HasSingleSaddle = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000AF8 RID: 2808
	public const global::BaseEntity.Flags Flag_HasDoubleSaddle = global::BaseEntity.Flags.Reserved10;

	// Token: 0x04000AF9 RID: 2809
	private float equipmentSpeedMod;

	// Token: 0x04000AFA RID: 2810
	private int numStorageSlots;

	// Token: 0x04000AFB RID: 2811
	private int prevBreed;

	// Token: 0x04000AFC RID: 2812
	private int prevSlots;

	// Token: 0x04000AFD RID: 2813
	private static Material[] breedAssignmentArray = new Material[2];

	// Token: 0x04000AFE RID: 2814
	private float distanceRecordingSpacing = 5f;

	// Token: 0x04000AFF RID: 2815
	private HitchTrough currentHitch;

	// Token: 0x04000B00 RID: 2816
	private float totalDistance;

	// Token: 0x04000B01 RID: 2817
	private float kmDistance;

	// Token: 0x04000B02 RID: 2818
	private float tempDistanceTravelled;

	// Token: 0x04000B03 RID: 2819
	private int numEquipmentSlots = 4;
}
