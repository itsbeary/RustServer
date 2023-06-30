using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000083 RID: 131
public class HotAirBalloon : BaseCombatEntity, SamSite.ISamSiteTarget
{
	// Token: 0x06000C4F RID: 3151 RVA: 0x0006A870 File Offset: 0x00068A70
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HotAirBalloon.OnRpcMessage", 0))
		{
			if (rpc == 578721460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - EngineSwitch ");
				}
				using (TimeWarning.New("EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(578721460U, "EngineSwitch", this, player, 3f))
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
							this.EngineSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in EngineSwitch");
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

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0006AB20 File Offset: 0x00068D20
	public bool IsFullyInflated
	{
		get
		{
			return this.inflationLevel >= 1f;
		}
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0006AB32 File Offset: 0x00068D32
	public override void InitShared()
	{
		this.fuelSystem = new EntityFuelSystem(base.isServer, this.fuelStoragePrefab, this.children, true);
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0006AB54 File Offset: 0x00068D54
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			this.inflationLevel = info.msg.hotAirBalloon.inflationAmount;
			if (info.fromDisk && this.myRigidbody)
			{
				this.myRigidbody.velocity = info.msg.hotAirBalloon.velocity;
			}
		}
		if (info.msg.motorBoat != null)
		{
			this.fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0006AC03 File Offset: 0x00068E03
	public bool WaterLogged()
	{
		return WaterLevel.Test(this.engineHeight.position, true, true, this);
	}

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000C54 RID: 3156 RVA: 0x0000719C File Offset: 0x0000539C
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeVehicle;
		}
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x0006AC18 File Offset: 0x00068E18
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.fuelSystem.CheckNewChild(child);
			}
			if (child.prefabID == this.storageUnitPrefab.GetEntity().prefabID)
			{
				this.storageUnitInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x0006AC71 File Offset: 0x00068E71
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && this.storageUnitInstance.IsValid(base.isServer))
		{
			this.storageUnitInstance.Get(base.isServer).DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x0006ACAA File Offset: 0x00068EAA
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		if (staticRespawn)
		{
			return this.IsFullyInflated;
		}
		return this.IsFullyInflated && !global::BaseVehicle.InSafeZone(this.triggers, base.transform.position);
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x0006ACD9 File Offset: 0x00068ED9
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x0006ACEC File Offset: 0x00068EEC
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		this.fuelSystem.LootFuel(player);
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x0006AD18 File Offset: 0x00068F18
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.hotAirBalloon = Facepunch.Pool.Get<ProtoBuf.HotAirBalloon>();
		info.msg.hotAirBalloon.inflationAmount = this.inflationLevel;
		if (info.forDisk && this.myRigidbody)
		{
			info.msg.hotAirBalloon.velocity = this.myRigidbody.velocity;
		}
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelSystem.fuelStorageInstance.uid;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0006ADD0 File Offset: 0x00068FD0
	public override void ServerInit()
	{
		this.myRigidbody.centerOfMass = this.centerOfMass.localPosition;
		this.myRigidbody.isKinematic = false;
		this.avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		base.ServerInit();
		this.bounds = this.collapsedBounds;
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
		base.InvokeRandomized(new Action(this.UpdateIsGrounded), 0f, 3f, 0.2f);
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0006AE78 File Offset: 0x00069078
	public void DecayTick()
	{
		if (base.healthFraction == 0f || this.IsFullyInflated)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastBlastTime + 600f)
		{
			return;
		}
		float num = 1f / global::HotAirBalloon.outsidedecayminutes;
		if (this.IsOutside())
		{
			base.Hurt(this.MaxHealth() * num, DamageType.Decay, this, false);
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0006AED8 File Offset: 0x000690D8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void EngineSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		base.SetFlag(global::BaseEntity.Flags.On, flag, false, true);
		if (base.IsOn())
		{
			base.Invoke(new Action(this.ScheduleOff), 60f);
			return;
		}
		base.CancelInvoke(new Action(this.ScheduleOff));
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x00062B09 File Offset: 0x00060D09
	public void ScheduleOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0006AF30 File Offset: 0x00069130
	public void UpdateIsGrounded()
	{
		if (this.lastBlastTime + 5f > UnityEngine.Time.time)
		{
			return;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(this.groundSample.transform.position, 1.25f, list, 1218511105, QueryTriggerInteraction.Ignore);
		this.grounded = list.Count > 0;
		this.CheckGlobal(this.flags);
		Facepunch.Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0006AF9A File Offset: 0x0006919A
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			this.CheckGlobal(next);
		}
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0006AFB4 File Offset: 0x000691B4
	private void CheckGlobal(global::BaseEntity.Flags flags)
	{
		bool flag = flags.HasFlag(global::BaseEntity.Flags.On) || flags.HasFlag(global::BaseEntity.Flags.Reserved2) || flags.HasFlag(global::BaseEntity.Flags.Reserved1) || !this.grounded;
		base.EnableGlobalBroadcast(flag);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0006B018 File Offset: 0x00069218
	protected void FixedUpdate()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		if (!this.fuelSystem.HasFuel(false) || this.WaterLogged())
		{
			base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		}
		if (base.IsOn())
		{
			this.fuelSystem.TryUseFuel(UnityEngine.Time.fixedDeltaTime, this.fuelPerSec);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, this.fuelSystem.HasFuel(false), false, true);
		bool flag = (this.IsFullyInflated && this.myRigidbody.velocity.y < 0f) || this.myRigidbody.velocity.y < 0.75f;
		foreach (GameObject gameObject in this.killTriggers)
		{
			if (gameObject.activeSelf != flag)
			{
				gameObject.SetActive(flag);
			}
		}
		float num = this.inflationLevel;
		if (base.IsOn() && !this.IsFullyInflated)
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel + UnityEngine.Time.fixedDeltaTime / 10f);
		}
		else if (this.grounded && this.inflationLevel > 0f && !base.IsOn() && (UnityEngine.Time.time > this.lastBlastTime + 30f || this.WaterLogged()))
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel - UnityEngine.Time.fixedDeltaTime / 10f);
		}
		if (num != this.inflationLevel)
		{
			if (this.IsFullyInflated)
			{
				this.bounds = this.raisedBounds;
			}
			else if (this.inflationLevel == 0f)
			{
				this.bounds = this.collapsedBounds;
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved1, this.inflationLevel > 0.3f, false, true);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, this.inflationLevel >= 1f, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			float num2 = this.inflationLevel;
		}
		bool flag2 = !this.myRigidbody.IsSleeping() || this.inflationLevel > 0f;
		foreach (GameObject gameObject2 in this.balloonColliders)
		{
			if (gameObject2.activeSelf != flag2)
			{
				gameObject2.SetActive(flag2);
			}
		}
		if (base.IsOn())
		{
			if (this.IsFullyInflated)
			{
				this.currentBuoyancy += UnityEngine.Time.fixedDeltaTime * 0.2f;
				this.lastBlastTime = UnityEngine.Time.time;
			}
		}
		else
		{
			this.currentBuoyancy -= UnityEngine.Time.fixedDeltaTime * 0.1f;
		}
		this.currentBuoyancy = Mathf.Clamp(this.currentBuoyancy, 0f, 0.8f + 0.2f * base.healthFraction);
		if (this.inflationLevel > 0f)
		{
			this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), UnityEngine.Time.deltaTime);
			float num3 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + global::HotAirBalloon.serviceCeiling - 20f, this.avgTerrainHeight + global::HotAirBalloon.serviceCeiling, this.buoyancyPoint.position.y);
			this.myRigidbody.AddForceAtPosition(Vector3.up * -UnityEngine.Physics.gravity.y * this.myRigidbody.mass * 0.5f * this.inflationLevel, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForceAtPosition(Vector3.up * this.liftAmount * this.currentBuoyancy * num3, this.buoyancyPoint.position, ForceMode.Force);
			Vector3 windAtPos = this.GetWindAtPos(this.buoyancyPoint.position);
			float magnitude = windAtPos.magnitude;
			float num4 = 1f;
			float num5 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(this.buoyancyPoint.position), TerrainMeta.WaterMap.GetHeight(this.buoyancyPoint.position));
			float num6 = Mathf.InverseLerp(num5 + 20f, num5 + 60f, this.buoyancyPoint.position.y);
			float num7 = 1f;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + Vector3.up * 2f, Vector3.down), 1.5f, out raycastHit, 5f, 1218511105))
			{
				num7 = Mathf.Clamp01(raycastHit.distance / 5f);
			}
			num4 *= num6 * num3 * num7;
			num4 *= 0.2f + 0.8f * base.healthFraction;
			Vector3 vector = windAtPos.normalized * num4 * this.windForce;
			this.currentWindVec = Vector3.Lerp(this.currentWindVec, vector, UnityEngine.Time.fixedDeltaTime * 0.25f);
			this.myRigidbody.AddForceAtPosition(vector * 0.1f, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForce(vector * 0.9f, ForceMode.Force);
		}
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x0006B528 File Offset: 0x00069728
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.myRigidbody == null)
		{
			return Vector3.zero;
		}
		return this.myRigidbody.velocity;
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0006B54C File Offset: 0x0006974C
	public override Quaternion GetAngularVelocityServer()
	{
		if (this.myRigidbody == null)
		{
			return Quaternion.identity;
		}
		if (this.myRigidbody.angularVelocity.sqrMagnitude < 0.1f)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(this.myRigidbody.angularVelocity, base.transform.up);
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0006B5A8 File Offset: 0x000697A8
	public Vector3 GetWindAtPos(Vector3 pos)
	{
		float num = pos.y * 6f;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f), 0f, Mathf.Cos(num * 0.017453292f));
		return vector.normalized * 1f;
	}

	// Token: 0x040007DD RID: 2013
	protected const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved6;

	// Token: 0x040007DE RID: 2014
	protected const global::BaseEntity.Flags Flag_HalfInflated = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040007DF RID: 2015
	protected const global::BaseEntity.Flags Flag_FullInflated = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040007E0 RID: 2016
	public Transform centerOfMass;

	// Token: 0x040007E1 RID: 2017
	public Rigidbody myRigidbody;

	// Token: 0x040007E2 RID: 2018
	public Transform buoyancyPoint;

	// Token: 0x040007E3 RID: 2019
	public float liftAmount = 10f;

	// Token: 0x040007E4 RID: 2020
	public Transform windSock;

	// Token: 0x040007E5 RID: 2021
	public Transform[] windFlags;

	// Token: 0x040007E6 RID: 2022
	public GameObject staticBalloonDeflated;

	// Token: 0x040007E7 RID: 2023
	public GameObject staticBalloon;

	// Token: 0x040007E8 RID: 2024
	public GameObject animatedBalloon;

	// Token: 0x040007E9 RID: 2025
	public Animator balloonAnimator;

	// Token: 0x040007EA RID: 2026
	public Transform groundSample;

	// Token: 0x040007EB RID: 2027
	public float inflationLevel;

	// Token: 0x040007EC RID: 2028
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x040007ED RID: 2029
	public float fuelPerSec = 0.25f;

	// Token: 0x040007EE RID: 2030
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x040007EF RID: 2031
	public EntityRef<StorageContainer> storageUnitInstance;

	// Token: 0x040007F0 RID: 2032
	[Header("Damage")]
	public DamageRenderer damageRenderer;

	// Token: 0x040007F1 RID: 2033
	public Transform engineHeight;

	// Token: 0x040007F2 RID: 2034
	public GameObject[] killTriggers;

	// Token: 0x040007F3 RID: 2035
	private EntityFuelSystem fuelSystem;

	// Token: 0x040007F4 RID: 2036
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	// Token: 0x040007F5 RID: 2037
	[ServerVar(Help = "How long before a HAB loses all its health while outside")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x040007F6 RID: 2038
	public float windForce = 30000f;

	// Token: 0x040007F7 RID: 2039
	public Vector3 currentWindVec = Vector3.zero;

	// Token: 0x040007F8 RID: 2040
	public Bounds collapsedBounds;

	// Token: 0x040007F9 RID: 2041
	public Bounds raisedBounds;

	// Token: 0x040007FA RID: 2042
	public GameObject[] balloonColliders;

	// Token: 0x040007FB RID: 2043
	[ServerVar]
	public static float serviceCeiling = 200f;

	// Token: 0x040007FC RID: 2044
	private float currentBuoyancy;

	// Token: 0x040007FD RID: 2045
	private float lastBlastTime;

	// Token: 0x040007FE RID: 2046
	private float avgTerrainHeight;

	// Token: 0x040007FF RID: 2047
	protected bool grounded;
}
