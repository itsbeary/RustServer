using System;
using System.Collections.Generic;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EA RID: 234
public class VehicleModuleSeating : BaseVehicleModule, IPrefabPreProcess
{
	// Token: 0x06001498 RID: 5272 RVA: 0x000A25A4 File Offset: 0x000A07A4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleSeating.OnRpcMessage", 0))
		{
			if (rpc == 2791546333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DestroyLock ");
				}
				using (TimeWarning.New("RPC_DestroyLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2791546333U, "RPC_DestroyLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DestroyLock(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_DestroyLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06001499 RID: 5273 RVA: 0x000A270C File Offset: 0x000A090C
	public override bool HasSeating
	{
		get
		{
			return this.mountPoints.Count > 0;
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x0600149A RID: 5274 RVA: 0x000A271C File Offset: 0x000A091C
	// (set) Token: 0x0600149B RID: 5275 RVA: 0x000A2724 File Offset: 0x000A0924
	public ModularCar Car { get; private set; }

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x0600149C RID: 5276 RVA: 0x000A272D File Offset: 0x000A092D
	protected bool IsOnACar
	{
		get
		{
			return this.Car != null;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x0600149D RID: 5277 RVA: 0x000A273B File Offset: 0x000A093B
	protected bool IsOnAVehicleLockUser
	{
		get
		{
			return this.VehicleLockUser != null;
		}
	}

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x0600149E RID: 5278 RVA: 0x000A2746 File Offset: 0x000A0946
	public bool DoorsAreLockable
	{
		get
		{
			return this.seating.doorsAreLockable;
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000A2754 File Offset: 0x000A0954
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (this.seating.steeringWheel != null)
		{
			this.steerAngle = this.seating.steeringWheel.localEulerAngles;
		}
		if (this.seating.accelPedal != null)
		{
			this.accelAngle = this.seating.accelPedal.localEulerAngles;
		}
		if (this.seating.brakePedal != null)
		{
			this.brakeAngle = this.seating.brakePedal.localEulerAngles;
		}
		if (this.seating.speedometer != null)
		{
			this.speedometerAngle = new Vector3(-160f, 0f, -40f);
		}
		if (this.seating.fuelGauge != null)
		{
			this.fuelAngle = this.seating.fuelGauge.localEulerAngles;
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000A2844 File Offset: 0x000A0A44
	public virtual bool IsOnThisModule(BasePlayer player)
	{
		BaseMountable mounted = player.GetMounted();
		return mounted != null && mounted.GetParentEntity() as VehicleModuleSeating == this;
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x000A2874 File Offset: 0x000A0A74
	public bool HasADriverSeat()
	{
		using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
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

	// Token: 0x060014A2 RID: 5282 RVA: 0x000A28D0 File Offset: 0x000A0AD0
	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		base.ModuleAdded(vehicle, firstSocketIndex);
		this.Car = vehicle as ModularCar;
		this.VehicleLockUser = vehicle as IVehicleLockUser;
		if (this.HasSeating && base.isServer)
		{
			using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModularCarSeat modularCarSeat;
					if ((modularCarSeat = enumerator.Current.mountable as ModularCarSeat) != null)
					{
						modularCarSeat.associatedSeatingModule = this;
					}
				}
			}
		}
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x000A2960 File Offset: 0x000A0B60
	public override void ModuleRemoved()
	{
		base.ModuleRemoved();
		this.Car = null;
		this.VehicleLockUser = null;
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000A2978 File Offset: 0x000A0B78
	public bool PlayerCanDestroyLock(BasePlayer player)
	{
		return this.IsOnAVehicleLockUser && !(player == null) && !base.Vehicle.IsDead() && this.HasADriverSeat() && this.VehicleLockUser.PlayerCanDestroyLock(player, this) && (!player.isMounted || !this.VehicleLockUser.PlayerHasUnlockPermission(player));
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x000A29DB File Offset: 0x000A0BDB
	protected BaseVehicleSeat GetSeatAtIndex(int index)
	{
		return this.mountPoints[index].mountable as BaseVehicleSeat;
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x000A29F3 File Offset: 0x000A0BF3
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		if (this.passengerProtection != null)
		{
			this.passengerProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x000A2A24 File Offset: 0x000A0C24
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.hornLoop != null && this.IsOnThisModule(player))
		{
			bool flag = inputState.IsDown(BUTTON.FIRE_PRIMARY);
			if (flag != base.HasFlag(BaseEntity.Flags.Reserved8))
			{
				base.SetFlag(BaseEntity.Flags.Reserved8, flag, false, true);
			}
			if (flag)
			{
				this.hornPlayer = player;
			}
		}
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x000A2A82 File Offset: 0x000A0C82
	public override void OnPlayerDismountedVehicle(BasePlayer player)
	{
		base.OnPlayerDismountedVehicle(player);
		if (base.HasFlag(BaseEntity.Flags.Reserved8) && player == this.hornPlayer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		}
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x000A2AB4 File Offset: 0x000A0CB4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DestroyLock(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (this.IsOnAVehicleLockUser)
		{
			if (!this.PlayerCanDestroyLock(player))
			{
				return;
			}
			this.VehicleLockUser.RemoveLock();
		}
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x000A2AE5 File Offset: 0x000A0CE5
	protected virtual Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		return desiredPos;
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x000A2AE8 File Offset: 0x000A0CE8
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		if (!GameInfo.HasAchievements || base.isClient || newState != VehicleEngineController<GroundVehicle>.EngineState.On || this.mountPoints == null)
		{
			return;
		}
		bool flag = true;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.Car.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleEngine vehicleModuleEngine;
				if ((vehicleModuleEngine = enumerator.Current as VehicleModuleEngine) != null && !vehicleModuleEngine.AtPeakPerformance)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable.GetMounted() != null)
				{
					mountPointInfo.mountable.GetMounted().GiveAchievement("BUCKLE_UP");
				}
			}
		}
	}

	// Token: 0x04000CF8 RID: 3320
	[SerializeField]
	private ProtectionProperties passengerProtection;

	// Token: 0x04000CF9 RID: 3321
	[SerializeField]
	private ModularCarCodeLockVisuals codeLockVisuals;

	// Token: 0x04000CFA RID: 3322
	[SerializeField]
	private VehicleModuleSeating.Seating seating;

	// Token: 0x04000CFB RID: 3323
	[SerializeField]
	[HideInInspector]
	private Vector3 steerAngle;

	// Token: 0x04000CFC RID: 3324
	[SerializeField]
	[HideInInspector]
	private Vector3 accelAngle;

	// Token: 0x04000CFD RID: 3325
	[SerializeField]
	[HideInInspector]
	private Vector3 brakeAngle;

	// Token: 0x04000CFE RID: 3326
	[SerializeField]
	[HideInInspector]
	private Vector3 speedometerAngle;

	// Token: 0x04000CFF RID: 3327
	[SerializeField]
	[HideInInspector]
	private Vector3 fuelAngle;

	// Token: 0x04000D00 RID: 3328
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04000D01 RID: 3329
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04000D02 RID: 3330
	private const BaseEntity.Flags FLAG_HORN = BaseEntity.Flags.Reserved8;

	// Token: 0x04000D03 RID: 3331
	private float steerPercent;

	// Token: 0x04000D04 RID: 3332
	private float throttlePercent;

	// Token: 0x04000D05 RID: 3333
	private float brakePercent;

	// Token: 0x04000D06 RID: 3334
	private bool? checkEngineLightOn;

	// Token: 0x04000D07 RID: 3335
	private bool? fuelLightOn;

	// Token: 0x04000D09 RID: 3337
	protected IVehicleLockUser VehicleLockUser;

	// Token: 0x04000D0A RID: 3338
	private MaterialPropertyBlock dashboardLightPB;

	// Token: 0x04000D0B RID: 3339
	private static int emissionColorID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000D0C RID: 3340
	private BasePlayer hornPlayer;

	// Token: 0x02000C20 RID: 3104
	[Serializable]
	public class MountHotSpot
	{
		// Token: 0x04004291 RID: 17041
		public Transform transform;

		// Token: 0x04004292 RID: 17042
		public Vector2 size;
	}

	// Token: 0x02000C21 RID: 3105
	[Serializable]
	public class Seating
	{
		// Token: 0x04004293 RID: 17043
		[Header("Seating & Controls")]
		public bool doorsAreLockable = true;

		// Token: 0x04004294 RID: 17044
		[Obsolete("Use BaseVehicle.mountPoints instead")]
		[HideInInspector]
		public BaseVehicle.MountPointInfo[] mountPoints;

		// Token: 0x04004295 RID: 17045
		public Transform steeringWheel;

		// Token: 0x04004296 RID: 17046
		public Transform accelPedal;

		// Token: 0x04004297 RID: 17047
		public Transform brakePedal;

		// Token: 0x04004298 RID: 17048
		public Transform steeringWheelLeftGrip;

		// Token: 0x04004299 RID: 17049
		public Transform steeringWheelRightGrip;

		// Token: 0x0400429A RID: 17050
		public Transform accelPedalGrip;

		// Token: 0x0400429B RID: 17051
		public Transform brakePedalGrip;

		// Token: 0x0400429C RID: 17052
		public VehicleModuleSeating.MountHotSpot[] mountHotSpots;

		// Token: 0x0400429D RID: 17053
		[Header("Dashboard")]
		public Transform speedometer;

		// Token: 0x0400429E RID: 17054
		public Transform fuelGauge;

		// Token: 0x0400429F RID: 17055
		public Renderer dashboardRenderer;

		// Token: 0x040042A0 RID: 17056
		[Range(0f, 3f)]
		public int checkEngineLightMatIndex = 2;

		// Token: 0x040042A1 RID: 17057
		[ColorUsage(true, true)]
		public Color checkEngineLightEmission;

		// Token: 0x040042A2 RID: 17058
		[Range(0f, 3f)]
		public int fuelLightMatIndex = 3;

		// Token: 0x040042A3 RID: 17059
		[ColorUsage(true, true)]
		public Color fuelLightEmission;
	}
}
