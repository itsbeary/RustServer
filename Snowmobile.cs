using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D2 RID: 210
public class Snowmobile : GroundVehicle, CarPhysics<global::Snowmobile>.ICar, TriggerHurtNotChild.IHurtTriggerUser, VehicleChassisVisuals<global::Snowmobile>.IClientWheelUser, IPrefabPreProcess
{
	// Token: 0x060012A8 RID: 4776 RVA: 0x0009656C File Offset: 0x0009476C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Snowmobile.OnRpcMessage", 0))
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
			if (rpc == 924237371U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenItemStorage ");
				}
				using (TimeWarning.New("RPC_OpenItemStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(924237371U, "RPC_OpenItemStorage", this, player, 3f))
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
							this.RPC_OpenItemStorage(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenItemStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x060012A9 RID: 4777 RVA: 0x00096820 File Offset: 0x00094A20
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

	// Token: 0x060012AA RID: 4778 RVA: 0x00096838 File Offset: 0x00094A38
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsed = 0f;
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.rigidBody.inertiaTensor = new Vector3(450f, 200f, 200f);
		this.carPhysics = new CarPhysics<global::Snowmobile>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.SnowmobileDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00096908 File Offset: 0x00094B08
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.serverTerrainHandler.FixedUpdate();
		if (base.IsOn())
		{
			float num = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.ThrottleInput));
			this.engineController.TickFuel(num);
		}
		this.engineController.CheckEngineState();
		RaycastHit raycastHit;
		if (!this.carPhysics.IsGrounded() && UnityEngine.Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, 1218511105, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector = raycastHit.normal;
			Vector3 right = base.transform.right;
			right.y = 0f;
			vector = Vector3.ProjectOnPlane(vector, right);
			float num2 = Vector3.Angle(vector, Vector3.up);
			float num3 = this.rigidBody.angularVelocity.magnitude * 57.29578f * this.airControlStability / this.airControlPower;
			if (num2 <= 45f)
			{
				Vector3 vector2 = Vector3.Cross(Quaternion.AngleAxis(num3, this.rigidBody.angularVelocity) * base.transform.up, vector) * this.airControlPower * this.airControlPower;
				this.rigidBody.AddTorque(vector2);
			}
		}
		this.hurtTriggerFront.gameObject.SetActive(speed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(speed < -this.hurtTriggerMinSpeed);
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00096A98 File Offset: 0x00094C98
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		this.timeSinceLastUsed = 0f;
		if (inputState.IsDown(BUTTON.DUCK))
		{
			this.SteerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			this.SteerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.SteerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.SteerInput = 1f;
			}
		}
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		this.ThrottleInput = 0f;
		this.BrakeInput = 0f;
		if (base.GetSpeed() > 3f && num < -0.1f)
		{
			this.ThrottleInput = 0f;
			this.BrakeInput = -num;
		}
		else
		{
			this.ThrottleInput = num;
			this.BrakeInput = 0f;
		}
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00096BD4 File Offset: 0x00094DD4
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float num = Mathf.Lerp(0.3f, 0.75f, this.GetPerformanceFraction());
		float num2 = MathEx.BiasedLerp(1f - absSpeed / topSpeed, num);
		return maxDriveForce * num2;
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00096C10 File Offset: 0x00094E10
	public override float GetModifiedDrag()
	{
		float num = base.GetModifiedDrag();
		if (!global::Snowmobile.allTerrain)
		{
			VehicleTerrainHandler.Surface onSurface = this.serverTerrainHandler.OnSurface;
			if (this.serverTerrainHandler.IsGrounded && onSurface != VehicleTerrainHandler.Surface.Frictionless && onSurface != VehicleTerrainHandler.Surface.Sand && onSurface != VehicleTerrainHandler.Surface.Snow && onSurface != VehicleTerrainHandler.Surface.Ice)
			{
				float num2 = Mathf.Max(num, this.badTerrainDrag);
				if (num2 <= this.prevTerrainModDrag)
				{
					num = this.prevTerrainModDrag;
				}
				else
				{
					num = Mathf.MoveTowards(this.prevTerrainModDrag, num2, 0.33f * this.timeSinceTerrainModCheck);
				}
				this.prevTerrainModDrag = num;
			}
			else
			{
				this.prevTerrainModDrag = 0f;
			}
		}
		this.timeSinceTerrainModCheck = 0f;
		this.InSlowMode = num >= this.badTerrainDrag;
		return num;
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00074E15 File Offset: 0x00073015
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x00096CC8 File Offset: 0x00094EC8
	public CarWheel[] GetWheels()
	{
		if (this.wheels == null)
		{
			this.wheels = new CarWheel[] { this.wheelSkiFL, this.wheelSkiFR, this.wheelTreadFL, this.wheelTreadFR, this.wheelTreadRL, this.wheelTreadRR };
		}
		return this.wheels;
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x00096D25 File Offset: 0x00094F25
	public float GetWheelsMidPos()
	{
		return (this.wheelSkiFL.wheelCollider.transform.localPosition.z - this.wheelTreadRL.wheelCollider.transform.localPosition.z) * 0.5f;
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00096D64 File Offset: 0x00094F64
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.snowmobile = Facepunch.Pool.Get<ProtoBuf.Snowmobile>();
		info.msg.snowmobile.steerInput = this.SteerInput;
		info.msg.snowmobile.driveWheelVel = this.DriveWheelVelocity;
		info.msg.snowmobile.throttleInput = this.ThrottleInput;
		info.msg.snowmobile.brakeInput = this.BrakeInput;
		info.msg.snowmobile.storageID = this.itemStorageInstance.uid;
		info.msg.snowmobile.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int StartingFuelUnits()
	{
		return 0;
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00096E1C File Offset: 0x0009501C
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned && child.prefabID == this.itemStoragePrefab.GetEntity().prefabID)
		{
			this.itemStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x00096E6C File Offset: 0x0009506C
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			StorageContainer storageContainer = this.itemStorageInstance.Get(base.isServer);
			if (storageContainer != null && storageContainer.IsValid())
			{
				storageContainer.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x00074E35 File Offset: 0x00073035
	public override bool MeetsEngineRequirements()
	{
		return base.HasDriver();
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x00096EB0 File Offset: 0x000950B0
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (global::Snowmobile.allowPassengerOnly)
		{
			base.AttemptMount(player, doMountChecks);
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable baseMountable;
		if (!base.HasDriver())
		{
			baseMountable = this.mountPoints[0].mountable;
		}
		else
		{
			baseMountable = base.GetIdealMountPointFor(player);
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

	// Token: 0x060012B8 RID: 4792 RVA: 0x00096F20 File Offset: 0x00095120
	public void SnowmobileDecay()
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.timeSinceLastUsed < 2700f)
		{
			return;
		}
		float num = (this.IsOutside() ? global::Snowmobile.outsideDecayMinutes : float.PositiveInfinity);
		if (!float.IsPositiveInfinity(num))
		{
			float num2 = 1f / num;
			base.Hurt(this.MaxHealth() * num2, DamageType.Decay, this, false);
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00096F80 File Offset: 0x00095180
	public StorageContainer GetItemContainer()
	{
		global::BaseEntity baseEntity = this.itemStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x00096FB8 File Offset: 0x000951B8
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.ThrottleInput + 1f) * 7f));
			byte b = (byte)(this.BrakeInput * 15f);
			byte b2 = (byte)(num + ((int)b << 4));
			base.ClientRPC<float, byte, float, float>(null, "SnowmobileUpdate", this.SteerInput, b2, this.DriveWheelVelocity, this.GetFuelFraction());
		}
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x0004E728 File Offset: 0x0004C928
	public override void OnEngineStartFailed()
	{
		base.ClientRPC(null, "EngineStartFailed");
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x00097013 File Offset: 0x00095213
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.riderProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x00097034 File Offset: 0x00095234
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x00097068 File Offset: 0x00095268
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenItemStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer itemContainer = this.GetItemContainer();
		if (itemContainer != null)
		{
			itemContainer.PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x060012BF RID: 4799 RVA: 0x000970A4 File Offset: 0x000952A4
	// (set) Token: 0x060012C0 RID: 4800 RVA: 0x000970BF File Offset: 0x000952BF
	public float ThrottleInput
	{
		get
		{
			if (!this.engineController.IsOn)
			{
				return 0f;
			}
			return this._throttle;
		}
		protected set
		{
			this._throttle = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x060012C1 RID: 4801 RVA: 0x000970D7 File Offset: 0x000952D7
	// (set) Token: 0x060012C2 RID: 4802 RVA: 0x000970DF File Offset: 0x000952DF
	public float BrakeInput
	{
		get
		{
			return this._brake;
		}
		protected set
		{
			this._brake = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x060012C3 RID: 4803 RVA: 0x000970F7 File Offset: 0x000952F7
	public bool IsBraking
	{
		get
		{
			return this.BrakeInput > 0f;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x060012C4 RID: 4804 RVA: 0x00097106 File Offset: 0x00095306
	// (set) Token: 0x060012C5 RID: 4805 RVA: 0x0009710E File Offset: 0x0009530E
	public float SteerInput
	{
		get
		{
			return this._steer;
		}
		protected set
		{
			this._steer = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00097126 File Offset: 0x00095326
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

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060012C7 RID: 4807 RVA: 0x00097141 File Offset: 0x00095341
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

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060012C8 RID: 4808 RVA: 0x0009715C File Offset: 0x0009535C
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

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060012C9 RID: 4809 RVA: 0x00097177 File Offset: 0x00095377
	public float MaxSteerAngle
	{
		get
		{
			return this.carSettings.maxSteerAngle;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060012CA RID: 4810 RVA: 0x00003278 File Offset: 0x00001478
	// (set) Token: 0x060012CB RID: 4811 RVA: 0x00097184 File Offset: 0x00095384
	public bool InSlowMode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
		private set
		{
			if (this.InSlowMode != value)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, value, false, true);
			}
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060012CC RID: 4812 RVA: 0x0009719D File Offset: 0x0009539D
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000971BC File Offset: 0x000953BC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.snowmobile == null)
		{
			return;
		}
		this.itemStorageInstance.uid = info.msg.snowmobile.storageID;
		this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.snowmobile.fuelStorageID;
		this.cachedFuelFraction = info.msg.snowmobile.fuelFraction;
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00097234 File Offset: 0x00095434
	public float GetMaxDriveForce()
	{
		return (float)this.engineKW * 10f * this.GetPerformanceFraction();
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x0009724A File Offset: 0x0009544A
	public override float GetMaxForwardSpeed()
	{
		return this.GetMaxDriveForce() / this.Mass * 15f;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x0009725F File Offset: 0x0009545F
	public override float GetThrottleInput()
	{
		return this.ThrottleInput;
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x00097267 File Offset: 0x00095467
	public override float GetBrakeInput()
	{
		return this.BrakeInput;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x0009726F File Offset: 0x0009546F
	public float GetSteerInput()
	{
		return this.SteerInput;
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool GetSteerModInput()
	{
		return false;
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00097278 File Offset: 0x00095478
	public float GetPerformanceFraction()
	{
		float num = Mathf.InverseLerp(0.25f, 0.5f, base.healthFraction);
		return Mathf.Lerp(0.5f, 1f, num);
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000972AB File Offset: 0x000954AB
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelFraction();
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x00075816 File Offset: 0x00073A16
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || !base.IsOn());
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000972CC File Offset: 0x000954CC
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && GameInfo.HasAchievements && !old.HasFlag(global::BaseEntity.Flags.On) && next.HasFlag(global::BaseEntity.Flags.On))
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver != null && driver.FindTrigger<TriggerSnowmobileAchievement>() != null)
			{
				driver.GiveAchievement("DRIVE_SNOWMOBILE");
			}
		}
	}

	// Token: 0x04000B98 RID: 2968
	private CarPhysics<global::Snowmobile> carPhysics;

	// Token: 0x04000B99 RID: 2969
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x04000B9A RID: 2970
	private CarWheel[] wheels;

	// Token: 0x04000B9B RID: 2971
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000B9C RID: 2972
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000B9D RID: 2973
	private float prevTerrainModDrag;

	// Token: 0x04000B9E RID: 2974
	private TimeSince timeSinceTerrainModCheck;

	// Token: 0x04000B9F RID: 2975
	[Header("Snowmobile")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000BA0 RID: 2976
	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	// Token: 0x04000BA1 RID: 2977
	[SerializeField]
	private VisualCarWheel wheelSkiFL;

	// Token: 0x04000BA2 RID: 2978
	[SerializeField]
	private VisualCarWheel wheelSkiFR;

	// Token: 0x04000BA3 RID: 2979
	[SerializeField]
	private VisualCarWheel wheelTreadFL;

	// Token: 0x04000BA4 RID: 2980
	[SerializeField]
	private VisualCarWheel wheelTreadFR;

	// Token: 0x04000BA5 RID: 2981
	[SerializeField]
	private VisualCarWheel wheelTreadRL;

	// Token: 0x04000BA6 RID: 2982
	[SerializeField]
	private VisualCarWheel wheelTreadRR;

	// Token: 0x04000BA7 RID: 2983
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000BA8 RID: 2984
	[SerializeField]
	private int engineKW = 59;

	// Token: 0x04000BA9 RID: 2985
	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	// Token: 0x04000BAA RID: 2986
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000BAB RID: 2987
	[SerializeField]
	private float airControlStability = 10f;

	// Token: 0x04000BAC RID: 2988
	[SerializeField]
	private float airControlPower = 40f;

	// Token: 0x04000BAD RID: 2989
	[SerializeField]
	private float badTerrainDrag = 1f;

	// Token: 0x04000BAE RID: 2990
	[SerializeField]
	private ProtectionProperties riderProtection;

	// Token: 0x04000BAF RID: 2991
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000BB0 RID: 2992
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000BB1 RID: 2993
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x04000BB2 RID: 2994
	[Header("Snowmobile Visuals")]
	public float minGroundFXSpeed;

	// Token: 0x04000BB3 RID: 2995
	[SerializeField]
	private SnowmobileChassisVisuals chassisVisuals;

	// Token: 0x04000BB4 RID: 2996
	[SerializeField]
	private VehicleLight[] lights;

	// Token: 0x04000BB5 RID: 2997
	[SerializeField]
	private Transform steeringLeftIK;

	// Token: 0x04000BB6 RID: 2998
	[SerializeField]
	private Transform steeringRightIK;

	// Token: 0x04000BB7 RID: 2999
	[SerializeField]
	private Transform leftFootIK;

	// Token: 0x04000BB8 RID: 3000
	[SerializeField]
	private Transform rightFootIK;

	// Token: 0x04000BB9 RID: 3001
	[SerializeField]
	private Transform starterKey;

	// Token: 0x04000BBA RID: 3002
	[SerializeField]
	private Vector3 engineOffKeyRot;

	// Token: 0x04000BBB RID: 3003
	[SerializeField]
	private Vector3 engineOnKeyRot;

	// Token: 0x04000BBC RID: 3004
	[ServerVar(Help = "How long before a snowmobile loses all its health while outside")]
	public static float outsideDecayMinutes = 1440f;

	// Token: 0x04000BBD RID: 3005
	[ServerVar(Help = "Allow mounting as a passenger when there's no driver")]
	public static bool allowPassengerOnly = false;

	// Token: 0x04000BBE RID: 3006
	[ServerVar(Help = "If true, snowmobile goes fast on all terrain types")]
	public static bool allTerrain = false;

	// Token: 0x04000BBF RID: 3007
	private float _throttle;

	// Token: 0x04000BC0 RID: 3008
	private float _brake;

	// Token: 0x04000BC1 RID: 3009
	private float _steer;

	// Token: 0x04000BC2 RID: 3010
	private float _mass = -1f;

	// Token: 0x04000BC3 RID: 3011
	public const global::BaseEntity.Flags Flag_Slowmode = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000BC4 RID: 3012
	private EntityRef<StorageContainer> itemStorageInstance;

	// Token: 0x04000BC5 RID: 3013
	private float cachedFuelFraction;

	// Token: 0x04000BC6 RID: 3014
	private const float FORCE_MULTIPLIER = 10f;
}
