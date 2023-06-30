using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.AI;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000050 RID: 80
public class BradleyAPC : BaseCombatEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x06000884 RID: 2180 RVA: 0x00052074 File Offset: 0x00050274
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BradleyAPC.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x000520B4 File Offset: 0x000502B4
	public bool HasPath()
	{
		return this.currentPath != null && this.currentPath.Count > 0;
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x000520CE File Offset: 0x000502CE
	public void ClearPath()
	{
		this.currentPath.Clear();
		this.currentPathIndex = -1;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000520E2 File Offset: 0x000502E2
	public bool IndexValid(int index)
	{
		return this.HasPath() && index >= 0 && index < this.currentPath.Count;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00052102 File Offset: 0x00050302
	public Vector3 GetFinalDestination()
	{
		if (!this.HasPath())
		{
			return base.transform.position;
		}
		return this.finalDestination;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0005211E File Offset: 0x0005031E
	public Vector3 GetCurrentPathDestination()
	{
		if (!this.HasPath())
		{
			return base.transform.position;
		}
		return this.currentPath[this.currentPathIndex];
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00052145 File Offset: 0x00050345
	public bool PathComplete()
	{
		return !this.HasPath() || (this.currentPathIndex == this.currentPath.Count - 1 && this.AtCurrentPathNode());
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00052170 File Offset: 0x00050370
	public bool AtCurrentPathNode()
	{
		return this.currentPathIndex >= 0 && this.currentPathIndex < this.currentPath.Count && Vector3.Distance(base.transform.position, this.currentPath[this.currentPathIndex]) <= this.stoppingDist;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x000521C8 File Offset: 0x000503C8
	public int GetLoopedIndex(int index)
	{
		if (!this.HasPath())
		{
			Debug.LogWarning("Warning, GetLoopedIndex called without a path");
			return 0;
		}
		if (!this.pathLooping)
		{
			return Mathf.Clamp(index, 0, this.currentPath.Count - 1);
		}
		if (index >= this.currentPath.Count)
		{
			return index % this.currentPath.Count;
		}
		if (index < 0)
		{
			return this.currentPath.Count - Mathf.Abs(index % this.currentPath.Count);
		}
		return index;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00052248 File Offset: 0x00050448
	public Vector3 PathDirection(int index)
	{
		if (!this.HasPath() || this.currentPath.Count <= 1)
		{
			return base.transform.forward;
		}
		index = this.GetLoopedIndex(index);
		Vector3 vector;
		Vector3 vector2;
		if (this.pathLooping)
		{
			int loopedIndex = this.GetLoopedIndex(index - 1);
			vector = this.currentPath[loopedIndex];
			vector2 = this.currentPath[this.GetLoopedIndex(index)];
		}
		else
		{
			if (index - 1 < 0)
			{
				vector = base.transform.position;
			}
			else
			{
				vector = this.currentPath[index - 1];
			}
			vector2 = this.currentPath[index];
		}
		return (vector2 - vector).normalized;
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x000522F4 File Offset: 0x000504F4
	public Vector3 IdealPathPosition()
	{
		if (!this.HasPath())
		{
			return base.transform.position;
		}
		int loopedIndex = this.GetLoopedIndex(this.currentPathIndex - 1);
		if (loopedIndex == this.currentPathIndex)
		{
			return this.currentPath[this.currentPathIndex];
		}
		return this.ClosestPointAlongPath(this.currentPath[loopedIndex], this.currentPath[this.currentPathIndex], base.transform.position);
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00052370 File Offset: 0x00050570
	public void AdvancePathMovement(bool force)
	{
		if (!this.HasPath())
		{
			return;
		}
		if (force || this.AtCurrentPathNode() || this.currentPathIndex == -1)
		{
			this.currentPathIndex = this.GetLoopedIndex(this.currentPathIndex + 1);
		}
		if (this.PathComplete())
		{
			this.ClearPath();
			return;
		}
		Vector3 vector = this.IdealPathPosition();
		Vector3 vector2 = this.currentPath[this.currentPathIndex];
		float num = Vector3.Distance(vector, vector2);
		float num2 = Vector3.Distance(base.transform.position, vector);
		float num3 = Mathf.InverseLerp(8f, 0f, num2);
		vector += global::BradleyAPC.Direction2D(vector2, vector) * Mathf.Min(num, num3 * 20f);
		this.SetDestination(vector);
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0005242C File Offset: 0x0005062C
	public bool GetPathToClosestTurnableNode(IAIPathNode start, Vector3 forward, ref List<IAIPathNode> nodes)
	{
		float num = float.NegativeInfinity;
		IAIPathNode iaipathNode = null;
		foreach (IAIPathNode iaipathNode2 in start.Linked)
		{
			float num2 = Vector3.Dot(forward, (iaipathNode2.Position - start.Position).normalized);
			if (num2 > num)
			{
				num = num2;
				iaipathNode = iaipathNode2;
			}
		}
		if (iaipathNode != null)
		{
			nodes.Add(iaipathNode);
			return !iaipathNode.Straightaway || this.GetPathToClosestTurnableNode(iaipathNode, (iaipathNode.Position - start.Position).normalized, ref nodes);
		}
		return false;
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x000524E0 File Offset: 0x000506E0
	public bool GetEngagementPath(ref List<IAIPathNode> nodes)
	{
		IAIPathNode closestToPoint = this.patrolPath.GetClosestToPoint(base.transform.position);
		Vector3 normalized = (closestToPoint.Position - base.transform.position).normalized;
		if (Vector3.Dot(base.transform.forward, normalized) > 0f)
		{
			nodes.Add(closestToPoint);
			if (!closestToPoint.Straightaway)
			{
				return true;
			}
		}
		return this.GetPathToClosestTurnableNode(closestToPoint, base.transform.forward, ref nodes);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00052560 File Offset: 0x00050760
	public void AddOrUpdateTarget(global::BaseEntity ent, Vector3 pos, float damageFrom = 0f)
	{
		if (AI.ignoreplayers && !ent.IsNpc)
		{
			return;
		}
		if (!(ent is global::BasePlayer))
		{
			return;
		}
		global::BradleyAPC.TargetInfo targetInfo = null;
		foreach (global::BradleyAPC.TargetInfo targetInfo2 in this.targetList)
		{
			if (targetInfo2.entity == ent)
			{
				targetInfo = targetInfo2;
				break;
			}
		}
		if (targetInfo == null)
		{
			targetInfo = Facepunch.Pool.Get<global::BradleyAPC.TargetInfo>();
			targetInfo.Setup(ent, UnityEngine.Time.time - 1f);
			this.targetList.Add(targetInfo);
		}
		targetInfo.lastSeenPosition = pos;
		targetInfo.damageReceivedFrom += damageFrom;
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00052618 File Offset: 0x00050818
	public void UpdateTargetList()
	{
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position, this.searchRange, list, 133120, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if ((!AI.ignoreplayers || baseEntity.IsNpc) && baseEntity is global::BasePlayer)
			{
				global::BasePlayer basePlayer = baseEntity as global::BasePlayer;
				if (!basePlayer.IsDead() && !(basePlayer is HumanNPC) && !(basePlayer is NPCPlayer) && (!basePlayer.InSafeZone() || basePlayer.IsHostile()) && this.VisibilityTest(baseEntity))
				{
					bool flag = false;
					foreach (global::BradleyAPC.TargetInfo targetInfo in this.targetList)
					{
						if (targetInfo.entity == baseEntity)
						{
							targetInfo.lastSeenTime = UnityEngine.Time.time;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						global::BradleyAPC.TargetInfo targetInfo2 = Facepunch.Pool.Get<global::BradleyAPC.TargetInfo>();
						targetInfo2.Setup(baseEntity, UnityEngine.Time.time);
						this.targetList.Add(targetInfo2);
					}
				}
			}
		}
		for (int i = this.targetList.Count - 1; i >= 0; i--)
		{
			global::BradleyAPC.TargetInfo targetInfo3 = this.targetList[i];
			global::BasePlayer basePlayer2 = targetInfo3.entity as global::BasePlayer;
			if (targetInfo3.entity == null || UnityEngine.Time.time - targetInfo3.lastSeenTime > this.memoryDuration || basePlayer2.IsDead() || (basePlayer2.InSafeZone() && !basePlayer2.IsHostile()) || (AI.ignoreplayers && !basePlayer2.IsNpc))
			{
				this.targetList.Remove(targetInfo3);
				Facepunch.Pool.Free<global::BradleyAPC.TargetInfo>(ref targetInfo3);
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		this.targetList.Sort(new Comparison<global::BradleyAPC.TargetInfo>(this.SortTargets));
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00052830 File Offset: 0x00050A30
	public int SortTargets(global::BradleyAPC.TargetInfo t1, global::BradleyAPC.TargetInfo t2)
	{
		return t2.GetPriorityScore(this).CompareTo(t1.GetPriorityScore(this));
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00052854 File Offset: 0x00050A54
	public Vector3 GetAimPoint(global::BaseEntity ent)
	{
		global::BasePlayer basePlayer = ent as global::BasePlayer;
		if (basePlayer != null)
		{
			return basePlayer.eyes.position;
		}
		return ent.CenterPoint();
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00052884 File Offset: 0x00050A84
	public bool VisibilityTest(global::BaseEntity ent)
	{
		if (ent == null)
		{
			return false;
		}
		if (Vector3.Distance(ent.transform.position, base.transform.position) >= this.viewDistance)
		{
			return false;
		}
		bool flag;
		if (ent is global::BasePlayer)
		{
			global::BasePlayer basePlayer = ent as global::BasePlayer;
			Vector3 position = this.mainTurret.transform.position;
			flag = base.IsVisible(basePlayer.eyes.position, position, float.PositiveInfinity) || base.IsVisible(basePlayer.transform.position + Vector3.up * 0.1f, position, float.PositiveInfinity);
			if (!flag && basePlayer.isMounted && basePlayer.GetMounted().VehicleParent() != null && basePlayer.GetMounted().VehicleParent().AlwaysAllowBradleyTargeting)
			{
				flag = base.IsVisible(basePlayer.GetMounted().VehicleParent().bounds.center, position, float.PositiveInfinity);
			}
			if (flag)
			{
				flag = !UnityEngine.Physics.SphereCast(new Ray(position, Vector3Ex.Direction(basePlayer.eyes.position, position)), 0.05f, Vector3.Distance(basePlayer.eyes.position, position), 10551297);
			}
		}
		else
		{
			Debug.LogWarning("Standard vis test!");
			flag = base.IsVisible(ent.CenterPoint(), float.PositiveInfinity);
		}
		return flag;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x000529E0 File Offset: 0x00050BE0
	public void UpdateTargetVisibilities()
	{
		foreach (global::BradleyAPC.TargetInfo targetInfo in this.targetList)
		{
			if (targetInfo.IsValid() && this.VisibilityTest(targetInfo.entity))
			{
				targetInfo.lastSeenTime = UnityEngine.Time.time;
				targetInfo.lastSeenPosition = targetInfo.entity.transform.position;
			}
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00052A64 File Offset: 0x00050C64
	public void DoWeaponAiming()
	{
		this.desiredAimVector = ((this.mainGunTarget != null) ? (this.GetAimPoint(this.mainGunTarget) - this.mainTurretEyePos.transform.position).normalized : this.desiredAimVector);
		global::BaseEntity baseEntity = null;
		if (this.targetList.Count > 0)
		{
			if (this.targetList.Count > 1 && this.targetList[1].IsValid() && this.targetList[1].IsVisible())
			{
				baseEntity = this.targetList[1].entity;
			}
			else if (this.targetList[0].IsValid() && this.targetList[0].IsVisible())
			{
				baseEntity = this.targetList[0].entity;
			}
		}
		this.desiredTopTurretAimVector = ((baseEntity != null) ? (this.GetAimPoint(baseEntity) - this.topTurretEyePos.transform.position).normalized : base.transform.forward);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x00052B8C File Offset: 0x00050D8C
	public void DoWeapons()
	{
		if (this.mainGunTarget != null && Vector3.Dot(this.turretAimVector, (this.GetAimPoint(this.mainGunTarget) - this.mainTurretEyePos.transform.position).normalized) >= 0.99f)
		{
			bool flag = this.VisibilityTest(this.mainGunTarget);
			float num = Vector3.Distance(this.mainGunTarget.transform.position, base.transform.position);
			if (UnityEngine.Time.time > this.nextCoaxTime && flag && num <= 40f)
			{
				this.numCoaxBursted++;
				this.FireGun(this.GetAimPoint(this.mainGunTarget), 3f, true);
				this.nextCoaxTime = UnityEngine.Time.time + this.coaxFireRate;
				if (this.numCoaxBursted >= this.coaxBurstLength)
				{
					this.nextCoaxTime = UnityEngine.Time.time + 1f;
					this.numCoaxBursted = 0;
				}
			}
			if (num >= 10f && flag)
			{
				this.FireGunTest();
			}
		}
		if (this.targetList.Count > 1)
		{
			global::BaseEntity entity = this.targetList[1].entity;
			if (entity != null && UnityEngine.Time.time > this.nextTopTurretTime && this.VisibilityTest(entity))
			{
				this.FireGun(this.GetAimPoint(this.targetList[1].entity), 3f, false);
				this.nextTopTurretTime = UnityEngine.Time.time + this.topTurretFireRate;
			}
		}
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00052D18 File Offset: 0x00050F18
	public void FireGun(Vector3 targetPos, float aimCone, bool isCoax)
	{
		Transform transform = (isCoax ? this.coaxMuzzle : this.topTurretMuzzle);
		Vector3 vector = transform.transform.position - transform.forward * 0.25f;
		Vector3 normalized = (targetPos - vector).normalized;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
		targetPos = vector + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0f, list, 300f, 1220225809, QueryTriggerInteraction.UseGlobal, null);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit raycastHit = list[i];
			global::BaseEntity entity = raycastHit.GetEntity();
			if (!(entity != null) || (!(entity == this) && !entity.EqualNetID(this)))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					this.ApplyDamage(baseCombatEntity, raycastHit.point, modifiedAimConeDirection);
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					targetPos = raycastHit.point;
					break;
				}
			}
		}
		base.ClientRPC<bool, Vector3>(null, "CLIENT_FireGun", isCoax, targetPos);
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x00052E4C File Offset: 0x0005104C
	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		float num = this.bulletDamage * UnityEngine.Random.Range(0.9f, 1.1f);
		HitInfo hitInfo = new HitInfo(this, entity, DamageType.Bullet, num, point);
		entity.OnAttacked(hitInfo);
		if (entity is global::BasePlayer || entity is BaseNpc)
		{
			Effect.server.ImpactEffect(new HitInfo
			{
				HitPositionWorld = point,
				HitNormalWorld = -normal,
				HitMaterial = StringPool.Get("Flesh")
			});
		}
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x00052EC0 File Offset: 0x000510C0
	public void AimWeaponAt(Transform weaponYaw, Transform weaponPitch, Vector3 direction, float minPitch = -360f, float maxPitch = 360f, float maxYaw = 360f, Transform parentOverride = null)
	{
		Vector3 vector = weaponYaw.parent.InverseTransformDirection(direction);
		Quaternion quaternion = Quaternion.LookRotation(vector);
		Vector3 eulerAngles = quaternion.eulerAngles;
		for (int i = 0; i < 3; i++)
		{
			eulerAngles[i] -= ((eulerAngles[i] > 180f) ? 360f : 0f);
		}
		Quaternion quaternion2 = Quaternion.Euler(0f, Mathf.Clamp(eulerAngles.y, -maxYaw, maxYaw), 0f);
		Quaternion quaternion3 = Quaternion.Euler(Mathf.Clamp(eulerAngles.x, minPitch, maxPitch), 0f, 0f);
		if (weaponYaw == null && weaponPitch != null)
		{
			weaponPitch.transform.localRotation = quaternion3;
			return;
		}
		if (weaponPitch == null && weaponYaw != null)
		{
			weaponYaw.transform.localRotation = quaternion;
			return;
		}
		weaponYaw.transform.localRotation = quaternion2;
		weaponPitch.transform.localRotation = quaternion3;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00052FC4 File Offset: 0x000511C4
	public void LateUpdate()
	{
		float num = UnityEngine.Time.time - this.lastLateUpdate;
		this.lastLateUpdate = UnityEngine.Time.time;
		if (base.isServer)
		{
			float num2 = 2.0943952f;
			this.turretAimVector = Vector3.RotateTowards(this.turretAimVector, this.desiredAimVector, num2 * num, 0f);
		}
		else
		{
			this.turretAimVector = Vector3.Lerp(this.turretAimVector, this.desiredAimVector, UnityEngine.Time.deltaTime * 10f);
		}
		this.AimWeaponAt(this.mainTurret, this.coaxPitch, this.turretAimVector, -90f, 90f, 360f, null);
		this.AimWeaponAt(this.mainTurret, this.CannonPitch, this.turretAimVector, -90f, 7f, 360f, null);
		this.topTurretAimVector = Vector3.Lerp(this.topTurretAimVector, this.desiredTopTurretAimVector, UnityEngine.Time.deltaTime * 5f);
		this.AimWeaponAt(this.topTurretYaw, this.topTurretPitch, this.topTurretAimVector, -360f, 360f, 360f, this.mainTurret);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x000530DC File Offset: 0x000512DC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.bradley != null && !info.fromDisk)
		{
			this.throttle = info.msg.bradley.engineThrottle;
			this.rightThrottle = info.msg.bradley.throttleRight;
			this.leftThrottle = info.msg.bradley.throttleLeft;
			this.desiredAimVector = info.msg.bradley.mainGunVec;
			this.desiredTopTurretAimVector = info.msg.bradley.topTurretVec;
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x00053174 File Offset: 0x00051374
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.bradley = Facepunch.Pool.Get<ProtoBuf.BradleyAPC>();
			info.msg.bradley.engineThrottle = this.throttle;
			info.msg.bradley.throttleLeft = this.leftThrottle;
			info.msg.bradley.throttleRight = this.rightThrottle;
			info.msg.bradley.mainGunVec = this.turretAimVector;
			info.msg.bradley.topTurretVec = this.topTurretAimVector;
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00053210 File Offset: 0x00051410
	public static global::BradleyAPC SpawnRoadDrivingBradley(Vector3 spawnPos, Quaternion spawnRot)
	{
		RuntimePath runtimePath = new RuntimePath();
		PathList pathList = null;
		float num = float.PositiveInfinity;
		foreach (PathList pathList2 in TerrainMeta.Path.Roads)
		{
			Vector3 zero = Vector3.zero;
			float num2 = float.PositiveInfinity;
			foreach (Vector3 vector in pathList2.Path.Points)
			{
				float num3 = Vector3.Distance(vector, spawnPos);
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			if (num2 < num)
			{
				pathList = pathList2;
				num = num2;
			}
		}
		if (pathList == null)
		{
			return null;
		}
		Vector3 startPoint = pathList.Path.GetStartPoint();
		Vector3 endPoint = pathList.Path.GetEndPoint();
		bool flag = startPoint == endPoint;
		int num4 = (flag ? (pathList.Path.Points.Length - 1) : pathList.Path.Points.Length);
		RuntimePath runtimePath2 = runtimePath;
		IAIPathNode[] array = new RuntimePathNode[num4];
		runtimePath2.Nodes = array;
		IAIPathNode iaipathNode = null;
		int num5 = 0;
		int num6 = (flag ? (pathList.Path.MaxIndex - 1) : pathList.Path.MaxIndex);
		for (int j = pathList.Path.MinIndex; j <= num6; j++)
		{
			IAIPathNode iaipathNode2 = new RuntimePathNode(pathList.Path.Points[j] + Vector3.up * 1f);
			if (iaipathNode != null)
			{
				iaipathNode2.AddLink(iaipathNode);
				iaipathNode.AddLink(iaipathNode2);
			}
			runtimePath.Nodes[num5] = iaipathNode2;
			iaipathNode = iaipathNode2;
			num5++;
		}
		if (flag)
		{
			runtimePath.Nodes[0].AddLink(runtimePath.Nodes[runtimePath.Nodes.Length - 1]);
			runtimePath.Nodes[runtimePath.Nodes.Length - 1].AddLink(runtimePath.Nodes[0]);
		}
		else
		{
			RuntimeInterestNode runtimeInterestNode = new RuntimeInterestNode(startPoint + Vector3.up * 1f);
			runtimePath.AddInterestNode(runtimeInterestNode);
			RuntimeInterestNode runtimeInterestNode2 = new RuntimeInterestNode(endPoint + Vector3.up * 1f);
			runtimePath.AddInterestNode(runtimeInterestNode2);
		}
		int num7 = Mathf.CeilToInt(pathList.Path.Length / 500f);
		num7 = Mathf.Clamp(num7, 1, 3);
		if (flag)
		{
			num7++;
		}
		for (int k = 0; k < num7; k++)
		{
			int num8 = UnityEngine.Random.Range(0, pathList.Path.Points.Length);
			RuntimeInterestNode runtimeInterestNode3 = new RuntimeInterestNode(pathList.Path.Points[num8] + Vector3.up * 1f);
			runtimePath.AddInterestNode(runtimeInterestNode3);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/m2bradley/bradleyapc.prefab", spawnPos, spawnRot, true);
		global::BradleyAPC bradleyAPC = null;
		if (baseEntity)
		{
			bradleyAPC = baseEntity.GetComponent<global::BradleyAPC>();
			if (bradleyAPC)
			{
				bradleyAPC.Spawn();
				bradleyAPC.InstallPatrolPath(runtimePath);
			}
			else
			{
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		return bradleyAPC;
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00053520 File Offset: 0x00051720
	[ServerVar(Name = "spawnroadbradley")]
	public static string svspawnroadbradley(Vector3 pos, Vector3 dir)
	{
		if (!(global::BradleyAPC.SpawnRoadDrivingBradley(pos, Quaternion.LookRotation(dir, Vector3.up)) != null))
		{
			return "Failed to spawn road-driving Bradley.";
		}
		return "Spawned road-driving Bradley.";
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00053546 File Offset: 0x00051746
	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x00053550 File Offset: 0x00051750
	public override void ServerInit()
	{
		base.ServerInit();
		this.Initialize();
		base.InvokeRepeating(new Action(this.UpdateTargetList), 0f, 2f);
		base.InvokeRepeating(new Action(this.UpdateTargetVisibilities), 0f, global::BradleyAPC.sightUpdateRate);
		this.obstacleHitMask = LayerMask.GetMask(new string[] { "Vehicle World" });
		this.timeSinceSeemingStuck = 0f;
		this.timeSinceStuckReverseStart = float.MaxValue;
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void OnCollision(Collision collision, global::BaseEntity hitEntity)
	{
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x000535DF File Offset: 0x000517DF
	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
		this.finalDestination = base.transform.position;
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0005361C File Offset: 0x0005181C
	public global::BasePlayer FollowPlayer()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (basePlayer.IsAdmin && basePlayer.IsAlive() && !basePlayer.IsSleeping() && basePlayer.GetActiveItem() != null && basePlayer.GetActiveItem().info.shortname == "tool.binoculars")
			{
				return basePlayer;
			}
		}
		return null;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x000536AC File Offset: 0x000518AC
	public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
	{
		return (new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z)).normalized;
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060008A8 RID: 2216 RVA: 0x0002C198 File Offset: 0x0002A398
	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x000536F2 File Offset: 0x000518F2
	public bool IsAtDestination()
	{
		return Vector3Ex.Distance2D(base.transform.position, this.destination) <= this.stoppingDist;
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00053715 File Offset: 0x00051915
	public bool IsAtFinalDestination()
	{
		return Vector3Ex.Distance2D(base.transform.position, this.finalDestination) <= this.stoppingDist;
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x00053738 File Offset: 0x00051938
	public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
	{
		Vector3 vector = end - start;
		Vector3 vector2 = fromPos - start;
		float num = Vector3.Dot(vector, vector2);
		float num2 = Vector3.SqrMagnitude(end - start);
		float num3 = Mathf.Clamp01(num / num2);
		return start + vector * num3;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0005378C File Offset: 0x0005198C
	public void FireGunTest()
	{
		if (UnityEngine.Time.time < this.nextFireTime)
		{
			return;
		}
		this.nextFireTime = UnityEngine.Time.time + 0.25f;
		this.numBursted++;
		if (this.numBursted >= 4)
		{
			this.nextFireTime = UnityEngine.Time.time + 5f;
			this.numBursted = 0;
		}
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(2f, this.CannonMuzzle.rotation * Vector3.forward, true);
		Vector3 normalized = (this.CannonPitch.transform.rotation * Vector3.back + base.transform.up * -1f).normalized;
		this.myRigidBody.AddForceAtPosition(normalized * this.recoilScale, this.CannonPitch.transform.position, ForceMode.Impulse);
		Effect.server.Run(this.mainCannonMuzzleFlash.resourcePath, this, StringPool.Get(this.CannonMuzzle.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.mainCannonProjectile.resourcePath, this.CannonMuzzle.transform.position, Quaternion.LookRotation(modifiedAimConeDirection), true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(modifiedAimConeDirection * component.speed);
		}
		baseEntity.Spawn();
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x000538FE File Offset: 0x00051AFE
	public void InstallPatrolPath(IAIPath path)
	{
		this.patrolPath = path;
		this.currentPath = new List<Vector3>();
		this.currentPathIndex = -1;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0005391C File Offset: 0x00051B1C
	public void UpdateMovement_Patrol()
	{
		if (this.patrolPath == null)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextPatrolTime)
		{
			return;
		}
		this.nextPatrolTime = UnityEngine.Time.time + 20f;
		if (this.HasPath() && !this.IsAtFinalDestination())
		{
			return;
		}
		IAIPathInterestNode randomInterestNodeAwayFrom = this.patrolPath.GetRandomInterestNodeAwayFrom(base.transform.position, 10f);
		IAIPathNode closestToPoint = this.patrolPath.GetClosestToPoint(randomInterestNodeAwayFrom.Position);
		bool flag = false;
		List<IAIPathNode> list = Facepunch.Pool.GetList<IAIPathNode>();
		IAIPathNode iaipathNode;
		if (this.GetEngagementPath(ref list))
		{
			flag = true;
			iaipathNode = list[list.Count - 1];
		}
		else
		{
			iaipathNode = this.patrolPath.GetClosestToPoint(base.transform.position);
		}
		if (Vector3.Distance(this.finalDestination, closestToPoint.Position) > 2f)
		{
			if (closestToPoint == iaipathNode)
			{
				this.currentPath.Clear();
				this.currentPath.Add(closestToPoint.Position);
				this.currentPathIndex = -1;
				this.pathLooping = false;
				this.finalDestination = closestToPoint.Position;
				return;
			}
			Stack<IAIPathNode> stack;
			float num;
			if (AStarPath.FindPath(iaipathNode, closestToPoint, out stack, out num))
			{
				this.currentPath.Clear();
				if (flag)
				{
					for (int i = 0; i < list.Count - 1; i++)
					{
						this.currentPath.Add(list[i].Position);
					}
				}
				foreach (IAIPathNode iaipathNode2 in stack)
				{
					this.currentPath.Add(iaipathNode2.Position);
				}
				this.currentPathIndex = -1;
				this.pathLooping = false;
				this.finalDestination = closestToPoint.Position;
			}
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00053AE0 File Offset: 0x00051CE0
	public void UpdateMovement_Hunt()
	{
		if (this.patrolPath == null)
		{
			return;
		}
		global::BradleyAPC.TargetInfo targetInfo = this.targetList[0];
		if (targetInfo.IsValid())
		{
			if (this.HasPath() && targetInfo.IsVisible())
			{
				if (this.currentPath.Count > 1)
				{
					Vector3 vector = this.currentPath[this.currentPathIndex];
					this.ClearPath();
					this.currentPath.Add(vector);
					this.finalDestination = vector;
					this.currentPathIndex = 0;
					return;
				}
			}
			else if (UnityEngine.Time.time > this.nextEngagementPathTime && !this.HasPath() && !targetInfo.IsVisible())
			{
				bool flag = false;
				IAIPathNode iaipathNode = this.patrolPath.GetClosestToPoint(base.transform.position);
				List<IAIPathNode> list = Facepunch.Pool.GetList<IAIPathNode>();
				if (this.GetEngagementPath(ref list))
				{
					flag = true;
					iaipathNode = list[list.Count - 1];
				}
				IAIPathNode iaipathNode2 = null;
				List<IAIPathNode> list2 = Facepunch.Pool.GetList<IAIPathNode>();
				this.patrolPath.GetNodesNear(targetInfo.lastSeenPosition, ref list2, 30f);
				Stack<IAIPathNode> stack = null;
				float num = float.PositiveInfinity;
				float y = this.mainTurretEyePos.localPosition.y;
				foreach (IAIPathNode iaipathNode3 in list2)
				{
					Stack<IAIPathNode> stack2 = new Stack<IAIPathNode>();
					float num2;
					if (targetInfo.entity.IsVisible(iaipathNode3.Position + new Vector3(0f, y, 0f), float.PositiveInfinity) && AStarPath.FindPath(iaipathNode, iaipathNode3, out stack2, out num2) && num2 < num)
					{
						stack = stack2;
						num = num2;
						iaipathNode2 = iaipathNode3;
					}
				}
				if (stack == null && list2.Count > 0)
				{
					Stack<IAIPathNode> stack3 = new Stack<IAIPathNode>();
					IAIPathNode iaipathNode4 = list2[UnityEngine.Random.Range(0, list2.Count)];
					float num3;
					if (AStarPath.FindPath(iaipathNode, iaipathNode4, out stack3, out num3) && num3 < num)
					{
						stack = stack3;
						iaipathNode2 = iaipathNode4;
					}
				}
				if (stack != null)
				{
					this.currentPath.Clear();
					if (flag)
					{
						for (int i = 0; i < list.Count - 1; i++)
						{
							this.currentPath.Add(list[i].Position);
						}
					}
					foreach (IAIPathNode iaipathNode5 in stack)
					{
						this.currentPath.Add(iaipathNode5.Position);
					}
					this.currentPathIndex = -1;
					this.pathLooping = false;
					this.finalDestination = iaipathNode2.Position;
				}
				Facepunch.Pool.FreeList<IAIPathNode>(ref list2);
				Facepunch.Pool.FreeList<IAIPathNode>(ref list);
				this.nextEngagementPathTime = UnityEngine.Time.time + 5f;
			}
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00053DA8 File Offset: 0x00051FA8
	public void DoSimpleAI()
	{
		if (base.isClient)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		if (!this.DoAI)
		{
			return;
		}
		if (this.targetList.Count > 0)
		{
			if (this.targetList[0].IsValid() && this.targetList[0].IsVisible())
			{
				this.mainGunTarget = this.targetList[0].entity as BaseCombatEntity;
			}
			else
			{
				this.mainGunTarget = null;
			}
			this.UpdateMovement_Hunt();
		}
		else
		{
			this.mainGunTarget = null;
			this.UpdateMovement_Patrol();
		}
		this.AdvancePathMovement(false);
		float num = Vector3.Distance(base.transform.position, this.destination);
		float num2 = Vector3.Distance(base.transform.position, this.finalDestination);
		if (num > this.stoppingDist)
		{
			Vector3 vector = global::BradleyAPC.Direction2D(this.destination, base.transform.position);
			float num3 = Vector3.Dot(vector, base.transform.right);
			float num4 = Vector3.Dot(vector, base.transform.right);
			float num5 = Vector3.Dot(vector, -base.transform.right);
			if (Vector3.Dot(vector, -base.transform.forward) > num3)
			{
				if (num4 >= num5)
				{
					this.turning = 1f;
				}
				else
				{
					this.turning = -1f;
				}
			}
			else
			{
				this.turning = Mathf.Clamp(num3 * 3f, -1f, 1f);
			}
			float num6 = 1f - Mathf.InverseLerp(0f, 0.3f, Mathf.Abs(this.turning));
			this.AvoidObstacles(ref num6);
			float num7 = Vector3.Dot(this.myRigidBody.velocity, base.transform.forward);
			if (this.throttle <= 0f || num7 >= 0.5f)
			{
				this.timeSinceSeemingStuck = 0f;
			}
			else if (this.timeSinceSeemingStuck > 10f)
			{
				this.timeSinceStuckReverseStart = 0f;
				this.timeSinceSeemingStuck = 0f;
			}
			float num8 = Mathf.InverseLerp(0.1f, 0.4f, Vector3.Dot(base.transform.forward, Vector3.up));
			if (this.timeSinceStuckReverseStart < 3f)
			{
				this.throttle = -0.75f;
				this.turning = 1f;
			}
			else
			{
				this.throttle = (0.1f + Mathf.InverseLerp(0f, 20f, num2) * 1f) * num6 + num8;
			}
		}
		this.DoWeaponAiming();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00054059 File Offset: 0x00052259
	public void FixedUpdate()
	{
		this.DoSimpleAI();
		this.DoPhysicsMove();
		this.DoWeapons();
		this.DoHealing();
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00054074 File Offset: 0x00052274
	private void AvoidObstacles(ref float throttleScaleFromTurn)
	{
		Ray ray = new Ray(base.transform.position + base.transform.forward * (this.bounds.extents.z - 1f), base.transform.forward);
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 3f, out raycastHit, 20f, this.obstacleHitMask, QueryTriggerInteraction.Ignore, this))
		{
			return;
		}
		if (raycastHit.point == Vector3.zero)
		{
			raycastHit.point = raycastHit.collider.ClosestPointOnBounds(ray.origin);
		}
		float num = base.transform.AngleToPos(raycastHit.point);
		float num2 = Mathf.Abs(num);
		if (num2 > 75f)
		{
			return;
		}
		if (!(raycastHit.collider.ToBaseEntity() is global::BradleyAPC))
		{
			return;
		}
		bool flag = false;
		if (num2 < 5f)
		{
			float num3 = ((this.throttle < 0f) ? 150f : 50f);
			if (Vector3.SqrMagnitude(base.transform.position - raycastHit.point) < num3)
			{
				flag = true;
			}
		}
		if (num > 30f)
		{
			this.turning = -1f;
		}
		else
		{
			this.turning = 1f;
		}
		throttleScaleFromTurn = (flag ? (-1f) : 1f);
		int num4 = this.currentPathIndex;
		int num5 = this.currentPathIndex;
		float num6 = Vector3.Distance(base.transform.position, this.destination);
		while (this.HasPath() && (double)num6 < 26.6 && this.currentPathIndex >= 0)
		{
			int num7 = this.currentPathIndex;
			this.AdvancePathMovement(true);
			num6 = Vector3.Distance(base.transform.position, this.destination);
			if (this.currentPathIndex == num4 || this.currentPathIndex == num7)
			{
				break;
			}
		}
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x00054254 File Offset: 0x00052454
	public void DoPhysicsMove()
	{
		if (base.isClient)
		{
			return;
		}
		Vector3 velocity = this.myRigidBody.velocity;
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		this.leftThrottle = this.throttle;
		this.rightThrottle = this.throttle;
		if (this.turning > 0f)
		{
			this.rightThrottle = -this.turning;
			this.leftThrottle = this.turning;
		}
		else if (this.turning < 0f)
		{
			this.leftThrottle = this.turning;
			this.rightThrottle = this.turning * -1f;
		}
		Vector3.Distance(base.transform.position, this.GetFinalDestination());
		float num = Vector3.Distance(base.transform.position, this.GetCurrentPathDestination());
		float num2 = 15f;
		if (num < 20f)
		{
			float num3 = Vector3.Dot(this.PathDirection(this.currentPathIndex), this.PathDirection(this.currentPathIndex + 1));
			float num4 = Mathf.InverseLerp(2f, 10f, num);
			float num5 = Mathf.InverseLerp(0.5f, 0.8f, num3);
			num2 = 15f - 14f * ((1f - num5) * (1f - num4));
		}
		if (this.patrolPath != null)
		{
			float num6 = num2;
			foreach (IAIPathSpeedZone iaipathSpeedZone in this.patrolPath.SpeedZones)
			{
				if (iaipathSpeedZone.WorldSpaceBounds().Contains(base.transform.position))
				{
					num6 = Mathf.Min(num6, iaipathSpeedZone.GetMaxSpeed());
				}
			}
			this.currentSpeedZoneLimit = Mathf.Lerp(this.currentSpeedZoneLimit, num6, UnityEngine.Time.deltaTime);
			num2 = Mathf.Min(num2, this.currentSpeedZoneLimit);
		}
		if (this.PathComplete())
		{
			num2 = 0f;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log(string.Concat(new object[] { "velocity:", velocity.magnitude, "max : ", num2 }));
		}
		this.brake = velocity.magnitude >= num2;
		this.ApplyBrakes(this.brake ? 1f : 0f);
		float num7 = this.throttle;
		this.leftThrottle = Mathf.Clamp(this.leftThrottle + num7, -1f, 1f);
		this.rightThrottle = Mathf.Clamp(this.rightThrottle + num7, -1f, 1f);
		float num8 = Mathf.InverseLerp(2f, 1f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		float num9 = Mathf.Lerp(this.moveForceMax, this.turnForce, num8);
		float num10 = Mathf.InverseLerp(5f, 1.5f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		this.ScaleSidewaysFriction(1f - num10);
		this.SetMotorTorque(this.leftThrottle, false, num9);
		this.SetMotorTorque(this.rightThrottle, true, num9);
		this.impactDamager.damageEnabled = this.myRigidBody.velocity.magnitude > 2f;
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x000545D4 File Offset: 0x000527D4
	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x000545E8 File Offset: 0x000527E8
	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			num += wheelCollider.motorTorque;
		}
		return num / (float)this.rightWheels.Length;
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x00054638 File Offset: 0x00052838
	public void ScaleSidewaysFriction(float scale)
	{
		float num = 0.75f + 0.75f * scale;
		foreach (WheelCollider wheelCollider in this.rightWheels)
		{
			WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
			sidewaysFriction.stiffness = num;
			wheelCollider.sidewaysFriction = sidewaysFriction;
		}
		foreach (WheelCollider wheelCollider2 in this.leftWheels)
		{
			WheelFrictionCurve sidewaysFriction2 = wheelCollider2.sidewaysFriction;
			sidewaysFriction2.stiffness = num;
			wheelCollider2.sidewaysFriction = sidewaysFriction2;
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x000546B0 File Offset: 0x000528B0
	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float num = torqueAmount * newThrottle;
		int num2 = (rightSide ? this.rightWheels.Length : this.leftWheels.Length);
		int num3 = 0;
		WheelCollider[] array = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			WheelHit wheelHit;
			if (array[i].GetGroundHit(out wheelHit))
			{
				num3++;
			}
		}
		float num4 = 1f;
		if (num3 > 0)
		{
			num4 = (float)(num2 / num3);
		}
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			WheelHit wheelHit2;
			if (wheelCollider.GetGroundHit(out wheelHit2))
			{
				wheelCollider.motorTorque = num * num4;
			}
			else
			{
				wheelCollider.motorTorque = num;
			}
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00054780 File Offset: 0x00052980
	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = this.brakeForce * amount;
		}
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x000547BD File Offset: 0x000529BD
	public void CreateExplosionMarker(float durationMinutes)
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SendMessage("SetDuration", durationMinutes, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x000547FC File Offset: 0x000529FC
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		this.CreateExplosionMarker(10f);
		Effect.server.Run(this.explosionEffect.resourcePath, this.mainTurretEyePos.transform.position, Vector3.up, null, true);
		Vector3 zero = Vector3.zero;
		GameObject gibSource = this.servergibs.Get().GetComponent<global::ServerGib>()._gibSource;
		List<global::ServerGib> list = global::ServerGib.CreateGibs(this.servergibs.resourcePath, base.gameObject, gibSource, zero, 3f);
		for (int i = 0; i < 12 - this.maxCratesToSpawn; i++)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				float num = 3f;
				float num2 = 10f;
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-4f, 4f);
				Collider component = baseEntity.GetComponent<Collider>();
				baseEntity.Spawn();
				baseEntity.SetVelocity(zero + onUnitSphere * UnityEngine.Random.Range(num, num2));
				foreach (global::ServerGib serverGib in list)
				{
					UnityEngine.Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
				}
			}
		}
		for (int j = 0; j < this.maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = UnityEngine.Random.onUnitSphere;
			onUnitSphere2.y = 0f;
			onUnitSphere2.Normalize();
			Vector3 vector = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * UnityEngine.Random.Range(2f, 3f);
			global::BaseEntity baseEntity2 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, vector, Quaternion.LookRotation(onUnitSphere2), true);
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (lootContainer)
			{
				lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
			}
			Collider component2 = baseEntity2.GetComponent<Collider>();
			Rigidbody rigidbody = baseEntity2.gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.mass = 2f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.velocity = zero + onUnitSphere2 * UnityEngine.Random.Range(1f, 3f);
			rigidbody.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			rigidbody.drag = 0.5f * (rigidbody.mass / 5f);
			rigidbody.angularDrag = 0.2f * (rigidbody.mass / 5f);
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath, default(Vector3), default(Quaternion), true) as FireBall;
			if (fireBall)
			{
				fireBall.SetParent(baseEntity2, false, false);
				fireBall.Spawn();
				fireBall.GetComponent<Rigidbody>().isKinematic = true;
				fireBall.GetComponent<Collider>().enabled = false;
			}
			baseEntity2.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);
			foreach (global::ServerGib serverGib2 in list)
			{
				UnityEngine.Physics.IgnoreCollision(component2, serverGib2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00054BE4 File Offset: 0x00052DE4
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		global::BasePlayer basePlayer = info.Initiator as global::BasePlayer;
		if (basePlayer != null)
		{
			this.AddOrUpdateTarget(basePlayer, info.PointStart, info.damageTypes.Total());
		}
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00054C28 File Offset: 0x00052E28
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (!base.isServer)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.healthFraction <= 0.75f, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, base.healthFraction < 0.4f, false, true);
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00054C80 File Offset: 0x00052E80
	public void DoHealing()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.healthFraction < 1f && base.SecondsSinceAttacked > 600f)
		{
			float num = this.MaxHealth() / 300f * UnityEngine.Time.fixedDeltaTime;
			this.Heal(num);
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public global::BasePlayer GetPlayerDamageInitiator()
	{
		return null;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00054CCC File Offset: 0x00052ECC
	public float GetDamageMultiplier(global::BaseEntity ent)
	{
		float num = ((this.throttle > 0f) ? 10f : 0f);
		float num2 = Vector3.Dot(this.myRigidBody.velocity, base.transform.forward);
		if (num2 > 0f)
		{
			num += num2 * 0.5f;
		}
		if (ent is global::BaseVehicle)
		{
			num *= 10f;
		}
		return num;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnHurtTriggerOccupant(global::BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	// Token: 0x04000599 RID: 1433
	[Header("Sound")]
	public BlendedLoopEngineSound engineSound;

	// Token: 0x0400059A RID: 1434
	public SoundDefinition treadLoopDef;

	// Token: 0x0400059B RID: 1435
	public AnimationCurve treadGainCurve;

	// Token: 0x0400059C RID: 1436
	public AnimationCurve treadPitchCurve;

	// Token: 0x0400059D RID: 1437
	public AnimationCurve treadFreqCurve;

	// Token: 0x0400059E RID: 1438
	private Sound treadLoop;

	// Token: 0x0400059F RID: 1439
	private SoundModulation.Modulator treadGain;

	// Token: 0x040005A0 RID: 1440
	private SoundModulation.Modulator treadPitch;

	// Token: 0x040005A1 RID: 1441
	public SoundDefinition chasisLurchSoundDef;

	// Token: 0x040005A2 RID: 1442
	public float chasisLurchAngleDelta = 2f;

	// Token: 0x040005A3 RID: 1443
	public float chasisLurchSpeedDelta = 2f;

	// Token: 0x040005A4 RID: 1444
	private float lastAngle;

	// Token: 0x040005A5 RID: 1445
	private float lastSpeed;

	// Token: 0x040005A6 RID: 1446
	public SoundDefinition turretTurnLoopDef;

	// Token: 0x040005A7 RID: 1447
	public float turretLoopGainSpeed = 3f;

	// Token: 0x040005A8 RID: 1448
	public float turretLoopPitchSpeed = 3f;

	// Token: 0x040005A9 RID: 1449
	public float turretLoopMinAngleDelta;

	// Token: 0x040005AA RID: 1450
	public float turretLoopMaxAngleDelta = 10f;

	// Token: 0x040005AB RID: 1451
	public float turretLoopPitchMin = 0.5f;

	// Token: 0x040005AC RID: 1452
	public float turretLoopPitchMax = 1f;

	// Token: 0x040005AD RID: 1453
	public float turretLoopGainThreshold = 0.0001f;

	// Token: 0x040005AE RID: 1454
	private Sound turretTurnLoop;

	// Token: 0x040005AF RID: 1455
	private SoundModulation.Modulator turretTurnLoopGain;

	// Token: 0x040005B0 RID: 1456
	private SoundModulation.Modulator turretTurnLoopPitch;

	// Token: 0x040005B1 RID: 1457
	public float enginePitch = 0.9f;

	// Token: 0x040005B2 RID: 1458
	public float rpmMultiplier = 0.6f;

	// Token: 0x040005B3 RID: 1459
	private TreadAnimator treadAnimator;

	// Token: 0x040005B4 RID: 1460
	[Header("Pathing")]
	public List<Vector3> currentPath;

	// Token: 0x040005B5 RID: 1461
	public int currentPathIndex;

	// Token: 0x040005B6 RID: 1462
	public bool pathLooping;

	// Token: 0x040005B7 RID: 1463
	[Header("Targeting")]
	public float viewDistance = 100f;

	// Token: 0x040005B8 RID: 1464
	public float searchRange = 100f;

	// Token: 0x040005B9 RID: 1465
	public float searchFrequency = 2f;

	// Token: 0x040005BA RID: 1466
	public float memoryDuration = 20f;

	// Token: 0x040005BB RID: 1467
	public static float sightUpdateRate = 0.5f;

	// Token: 0x040005BC RID: 1468
	public List<global::BradleyAPC.TargetInfo> targetList = new List<global::BradleyAPC.TargetInfo>();

	// Token: 0x040005BD RID: 1469
	private BaseCombatEntity mainGunTarget;

	// Token: 0x040005BE RID: 1470
	[Header("Coax")]
	public float coaxFireRate = 0.06667f;

	// Token: 0x040005BF RID: 1471
	public int coaxBurstLength = 10;

	// Token: 0x040005C0 RID: 1472
	public float coaxAimCone = 3f;

	// Token: 0x040005C1 RID: 1473
	public float bulletDamage = 15f;

	// Token: 0x040005C2 RID: 1474
	[Header("TopTurret")]
	public float topTurretFireRate = 0.25f;

	// Token: 0x040005C3 RID: 1475
	private float nextCoaxTime;

	// Token: 0x040005C4 RID: 1476
	private int numCoaxBursted;

	// Token: 0x040005C5 RID: 1477
	private float nextTopTurretTime = 0.3f;

	// Token: 0x040005C6 RID: 1478
	public GameObjectRef gun_fire_effect;

	// Token: 0x040005C7 RID: 1479
	public GameObjectRef bulletEffect;

	// Token: 0x040005C8 RID: 1480
	private float lastLateUpdate;

	// Token: 0x040005C9 RID: 1481
	[Header("Wheels")]
	public WheelCollider[] leftWheels;

	// Token: 0x040005CA RID: 1482
	public WheelCollider[] rightWheels;

	// Token: 0x040005CB RID: 1483
	[Header("Movement Config")]
	public float moveForceMax = 2000f;

	// Token: 0x040005CC RID: 1484
	public float brakeForce = 100f;

	// Token: 0x040005CD RID: 1485
	public float turnForce = 2000f;

	// Token: 0x040005CE RID: 1486
	public float sideStiffnessMax = 1f;

	// Token: 0x040005CF RID: 1487
	public float sideStiffnessMin = 0.5f;

	// Token: 0x040005D0 RID: 1488
	public Transform centerOfMass;

	// Token: 0x040005D1 RID: 1489
	public float stoppingDist = 5f;

	// Token: 0x040005D2 RID: 1490
	[Header("Control")]
	public float throttle = 1f;

	// Token: 0x040005D3 RID: 1491
	public float turning;

	// Token: 0x040005D4 RID: 1492
	public float rightThrottle;

	// Token: 0x040005D5 RID: 1493
	public float leftThrottle;

	// Token: 0x040005D6 RID: 1494
	public bool brake;

	// Token: 0x040005D7 RID: 1495
	[Header("Other")]
	public Rigidbody myRigidBody;

	// Token: 0x040005D8 RID: 1496
	public Collider myCollider;

	// Token: 0x040005D9 RID: 1497
	public Vector3 destination;

	// Token: 0x040005DA RID: 1498
	private Vector3 finalDestination;

	// Token: 0x040005DB RID: 1499
	public Transform followTest;

	// Token: 0x040005DC RID: 1500
	public TriggerHurtEx impactDamager;

	// Token: 0x040005DD RID: 1501
	[Header("Weapons")]
	public Transform mainTurretEyePos;

	// Token: 0x040005DE RID: 1502
	public Transform mainTurret;

	// Token: 0x040005DF RID: 1503
	public Transform CannonPitch;

	// Token: 0x040005E0 RID: 1504
	public Transform CannonMuzzle;

	// Token: 0x040005E1 RID: 1505
	public Transform coaxPitch;

	// Token: 0x040005E2 RID: 1506
	public Transform coaxMuzzle;

	// Token: 0x040005E3 RID: 1507
	public Transform topTurretEyePos;

	// Token: 0x040005E4 RID: 1508
	public Transform topTurretYaw;

	// Token: 0x040005E5 RID: 1509
	public Transform topTurretPitch;

	// Token: 0x040005E6 RID: 1510
	public Transform topTurretMuzzle;

	// Token: 0x040005E7 RID: 1511
	private Vector3 turretAimVector = Vector3.forward;

	// Token: 0x040005E8 RID: 1512
	private Vector3 desiredAimVector = Vector3.forward;

	// Token: 0x040005E9 RID: 1513
	private Vector3 topTurretAimVector = Vector3.forward;

	// Token: 0x040005EA RID: 1514
	private Vector3 desiredTopTurretAimVector = Vector3.forward;

	// Token: 0x040005EB RID: 1515
	[Header("Effects")]
	public GameObjectRef explosionEffect;

	// Token: 0x040005EC RID: 1516
	public GameObjectRef servergibs;

	// Token: 0x040005ED RID: 1517
	public GameObjectRef fireBall;

	// Token: 0x040005EE RID: 1518
	public GameObjectRef crateToDrop;

	// Token: 0x040005EF RID: 1519
	public GameObjectRef debrisFieldMarker;

	// Token: 0x040005F0 RID: 1520
	[Header("Loot")]
	public int maxCratesToSpawn;

	// Token: 0x040005F1 RID: 1521
	public int patrolPathIndex;

	// Token: 0x040005F2 RID: 1522
	public IAIPath patrolPath;

	// Token: 0x040005F3 RID: 1523
	public bool DoAI = true;

	// Token: 0x040005F4 RID: 1524
	public GameObjectRef mainCannonMuzzleFlash;

	// Token: 0x040005F5 RID: 1525
	public GameObjectRef mainCannonProjectile;

	// Token: 0x040005F6 RID: 1526
	public float recoilScale = 200f;

	// Token: 0x040005F7 RID: 1527
	public NavMeshPath navMeshPath;

	// Token: 0x040005F8 RID: 1528
	public int navMeshPathIndex;

	// Token: 0x040005F9 RID: 1529
	private LayerMask obstacleHitMask;

	// Token: 0x040005FA RID: 1530
	private TimeSince timeSinceSeemingStuck;

	// Token: 0x040005FB RID: 1531
	private TimeSince timeSinceStuckReverseStart;

	// Token: 0x040005FC RID: 1532
	private const string prefabPath = "assets/prefabs/npc/m2bradley/bradleyapc.prefab";

	// Token: 0x040005FD RID: 1533
	private float nextFireTime = 10f;

	// Token: 0x040005FE RID: 1534
	private int numBursted;

	// Token: 0x040005FF RID: 1535
	private float nextPatrolTime;

	// Token: 0x04000600 RID: 1536
	private float nextEngagementPathTime;

	// Token: 0x04000601 RID: 1537
	private float currentSpeedZoneLimit;

	// Token: 0x02000BCE RID: 3022
	[Serializable]
	public class TargetInfo : Facepunch.Pool.IPooled
	{
		// Token: 0x06004DB6 RID: 19894 RVA: 0x001A16B9 File Offset: 0x0019F8B9
		public void EnterPool()
		{
			this.entity = null;
			this.lastSeenPosition = Vector3.zero;
			this.lastSeenTime = 0f;
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x001A16D8 File Offset: 0x0019F8D8
		public void Setup(global::BaseEntity ent, float time)
		{
			this.entity = ent;
			this.lastSeenTime = time;
		}

		// Token: 0x06004DB8 RID: 19896 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06004DB9 RID: 19897 RVA: 0x001A16E8 File Offset: 0x0019F8E8
		public float GetPriorityScore(global::BradleyAPC apc)
		{
			global::BasePlayer basePlayer = this.entity as global::BasePlayer;
			if (basePlayer)
			{
				float num = Vector3.Distance(this.entity.transform.position, apc.transform.position);
				float num2 = (1f - Mathf.InverseLerp(10f, 80f, num)) * 50f;
				float num3 = ((basePlayer.GetHeldEntity() == null) ? 0f : basePlayer.GetHeldEntity().hostileScore);
				float num4 = Mathf.InverseLerp(4f, 20f, num3) * 100f;
				float num5 = Mathf.InverseLerp(10f, 3f, UnityEngine.Time.time - this.lastSeenTime) * 100f;
				float num6 = Mathf.InverseLerp(0f, 100f, this.damageReceivedFrom) * 50f;
				return num2 + num4 + num6 + num5;
			}
			return 0f;
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x001A17CF File Offset: 0x0019F9CF
		public bool IsVisible()
		{
			return this.lastSeenTime != -1f && UnityEngine.Time.time - this.lastSeenTime < global::BradleyAPC.sightUpdateRate * 2f;
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x001A17F9 File Offset: 0x0019F9F9
		public bool IsValid()
		{
			return this.entity != null;
		}

		// Token: 0x04004179 RID: 16761
		public float damageReceivedFrom;

		// Token: 0x0400417A RID: 16762
		public global::BaseEntity entity;

		// Token: 0x0400417B RID: 16763
		public float lastSeenTime;

		// Token: 0x0400417C RID: 16764
		public Vector3 lastSeenPosition;
	}
}
