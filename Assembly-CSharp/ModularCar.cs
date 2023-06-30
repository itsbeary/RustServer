using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A1 RID: 161
public class ModularCar : BaseModularVehicle, TakeCollisionDamage.ICanRestoreVelocity, CarPhysics<global::ModularCar>.ICar, IVehicleLockUser, VehicleChassisVisuals<global::ModularCar>.IClientWheelUser
{
	// Token: 0x06000E90 RID: 3728 RVA: 0x0007B754 File Offset: 0x00079954
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCar.OnRpcMessage", 0))
		{
			if (rpc == 1851540757U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuel ");
				}
				using (TimeWarning.New("RPC_OpenFuel", 0))
				{
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
							this.RPC_OpenFuel(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
			if (rpc == 1382140449U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuelWithKeycode ");
				}
				using (TimeWarning.New("RPC_OpenFuelWithKeycode", 0))
				{
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
							this.RPC_OpenFuelWithKeycode(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenFuelWithKeycode");
					}
				}
				return true;
			}
			if (rpc == 2818660542U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TryMountWithKeycode ");
				}
				using (TimeWarning.New("RPC_TryMountWithKeycode", 0))
				{
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
							this.RPC_TryMountWithKeycode(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_TryMountWithKeycode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000E91 RID: 3729 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool AlwaysAllowBradleyTargeting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000E92 RID: 3730 RVA: 0x0007BAC0 File Offset: 0x00079CC0
	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (this.serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return this.serverTerrainHandler.OnSurface;
		}
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0007BAD8 File Offset: 0x00079CD8
	public override void ServerInit()
	{
		base.ServerInit();
		this.carPhysics = new CarPhysics<global::ModularCar>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnPreassignedModules();
		}
		this.lastEngineOnTime = UnityEngine.Time.realtimeSinceStartup;
		global::ModularCar.allCarsList.Add(this);
		this.collisionCheckBounds = new Bounds(this.mainChassisCollider.center, new Vector3(this.mainChassisCollider.size.x - 0.5f, 0.05f, this.mainChassisCollider.size.z - 0.5f));
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0007BBD1 File Offset: 0x00079DD1
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		global::ModularCar.allCarsList.Remove(this);
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0007BBE5 File Offset: 0x00079DE5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.CarLock.PostServerLoad();
		if (this.IsDead())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0007BC08 File Offset: 0x00079E08
	public float GetSteerInput()
	{
		float num = 0f;
		BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].steerInput;
		}
		return Mathf.Clamp(num, -1f, 1f);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0007BC58 File Offset: 0x00079E58
	public bool GetSteerModInput()
	{
		BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].steerMod)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0007BC94 File Offset: 0x00079E94
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.engineController.CheckEngineState();
		this.hurtTriggerFront.gameObject.SetActive(speed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(speed < -this.hurtTriggerMinSpeed);
		this.serverTerrainHandler.FixedUpdate();
		float num = Mathf.Abs(speed);
		if (this.lastPosWasBad || num > 15f)
		{
			if (GamePhysics.CheckOBB(new OBB(this.mainChassisCollider.transform, this.collisionCheckBounds), 1218511105, QueryTriggerInteraction.Ignore))
			{
				this.rigidBody.position = this.lastGoodPos;
				this.rigidBody.rotation = this.lastGoodRot;
				base.transform.position = this.lastGoodPos;
				base.transform.rotation = this.lastGoodRot;
				this.rigidBody.velocity = Vector3.zero;
				this.rigidBody.angularVelocity = Vector3.zero;
				this.lastPosWasBad = true;
			}
			else
			{
				this.lastGoodPos = this.rigidBody.position;
				this.lastGoodRot = this.rigidBody.rotation;
				this.lastPosWasBad = false;
			}
		}
		else
		{
			this.lastGoodPos = this.rigidBody.position;
			this.lastGoodRot = this.rigidBody.rotation;
			this.lastPosWasBad = false;
		}
		if (base.IsMoving())
		{
			Vector3 commultiplier = this.GetCOMMultiplier();
			if (commultiplier != this.prevCOMMultiplier)
			{
				this.rigidBody.centerOfMass = Vector3.Scale(this.realLocalCOM, commultiplier);
				this.prevCOMMultiplier = commultiplier;
			}
		}
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0007BE44 File Offset: 0x0007A044
	protected override bool DetermineIfStationary()
	{
		bool flag = this.rigidBody.position == this.prevPosition && this.rigidBody.rotation == this.prevRotation;
		this.prevPosition = this.rigidBody.position;
		this.prevRotation = this.rigidBody.rotation;
		return flag;
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x0007BEA4 File Offset: 0x0007A0A4
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		global::BaseVehicle.MountPointInfo playerSeatInfo = base.GetPlayerSeatInfo(player);
		if (playerSeatInfo == null || !playerSeatInfo.isDriver)
		{
			return;
		}
		if (!this.driverSeatInputs.Contains(playerSeatInfo.mountable))
		{
			this.driverSeatInputs.Add(playerSeatInfo.mountable, new global::ModularCar.DriverSeatInputs());
		}
		global::ModularCar.DriverSeatInputs driverSeatInputs = this.driverSeatInputs[playerSeatInfo.mountable];
		if (inputState.IsDown(BUTTON.DUCK))
		{
			driverSeatInputs.steerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			driverSeatInputs.steerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				driverSeatInputs.steerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				driverSeatInputs.steerInput = 1f;
			}
		}
		driverSeatInputs.steerMod = inputState.IsDown(BUTTON.SPRINT);
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		driverSeatInputs.throttleInput = 0f;
		driverSeatInputs.brakeInput = 0f;
		if (base.GetSpeed() > 3f && num < -0.1f)
		{
			driverSeatInputs.throttleInput = 0f;
			driverSeatInputs.brakeInput = -num;
		}
		else
		{
			driverSeatInputs.throttleInput = num;
			driverSeatInputs.brakeInput = 0f;
		}
		for (int i = 0; i < base.NumAttachedModules; i++)
		{
			base.AttachedModuleEntities[i].PlayerServerInput(inputState, player);
		}
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0007C04C File Offset: 0x0007A24C
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		global::ModularCar.DriverSeatInputs driverSeatInputs;
		if (this.driverSeatInputs.TryGetValue(seat, out driverSeatInputs))
		{
			this.driverSeatInputs.Remove(seat);
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (baseVehicleModule != null)
			{
				baseVehicleModule.OnPlayerDismountedVehicle(player);
			}
		}
		this.CarLock.CheckEnableCentralLocking();
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0007C0D8 File Offset: 0x0007A2D8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.modularCar = Facepunch.Pool.Get<ProtoBuf.ModularCar>();
		info.msg.modularCar.steerAngle = this.SteerAngle;
		info.msg.modularCar.driveWheelVel = this.DriveWheelVelocity;
		info.msg.modularCar.throttleInput = this.GetThrottleInput();
		info.msg.modularCar.brakeInput = this.GetBrakeInput();
		info.msg.modularCar.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.modularCar.fuelFraction = this.GetFuelFraction();
		this.CarLock.Save(info);
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0007C198 File Offset: 0x0007A398
	public override void Hurt(HitInfo info)
	{
		if (!this.IsDead() && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			this.PropagateDamageToModules(info, 0.5f / (float)base.NumAttachedModules, 0.9f / (float)base.NumAttachedModules, null);
		}
		base.Hurt(info);
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0007C1EA File Offset: 0x0007A3EA
	public void TickFuel(float fuelUsedPerSecond)
	{
		this.engineController.TickFuel(fuelUsedPerSecond);
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0007C1FC File Offset: 0x0007A3FC
	public override bool MountEligable(global::BasePlayer player)
	{
		if (!base.MountEligable(player))
		{
			return false;
		}
		ModularCarSeat modularCarSeat = base.GetIdealMountPointFor(player) as ModularCarSeat;
		return (modularCarSeat != null && !modularCarSeat.associatedSeatingModule.DoorsAreLockable) || this.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0007C241 File Offset: 0x0007A441
	public override bool IsComplete()
	{
		return this.HasAnyEngines() && base.HasDriverMountPoints() && !this.IsDead();
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0007C260 File Offset: 0x0007A460
	public void DoDecayDamage(float damage)
	{
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (!baseVehicleModule.IsDestroyed)
			{
				baseVehicleModule.Hurt(damage, DamageType.Decay, null, true);
			}
		}
		if (!base.HasAnyModules)
		{
			base.Hurt(damage, DamageType.Decay, null, true);
		}
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0007C2D4 File Offset: 0x0007A4D4
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetAdjustedDriveForce(absSpeed, topSpeed);
		}
		return this.RollOffDriveForce(num);
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0007C31C File Offset: 0x0007A51C
	public bool HasAnyEngines()
	{
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			if (base.AttachedModuleEntities[i].HasAnEngine)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0007C355 File Offset: 0x0007A555
	public bool HasAnyWorkingEngines()
	{
		return this.GetMaxDriveForce() > 0f;
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0007C364 File Offset: 0x0007A564
	public override bool MeetsEngineRequirements()
	{
		return this.HasAnyWorkingEngines() && base.HasDriver();
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0007C378 File Offset: 0x0007A578
	public override void OnEngineStartFailed()
	{
		bool flag = !this.HasAnyWorkingEngines() || this.engineController.IsWaterlogged();
		base.ClientRPC<bool>(null, "EngineStartFailed", flag);
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0007C3A9 File Offset: 0x0007A5A9
	public CarWheel[] GetWheels()
	{
		if (this.wheels == null)
		{
			this.wheels = new CarWheel[] { this.wheelFL, this.wheelFR, this.wheelRL, this.wheelRR };
		}
		return this.wheels;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0007C3EC File Offset: 0x0007A5EC
	public float GetWheelsMidPos()
	{
		return (this.wheels[0].wheelCollider.transform.localPosition.z - this.wheels[2].wheelCollider.transform.localPosition.z) * 0.5f;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0007C438 File Offset: 0x0007A638
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.AdminFixUp(tier);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0007C4A0 File Offset: 0x0007A6A0
	public override void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info)
	{
		if (this.IsDead())
		{
			if (this.timeSinceDeath > 1f)
			{
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					this.deathDamageCounter += info.damageTypes.types[i];
				}
			}
			if (this.deathDamageCounter > 600f && !base.IsDestroyed)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return;
			}
		}
		else if (hurtModule.PropagateDamage && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			this.PropagateDamageToModules(info, 0.15f, 0.4f, hurtModule);
		}
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0007C544 File Offset: 0x0007A744
	private void PropagateDamageToModules(HitInfo info, float minPropagationPercent, float maxPropagationPercent, BaseVehicleModule ignoreModule)
	{
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (!(baseVehicleModule == ignoreModule) && baseVehicleModule.Health() > 0f)
			{
				if (this.IsDead())
				{
					break;
				}
				float num = UnityEngine.Random.Range(minPropagationPercent, maxPropagationPercent);
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					float num2 = info.damageTypes.types[i];
					if (num2 > 0f)
					{
						baseVehicleModule.AcceptPropagatedDamage(num2 * num, (DamageType)i, info.Initiator, info.UseProtection);
					}
					if (this.IsDead())
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0007C610 File Offset: 0x0007A810
	public override void ModuleReachedZeroHealth()
	{
		if (this.IsDead())
		{
			return;
		}
		bool flag = true;
		using (List<BaseVehicleModule>.Enumerator enumerator = base.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.health > 0f)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			this.Die(null);
		}
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0007C680 File Offset: 0x0007A880
	public override void OnKilled(HitInfo info)
	{
		this.DismountAllPlayers();
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.repair.enabled = false;
		}
		if (this.CarLock != null)
		{
			this.CarLock.RemoveLock();
		}
		this.timeSinceDeath = 0f;
		if (!vehicle.carwrecks)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (!base.HasAnyModules)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0007C728 File Offset: 0x0007A928
	public void RemoveLock()
	{
		this.CarLock.RemoveLock();
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0007C738 File Offset: 0x0007A938
	public void RestoreVelocity(Vector3 vel)
	{
		if (this.rigidBody.velocity.sqrMagnitude < vel.sqrMagnitude)
		{
			vel.y = this.rigidBody.velocity.y;
			this.rigidBody.velocity = vel;
		}
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0007C784 File Offset: 0x0007A984
	protected override Vector3 GetCOMMultiplier()
	{
		if (this.carPhysics == null || !this.carPhysics.IsGrounded() || !base.IsOn())
		{
			return this.airbourneCOMMultiplier;
		}
		return this.groundedCOMMultiplier;
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0007C7B4 File Offset: 0x0007A9B4
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.GetThrottleInput() + 1f) * 7f));
			byte b = (byte)(this.GetBrakeInput() * 15f);
			byte b2 = (byte)(num + ((int)b << 4));
			byte b3 = (byte)(this.GetFuelFraction() * 255f);
			base.ClientRPC<float, byte, float, byte>(null, "ModularCarUpdate", this.SteerAngle, b2, this.DriveWheelVelocity, b3);
		}
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0007C818 File Offset: 0x0007AA18
	private void DecayTick()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.IsOn() || this.immuneToDecay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f;
		if (this.IsDead())
		{
			int num2 = Mathf.Max(1, base.AttachedModuleEntities.Count);
			num /= 5f * (float)num2;
			this.DoDecayDamage(600f * num);
			return;
		}
		num /= global::ModularCar.outsidedecayminutes;
		if (!this.IsOutside())
		{
			num *= 0.1f;
		}
		float num3;
		if (!base.HasAnyModules)
		{
			num3 = this.MaxHealth();
		}
		else
		{
			num3 = base.AttachedModuleEntities.Max((BaseVehicleModule module) => module.MaxHealth());
		}
		this.DoDecayDamage(num3 * num);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0007C8EC File Offset: 0x0007AAEC
	protected override void DoCollisionDamage(global::BaseEntity hitEntity, float damage)
	{
		if (hitEntity == null)
		{
			return;
		}
		BaseVehicleModule baseVehicleModule;
		if ((baseVehicleModule = hitEntity as BaseVehicleModule) != null)
		{
			baseVehicleModule.Hurt(damage, DamageType.Collision, this, false);
			return;
		}
		if (hitEntity == this)
		{
			if (!base.HasAnyModules)
			{
				base.Hurt(damage, DamageType.Collision, this, false);
				return;
			}
			float num = damage / (float)base.NumAttachedModules;
			foreach (BaseVehicleModule baseVehicleModule2 in base.AttachedModuleEntities)
			{
				baseVehicleModule2.AcceptPropagatedDamage(num, DamageType.Collision, this, false);
			}
		}
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0007C988 File Offset: 0x0007AB88
	private void SpawnPreassignedModules()
	{
		if (!this.spawnSettings.useSpawnSettings)
		{
			return;
		}
		if (this.spawnSettings.configurationOptions.IsNullOrEmpty<ModularCarPresetConfig>())
		{
			return;
		}
		ModularCarPresetConfig modularCarPresetConfig = this.spawnSettings.configurationOptions[UnityEngine.Random.Range(0, this.spawnSettings.configurationOptions.Length)];
		for (int i = 0; i < modularCarPresetConfig.socketItemDefs.Length; i++)
		{
			ItemModVehicleModule itemModVehicleModule = modularCarPresetConfig.socketItemDefs[i];
			if (itemModVehicleModule != null && base.Inventory.SocketsAreFree(i, itemModVehicleModule.socketsTaken, null))
			{
				itemModVehicleModule.doNonUserSpawn = true;
				global::Item item = ItemManager.Create(itemModVehicleModule.GetComponent<ItemDefinition>(), 1, 0UL);
				float num = UnityEngine.Random.Range(this.spawnSettings.minStartHealthPercent, this.spawnSettings.maxStartHealthPercent);
				item.condition = item.maxCondition * num;
				if (!base.TryAddModule(item))
				{
					item.Remove(0f);
				}
			}
		}
		base.Invoke(new Action(this.HandleAdminBonus), 0f);
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x0007CA84 File Offset: 0x0007AC84
	private void HandleAdminBonus()
	{
		switch (this.spawnSettings.adminBonus)
		{
		case global::ModularCar.SpawnSettings.AdminBonus.T1PlusFuel:
			this.AdminFixUp(1);
			return;
		case global::ModularCar.SpawnSettings.AdminBonus.T2PlusFuel:
			this.AdminFixUp(2);
			return;
		case global::ModularCar.SpawnSettings.AdminBonus.T3PlusFuel:
			this.AdminFixUp(3);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x0007CACC File Offset: 0x0007ACCC
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
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
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0007CB00 File Offset: 0x0007AD00
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuelWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string text = msg.read.String(256);
		if (!this.CarLock.TryOpenWithCode(player, text))
		{
			base.ClientRPC(null, "CodeEntryFailed");
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0007CB64 File Offset: 0x0007AD64
	[global::BaseEntity.RPC_Server]
	public void RPC_TryMountWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string text = msg.read.String(256);
		if (this.CarLock.TryOpenWithCode(player, text))
		{
			base.WantsMount(player);
			return;
		}
		base.ClientRPC(null, "CodeEntryFailed");
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x0007CBB8 File Offset: 0x0007ADB8
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			VehicleModuleSeating vehicleModuleSeating;
			if (baseVehicleModule.HasSeating && (vehicleModuleSeating = baseVehicleModule as VehicleModuleSeating) != null && vehicleModuleSeating.IsOnThisModule(player))
			{
				baseVehicleModule.ScaleDamageForPlayer(player, info);
			}
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000EBA RID: 3770 RVA: 0x0007CC30 File Offset: 0x0007AE30
	public override float DriveWheelVelocity
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelVelocity;
			}
			return 0f;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0007CC4B File Offset: 0x0007AE4B
	public float DriveWheelSlip
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelSlip;
			}
			return 0f;
		}
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000EBC RID: 3772 RVA: 0x0007CC66 File Offset: 0x0007AE66
	public float SteerAngle
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.SteerAngle;
			}
			return 0f;
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000EBD RID: 3773 RVA: 0x000512B3 File Offset: 0x0004F4B3
	public ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000EBE RID: 3774 RVA: 0x0007CC81 File Offset: 0x0007AE81
	public float MaxSteerAngle
	{
		get
		{
			return this.carSettings.maxSteerAngle;
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0007CC8E File Offset: 0x0007AE8E
	public override bool IsLockable
	{
		get
		{
			return this.CarLock.HasALock;
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x0007CC9B File Offset: 0x0007AE9B
	// (set) Token: 0x06000EC1 RID: 3777 RVA: 0x0007CCA3 File Offset: 0x0007AEA3
	public ModularCarCodeLock CarLock { get; private set; }

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0007CCAC File Offset: 0x0007AEAC
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.damageShowingRenderers = base.GetComponentsInChildren<MeshRenderer>();
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0007CCC9 File Offset: 0x0007AEC9
	public override void InitShared()
	{
		base.InitShared();
		if (this.CarLock == null)
		{
			this.CarLock = new ModularCarCodeLock(this, base.isServer);
		}
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0007CCEB File Offset: 0x0007AEEB
	public override float MaxHealth()
	{
		return this.AssociatedItemDef.condition.max;
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0007CCEB File Offset: 0x0007AEEB
	public override float StartHealth()
	{
		return this.AssociatedItemDef.condition.max;
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0007CD00 File Offset: 0x0007AF00
	public float TotalHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].Health();
		}
		return num;
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0007CD40 File Offset: 0x0007AF40
	public float TotalMaxHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].MaxHealth();
		}
		return num;
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0007CD80 File Offset: 0x0007AF80
	public override float GetMaxForwardSpeed()
	{
		float num = this.GetMaxDriveForce() / base.TotalMass * 30f;
		return Mathf.Pow(0.9945f, num) * num;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x0007CDB0 File Offset: 0x0007AFB0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.modularCar != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.modularCar.fuelStorageID;
			this.cachedFuelFraction = info.msg.modularCar.fuelFraction;
			bool hasALock = this.CarLock.HasALock;
			this.CarLock.Load(info);
			if (this.CarLock.HasALock != hasALock)
			{
				for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
				{
					base.AttachedModuleEntities[i].RefreshConditionals(true);
				}
			}
		}
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0007CE5D File Offset: 0x0007B05D
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		this.RefreshEngineState();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0007CE74 File Offset: 0x0007B074
	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].throttleInput;
			}
			return Mathf.Clamp(num, -1f, 1f);
		}
		return 0f;
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0007CED4 File Offset: 0x0007B0D4
	public override float GetBrakeInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].brakeInput;
			}
			return Mathf.Clamp01(num);
		}
		return 0f;
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0007CF28 File Offset: 0x0007B128
	public float GetMaxDriveForce()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetMaxDriveForce();
		}
		return this.RollOffDriveForce(num);
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0007CF6C File Offset: 0x0007B16C
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelFraction();
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0007CF8D File Offset: 0x0007B18D
	public bool PlayerHasUnlockPermission(global::BasePlayer player)
	{
		return this.CarLock.HasLockPermission(player);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0007CF9B File Offset: 0x0007B19B
	public bool KeycodeEntryBlocked(global::BasePlayer player)
	{
		return this.CarLock.CodeEntryBlocked(player);
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0007CFA9 File Offset: 0x0007B1A9
	public override bool PlayerCanUseThis(global::BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return this.CarLock.PlayerCanUseThis(player, lockType);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0007CFB8 File Offset: 0x0007B1B8
	public bool PlayerCanDestroyLock(global::BasePlayer player, BaseVehicleModule viaModule)
	{
		return this.CarLock.PlayerCanDestroyLock(viaModule);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0007CFC6 File Offset: 0x0007B1C6
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return !(player == null) && (this.PlayerIsMounted(player) || (this.PlayerCanUseThis(player, ModularCarCodeLock.LockType.General) && !base.IsOn()));
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0007CFF3 File Offset: 0x0007B1F3
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && (!pusher.InSafeZone() || this.CarLock.HasLockPermission(pusher));
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0007D01C File Offset: 0x0007B21C
	protected bool RefreshEngineState()
	{
		if (this.lastSetEngineState == base.CurEngineState)
		{
			return false;
		}
		if (base.isServer && base.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.Off)
		{
			this.lastEngineOnTime = UnityEngine.Time.time;
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.OnEngineStateChanged(this.lastSetEngineState, base.CurEngineState);
		}
		if (base.isServer && GameInfo.HasAchievements && base.NumMounted() >= 5)
		{
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null)
				{
					mountPointInfo.mountable.GetMounted().GiveAchievement("BATTLE_BUS");
				}
			}
		}
		this.lastSetEngineState = base.CurEngineState;
		return true;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0007D13C File Offset: 0x0007B33C
	private float RollOffDriveForce(float driveForce)
	{
		return Mathf.Pow(0.9999175f, driveForce) * driveForce;
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0007D14B File Offset: 0x0007B34B
	private void RefreshChassisProtectionState()
	{
		if (base.HasAnyModules)
		{
			this.baseProtection = this.immortalProtection;
			if (base.isServer)
			{
				base.SetHealth(this.MaxHealth());
				return;
			}
		}
		else
		{
			this.baseProtection = this.mortalProtection;
		}
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0007D182 File Offset: 0x0007B382
	protected override void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		base.ModuleEntityAdded(addedModule);
		this.RefreshChassisProtectionState();
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0007D191 File Offset: 0x0007B391
	protected override void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		base.ModuleEntityRemoved(removedModule);
		this.RefreshChassisProtectionState();
	}

	// Token: 0x0400097C RID: 2428
	public static HashSet<global::ModularCar> allCarsList = new HashSet<global::ModularCar>();

	// Token: 0x0400097D RID: 2429
	private readonly ListDictionary<BaseMountable, global::ModularCar.DriverSeatInputs> driverSeatInputs = new ListDictionary<BaseMountable, global::ModularCar.DriverSeatInputs>();

	// Token: 0x0400097E RID: 2430
	private CarPhysics<global::ModularCar> carPhysics;

	// Token: 0x0400097F RID: 2431
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x04000980 RID: 2432
	private CarWheel[] wheels;

	// Token: 0x04000981 RID: 2433
	private float lastEngineOnTime;

	// Token: 0x04000982 RID: 2434
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000983 RID: 2435
	private const float INSIDE_DECAY_MULTIPLIER = 0.1f;

	// Token: 0x04000984 RID: 2436
	private const float CORPSE_DECAY_MINUTES = 5f;

	// Token: 0x04000985 RID: 2437
	private Vector3 prevPosition;

	// Token: 0x04000986 RID: 2438
	private Quaternion prevRotation;

	// Token: 0x04000987 RID: 2439
	private Bounds collisionCheckBounds;

	// Token: 0x04000988 RID: 2440
	private Vector3 lastGoodPos;

	// Token: 0x04000989 RID: 2441
	private Quaternion lastGoodRot;

	// Token: 0x0400098A RID: 2442
	private bool lastPosWasBad;

	// Token: 0x0400098B RID: 2443
	private float deathDamageCounter;

	// Token: 0x0400098C RID: 2444
	private const float DAMAGE_TO_GIB = 600f;

	// Token: 0x0400098D RID: 2445
	private TimeSince timeSinceDeath;

	// Token: 0x0400098E RID: 2446
	private const float IMMUNE_TIME = 1f;

	// Token: 0x0400098F RID: 2447
	protected readonly Vector3 groundedCOMMultiplier = new Vector3(0.25f, 0.3f, 0.25f);

	// Token: 0x04000990 RID: 2448
	protected readonly Vector3 airbourneCOMMultiplier = new Vector3(0.25f, 0.75f, 0.25f);

	// Token: 0x04000991 RID: 2449
	private Vector3 prevCOMMultiplier;

	// Token: 0x04000992 RID: 2450
	[Header("Modular Car")]
	public ModularCarChassisVisuals chassisVisuals;

	// Token: 0x04000993 RID: 2451
	public VisualCarWheel wheelFL;

	// Token: 0x04000994 RID: 2452
	public VisualCarWheel wheelFR;

	// Token: 0x04000995 RID: 2453
	public VisualCarWheel wheelRL;

	// Token: 0x04000996 RID: 2454
	public VisualCarWheel wheelRR;

	// Token: 0x04000997 RID: 2455
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000998 RID: 2456
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000999 RID: 2457
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x0400099A RID: 2458
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x0400099B RID: 2459
	[SerializeField]
	private ProtectionProperties immortalProtection;

	// Token: 0x0400099C RID: 2460
	[SerializeField]
	private ProtectionProperties mortalProtection;

	// Token: 0x0400099D RID: 2461
	[SerializeField]
	private BoxCollider mainChassisCollider;

	// Token: 0x0400099E RID: 2462
	[SerializeField]
	private global::ModularCar.SpawnSettings spawnSettings;

	// Token: 0x0400099F RID: 2463
	[SerializeField]
	[HideInInspector]
	private MeshRenderer[] damageShowingRenderers;

	// Token: 0x040009A0 RID: 2464
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 3f;

	// Token: 0x040009A1 RID: 2465
	[ServerVar(Help = "How many minutes before a ModularCar loses all its health while outside")]
	public static float outsidedecayminutes = 864f;

	// Token: 0x040009A2 RID: 2466
	public const BUTTON RapidSteerButton = BUTTON.SPRINT;

	// Token: 0x040009A4 RID: 2468
	private VehicleEngineController<GroundVehicle>.EngineState lastSetEngineState;

	// Token: 0x040009A5 RID: 2469
	private float cachedFuelFraction;

	// Token: 0x02000BF2 RID: 3058
	private class DriverSeatInputs
	{
		// Token: 0x040041EA RID: 16874
		public float steerInput;

		// Token: 0x040041EB RID: 16875
		public bool steerMod;

		// Token: 0x040041EC RID: 16876
		public float brakeInput;

		// Token: 0x040041ED RID: 16877
		public float throttleInput;
	}

	// Token: 0x02000BF3 RID: 3059
	[Serializable]
	public class SpawnSettings
	{
		// Token: 0x040041EE RID: 16878
		[Tooltip("Must be true to use any of these settings.")]
		public bool useSpawnSettings;

		// Token: 0x040041EF RID: 16879
		[Tooltip("Specify a list of possible module configurations that'll automatically spawn with this vehicle.")]
		public ModularCarPresetConfig[] configurationOptions;

		// Token: 0x040041F0 RID: 16880
		[Tooltip("Min health % at spawn for any modules that spawn with this chassis.")]
		public float minStartHealthPercent = 0.15f;

		// Token: 0x040041F1 RID: 16881
		[Tooltip("Max health  % at spawn for any modules that spawn with this chassis.")]
		public float maxStartHealthPercent = 0.5f;

		// Token: 0x040041F2 RID: 16882
		public global::ModularCar.SpawnSettings.AdminBonus adminBonus;

		// Token: 0x02000FDB RID: 4059
		public enum AdminBonus
		{
			// Token: 0x0400516F RID: 20847
			None,
			// Token: 0x04005170 RID: 20848
			T1PlusFuel,
			// Token: 0x04005171 RID: 20849
			T2PlusFuel,
			// Token: 0x04005172 RID: 20850
			T3PlusFuel
		}
	}
}
