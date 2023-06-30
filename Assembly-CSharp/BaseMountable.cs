using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x02000043 RID: 67
public class BaseMountable : BaseCombatEntity
{
	// Token: 0x06000428 RID: 1064 RVA: 0x0003444C File Offset: 0x0003264C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseMountable.OnRpcMessage", 0))
		{
			if (rpc == 1735799362U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsDismount ");
				}
				using (TimeWarning.New("RPC_WantsDismount", 0))
				{
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
							this.RPC_WantsDismount(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_WantsDismount");
					}
				}
				return true;
			}
			if (rpc == 4014300952U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsMount ");
				}
				using (TimeWarning.New("RPC_WantsMount", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4014300952U, "RPC_WantsMount", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsMount(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_WantsMount");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00034700 File Offset: 0x00032900
	public virtual bool CanHoldItems()
	{
		return this.canWieldItems;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00034708 File Offset: 0x00032908
	public virtual BasePlayer.CameraMode GetMountedCameraMode()
	{
		return this.MountedCameraMode;
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool DirectlyMountable()
	{
		return true;
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00034710 File Offset: 0x00032910
	public virtual Transform GetEyeOverride()
	{
		if (this.eyePositionOverride != null)
		{
			return this.eyePositionOverride;
		}
		return base.transform;
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x0003472D File Offset: 0x0003292D
	public virtual Quaternion GetMountedBodyAngles()
	{
		return this.GetEyeOverride().rotation;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool ModifiesThirdPersonCamera()
	{
		return false;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x0003473A File Offset: 0x0003293A
	public virtual Vector2 GetPitchClamp()
	{
		return this.pitchClamp;
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00034742 File Offset: 0x00032942
	public virtual Vector2 GetYawClamp()
	{
		return this.yawClamp;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0003474A File Offset: 0x0003294A
	public virtual bool AnyMounted()
	{
		return base.IsBusy();
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00034752 File Offset: 0x00032952
	public bool IsMounted()
	{
		return this.AnyMounted();
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x0003475A File Offset: 0x0003295A
	public virtual Vector3 EyePositionForPlayer(BasePlayer player, Quaternion lookRot)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.eyePositionOverride.transform.position;
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00034780 File Offset: 0x00032980
	public virtual Vector3 EyeCenterForPlayer(BasePlayer player, Quaternion lookRot)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.eyeCenterOverride.transform.position;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x000347A8 File Offset: 0x000329A8
	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		return WaterLevel.Factor(player.WorldSpaceBounds().ToBounds(), true, true, this);
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x000347CC File Offset: 0x000329CC
	public override float MaxVelocity()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			return parentEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x000347F5 File Offset: 0x000329F5
	public virtual bool PlayerIsMounted(BasePlayer player)
	{
		return player.IsValid() && player.GetMounted() == this;
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0003480D File Offset: 0x00032A0D
	public virtual BaseVehicle VehicleParent()
	{
		if (this.ignoreVehicleParent)
		{
			return null;
		}
		return base.GetParentEntity() as BaseVehicle;
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00034824 File Offset: 0x00032A24
	public virtual bool HasValidDismountPosition(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.HasValidDismountPosition(player);
		}
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0003487C File Offset: 0x00032A7C
	public virtual bool ValidDismountPosition(BasePlayer player, Vector3 disPos)
	{
		bool debugDismounts = Debugging.DebugDismounts;
		Vector3 dismountCheckStart = this.GetDismountCheckStart(player);
		if (debugDismounts)
		{
			Debug.Log(string.Format("ValidDismountPosition debug: Checking dismount point {0} from {1}.", disPos, dismountCheckStart));
		}
		Vector3 vector = disPos + new Vector3(0f, 0.5f, 0f);
		Vector3 vector2 = disPos + new Vector3(0f, 1.3f, 0f);
		if (!UnityEngine.Physics.CheckCapsule(vector, vector2, 0.5f, 1537286401))
		{
			Vector3 vector3 = disPos + base.transform.up * 0.5f;
			if (debugDismounts)
			{
				Debug.Log(string.Format("ValidDismountPosition debug: Dismount point {0} capsule check is OK.", disPos));
			}
			if (base.IsVisible(vector3, float.PositiveInfinity))
			{
				Vector3 vector4 = disPos + player.NoClipOffset();
				if (debugDismounts)
				{
					Debug.Log(string.Format("ValidDismountPosition debug: Dismount point {0} is visible.", disPos));
				}
				Collider collider;
				if (!global::AntiHack.TestNoClipping(dismountCheckStart, vector4, player.NoClipRadius(ConVar.AntiHack.noclip_margin_dismount), ConVar.AntiHack.noclip_backtracking, true, out collider, false, this.legacyDismount ? null : this))
				{
					if (debugDismounts)
					{
						Debug.Log(string.Format("<color=green>ValidDismountPosition debug: Dismount point {0} is valid</color>.", disPos));
						Debug.DrawLine(dismountCheckStart, vector4, Color.green, 10f);
					}
					return true;
				}
			}
		}
		if (debugDismounts)
		{
			Debug.DrawLine(dismountCheckStart, disPos, Color.red, 10f);
			if (debugDismounts)
			{
				Debug.Log(string.Format("<color=red>ValidDismountPosition debug: Dismount point {0} is invalid</color>.", disPos));
			}
		}
		return false;
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x000349EA File Offset: 0x00032BEA
	public BasePlayer GetMounted()
	{
		return this._mounted;
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void LightToggle(BasePlayer player)
	{
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanSwapToThis(BasePlayer player)
	{
		return true;
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600043F RID: 1087 RVA: 0x000349F2 File Offset: 0x00032BF2
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x000349F9 File Offset: 0x00032BF9
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.AnyMounted();
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00034A0F File Offset: 0x00032C0F
	public override void OnKilled(HitInfo info)
	{
		this.DismountAllPlayers();
		base.OnKilled(info);
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00034A1E File Offset: 0x00032C1E
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_WantsMount(BaseEntity.RPCMessage msg)
	{
		this.WantsMount(msg.player);
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00034A2C File Offset: 0x00032C2C
	public void WantsMount(BasePlayer player)
	{
		if (!player.IsValid() || !player.CanInteract())
		{
			return;
		}
		if (!this.DirectlyMountable())
		{
			BaseVehicle baseVehicle = this.VehicleParent();
			if (baseVehicle != null)
			{
				baseVehicle.WantsMount(player);
				return;
			}
		}
		this.AttemptMount(player, true);
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00034A74 File Offset: 0x00032C74
	public virtual void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (!player.CanMountMountablesNow())
		{
			return;
		}
		if (doMountChecks)
		{
			if (this.checkPlayerLosOnMount && UnityEngine.Physics.Linecast(player.eyes.position, this.mountAnchor.position + base.transform.up * this.mountLOSVertOffset, 1218652417))
			{
				Debug.Log("No line of sight to mount pos");
				return;
			}
			if (!this.HasValidDismountPosition(player))
			{
				Debug.Log("no valid dismount");
				return;
			}
		}
		this.MountPlayer(player);
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00034B10 File Offset: 0x00032D10
	public virtual bool AttemptDismount(BasePlayer player)
	{
		if (player != this._mounted)
		{
			return false;
		}
		this.DismountPlayer(player, false);
		return true;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00034B2C File Offset: 0x00032D2C
	[BaseEntity.RPC_Server]
	public void RPC_WantsDismount(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.HasValidDismountPosition(player))
		{
			return;
		}
		this.AttemptDismount(player);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00034B54 File Offset: 0x00032D54
	public void MountPlayer(BasePlayer player)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (this.mountAnchor == null)
		{
			return;
		}
		player.EnsureDismounted();
		this._mounted = player;
		Transform transform = this.mountAnchor.transform;
		player.MountObject(this, 0);
		player.MovePosition(transform.position);
		player.transform.rotation = transform.rotation;
		player.ServerRotation = transform.rotation;
		player.OverrideViewAngles(transform.rotation.eulerAngles);
		this._mounted.eyes.NetworkUpdate(transform.rotation);
		player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", player.transform.position);
		this.OnPlayerMounted();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00034C11 File Offset: 0x00032E11
	public virtual void OnPlayerMounted()
	{
		this.UpdateMountFlags();
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00034C11 File Offset: 0x00032E11
	public virtual void OnPlayerDismounted(BasePlayer player)
	{
		this.UpdateMountFlags();
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00034C1C File Offset: 0x00032E1C
	public virtual void UpdateMountFlags()
	{
		base.SetFlag(BaseEntity.Flags.Busy, this._mounted != null, false, true);
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00034C58 File Offset: 0x00032E58
	public virtual void DismountAllPlayers()
	{
		if (this._mounted)
		{
			this.DismountPlayer(this._mounted, false);
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00034C74 File Offset: 0x00032E74
	public void DismountPlayer(BasePlayer player, bool lite = false)
	{
		if (this._mounted == null)
		{
			return;
		}
		if (this._mounted != player)
		{
			return;
		}
		BaseVehicle baseVehicle = this.VehicleParent();
		if (lite)
		{
			if (baseVehicle != null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			this._mounted.DismountObject();
			this._mounted = null;
			if (baseVehicle != null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			this.OnPlayerDismounted(player);
			return;
		}
		Vector3 position;
		if (!this.GetDismountPosition(player, out position) || base.Distance(position) > 10f)
		{
			if (baseVehicle != null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			position = player.transform.position;
			this._mounted.DismountObject();
			this._mounted.MovePosition(position);
			this._mounted.ClientRPCPlayer<Vector3>(null, this._mounted, "ForcePositionTo", position);
			BasePlayer mounted = this._mounted;
			this._mounted = null;
			Debug.LogWarning(string.Concat(new object[]
			{
				"Killing player due to invalid dismount point :",
				player.displayName,
				" / ",
				player.userID,
				" on obj : ",
				base.gameObject.name
			}));
			mounted.Hurt(1000f, DamageType.Suicide, mounted, false);
			if (baseVehicle != null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			this.OnPlayerDismounted(player);
			return;
		}
		if (baseVehicle != null)
		{
			baseVehicle.PrePlayerDismount(player, this);
		}
		this._mounted.DismountObject();
		this._mounted.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		this._mounted.MovePosition(position);
		this._mounted.SendNetworkUpdateImmediate(false);
		this._mounted.SendModelState(true);
		this._mounted = null;
		if (baseVehicle != null)
		{
			baseVehicle.PlayerDismounted(player, this);
		}
		player.ForceUpdateTriggers(true, true, true);
		if (player.GetParentEntity())
		{
			BaseEntity parentEntity = player.GetParentEntity();
			player.ClientRPCPlayer<Vector3, NetworkableId>(null, player, "ForcePositionToParentOffset", parentEntity.transform.InverseTransformPoint(position), parentEntity.net.ID);
		}
		else
		{
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", position);
		}
		this.OnPlayerDismounted(player);
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00034EA0 File Offset: 0x000330A0
	public virtual bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null && baseVehicle.IsVehicleMountPoint(this))
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		int num = 0;
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				res = transform.transform.position;
				return true;
			}
			num++;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"Failed to find dismount position for player :",
			player.displayName,
			" / ",
			player.userID,
			" on obj : ",
			base.gameObject.name
		}));
		res = player.transform.position;
		return false;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00034F77 File Offset: 0x00033177
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.isMobile)
		{
			BaseMountable.FixedUpdateMountables.Add(this);
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00034F92 File Offset: 0x00033192
	internal override void DoServerDestroy()
	{
		BaseMountable.FixedUpdateMountables.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00034FA8 File Offset: 0x000331A8
	public static void FixedUpdateCycle()
	{
		for (int i = BaseMountable.FixedUpdateMountables.Count - 1; i >= 0; i--)
		{
			BaseMountable baseMountable = BaseMountable.FixedUpdateMountables[i];
			if (baseMountable == null)
			{
				BaseMountable.FixedUpdateMountables.RemoveAt(i);
			}
			else if (baseMountable.isSpawned)
			{
				baseMountable.VehicleFixedUpdate();
			}
		}
		for (int j = BaseMountable.FixedUpdateMountables.Count - 1; j >= 0; j--)
		{
			BaseMountable baseMountable2 = BaseMountable.FixedUpdateMountables[j];
			if (baseMountable2 == null)
			{
				BaseMountable.FixedUpdateMountables.RemoveAt(j);
			}
			else if (baseMountable2.isSpawned)
			{
				baseMountable2.PostVehicleFixedUpdate();
			}
		}
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00035044 File Offset: 0x00033244
	public virtual void VehicleFixedUpdate()
	{
		if (this._mounted)
		{
			this._mounted.transform.rotation = this.mountAnchor.transform.rotation;
			this._mounted.ServerRotation = this.mountAnchor.transform.rotation;
			this._mounted.MovePosition(this.mountAnchor.transform.position);
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostVehicleFixedUpdate()
	{
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
	{
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float GetComfort()
	{
		return 0f;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x000350B4 File Offset: 0x000332B4
	public bool TryFireProjectile(StorageContainer ammoStorage, AmmoTypes ammoType, Vector3 firingPos, Vector3 firingDir, BasePlayer driver, float launchOffset, float minSpeed, out ServerProjectile projectile)
	{
		projectile = null;
		if (ammoStorage == null)
		{
			return false;
		}
		bool flag = false;
		List<Item> list = Facepunch.Pool.GetList<Item>();
		ammoStorage.inventory.FindAmmo(list, ammoType);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].amount <= 0)
			{
				list.RemoveAt(i);
			}
		}
		if (list.Count > 0)
		{
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(firingPos, firingDir, out raycastHit, launchOffset, 1237003025))
			{
				launchOffset = raycastHit.distance - 0.1f;
			}
			Item item = list[list.Count - 1];
			ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();
			BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, firingPos + firingDir * launchOffset, default(Quaternion), true);
			projectile = baseEntity.GetComponent<ServerProjectile>();
			Vector3 vector = projectile.initialVelocity + firingDir * projectile.speed;
			if (minSpeed > 0f)
			{
				float num = Vector3.Dot(vector, firingDir) - minSpeed;
				if (num < 0f)
				{
					vector += firingDir * -num;
				}
			}
			projectile.InitializeVelocity(vector);
			if (driver.IsValid())
			{
				baseEntity.creatorEntity = driver;
				baseEntity.OwnerID = driver.userID;
			}
			baseEntity.Spawn();
			Analytics.Azure.OnExplosiveLaunched(driver, baseEntity, this);
			item.UseItem(1);
			flag = true;
		}
		Facepunch.Pool.FreeList<Item>(ref list);
		return flag;
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000457 RID: 1111 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsSummerDlcVehicle
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsInstrument()
	{
		return false;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00035230 File Offset: 0x00033430
	public Vector3 GetDismountCheckStart(BasePlayer player)
	{
		Vector3 vector = this.GetMountedPosition() + player.NoClipOffset();
		Vector3 vector2 = ((this.mountAnchor == null) ? base.transform.forward : this.mountAnchor.transform.forward);
		Vector3 vector3 = ((this.mountAnchor == null) ? base.transform.up : this.mountAnchor.transform.up);
		if (this.mountPose == PlayerModel.MountPoses.Chair)
		{
			vector += -vector2 * 0.32f;
			vector += vector3 * 0.25f;
		}
		else if (this.mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			vector += -vector2 * 0.26f;
			vector += vector3 * 0.25f;
		}
		else if (this.mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			vector += -vector2 * 0.26f;
		}
		return vector;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0003532F File Offset: 0x0003352F
	public Vector3 GetMountedPosition()
	{
		if (this.mountAnchor == null)
		{
			return base.transform.position;
		}
		return this.mountAnchor.transform.position;
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0003535C File Offset: 0x0003355C
	public bool NearMountPoint(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.mountAnchor == null)
		{
			return false;
		}
		if (Vector3.Distance(player.transform.position, this.mountAnchor.position) <= this.maxMountDistance)
		{
			RaycastHit raycastHit;
			if (!UnityEngine.Physics.SphereCast(player.eyes.HeadRay(), 0.25f, out raycastHit, 2f, 1218652417))
			{
				return false;
			}
			BaseEntity entity = raycastHit.GetEntity();
			if (entity != null)
			{
				if (entity == this || base.EqualNetID(entity))
				{
					return true;
				}
				BasePlayer basePlayer;
				if ((basePlayer = entity as BasePlayer) != null)
				{
					BaseMountable mounted = basePlayer.GetMounted();
					if (mounted == this)
					{
						return true;
					}
					if (mounted != null && mounted.VehicleParent() == this)
					{
						return true;
					}
				}
				BaseEntity parentEntity = entity.GetParentEntity();
				if (raycastHit.IsOnLayer(Rust.Layer.Vehicle_Detailed) && (parentEntity == this || base.EqualNetID(parentEntity)))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00035458 File Offset: 0x00033658
	public static Vector3 ConvertVector(Vector3 vec)
	{
		for (int i = 0; i < 3; i++)
		{
			if (vec[i] > 180f)
			{
				ref Vector3 ptr = ref vec;
				int num = i;
				ptr[num] -= 360f;
			}
			else if (vec[i] < -180f)
			{
				ref Vector3 ptr = ref vec;
				int num = i;
				ptr[num] += 360f;
			}
		}
		return vec;
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x0600045D RID: 1117 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool BlocksDoors
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0400032E RID: 814
	public static Translate.Phrase dismountPhrase = new Translate.Phrase("dismount", "Dismount");

	// Token: 0x0400032F RID: 815
	[Header("View")]
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyePositionOverride;

	// Token: 0x04000330 RID: 816
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyeCenterOverride;

	// Token: 0x04000331 RID: 817
	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	// Token: 0x04000332 RID: 818
	public Vector2 yawClamp = new Vector2(-80f, 80f);

	// Token: 0x04000333 RID: 819
	public bool canWieldItems = true;

	// Token: 0x04000334 RID: 820
	public bool relativeViewAngles = true;

	// Token: 0x04000335 RID: 821
	[Header("Mounting")]
	public Transform mountAnchor;

	// Token: 0x04000336 RID: 822
	public float mountLOSVertOffset = 0.5f;

	// Token: 0x04000337 RID: 823
	public PlayerModel.MountPoses mountPose;

	// Token: 0x04000338 RID: 824
	public float maxMountDistance = 1.5f;

	// Token: 0x04000339 RID: 825
	public Transform[] dismountPositions;

	// Token: 0x0400033A RID: 826
	public bool checkPlayerLosOnMount;

	// Token: 0x0400033B RID: 827
	public bool disableMeshCullingForPlayers;

	// Token: 0x0400033C RID: 828
	public bool allowHeadLook;

	// Token: 0x0400033D RID: 829
	public bool ignoreVehicleParent;

	// Token: 0x0400033E RID: 830
	public bool legacyDismount;

	// Token: 0x0400033F RID: 831
	[FormerlySerializedAs("modifyPlayerCollider")]
	public bool modifiesPlayerCollider;

	// Token: 0x04000340 RID: 832
	public BasePlayer.CapsuleColliderInfo customPlayerCollider;

	// Token: 0x04000341 RID: 833
	public SoundDefinition mountSoundDef;

	// Token: 0x04000342 RID: 834
	public SoundDefinition swapSoundDef;

	// Token: 0x04000343 RID: 835
	public SoundDefinition dismountSoundDef;

	// Token: 0x04000344 RID: 836
	public BaseMountable.MountStatType mountTimeStatType;

	// Token: 0x04000345 RID: 837
	public BaseMountable.MountGestureType allowedGestures;

	// Token: 0x04000346 RID: 838
	public bool canDrinkWhileMounted = true;

	// Token: 0x04000347 RID: 839
	public bool allowSleeperMounting;

	// Token: 0x04000348 RID: 840
	[Help("Set this to true if the mountable is enclosed so it doesn't move inside cars and such")]
	public bool animateClothInLocalSpace = true;

	// Token: 0x04000349 RID: 841
	[Header("Camera")]
	public BasePlayer.CameraMode MountedCameraMode;

	// Token: 0x0400034A RID: 842
	[FormerlySerializedAs("needsVehicleTick")]
	public bool isMobile;

	// Token: 0x0400034B RID: 843
	public float SideLeanAmount = 0.2f;

	// Token: 0x0400034C RID: 844
	public const float playerHeight = 1.8f;

	// Token: 0x0400034D RID: 845
	public const float playerRadius = 0.5f;

	// Token: 0x0400034E RID: 846
	protected BasePlayer _mounted;

	// Token: 0x0400034F RID: 847
	public static ListHashSet<BaseMountable> FixedUpdateMountables = new ListHashSet<BaseMountable>(8);

	// Token: 0x02000B9B RID: 2971
	public enum MountStatType
	{
		// Token: 0x0400403C RID: 16444
		None,
		// Token: 0x0400403D RID: 16445
		Boating,
		// Token: 0x0400403E RID: 16446
		Flying,
		// Token: 0x0400403F RID: 16447
		Driving
	}

	// Token: 0x02000B9C RID: 2972
	public enum MountGestureType
	{
		// Token: 0x04004041 RID: 16449
		None,
		// Token: 0x04004042 RID: 16450
		UpperBody
	}
}
