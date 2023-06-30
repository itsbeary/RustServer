using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A4 RID: 164
public class MotorRowboat : BaseBoat
{
	// Token: 0x06000F1D RID: 3869 RVA: 0x0007F2F8 File Offset: 0x0007D4F8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MotorRowboat.OnRpcMessage", 0))
		{
			if (rpc == 1873751172U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EngineToggle ");
				}
				using (TimeWarning.New("RPC_EngineToggle", 0))
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
							this.RPC_EngineToggle(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_EngineToggle");
					}
				}
				return true;
			}
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
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0007F558 File Offset: 0x0007D758
	public override void InitShared()
	{
		base.InitShared();
		this.fuelSystem = new EntityFuelSystem(base.isServer, this.fuelStoragePrefab, this.children, true);
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0007F580 File Offset: 0x0007D780
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsedFuel = 0f;
		base.InvokeRandomized(new Action(this.BoatDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0007F5D0 File Offset: 0x0007D7D0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.fuelSystem.CheckNewChild(child);
			}
			if (this.storageUnitPrefab.isValid && child.prefabID == this.storageUnitPrefab.GetEntity().prefabID)
			{
				this.storageUnitInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0007F636 File Offset: 0x0007D836
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && this.storageUnitInstance.IsValid(base.isServer))
		{
			this.storageUnitInstance.Get(base.isServer).DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0007F66F File Offset: 0x0007D86F
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.fuelSystem;
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0004DD5A File Offset: 0x0004BF5A
	public override int StartingFuelUnits()
	{
		return 50;
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0007F677 File Offset: 0x0007D877
	public void BoatDecay()
	{
		if (this.dying)
		{
			return;
		}
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsedFuel, MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes);
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0007F6A4 File Offset: 0x0007D8A4
	protected override void DoPushAction(global::BasePlayer player)
	{
		if (base.IsFlipped())
		{
			ref Vector3 ptr = base.transform.InverseTransformPoint(player.transform.position);
			float num = 4f;
			if (ptr.x > 0f)
			{
				num = -num;
			}
			this.rigidBody.AddRelativeTorque(Vector3.forward * num, ForceMode.VelocityChange);
			this.rigidBody.AddForce(Vector3.up * 4f, ForceMode.VelocityChange);
			this.startedFlip = 0f;
			base.InvokeRepeatingFixedTime(new Action(this.FlipMonitor));
		}
		else
		{
			Vector3 vector = Vector3Ex.Direction2D(player.transform.position, base.transform.position);
			Vector3 vector2 = Vector3Ex.Direction2D(player.transform.position + player.eyes.BodyForward() * 3f, player.transform.position);
			vector2 = (Vector3.up * 0.1f + vector2).normalized;
			Vector3 vector3 = base.transform.position + vector * 2f;
			float num2 = 3f;
			float num3 = Vector3.Dot(base.transform.forward, vector2);
			num2 += Mathf.InverseLerp(0.8f, 1f, num3) * 3f;
			this.rigidBody.AddForceAtPosition(vector2 * num2, vector3, ForceMode.VelocityChange);
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved9))
		{
			if (this.pushWaterEffect.isValid)
			{
				Effect.server.Run(this.pushWaterEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		else if (this.pushLandEffect.isValid)
		{
			Effect.server.Run(this.pushLandEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.WakeUp();
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0007F884 File Offset: 0x0007DA84
	private void FlipMonitor()
	{
		float num = Vector3.Dot(Vector3.up, base.transform.up);
		this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, Vector3.zero, UnityEngine.Time.fixedDeltaTime * 8f * num);
		if (this.startedFlip > 3f)
		{
			base.CancelInvokeFixedTime(new Action(this.FlipMonitor));
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0007F8F8 File Offset: 0x0007DAF8
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		this.fuelSystem.LootFuel(player);
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0007F92C File Offset: 0x0007DB2C
	[global::BaseEntity.RPC_Server]
	public void RPC_EngineToggle(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (base.InDryDock())
		{
			flag = false;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		if (flag == this.EngineOn())
		{
			return;
		}
		this.EngineToggle(flag);
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0007F97B File Offset: 0x0007DB7B
	public void EngineToggle(bool wantsOn)
	{
		if (!this.fuelSystem.HasFuel(true))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, wantsOn, false, true);
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0007F99C File Offset: 0x0007DB9C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.CheckInvalidBoat), 1f);
		if (base.health <= 0f)
		{
			base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
			this.buoyancy.buoyancyScale = 0f;
			this.dying = true;
		}
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0007FA04 File Offset: 0x0007DC04
	public void CheckInvalidBoat()
	{
		bool flag = this.fuelStoragePrefab.isValid && !this.fuelSystem.fuelStorageInstance.IsValid(base.isServer);
		bool flag2 = this.storageUnitPrefab.isValid && !this.storageUnitInstance.IsValid(base.isServer);
		if (flag || flag2)
		{
			Debug.Log("Destroying invalid boat ");
			base.Invoke(new Action(this.ActualDeath), 1f);
			return;
		}
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0007FA85 File Offset: 0x0007DC85
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x000233C8 File Offset: 0x000215C8
	public override bool EngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0007FA8F File Offset: 0x0007DC8F
	public float TimeSinceDriver()
	{
		return UnityEngine.Time.time - this.lastHadDriverTime;
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x0007FA9D File Offset: 0x0007DC9D
	public override void DriverInput(InputState inputState, global::BasePlayer player)
	{
		base.DriverInput(inputState, player);
		this.lastHadDriverTime = UnityEngine.Time.time;
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0007FAB4 File Offset: 0x0007DCB4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float num = this.TimeSinceDriver();
		if (num > 15f)
		{
			this.steering += Mathf.InverseLerp(15f, 30f, num);
			this.steering = Mathf.Clamp(-1f, 1f, this.steering);
			if (num > 75f)
			{
				this.gasPedal = 0f;
			}
		}
		this.SetFlags();
		this.UpdateDrag();
		if (this.dying)
		{
			this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, 0f, UnityEngine.Time.fixedDeltaTime * 0.1f);
		}
		else
		{
			float num2 = 1f;
			float num3 = this.rigidBody.velocity.Magnitude2D();
			float num4 = Mathf.InverseLerp(1f, 10f, num3) * 0.5f * base.healthFraction;
			if (!this.EngineOn())
			{
				num4 = 0f;
			}
			float num5 = 1f - 0.3f * (1f - base.healthFraction);
			this.buoyancy.buoyancyScale = (num2 + num4) * num5;
		}
		if (this.EngineOn())
		{
			float num6 = (base.HasFlag(global::BaseEntity.Flags.Reserved2) ? 1f : 0.0333f);
			this.fuelSystem.TryUseFuel(UnityEngine.Time.fixedDeltaTime * num6, this.fuelPerSec);
			this.timeSinceLastUsedFuel = 0f;
		}
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0007FC20 File Offset: 0x0007DE20
	private void SetFlags()
	{
		using (TimeWarning.New("SetFlag", 0))
		{
			bool flag = this.EngineOn() && !base.IsFlipped() && base.healthFraction > 0f && this.fuelSystem.HasFuel(false) && this.TimeSinceDriver() < 75f;
			global::BaseEntity.Flags flags = this.flags;
			base.SetFlag(global::BaseEntity.Flags.Reserved3, this.steering > 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved4, this.steering < 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, this.EngineOn() && this.gasPedal != 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved9, this.buoyancy.submergedFraction > 0.85f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, this.fuelSystem.HasFuel(false), false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved8, base.RecentlyPushed, false, false);
			if (flags != this.flags)
			{
				base.Invoke(new Action(base.SendNetworkUpdate_Flags), 0f);
			}
		}
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0007FD78 File Offset: 0x0007DF78
	protected override bool DetermineIfStationary()
	{
		return base.GetLocalVelocity().sqrMagnitude < 0.5f && !this.AnyMounted();
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0007FDA8 File Offset: 0x0007DFA8
	public override void SeatClippedWorld(BaseMountable mountable)
	{
		global::BasePlayer mounted = mountable.GetMounted();
		if (mounted == null)
		{
			return;
		}
		if (base.IsDriver(mounted))
		{
			this.steering = 0f;
			this.gasPedal = 0f;
		}
		float num = Mathf.InverseLerp(4f, 20f, this.rigidBody.velocity.magnitude);
		if (num > 0f)
		{
			mounted.Hurt(num * 100f, DamageType.Blunt, this, false);
		}
		if (mounted != null && mounted.isMounted)
		{
			base.SeatClippedWorld(mountable);
		}
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0007FE3C File Offset: 0x0007E03C
	public void UpdateDrag()
	{
		float num = this.rigidBody.velocity.SqrMagnitude2D();
		float num2 = Mathf.InverseLerp(0f, 2f, num);
		this.rigidBody.angularDrag = this.angularDragBase + this.angularDragVelocity * num2;
		this.rigidBody.drag = this.landDrag + this.waterDrag * Mathf.InverseLerp(0f, 1f, this.buoyancy.submergedFraction);
		if (this.offAxisDrag > 0f)
		{
			float num3 = Vector3.Dot(base.transform.forward, this.rigidBody.velocity.normalized);
			float num4 = Mathf.InverseLerp(0.98f, 0.92f, num3);
			this.rigidBody.drag += num4 * this.offAxisDrag * this.buoyancy.submergedFraction;
		}
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0007FF21 File Offset: 0x0007E121
	public override void OnKilled(HitInfo info)
	{
		if (this.dying)
		{
			return;
		}
		this.dying = true;
		this.repair.enabled = false;
		base.Invoke(new Action(this.DismountAllPlayers), 10f);
		this.EnterCorpseState();
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0007FF5D File Offset: 0x0007E15D
	protected virtual void EnterCorpseState()
	{
		base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00029C50 File Offset: 0x00027E50
	public void ActualDeath()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0007FF78 File Offset: 0x0007E178
	public override bool MountEligable(global::BasePlayer player)
	{
		return !this.dying && (this.rigidBody.velocity.magnitude < 5f || !base.HasDriver()) && base.MountEligable(player);
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0007FFBC File Offset: 0x0007E1BC
	public override bool HasValidDismountPosition(global::BasePlayer player)
	{
		if (base.GetWorldVelocity().magnitude <= 4f)
		{
			foreach (Transform transform in this.stationaryDismounts)
			{
				if (this.ValidDismountPosition(player, transform.transform.position))
				{
					return true;
				}
			}
		}
		return base.HasValidDismountPosition(player);
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x00080014 File Offset: 0x0007E214
	public override bool GetDismountPosition(global::BasePlayer player, out Vector3 res)
	{
		if (this.rigidBody.velocity.magnitude <= 4f)
		{
			List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
			foreach (Transform transform in this.stationaryDismounts)
			{
				if (this.ValidDismountPosition(player, transform.transform.position))
				{
					list.Add(transform.transform.position);
				}
			}
			if (list.Count > 0)
			{
				Vector3 pos = player.transform.position;
				list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
				res = list[0];
				Facepunch.Pool.FreeList<Vector3>(ref list);
				return true;
			}
			Facepunch.Pool.FreeList<Vector3>(ref list);
		}
		return base.GetDismountPosition(player, out res);
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x000800E0 File Offset: 0x0007E2E0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelSystem.fuelStorageInstance.uid;
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00080140 File Offset: 0x0007E340
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && base.IsStationary() && (pusher.WaterFactor() <= 0.6f || base.IsFlipped()) && (base.IsFlipped() || !pusher.IsStandingOnEntity(this, 8192)) && Vector3.Distance(pusher.transform.position, base.transform.position) <= 5f && !this.dying && (!pusher.isMounted && pusher.IsOnGround() && base.healthFraction > 0f) && this.ShowPushMenu(pusher);
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x000801E3 File Offset: 0x0007E3E3
	private bool ShowPushMenu(global::BasePlayer player)
	{
		return (base.IsFlipped() || !player.IsStandingOnEntity(this, 8192)) && base.IsStationary() && (player.WaterFactor() <= 0.6f || base.IsFlipped());
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0008021C File Offset: 0x0007E41C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x040009CA RID: 2506
	[Header("Audio")]
	public BlendedSoundLoops engineLoops;

	// Token: 0x040009CB RID: 2507
	public BlendedSoundLoops waterLoops;

	// Token: 0x040009CC RID: 2508
	public SoundDefinition engineStartSoundDef;

	// Token: 0x040009CD RID: 2509
	public SoundDefinition engineStopSoundDef;

	// Token: 0x040009CE RID: 2510
	public SoundDefinition movementSplashAccentSoundDef;

	// Token: 0x040009CF RID: 2511
	public SoundDefinition engineSteerSoundDef;

	// Token: 0x040009D0 RID: 2512
	public GameObjectRef pushLandEffect;

	// Token: 0x040009D1 RID: 2513
	public GameObjectRef pushWaterEffect;

	// Token: 0x040009D2 RID: 2514
	public float waterSpeedDivisor = 10f;

	// Token: 0x040009D3 RID: 2515
	public float turnPitchModScale = -0.25f;

	// Token: 0x040009D4 RID: 2516
	public float tiltPitchModScale = 0.3f;

	// Token: 0x040009D5 RID: 2517
	public float splashAccentFrequencyMin = 1f;

	// Token: 0x040009D6 RID: 2518
	public float splashAccentFrequencyMax = 10f;

	// Token: 0x040009D7 RID: 2519
	protected const global::BaseEntity.Flags Flag_EngineOn = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040009D8 RID: 2520
	protected const global::BaseEntity.Flags Flag_ThrottleOn = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040009D9 RID: 2521
	protected const global::BaseEntity.Flags Flag_TurnLeft = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040009DA RID: 2522
	protected const global::BaseEntity.Flags Flag_TurnRight = global::BaseEntity.Flags.Reserved4;

	// Token: 0x040009DB RID: 2523
	protected const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved6;

	// Token: 0x040009DC RID: 2524
	protected const global::BaseEntity.Flags Flag_RecentlyPushed = global::BaseEntity.Flags.Reserved8;

	// Token: 0x040009DD RID: 2525
	protected const global::BaseEntity.Flags Flag_Submerged = global::BaseEntity.Flags.Reserved9;

	// Token: 0x040009DE RID: 2526
	private const float submergeFractionMinimum = 0.85f;

	// Token: 0x040009DF RID: 2527
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x040009E0 RID: 2528
	public float fuelPerSec;

	// Token: 0x040009E1 RID: 2529
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x040009E2 RID: 2530
	public EntityRef<StorageContainer> storageUnitInstance;

	// Token: 0x040009E3 RID: 2531
	[Header("Effects")]
	public Transform boatRear;

	// Token: 0x040009E4 RID: 2532
	public ParticleSystemContainer wakeEffect;

	// Token: 0x040009E5 RID: 2533
	public ParticleSystemContainer engineEffectIdle;

	// Token: 0x040009E6 RID: 2534
	public ParticleSystemContainer engineEffectThrottle;

	// Token: 0x040009E7 RID: 2535
	[Tooltip("If not supplied, with use engineEffectThrottle for both")]
	public ParticleSystemContainer engineEffectThrottleReverse;

	// Token: 0x040009E8 RID: 2536
	[Tooltip("Only needed if using a forwardTravelEffect")]
	public Transform boatFront;

	// Token: 0x040009E9 RID: 2537
	public ParticleSystemContainer forwardTravelEffect;

	// Token: 0x040009EA RID: 2538
	public float forwardTravelEffectMinSpeed = 1f;

	// Token: 0x040009EB RID: 2539
	public Projector causticsProjector;

	// Token: 0x040009EC RID: 2540
	public Transform causticsDepthTest;

	// Token: 0x040009ED RID: 2541
	public Transform engineLeftHandPosition;

	// Token: 0x040009EE RID: 2542
	public Transform engineRotate;

	// Token: 0x040009EF RID: 2543
	public float engineRotateRangeMultiplier = 1f;

	// Token: 0x040009F0 RID: 2544
	public Transform propellerRotate;

	// Token: 0x040009F1 RID: 2545
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	// Token: 0x040009F2 RID: 2546
	[ServerVar(Help = "How long before a boat loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x040009F3 RID: 2547
	[ServerVar(Help = "How long before a boat loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	// Token: 0x040009F4 RID: 2548
	protected EntityFuelSystem fuelSystem;

	// Token: 0x040009F5 RID: 2549
	private TimeSince timeSinceLastUsedFuel;

	// Token: 0x040009F6 RID: 2550
	public Transform[] stationaryDismounts;

	// Token: 0x040009F7 RID: 2551
	public float angularDragBase = 0.5f;

	// Token: 0x040009F8 RID: 2552
	public float angularDragVelocity = 0.5f;

	// Token: 0x040009F9 RID: 2553
	public float landDrag = 0.2f;

	// Token: 0x040009FA RID: 2554
	public float waterDrag = 0.8f;

	// Token: 0x040009FB RID: 2555
	public float offAxisDrag = 1f;

	// Token: 0x040009FC RID: 2556
	public float offAxisDot = 0.25f;

	// Token: 0x040009FD RID: 2557
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x040009FE RID: 2558
	private TimeSince startedFlip;

	// Token: 0x040009FF RID: 2559
	private float lastHadDriverTime;

	// Token: 0x04000A00 RID: 2560
	private bool dying;

	// Token: 0x04000A01 RID: 2561
	private const float maxVelForStationaryDismount = 4f;
}
