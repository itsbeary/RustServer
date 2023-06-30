using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x0200004B RID: 75
public class BaseVehicle : BaseMountable
{
	// Token: 0x060007DC RID: 2012 RVA: 0x0004EF6C File Offset: 0x0004D16C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseVehicle.OnRpcMessage", 0))
		{
			if (rpc == 2115395408U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsPush ");
				}
				using (TimeWarning.New("RPC_WantsPush", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2115395408U, "RPC_WantsPush", this, player, 5f))
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
							this.RPC_WantsPush(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_WantsPush");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060007DD RID: 2013 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool AlwaysAllowBradleyTargeting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060007DE RID: 2014 RVA: 0x0004F0D4 File Offset: 0x0004D2D4
	protected bool RecentlyPushed
	{
		get
		{
			return this.timeSinceLastPush < 1f;
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004F0E8 File Offset: 0x0004D2E8
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004F118 File Offset: 0x0004D318
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.ClearOwnerEntry();
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004F12C File Offset: 0x0004D32C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.baseVehicle = Facepunch.Pool.Get<ProtoBuf.BaseVehicle>();
			info.msg.baseVehicle.mountPoints = Facepunch.Pool.GetList<ProtoBuf.BaseVehicle.MountPoint>();
			for (int i = 0; i < this.mountPoints.Count; i++)
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[i];
				if (!(mountPointInfo.mountable == null))
				{
					ProtoBuf.BaseVehicle.MountPoint mountPoint = Facepunch.Pool.Get<ProtoBuf.BaseVehicle.MountPoint>();
					mountPoint.index = i;
					mountPoint.mountableId = mountPointInfo.mountable.net.ID;
					info.msg.baseVehicle.mountPoints.Add(mountPoint);
				}
			}
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004F1E8 File Offset: 0x0004D3E8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.baseVehicle != null)
		{
			ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
			if (baseVehicle != null)
			{
				baseVehicle.Dispose();
			}
			this.pendingLoad = info.msg.baseVehicle;
			info.msg.baseVehicle = null;
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004F248 File Offset: 0x0004D448
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.clippingChecks != global::BaseVehicle.ClippingCheckMode.OnMountOnly && this.AnyMounted() && UnityEngine.Physics.OverlapBox(base.transform.TransformPoint(this.bounds.center), this.bounds.extents, base.transform.rotation, this.GetClipCheckMask()).Length != 0)
		{
			this.CheckSeatsForClipping();
		}
		if (this.rigidBody != null)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, this.DetermineIfStationary(), false, true);
			bool flag = this.rigidBody.IsSleeping();
			if (this.prevSleeping && !flag)
			{
				this.OnServerWake();
			}
			else if (!this.prevSleeping && flag)
			{
				this.OnServerSleep();
			}
			this.prevSleeping = flag;
		}
		if (this.OnlyOwnerAccessible() && this.safeAreaRadius != -1f && Vector3.Distance(base.transform.position, this.safeAreaOrigin) > this.safeAreaRadius)
		{
			this.ClearOwnerEntry();
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004F340 File Offset: 0x0004D540
	private int GetClipCheckMask()
	{
		int num = (this.IsFlipped() ? 1218511105 : 1210122497);
		if (this.checkVehicleClipping)
		{
			num |= 8192;
		}
		return num;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004F373 File Offset: 0x0004D573
	protected virtual bool DetermineIfStationary()
	{
		return this.rigidBody.IsSleeping() && !this.AnyMounted();
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004F38D File Offset: 0x0004D58D
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004F3B0 File Offset: 0x0004D5B0
	public override Quaternion GetAngularVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Quaternion.identity;
		}
		if (this.rigidBody.angularVelocity.sqrMagnitude < 0.025f)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(this.rigidBody.angularVelocity, base.transform.up);
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0004F40C File Offset: 0x0004D60C
	public virtual int StartingFuelUnits()
	{
		EntityFuelSystem fuelSystem = this.GetFuelSystem();
		if (fuelSystem != null)
		{
			return Mathf.FloorToInt((float)fuelSystem.GetFuelCapacity() * 0.2f);
		}
		return 0;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0004F437 File Offset: 0x0004D637
	public bool InSafeZone()
	{
		return global::BaseVehicle.InSafeZone(this.triggers, base.transform.position);
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0004F450 File Offset: 0x0004D650
	public static bool InSafeZone(List<TriggerBase> triggers, Vector3 position)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null && !activeGameMode.safeZone)
		{
			return false;
		}
		float num = 0f;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!(triggerSafeZone == null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0004F4C0 File Offset: 0x0004D6C0
	public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
	{
		if (!this.doClippingAndVisChecks)
		{
			return true;
		}
		if (mountable == null)
		{
			return false;
		}
		Vector3 vector = mountable.transform.position + base.transform.up * 0.15f;
		return GamePhysics.LineOfSight(eyePos, vector, mask, null);
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0004F514 File Offset: 0x0004D714
	public virtual bool IsSeatClipping(BaseMountable mountable)
	{
		if (!this.doClippingAndVisChecks)
		{
			return false;
		}
		if (mountable == null)
		{
			return false;
		}
		int clipCheckMask = this.GetClipCheckMask();
		Vector3 position = mountable.eyePositionOverride.transform.position;
		Vector3 position2 = mountable.transform.position;
		Vector3 vector = position - position2;
		float num = 0.4f;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 vector2 = position - vector * (num - 0.2f);
		bool flag = false;
		if (this.checkVehicleClipping)
		{
			List<Collider> list = Facepunch.Pool.GetList<Collider>();
			if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
			{
				GamePhysics.OverlapSphere(vector2, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			else
			{
				Vector3 vector3 = position2 + vector * (num + 0.05f);
				GamePhysics.OverlapCapsule(vector2, vector3, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			foreach (Collider collider in list)
			{
				global::BaseEntity baseEntity = collider.ToBaseEntity();
				if (baseEntity != this && !base.EqualNetID(baseEntity))
				{
					flag = true;
					break;
				}
			}
			Facepunch.Pool.FreeList<Collider>(ref list);
		}
		else if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
		{
			flag = GamePhysics.CheckSphere(vector2, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		else
		{
			Vector3 vector4 = position2 + vector * (num + 0.05f);
			flag = GamePhysics.CheckCapsule(vector2, vector4, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		return flag;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0004F684 File Offset: 0x0004D884
	public virtual void CheckSeatsForClipping()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			BaseMountable mountable = mountPointInfo.mountable;
			if (!(mountable == null) && mountable.AnyMounted() && this.IsSeatClipping(mountable))
			{
				this.SeatClippedWorld(mountable);
			}
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0004F6F8 File Offset: 0x0004D8F8
	public virtual void SeatClippedWorld(BaseMountable mountable)
	{
		mountable.DismountPlayer(mountable.GetMounted(), false);
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void MounteeTookDamage(global::BasePlayer mountee, HitInfo info)
	{
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0004F708 File Offset: 0x0004D908
	public override void DismountAllPlayers()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				mountPointInfo.mountable.DismountAllPlayers();
			}
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0004F770 File Offset: 0x0004D970
	public override void ServerInit()
	{
		base.ServerInit();
		this.clearRecentDriverAction = new Action(this.ClearRecentDriver);
		this.prevSleeping = false;
		if (this.rigidBody != null)
		{
			this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0004F7B0 File Offset: 0x0004D9B0
	public virtual void SpawnSubEntities()
	{
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0004F7B8 File Offset: 0x0004D9B8
	public virtual bool AdminFixUp(int tier)
	{
		if (this.IsDead())
		{
			return false;
		}
		EntityFuelSystem fuelSystem = this.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AdminAddFuel();
		}
		base.SetHealth(this.MaxHealth());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0004F7F3 File Offset: 0x0004D9F3
	private void OnPhysicsNeighbourChanged()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0004F810 File Offset: 0x0004DA10
	private void CheckAndSpawnMountPoints()
	{
		ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
		if (((baseVehicle != null) ? baseVehicle.mountPoints : null) != null)
		{
			foreach (ProtoBuf.BaseVehicle.MountPoint mountPoint in this.pendingLoad.mountPoints)
			{
				EntityRef<BaseMountable> entityRef = new EntityRef<BaseMountable>(mountPoint.mountableId);
				if (!entityRef.IsValid(true))
				{
					Debug.LogError(string.Format("Loaded a mountpoint which doesn't exist: {0}", mountPoint.index), this);
				}
				else if (mountPoint.index < 0 || mountPoint.index >= this.mountPoints.Count)
				{
					Debug.LogError(string.Format("Loaded a mountpoint which has no info: {0}", mountPoint.index), this);
					entityRef.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[mountPoint.index];
					if (mountPointInfo.mountable != null)
					{
						Debug.LogError(string.Format("Loading a mountpoint after one was already set: {0}", mountPoint.index), this);
						mountPointInfo.mountable.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					mountPointInfo.mountable = entityRef.Get(true);
					if (!mountPointInfo.mountable.enableSaving)
					{
						mountPointInfo.mountable.EnableSaving(true);
					}
				}
			}
		}
		ProtoBuf.BaseVehicle baseVehicle2 = this.pendingLoad;
		if (baseVehicle2 != null)
		{
			baseVehicle2.Dispose();
		}
		this.pendingLoad = null;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			this.SpawnMountPoint(this.mountPoints[i], this.model);
		}
		this.UpdateMountFlags();
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004F9C8 File Offset: 0x0004DBC8
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer && !Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0004F9E8 File Offset: 0x0004DBE8
	public override void Hurt(HitInfo info)
	{
		if (!this.IsDead() && this.rigidBody != null && !this.rigidBody.isKinematic)
		{
			float num = info.damageTypes.Get(DamageType.Explosion) + info.damageTypes.Get(DamageType.AntiVehicle);
			if (num > 3f)
			{
				float num2 = Mathf.Min(num * this.explosionForceMultiplier, this.explosionForceMax);
				this.rigidBody.AddExplosionForce(num2, info.HitPositionWorld, 1f, 2.5f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0004FA74 File Offset: 0x0004DC74
	public int NumMounted()
	{
		if (this.HasMountPoints())
		{
			int num = 0;
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null)
				{
					num++;
				}
			}
			return num;
		}
		if (!this.AnyMounted())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0004FB00 File Offset: 0x0004DD00
	public virtual int MaxMounted()
	{
		if (!this.HasMountPoints())
		{
			return 1;
		}
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable != null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0004FB6C File Offset: 0x0004DD6C
	public bool HasDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver && mountPointInfo.mountable.AnyMounted())
					{
						return true;
					}
				}
				return false;
			}
		}
		return base.AnyMounted();
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0004FBF8 File Offset: 0x0004DDF8
	public bool IsDriver(global::BasePlayer player)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null && mounted == player)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted == player;
		}
		return false;
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0004FCAC File Offset: 0x0004DEAC
	public global::BasePlayer GetDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							return mounted;
						}
					}
				}
				goto IL_82;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted;
		}
		IL_82:
		return null;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0004FD50 File Offset: 0x0004DF50
	public void GetDrivers(List<global::BasePlayer> drivers)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							drivers.Add(mounted);
						}
					}
				}
				return;
			}
		}
		if (this._mounted != null)
		{
			drivers.Add(this._mounted);
		}
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0004FDF8 File Offset: 0x0004DFF8
	public global::BasePlayer GetPlayerDamageInitiator()
	{
		if (this.HasDriver())
		{
			return this.GetDriver();
		}
		if (this.recentDrivers.Count <= 0)
		{
			return null;
		}
		return this.recentDrivers.Peek();
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0004FE24 File Offset: 0x0004E024
	public int GetPlayerSeat(global::BasePlayer player)
	{
		if (!this.HasMountPoints() && base.GetMounted() == player)
		{
			return 0;
		}
		int num = 0;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0004FEBC File Offset: 0x0004E0BC
	public global::BaseVehicle.MountPointInfo GetPlayerSeatInfo(global::BasePlayer player)
	{
		if (!this.HasMountPoints())
		{
			return null;
		}
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return mountPointInfo;
			}
		}
		return null;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0004FF40 File Offset: 0x0004E140
	public bool IsVehicleMountPoint(BaseMountable bm)
	{
		if (!this.HasMountPoints() || bm == null)
		{
			return false;
		}
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable == bm)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000804 RID: 2052 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanSwapSeats
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsPlayerSeatSwapValid(global::BasePlayer player, int fromIndex, int toIndex)
	{
		return true;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004FFB8 File Offset: 0x0004E1B8
	public void SwapSeats(global::BasePlayer player, int targetSeat = 0)
	{
		if (!this.HasMountPoints() || !this.CanSwapSeats)
		{
			return;
		}
		int playerSeat = this.GetPlayerSeat(player);
		if (playerSeat == -1)
		{
			return;
		}
		BaseMountable mountable = this.GetMountPoint(playerSeat).mountable;
		int num = playerSeat;
		BaseMountable baseMountable = null;
		if (baseMountable == null)
		{
			int num2 = this.NumSwappableSeats();
			for (int i = 0; i < num2; i++)
			{
				num++;
				if (num >= num2)
				{
					num = 0;
				}
				global::BaseVehicle.MountPointInfo mountPoint = this.GetMountPoint(num);
				if (((mountPoint != null) ? mountPoint.mountable : null) != null && !mountPoint.mountable.AnyMounted() && mountPoint.mountable.CanSwapToThis(player) && !this.IsSeatClipping(mountPoint.mountable) && this.IsSeatVisible(mountPoint.mountable, player.eyes.position, 1218511105) && this.IsPlayerSeatSwapValid(player, playerSeat, num))
				{
					baseMountable = mountPoint.mountable;
					break;
				}
			}
		}
		if (baseMountable != null && baseMountable != mountable)
		{
			mountable.DismountPlayer(player, true);
			baseMountable.MountPlayer(player);
			player.MarkSwapSeat();
		}
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x000500D0 File Offset: 0x0004E2D0
	public virtual int NumSwappableSeats()
	{
		return this.MaxMounted();
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x000500D8 File Offset: 0x0004E2D8
	public bool HasDriverMountPoints()
	{
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isDriver)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0002A712 File Offset: 0x00028912
	public bool OnlyOwnerAccessible()
	{
		return base.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x00050134 File Offset: 0x0004E334
	public bool IsDespawnEligable()
	{
		return this.spawnTime == -1f || this.spawnTime + 300f < UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00050158 File Offset: 0x0004E358
	public void SetupOwner(global::BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		if (owner != null)
		{
			this.creatorEntity = owner;
			base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
			this.safeAreaRadius = newSafeAreaRadius;
			this.safeAreaOrigin = newSafeAreaOrigin;
			this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0005018E File Offset: 0x0004E38E
	public void ClearOwnerEntry()
	{
		this.creatorEntity = null;
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		this.safeAreaRadius = -1f;
		this.safeAreaOrigin = Vector3.zero;
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public virtual EntityFuelSystem GetFuelSystem()
	{
		return null;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x000501B8 File Offset: 0x0004E3B8
	public bool IsSafe()
	{
		return this.OnlyOwnerAccessible() && Vector3.Distance(this.safeAreaOrigin, base.transform.position) <= this.safeAreaRadius;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x000501E5 File Offset: 0x0004E3E5
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		if (this.IsSafe())
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00050208 File Offset: 0x0004E408
	public BaseMountable GetIdealMountPoint(Vector3 eyePos, Vector3 pos, global::BasePlayer playerFor = null)
	{
		if (playerFor == null)
		{
			return null;
		}
		if (!this.HasMountPoints())
		{
			return this;
		}
		global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
		bool flag = basePlayer != null;
		bool flag2 = flag && basePlayer.Team != null;
		bool flag3 = flag && playerFor == basePlayer;
		if (!flag3 && flag && this.OnlyOwnerAccessible() && playerFor != null && (playerFor.Team == null || !playerFor.Team.members.Contains(basePlayer.userID)))
		{
			return null;
		}
		BaseMountable baseMountable = null;
		float num = float.PositiveInfinity;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (!mountPointInfo.mountable.AnyMounted())
			{
				float num2 = Vector3.Distance(mountPointInfo.mountable.mountAnchor.position, pos);
				if (num2 <= num)
				{
					if (this.IsSeatClipping(mountPointInfo.mountable))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's clipping", mountPointInfo.mountable));
						}
					}
					else if (!this.IsSeatVisible(mountPointInfo.mountable, eyePos, 1218511105))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's not visible", mountPointInfo.mountable));
						}
					}
					else if (!this.OnlyOwnerAccessible() || !flag3 || flag2 || mountPointInfo.isDriver)
					{
						baseMountable = mountPointInfo.mountable;
						num = num2;
					}
				}
			}
		}
		return baseMountable;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x000503AC File Offset: 0x0004E5AC
	public virtual bool MountEligable(global::BasePlayer player)
	{
		if (this.creatorEntity != null && this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
			if (basePlayer != null && basePlayer.Team != null && !basePlayer.Team.members.Contains(player.userID))
			{
				return false;
			}
		}
		global::BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle != null) || baseVehicle.MountEligable(player);
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x00050430 File Offset: 0x0004E630
	public int GetIndexFromSeat(BaseMountable seat)
	{
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable == seat)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00050498 File Offset: 0x0004E698
	public virtual void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		this.recentDrivers.Enqueue(player);
		if (!base.IsInvoking(this.clearRecentDriverAction))
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000504C8 File Offset: 0x0004E6C8
	public void TryShowCollisionFX(Collision collision, GameObjectRef effectGO)
	{
		this.TryShowCollisionFX(collision.GetContact(0).point, effectGO);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x000504EC File Offset: 0x0004E6EC
	public void TryShowCollisionFX(Vector3 contactPoint, GameObjectRef effectGO)
	{
		if (UnityEngine.Time.time < this.nextCollisionFXTime)
		{
			return;
		}
		this.nextCollisionFXTime = UnityEngine.Time.time + 0.25f;
		if (effectGO.isValid)
		{
			contactPoint += (base.transform.position - contactPoint) * 0.25f;
			Effect.server.Run(effectGO.resourcePath, contactPoint, base.transform.up, null, false);
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0005055C File Offset: 0x0004E75C
	public void SetToKinematic()
	{
		if (this.rigidBody == null || this.rigidBody.isKinematic)
		{
			return;
		}
		this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		this.rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x000505AE File Offset: 0x0004E7AE
	public void SetToNonKinematic()
	{
		if (this.rigidBody == null || !this.rigidBody.isKinematic)
		{
			return;
		}
		this.rigidBody.isKinematic = false;
		this.rigidBody.collisionDetectionMode = this.savedCollisionDetectionMode;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x000505EC File Offset: 0x0004E7EC
	public override void UpdateMountFlags()
	{
		int num = this.NumMounted();
		base.SetFlag(global::BaseEntity.Flags.InUse, num > 0, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved11, num == this.MaxMounted(), false, true);
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0005063D File Offset: 0x0004E83D
	private void ClearRecentDriver()
	{
		if (this.recentDrivers.Count > 0)
		{
			this.recentDrivers.Dequeue();
		}
		if (this.recentDrivers.Count > 0)
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00050678 File Offset: 0x0004E878
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable idealMountPointFor = this.GetIdealMountPointFor(player);
		if (idealMountPointFor == null)
		{
			return;
		}
		if (idealMountPointFor == this)
		{
			base.AttemptMount(player, doMountChecks);
		}
		else
		{
			idealMountPointFor.AttemptMount(player, doMountChecks);
		}
		if (player.GetMountedVehicle() == this)
		{
			this.PlayerMounted(player, idealMountPointFor);
		}
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x000506E1 File Offset: 0x0004E8E1
	protected BaseMountable GetIdealMountPointFor(global::BasePlayer player)
	{
		return this.GetIdealMountPoint(player.eyes.position, player.eyes.position + player.eyes.HeadForward() * 1f, player);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0005071C File Offset: 0x0004E91C
	public override bool GetDismountPosition(global::BasePlayer player, out Vector3 res)
	{
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				list.Add(transform.transform.position);
				if (this.dismountStyle == global::BaseVehicle.DismountStyle.Ordered)
				{
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Failed to find dismount position for player :",
				player.displayName,
				" / ",
				player.userID,
				" on obj : ",
				base.gameObject.name
			}));
			Facepunch.Pool.FreeList<Vector3>(ref list);
			res = player.transform.position;
			return false;
		}
		Vector3 pos = player.transform.position;
		list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
		res = list[0];
		Facepunch.Pool.FreeList<Vector3>(ref list);
		return true;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00050844 File Offset: 0x0004EA44
	private BaseMountable SpawnMountPoint(global::BaseVehicle.MountPointInfo mountToSpawn, Model model)
	{
		if (mountToSpawn.mountable != null)
		{
			return mountToSpawn.mountable;
		}
		Vector3 vector = Quaternion.Euler(mountToSpawn.rot) * Vector3.forward;
		Vector3 vector2 = mountToSpawn.pos;
		Vector3 vector3 = Vector3.up;
		if (mountToSpawn.bone != "")
		{
			vector2 = model.FindBone(mountToSpawn.bone).transform.position + base.transform.TransformDirection(mountToSpawn.pos);
			vector = base.transform.TransformDirection(vector);
			vector3 = base.transform.up;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(mountToSpawn.prefab.resourcePath, vector2, Quaternion.LookRotation(vector, vector3), true);
		BaseMountable baseMountable = baseEntity as BaseMountable;
		if (baseMountable != null)
		{
			if (this.enableSaving != baseMountable.enableSaving)
			{
				baseMountable.EnableSaving(this.enableSaving);
			}
			if (mountToSpawn.bone != "")
			{
				baseMountable.SetParent(this, mountToSpawn.bone, true, true);
			}
			else
			{
				baseMountable.SetParent(this, false, false);
			}
			baseMountable.Spawn();
			mountToSpawn.mountable = baseMountable;
		}
		else
		{
			Debug.LogError("MountPointInfo prefab is not a BaseMountable. Cannot spawn mount point.");
			if (baseEntity != null)
			{
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		return baseMountable;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0005098C File Offset: 0x0004EB8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	public void RPC_WantsPush(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.isMounted)
		{
			return;
		}
		if (this.RecentlyPushed)
		{
			return;
		}
		if (!this.CanPushNow(player))
		{
			return;
		}
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			return;
		}
		player.metabolism.calories.Subtract(3f);
		player.metabolism.SendChangesToClient();
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
		this.DoPushAction(player);
		this.timeSinceLastPush = 0f;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00050A34 File Offset: 0x0004EC34
	protected virtual void DoPushAction(global::BasePlayer player)
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.IsFlipped())
		{
			float num = this.rigidBody.mass * 8f;
			Vector3 vector = Vector3.forward * num;
			if (Vector3.Dot(base.transform.InverseTransformVector(base.transform.position - player.transform.position), Vector3.right) > 0f)
			{
				vector *= -1f;
			}
			if (base.transform.up.y < 0f)
			{
				vector *= -1f;
			}
			this.rigidBody.AddRelativeTorque(vector, ForceMode.Impulse);
			return;
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.position - player.eyes.position, base.transform.up).normalized;
		float num2 = this.rigidBody.mass * 4f;
		this.rigidBody.AddForce(normalized * num2, ForceMode.Impulse);
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnServerWake()
	{
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnServerSleep()
	{
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0004B603 File Offset: 0x00049803
	public bool IsStationary()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00050B47 File Offset: 0x0004ED47
	public bool IsMoving()
	{
		return !base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000826 RID: 2086 RVA: 0x00050B57 File Offset: 0x0004ED57
	public bool IsMovingOrOn
	{
		get
		{
			return this.IsMoving() || base.IsOn();
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000827 RID: 2087 RVA: 0x00050B69 File Offset: 0x0004ED69
	public override float RealisticMass
	{
		get
		{
			if (this.rigidBody != null)
			{
				return this.rigidBody.mass;
			}
			return base.RealisticMass;
		}
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00050B8C File Offset: 0x0004ED8C
	public bool IsAuthed(global::BasePlayer player)
	{
		foreach (global::BaseEntity baseEntity in this.children)
		{
			VehiclePrivilege vehiclePrivilege = baseEntity as VehiclePrivilege;
			if (!(vehiclePrivilege == null))
			{
				return vehiclePrivilege.IsAuthed(player);
			}
		}
		return true;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00050BF4 File Offset: 0x0004EDF4
	public override bool AnyMounted()
	{
		return base.HasFlag(global::BaseEntity.Flags.InUse);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x00050C01 File Offset: 0x0004EE01
	public override bool PlayerIsMounted(global::BasePlayer player)
	{
		return player.IsValid() && player.GetMountedVehicle() == this;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00050C19 File Offset: 0x0004EE19
	protected virtual bool CanPushNow(global::BasePlayer pusher)
	{
		return !base.IsOn();
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00050C24 File Offset: 0x0004EE24
	public bool HasMountPoints()
	{
		if (this.mountPoints.Count > 0)
		{
			return true;
		}
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x00050C8C File Offset: 0x0004EE8C
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return this.IsAlive() && !base.IsDestroyed && player != null;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x00050CA7 File Offset: 0x0004EEA7
	public bool IsFlipped()
	{
		return Vector3.Dot(Vector3.up, base.transform.up) <= 0f;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsVehicleRoot()
	{
		return true;
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00050CC8 File Offset: 0x0004EEC8
	public override bool DirectlyMountable()
	{
		return this.IsVehicleRoot();
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public override global::BaseVehicle VehicleParent()
	{
		return null;
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00050CD0 File Offset: 0x0004EED0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = child as global::BaseVehicle) != null && !baseVehicle.IsVehicleRoot() && !this.childVehicles.Contains(baseVehicle))
		{
			this.childVehicles.Add(baseVehicle);
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00050D24 File Offset: 0x0004EF24
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = child as global::BaseVehicle) != null && !baseVehicle.IsVehicleRoot())
		{
			this.childVehicles.Remove(baseVehicle);
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000834 RID: 2100 RVA: 0x00050D57 File Offset: 0x0004EF57
	public global::BaseVehicle.Enumerable allMountPoints
	{
		get
		{
			return new global::BaseVehicle.Enumerable(this);
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00050D60 File Offset: 0x0004EF60
	public global::BaseVehicle.MountPointInfo GetMountPoint(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index < this.mountPoints.Count)
		{
			return this.mountPoints[index];
		}
		index -= this.mountPoints.Count;
		int num = 0;
		foreach (global::BaseVehicle baseVehicle in this.childVehicles)
		{
			if (!(baseVehicle == null))
			{
				foreach (global::BaseVehicle.MountPointInfo mountPointInfo in baseVehicle.allMountPoints)
				{
					if (num == index)
					{
						return mountPointInfo;
					}
					num++;
				}
			}
		}
		return null;
	}

	// Token: 0x04000561 RID: 1377
	private const float MIN_TIME_BETWEEN_PUSHES = 1f;

	// Token: 0x04000562 RID: 1378
	public TimeSince timeSinceLastPush;

	// Token: 0x04000563 RID: 1379
	private bool prevSleeping;

	// Token: 0x04000564 RID: 1380
	private float nextCollisionFXTime;

	// Token: 0x04000565 RID: 1381
	private CollisionDetectionMode savedCollisionDetectionMode;

	// Token: 0x04000566 RID: 1382
	private ProtoBuf.BaseVehicle pendingLoad;

	// Token: 0x04000567 RID: 1383
	private Queue<global::BasePlayer> recentDrivers = new Queue<global::BasePlayer>();

	// Token: 0x04000568 RID: 1384
	private Action clearRecentDriverAction;

	// Token: 0x04000569 RID: 1385
	private float safeAreaRadius;

	// Token: 0x0400056A RID: 1386
	private Vector3 safeAreaOrigin;

	// Token: 0x0400056B RID: 1387
	private float spawnTime = -1f;

	// Token: 0x0400056C RID: 1388
	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	// Token: 0x0400056D RID: 1389
	public global::BaseVehicle.ClippingCheckMode clippingChecks;

	// Token: 0x0400056E RID: 1390
	public bool checkVehicleClipping;

	// Token: 0x0400056F RID: 1391
	public global::BaseVehicle.DismountStyle dismountStyle;

	// Token: 0x04000570 RID: 1392
	public bool shouldShowHudHealth;

	// Token: 0x04000571 RID: 1393
	public bool ignoreDamageFromOutside;

	// Token: 0x04000572 RID: 1394
	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	// Token: 0x04000573 RID: 1395
	[Header("Mount Points")]
	public List<global::BaseVehicle.MountPointInfo> mountPoints;

	// Token: 0x04000574 RID: 1396
	public bool doClippingAndVisChecks = true;

	// Token: 0x04000575 RID: 1397
	[Header("Damage")]
	public DamageRenderer damageRenderer;

	// Token: 0x04000576 RID: 1398
	[FormerlySerializedAs("explosionDamageMultiplier")]
	public float explosionForceMultiplier = 400f;

	// Token: 0x04000577 RID: 1399
	public float explosionForceMax = 75000f;

	// Token: 0x04000578 RID: 1400
	public const global::BaseEntity.Flags Flag_OnlyOwnerEntry = global::BaseEntity.Flags.Locked;

	// Token: 0x04000579 RID: 1401
	public const global::BaseEntity.Flags Flag_Headlights = global::BaseEntity.Flags.Reserved5;

	// Token: 0x0400057A RID: 1402
	public const global::BaseEntity.Flags Flag_Stationary = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400057B RID: 1403
	public const global::BaseEntity.Flags Flag_SeatsFull = global::BaseEntity.Flags.Reserved11;

	// Token: 0x0400057C RID: 1404
	protected const global::BaseEntity.Flags Flag_AnyMounted = global::BaseEntity.Flags.InUse;

	// Token: 0x0400057D RID: 1405
	private readonly List<global::BaseVehicle> childVehicles = new List<global::BaseVehicle>(0);

	// Token: 0x02000BC6 RID: 3014
	public enum ClippingCheckMode
	{
		// Token: 0x0400415F RID: 16735
		OnMountOnly,
		// Token: 0x04004160 RID: 16736
		Always,
		// Token: 0x04004161 RID: 16737
		AlwaysHeadOnly
	}

	// Token: 0x02000BC7 RID: 3015
	public enum DismountStyle
	{
		// Token: 0x04004163 RID: 16739
		Closest,
		// Token: 0x04004164 RID: 16740
		Ordered
	}

	// Token: 0x02000BC8 RID: 3016
	[Serializable]
	public class MountPointInfo
	{
		// Token: 0x04004165 RID: 16741
		public bool isDriver;

		// Token: 0x04004166 RID: 16742
		public Vector3 pos;

		// Token: 0x04004167 RID: 16743
		public Vector3 rot;

		// Token: 0x04004168 RID: 16744
		public string bone = "";

		// Token: 0x04004169 RID: 16745
		public GameObjectRef prefab;

		// Token: 0x0400416A RID: 16746
		[HideInInspector]
		public BaseMountable mountable;
	}

	// Token: 0x02000BC9 RID: 3017
	public readonly struct Enumerable : IEnumerable<global::BaseVehicle.MountPointInfo>, IEnumerable
	{
		// Token: 0x06004DA7 RID: 19879 RVA: 0x001A1452 File Offset: 0x0019F652
		public Enumerable(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x001A146F File Offset: 0x0019F66F
		public global::BaseVehicle.Enumerator GetEnumerator()
		{
			return new global::BaseVehicle.Enumerator(this._vehicle);
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x001A147C File Offset: 0x0019F67C
		IEnumerator<global::BaseVehicle.MountPointInfo> IEnumerable<global::BaseVehicle.MountPointInfo>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x001A147C File Offset: 0x0019F67C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0400416B RID: 16747
		private readonly global::BaseVehicle _vehicle;
	}

	// Token: 0x02000BCA RID: 3018
	public struct Enumerator : IEnumerator<global::BaseVehicle.MountPointInfo>, IEnumerator, IDisposable
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06004DAB RID: 19883 RVA: 0x001A1489 File Offset: 0x0019F689
		// (set) Token: 0x06004DAC RID: 19884 RVA: 0x001A1491 File Offset: 0x0019F691
		public global::BaseVehicle.MountPointInfo Current { get; private set; }

		// Token: 0x06004DAD RID: 19885 RVA: 0x001A149A File Offset: 0x0019F69A
		public Enumerator(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
			this._state = global::BaseVehicle.Enumerator.State.Direct;
			this._index = -1;
			this._childIndex = -1;
			this._enumerator = null;
			this.Current = null;
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x001A14DC File Offset: 0x0019F6DC
		public bool MoveNext()
		{
			this.Current = null;
			switch (this._state)
			{
			case global::BaseVehicle.Enumerator.State.Direct:
				this._index++;
				if (this._index < this._vehicle.mountPoints.Count)
				{
					this.Current = this._vehicle.mountPoints[this._index];
					return true;
				}
				this._state = global::BaseVehicle.Enumerator.State.EnterChild;
				break;
			case global::BaseVehicle.Enumerator.State.EnterChild:
				break;
			case global::BaseVehicle.Enumerator.State.EnumerateChild:
				goto IL_11B;
			case global::BaseVehicle.Enumerator.State.Finished:
				return false;
			default:
				throw new NotSupportedException();
			}
			do
			{
				IL_76:
				this._childIndex++;
			}
			while (this._childIndex < this._vehicle.childVehicles.Count && this._vehicle.childVehicles[this._childIndex] == null);
			if (this._childIndex >= this._vehicle.childVehicles.Count)
			{
				this._state = global::BaseVehicle.Enumerator.State.Finished;
				return false;
			}
			this._enumerator = Facepunch.Pool.Get<global::BaseVehicle.Enumerator.Box>();
			this._enumerator.Value = this._vehicle.childVehicles[this._childIndex].allMountPoints.GetEnumerator();
			this._state = global::BaseVehicle.Enumerator.State.EnumerateChild;
			IL_11B:
			if (this._enumerator.Value.MoveNext())
			{
				this.Current = this._enumerator.Value.Current;
				return true;
			}
			this._enumerator.Value.Dispose();
			Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			this._state = global::BaseVehicle.Enumerator.State.EnterChild;
			goto IL_76;
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06004DAF RID: 19887 RVA: 0x001A165C File Offset: 0x0019F85C
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x001A1664 File Offset: 0x0019F864
		public void Dispose()
		{
			if (this._enumerator != null)
			{
				this._enumerator.Value.Dispose();
				Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			}
		}

		// Token: 0x06004DB1 RID: 19889 RVA: 0x00162D3B File Offset: 0x00160F3B
		public void Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x0400416C RID: 16748
		private readonly global::BaseVehicle _vehicle;

		// Token: 0x0400416D RID: 16749
		private global::BaseVehicle.Enumerator.State _state;

		// Token: 0x0400416E RID: 16750
		private int _index;

		// Token: 0x0400416F RID: 16751
		private int _childIndex;

		// Token: 0x04004170 RID: 16752
		private global::BaseVehicle.Enumerator.Box _enumerator;

		// Token: 0x02000FD8 RID: 4056
		private enum State
		{
			// Token: 0x04005165 RID: 20837
			Direct,
			// Token: 0x04005166 RID: 20838
			EnterChild,
			// Token: 0x04005167 RID: 20839
			EnumerateChild,
			// Token: 0x04005168 RID: 20840
			Finished
		}

		// Token: 0x02000FD9 RID: 4057
		private class Box : Facepunch.Pool.IPooled
		{
			// Token: 0x060055D7 RID: 21975 RVA: 0x001BB1C8 File Offset: 0x001B93C8
			public void EnterPool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}

			// Token: 0x060055D8 RID: 21976 RVA: 0x001BB1C8 File Offset: 0x001B93C8
			public void LeavePool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}

			// Token: 0x04005169 RID: 20841
			public global::BaseVehicle.Enumerator Value;
		}
	}
}
