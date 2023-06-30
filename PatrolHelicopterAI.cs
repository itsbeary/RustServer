using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class PatrolHelicopterAI : BaseMonoBehaviour
{
	// Token: 0x060023CD RID: 9165 RVA: 0x000E47C4 File Offset: 0x000E29C4
	public void UpdateTargetList()
	{
		Vector3 vector = Vector3.zero;
		bool flag = false;
		bool flag2 = false;
		for (int i = this._targetList.Count - 1; i >= 0; i--)
		{
			PatrolHelicopterAI.targetinfo targetinfo = this._targetList[i];
			if (targetinfo == null || targetinfo.ent == null)
			{
				this._targetList.Remove(targetinfo);
			}
			else
			{
				if (UnityEngine.Time.realtimeSinceStartup > targetinfo.nextLOSCheck)
				{
					targetinfo.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1f;
					if (this.PlayerVisible(targetinfo.ply))
					{
						targetinfo.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
						targetinfo.visibleFor += 1f;
					}
					else
					{
						targetinfo.visibleFor = 0f;
					}
				}
				bool flag3 = (targetinfo.ply ? targetinfo.ply.IsDead() : (targetinfo.ent.Health() <= 0f));
				if (targetinfo.TimeSinceSeen() >= 6f || flag3)
				{
					bool flag4 = UnityEngine.Random.Range(0f, 1f) >= 0f;
					if ((this.CanStrafe() || this.CanUseNapalm()) && this.IsAlive() && !flag && !flag3 && (targetinfo.ply == this.leftGun._target || targetinfo.ply == this.rightGun._target) && flag4)
					{
						flag2 = !this.ValidStrafeTarget(targetinfo.ply) || UnityEngine.Random.Range(0f, 1f) > 0.75f;
						flag = true;
						vector = targetinfo.ply.transform.position;
					}
					this._targetList.Remove(targetinfo);
				}
			}
		}
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.InSafeZone() && Vector3Ex.Distance2D(base.transform.position, basePlayer.transform.position) <= 150f)
			{
				bool flag5 = false;
				using (List<PatrolHelicopterAI.targetinfo>.Enumerator enumerator2 = this._targetList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.ply == basePlayer)
						{
							flag5 = true;
							break;
						}
					}
				}
				if (!flag5 && basePlayer.GetThreatLevel() > 0.5f && this.PlayerVisible(basePlayer))
				{
					this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
				}
			}
		}
		if (flag)
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(vector, flag2);
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000E4AA0 File Offset: 0x000E2CA0
	public bool PlayerVisible(BasePlayer ply)
	{
		Vector3 position = ply.eyes.position;
		if (TOD_Sky.Instance.IsNight && Vector3.Distance(position, this.interestZoneOrigin) > 40f)
		{
			return false;
		}
		Vector3 vector = base.transform.position - Vector3.up * 6f;
		float num = Vector3.Distance(position, vector);
		Vector3 normalized = (position - vector).normalized;
		RaycastHit raycastHit;
		return GamePhysics.Trace(new Ray(vector + normalized * 5f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == ply;
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000E4B68 File Offset: 0x000E2D68
	public void WasAttacked(HitInfo info)
	{
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if (basePlayer != null)
		{
			this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000E4B9C File Offset: 0x000E2D9C
	public void Awake()
	{
		if (PatrolHelicopter.lifetimeMinutes == 0f)
		{
			base.Invoke(new Action(this.DestroyMe), 1f);
			return;
		}
		base.InvokeRepeating(new Action(this.UpdateWind), 0f, 1f / this.windFrequency);
		this._lastPos = base.transform.position;
		this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		this.InitializeAI();
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000E4C14 File Offset: 0x000E2E14
	public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
	{
		this.hasInterestZone = true;
		this.interestZoneOrigin = dest;
		float x = TerrainMeta.Size.x;
		float num = dest.y + 25f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * mapScaleDistance;
		vector.y = num;
		if (mapScaleDistance == 0f)
		{
			vector = this.interestZoneOrigin + new Vector3(0f, 10f, 0f);
		}
		base.transform.position = vector;
		this.ExitCurrentState();
		this.State_Move_Enter(dest);
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000E4CC0 File Offset: 0x000E2EC0
	public void Retire()
	{
		if (this.isRetiring)
		{
			return;
		}
		this.isRetiring = true;
		base.Invoke(new Action(this.DestroyMe), 240f);
		float x = TerrainMeta.Size.x;
		float num = 200f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 20f;
		vector.y = num;
		this.ExitCurrentState();
		this.State_Move_Enter(vector);
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x000E4D4C File Offset: 0x000E2F4C
	public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
	{
		float num = ((rotationSpeedOverride == -1f) ? Mathf.Clamp01(this.moveSpeed / (this.maxSpeed * 0.5f)) : rotationSpeedOverride);
		this.rotationSpeed = num * this.maxRotationSpeed;
		this.targetRotation = newTargetRot;
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000E4D94 File Offset: 0x000E2F94
	public Quaternion GetYawRotationTo(Vector3 targetDest)
	{
		Vector3 vector = targetDest;
		vector.y = 0f;
		Vector3 position = base.transform.position;
		position.y = 0f;
		return Quaternion.LookRotation((vector - position).normalized);
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000E4DDC File Offset: 0x000E2FDC
	public void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
	{
		this.destination = targetDest;
		this.destination_min_dist = minDist;
		float num = Vector3.Distance(targetDest, base.transform.position);
		if (num > minDistForFacingRotation && !this.IsTargeting())
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.destination), -1f);
		}
		this.targetThrottleSpeed = this.GetThrottleForDistance(num);
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000E4E39 File Offset: 0x000E3039
	public bool AtDestination()
	{
		return Vector3.Distance(base.transform.position, this.destination) < this.destination_min_dist;
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000E4E5C File Offset: 0x000E305C
	public void MoveToDestination()
	{
		Vector3 vector = Vector3.Lerp(this._lastMoveDir, (this.destination - base.transform.position).normalized, UnityEngine.Time.deltaTime / this.courseAdjustLerpTime);
		this._lastMoveDir = vector;
		this.throttleSpeed = Mathf.Lerp(this.throttleSpeed, this.targetThrottleSpeed, UnityEngine.Time.deltaTime / 3f);
		float num = this.throttleSpeed * this.maxSpeed;
		this.TerrainPushback();
		base.transform.position += vector * num * UnityEngine.Time.deltaTime;
		this.windVec = Vector3.Lerp(this.windVec, this.targetWindVec, UnityEngine.Time.deltaTime);
		base.transform.position += this.windVec * this.windForce * UnityEngine.Time.deltaTime;
		this.moveSpeed = Mathf.Lerp(this.moveSpeed, Vector3.Distance(this._lastPos, base.transform.position) / UnityEngine.Time.deltaTime, UnityEngine.Time.deltaTime * 2f);
		this._lastPos = base.transform.position;
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000E4F9C File Offset: 0x000E319C
	public void TerrainPushback()
	{
		if (this._currentState == PatrolHelicopterAI.aiState.DEATH)
		{
			return;
		}
		Vector3 vector = base.transform.position + new Vector3(0f, 2f, 0f);
		Vector3 normalized = (this.destination - vector).normalized;
		float num = Vector3.Distance(this.destination, base.transform.position);
		Ray ray = new Ray(vector, normalized);
		float num2 = 5f;
		float num3 = Mathf.Min(100f, num);
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		Vector3 vector2 = Vector3.zero;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num2, out raycastHit, num3 - num2 * 0.5f, mask))
		{
			float num4 = 1f - raycastHit.distance / num3;
			float num5 = this.terrainPushForce * num4;
			vector2 = Vector3.up * num5;
		}
		Ray ray2 = new Ray(vector, this._lastMoveDir);
		float num6 = Mathf.Min(10f, num);
		RaycastHit raycastHit2;
		if (UnityEngine.Physics.SphereCast(ray2, num2, out raycastHit2, num6 - num2 * 0.5f, mask))
		{
			float num7 = 1f - raycastHit2.distance / num6;
			float num8 = this.obstaclePushForce * num7;
			vector2 += this._lastMoveDir * num8 * -1f;
			vector2 += Vector3.up * num8;
		}
		this.pushVec = Vector3.Lerp(this.pushVec, vector2, UnityEngine.Time.deltaTime);
		base.transform.position += this.pushVec * UnityEngine.Time.deltaTime;
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000E5154 File Offset: 0x000E3354
	public void UpdateRotation()
	{
		if (this.hasAimTarget)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			Vector3 aimTarget = this._aimTarget;
			aimTarget.y = 0f;
			Vector3 normalized = (aimTarget - position).normalized;
			Vector3 vector = Vector3.Cross(normalized, Vector3.up);
			float num = Vector3.Angle(normalized, base.transform.right);
			float num2 = Vector3.Angle(normalized, -base.transform.right);
			if (this.aimDoorSide)
			{
				if (num < num2)
				{
					this.targetRotation = Quaternion.LookRotation(vector);
				}
				else
				{
					this.targetRotation = Quaternion.LookRotation(-vector);
				}
			}
			else
			{
				this.targetRotation = Quaternion.LookRotation(normalized);
			}
		}
		this.rotationSpeed = Mathf.Lerp(this.rotationSpeed, this.maxRotationSpeed, UnityEngine.Time.deltaTime / 2f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.rotationSpeed * UnityEngine.Time.deltaTime);
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000E526C File Offset: 0x000E346C
	public void UpdateSpotlight()
	{
		if (this.hasInterestZone)
		{
			this.helicopterBase.spotlightTarget = new Vector3(this.interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(this.interestZoneOrigin), this.interestZoneOrigin.z);
			return;
		}
		this.helicopterBase.spotlightTarget = Vector3.zero;
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x000E52C8 File Offset: 0x000E34C8
	public void Update()
	{
		if (this.helicopterBase.isClient)
		{
			return;
		}
		PatrolHelicopterAI.heliInstance = this;
		this.UpdateTargetList();
		this.MoveToDestination();
		this.UpdateRotation();
		this.UpdateSpotlight();
		this.AIThink();
		this.DoMachineGuns();
		if (!this.isRetiring)
		{
			float num = Mathf.Max(this.spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, this.lastDamageTime + 120f);
			if (UnityEngine.Time.realtimeSinceStartup > num)
			{
				this.Retire();
			}
		}
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000E5348 File Offset: 0x000E3548
	public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastDamageTime;
		this.lastDamageTime = UnityEngine.Time.realtimeSinceStartup;
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		bool flag = this.ValidStrafeTarget(basePlayer);
		bool flag2 = flag && this.CanStrafe();
		bool flag3 = !flag && this.CanUseNapalm();
		if (num < 5f && basePlayer != null && (flag2 || flag3))
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(info.Initiator.transform.position, flag3);
		}
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000E53CA File Offset: 0x000E35CA
	public void CriticalDamage()
	{
		this.isDead = true;
		this.ExitCurrentState();
		this.State_Death_Enter();
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000E53E0 File Offset: 0x000E35E0
	public void DoMachineGuns()
	{
		if (this._targetList.Count > 0)
		{
			if (this.leftGun.NeedsNewTarget())
			{
				this.leftGun.UpdateTargetFromList(this._targetList);
			}
			if (this.rightGun.NeedsNewTarget())
			{
				this.rightGun.UpdateTargetFromList(this._targetList);
			}
		}
		this.leftGun.TurretThink();
		this.rightGun.TurretThink();
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000E5450 File Offset: 0x000E3650
	public void FireGun(Vector3 targetPos, float aimCone, bool left)
	{
		if (PatrolHelicopter.guns == 0)
		{
			return;
		}
		Vector3 vector = (left ? this.helicopterBase.left_gun_muzzle.transform : this.helicopterBase.right_gun_muzzle.transform).position;
		Vector3 normalized = (targetPos - vector).normalized;
		vector += normalized * 2f;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
		RaycastHit raycastHit;
		if (GamePhysics.Trace(new Ray(vector, modifiedAimConeDirection), 0f, out raycastHit, 300f, 1220225809, QueryTriggerInteraction.UseGlobal, null))
		{
			targetPos = raycastHit.point;
			if (raycastHit.collider)
			{
				BaseEntity entity = raycastHit.GetEntity();
				if (entity && entity != this.helicopterBase)
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					HitInfo hitInfo = new HitInfo(this.helicopterBase, entity, DamageType.Bullet, this.helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, raycastHit.point);
					if (baseCombatEntity)
					{
						baseCombatEntity.OnAttacked(hitInfo);
						if (baseCombatEntity is BasePlayer)
						{
							Effect.server.ImpactEffect(new HitInfo
							{
								HitPositionWorld = raycastHit.point - modifiedAimConeDirection * 0.25f,
								HitNormalWorld = -modifiedAimConeDirection,
								HitMaterial = StringPool.Get("Flesh")
							});
						}
					}
					else
					{
						entity.OnAttacked(hitInfo);
					}
				}
			}
		}
		else
		{
			targetPos = vector + modifiedAimConeDirection * 300f;
		}
		this.helicopterBase.ClientRPC<bool, Vector3>(null, "FireGun", left, targetPos);
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000E55E3 File Offset: 0x000E37E3
	public bool CanInterruptState()
	{
		return this._currentState != PatrolHelicopterAI.aiState.STRAFE && this._currentState != PatrolHelicopterAI.aiState.DEATH;
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000E55FC File Offset: 0x000E37FC
	public bool IsAlive()
	{
		return !this.isDead;
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000E5607 File Offset: 0x000E3807
	public void DestroyMe()
	{
		this.helicopterBase.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000E5615 File Offset: 0x000E3815
	public Vector3 GetLastMoveDir()
	{
		return this._lastMoveDir;
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000E5620 File Offset: 0x000E3820
	public Vector3 GetMoveDirection()
	{
		return (this.destination - base.transform.position).normalized;
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000E564B File Offset: 0x000E384B
	public float GetMoveSpeed()
	{
		return this.moveSpeed;
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000E5653 File Offset: 0x000E3853
	public float GetMaxRotationSpeed()
	{
		return this.maxRotationSpeed;
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000E565B File Offset: 0x000E385B
	public bool IsTargeting()
	{
		return this.hasAimTarget;
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000E5663 File Offset: 0x000E3863
	public void UpdateWind()
	{
		this.targetWindVec = UnityEngine.Random.onUnitSphere;
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000E5670 File Offset: 0x000E3870
	public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
	{
		if (this.movementLockingAiming)
		{
			return;
		}
		this.hasAimTarget = true;
		this._aimTarget = aimTarg;
		this.aimDoorSide = isDoorSide;
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000E5690 File Offset: 0x000E3890
	public void ClearAimTarget()
	{
		this.hasAimTarget = false;
		this._aimTarget = Vector3.zero;
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000E56A4 File Offset: 0x000E38A4
	public void State_Death_Think(float timePassed)
	{
		float num = UnityEngine.Time.realtimeSinceStartup * 0.25f;
		float num2 = Mathf.Sin(6.2831855f * num) * 10f;
		float num3 = Mathf.Cos(6.2831855f * num) * 10f;
		Vector3 vector = new Vector3(num2, 0f, num3);
		this.SetAimTarget(base.transform.position + vector, true);
		Ray ray = new Ray(base.transform.position, this.GetLastMoveDir());
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" });
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, 3f, out raycastHit, 5f, mask) || UnityEngine.Time.realtimeSinceStartup > this.deathTimeout)
		{
			this.helicopterBase.Hurt(this.helicopterBase.health * 2f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000E5794 File Offset: 0x000E3994
	public void State_Death_Enter()
	{
		this.maxRotationSpeed *= 8f;
		this._currentState = PatrolHelicopterAI.aiState.DEATH;
		Vector3 randomOffset = this.GetRandomOffset(base.transform.position, 20f, 60f, 20f, 30f);
		int num = 1237003025;
		Vector3 vector;
		Vector3 vector2;
		TransformUtil.GetGroundInfo(randomOffset - Vector3.up * 2f, out vector, out vector2, 500f, num, null);
		this.SetTargetDestination(vector, 5f, 30f);
		this.targetThrottleSpeed = 0.5f;
		this.deathTimeout = UnityEngine.Time.realtimeSinceStartup + 10f;
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Death_Leave()
	{
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000E583C File Offset: 0x000E3A3C
	public void State_Idle_Think(float timePassed)
	{
		this.ExitCurrentState();
		this.State_Patrol_Enter();
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000E584A File Offset: 0x000E3A4A
	public void State_Idle_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Idle_Leave()
	{
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000E5854 File Offset: 0x000E3A54
	public void State_Move_Think(float timePassed)
	{
		float num = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(num);
		if (this.AtDestination())
		{
			this.ExitCurrentState();
			this.State_Idle_Enter();
		}
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000E589C File Offset: 0x000E3A9C
	public void State_Move_Enter(Vector3 newPos)
	{
		this._currentState = PatrolHelicopterAI.aiState.MOVE;
		this.destination_min_dist = 5f;
		this.SetTargetDestination(newPos, 5f, 30f);
		float num = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(num);
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Move_Leave()
	{
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000E58F0 File Offset: 0x000E3AF0
	public void State_Orbit_Think(float timePassed)
	{
		if (this.breakingOrbit)
		{
			if (this.AtDestination())
			{
				this.ExitCurrentState();
				this.State_Idle_Enter();
			}
		}
		else
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.destination) > 15f)
			{
				return;
			}
			if (!this.hasEnteredOrbit)
			{
				this.hasEnteredOrbit = true;
				this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			float num = 6.2831855f * this.currentOrbitDistance;
			float num2 = 0.5f * this.maxSpeed;
			float num3 = num / num2;
			this.currentOrbitTime += timePassed / (num3 * 1.01f);
			float num4 = this.currentOrbitTime;
			Vector3 orbitPosition = this.GetOrbitPosition(num4);
			this.ClearAimTarget();
			this.SetTargetDestination(orbitPosition, 0f, 1f);
			this.targetThrottleSpeed = 0.5f;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.orbitStartTime > this.maxOrbitDuration && !this.breakingOrbit)
		{
			this.breakingOrbit = true;
			Vector3 appropriatePosition = this.GetAppropriatePosition(base.transform.position + base.transform.forward * 75f, 40f, 50f);
			this.SetTargetDestination(appropriatePosition, 10f, 0f);
		}
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000E5A28 File Offset: 0x000E3C28
	public Vector3 GetOrbitPosition(float rate)
	{
		float num = Mathf.Sin(6.2831855f * rate) * this.currentOrbitDistance;
		float num2 = Mathf.Cos(6.2831855f * rate) * this.currentOrbitDistance;
		Vector3 vector = new Vector3(num, 20f, num2);
		vector = this.interestZoneOrigin + vector;
		return vector;
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000E5A7C File Offset: 0x000E3C7C
	public void State_Orbit_Enter(float orbitDistance)
	{
		this._currentState = PatrolHelicopterAI.aiState.ORBIT;
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
		Vector3 vector = base.transform.position - this.interestZoneOrigin;
		this.currentOrbitTime = Mathf.Atan2(vector.x, vector.z);
		this.currentOrbitDistance = orbitDistance;
		this.ClearAimTarget();
		this.SetTargetDestination(this.GetOrbitPosition(this.currentOrbitTime), 20f, 0f);
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000E5B00 File Offset: 0x000E3D00
	public void State_Orbit_Leave()
	{
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.currentOrbitTime = 0f;
		this.ClearAimTarget();
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000E5B24 File Offset: 0x000E3D24
	public Vector3 GetRandomPatrolDestination()
	{
		Vector3 vector = Vector3.zero;
		if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			MonumentInfo monumentInfo = null;
			if (this._visitedMonuments.Count > 0)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (!monumentInfo2.IsSafeZone)
					{
						bool flag = false;
						foreach (MonumentInfo monumentInfo3 in this._visitedMonuments)
						{
							if (monumentInfo2 == monumentInfo3)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							monumentInfo = monumentInfo2;
							break;
						}
					}
				}
			}
			if (monumentInfo == null)
			{
				this._visitedMonuments.Clear();
				for (int i = 0; i < 5; i++)
				{
					monumentInfo = TerrainMeta.Path.Monuments[UnityEngine.Random.Range(0, TerrainMeta.Path.Monuments.Count)];
					if (!monumentInfo.IsSafeZone)
					{
						break;
					}
				}
			}
			if (monumentInfo)
			{
				vector = monumentInfo.transform.position;
				this._visitedMonuments.Add(monumentInfo);
				vector.y = TerrainMeta.HeightMap.GetHeight(vector) + 200f;
				RaycastHit raycastHit;
				if (TransformUtil.GetGroundInfo(vector, out raycastHit, 300f, 1235288065, null))
				{
					vector.y = raycastHit.point.y;
				}
				vector.y += 30f;
			}
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float num = 30f;
			vector = Vector3Ex.Range(-1f, 1f);
			vector.y = 0f;
			vector.Normalize();
			vector *= x * UnityEngine.Random.Range(0f, 0.75f);
			vector.y = num;
		}
		return vector;
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000E5D44 File Offset: 0x000E3F44
	public void State_Patrol_Think(float timePassed)
	{
		float num = Vector3Ex.Distance2D(base.transform.position, this.destination);
		if (num <= 25f)
		{
			this.targetThrottleSpeed = this.GetThrottleForDistance(num);
		}
		else
		{
			this.targetThrottleSpeed = 0.5f;
		}
		if (this.AtDestination() && this.arrivalTime == 0f)
		{
			this.arrivalTime = UnityEngine.Time.realtimeSinceStartup;
			this.ExitCurrentState();
			this.maxOrbitDuration = 20f;
			this.State_Orbit_Enter(75f);
		}
		if (this._targetList.Count > 0)
		{
			this.interestZoneOrigin = this._targetList[0].ply.transform.position + new Vector3(0f, 20f, 0f);
			this.ExitCurrentState();
			this.maxOrbitDuration = 10f;
			this.State_Orbit_Enter(75f);
		}
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000E5E2C File Offset: 0x000E402C
	public void State_Patrol_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.PATROL;
		Vector3 randomPatrolDestination = this.GetRandomPatrolDestination();
		this.SetTargetDestination(randomPatrolDestination, 10f, 30f);
		this.interestZoneOrigin = randomPatrolDestination;
		this.arrivalTime = 0f;
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Patrol_Leave()
	{
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000E5E6A File Offset: 0x000E406A
	public int ClipRocketsLeft()
	{
		return this.numRocketsLeft;
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000E5E72 File Offset: 0x000E4072
	public bool CanStrafe()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastStrafeTime >= 20f && this.CanInterruptState();
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000E5E8F File Offset: 0x000E408F
	public bool CanUseNapalm()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastNapalmTime >= 30f;
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000E5EA8 File Offset: 0x000E40A8
	public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
	{
		if (this.CanUseNapalm() && shouldUseNapalm)
		{
			this.useNapalm = shouldUseNapalm;
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		this._currentState = PatrolHelicopterAI.aiState.STRAFE;
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" });
		Vector3 vector;
		Vector3 vector2;
		if (TransformUtil.GetGroundInfo(strafePos, out vector, out vector2, 100f, mask, base.transform))
		{
			this.strafe_target_position = vector;
		}
		else
		{
			this.strafe_target_position = strafePos;
		}
		this.numRocketsLeft = 12;
		this.lastRocketTime = 0f;
		this.movementLockingAiming = true;
		Vector3 randomOffset = this.GetRandomOffset(strafePos, 175f, 192.5f, 20f, 30f);
		this.SetTargetDestination(randomOffset, 10f, 30f);
		this.SetIdealRotation(this.GetYawRotationTo(randomOffset), -1f);
		this.puttingDistance = true;
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000E5F9C File Offset: 0x000E419C
	public void State_Strafe_Think(float timePassed)
	{
		if (this.puttingDistance)
		{
			if (this.AtDestination())
			{
				this.puttingDistance = false;
				this.SetTargetDestination(this.strafe_target_position + new Vector3(0f, 40f, 0f), 10f, 30f);
				this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
				return;
			}
		}
		else
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
			float num = Vector3Ex.Distance2D(this.strafe_target_position, base.transform.position);
			if (num <= 150f && this.ClipRocketsLeft() > 0 && UnityEngine.Time.realtimeSinceStartup - this.lastRocketTime > this.timeBetweenRockets)
			{
				float num2 = Vector3.Distance(this.strafe_target_position, base.transform.position) - 10f;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (!UnityEngine.Physics.Raycast(base.transform.position, (this.strafe_target_position - base.transform.position).normalized, num2, LayerMask.GetMask(new string[] { "Terrain", "World" })))
				{
					this.FireRocket();
				}
			}
			if (this.ClipRocketsLeft() <= 0 || num <= 15f)
			{
				this.ExitCurrentState();
				this.State_Move_Enter(this.GetAppropriatePosition(this.strafe_target_position + base.transform.forward * 120f, 20f, 30f));
			}
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000E6133 File Offset: 0x000E4333
	public bool ValidStrafeTarget(BasePlayer ply)
	{
		return !ply.IsNearEnemyBase();
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000E613E File Offset: 0x000E433E
	public void State_Strafe_Leave()
	{
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.useNapalm)
		{
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.useNapalm = false;
		this.movementLockingAiming = false;
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000E616C File Offset: 0x000E436C
	public void FireRocket()
	{
		this.numRocketsLeft--;
		this.lastRocketTime = UnityEngine.Time.realtimeSinceStartup;
		float num = 4f;
		bool flag = this.leftTubeFiredLast;
		this.leftTubeFiredLast = !this.leftTubeFiredLast;
		Transform transform = (flag ? this.helicopterBase.rocket_tube_left.transform : this.helicopterBase.rocket_tube_right.transform);
		Vector3 vector = transform.position + transform.forward * 1f;
		Vector3 vector2 = (this.strafe_target_position - vector).normalized;
		if (num > 0f)
		{
			vector2 = AimConeUtil.GetModifiedAimConeDirection(num, vector2, true);
		}
		float num2 = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector, vector2, out raycastHit, num2, 1237003025))
		{
			num2 = raycastHit.distance - 0.1f;
		}
		Effect.server.Run(this.helicopterBase.rocket_fire_effect.resourcePath, this.helicopterBase, StringPool.Get(flag ? "rocket_tube_left" : "rocket_tube_right"), Vector3.zero, Vector3.forward, null, true);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.useNapalm ? this.rocketProjectile_Napalm.resourcePath : this.rocketProjectile.resourcePath, vector, default(Quaternion), true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(vector2 * component.speed);
		}
		baseEntity.Spawn();
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000E62EF File Offset: 0x000E44EF
	public void InitializeAI()
	{
		this._lastThinkTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000E62FC File Offset: 0x000E44FC
	public void OnCurrentStateExit()
	{
		switch (this._currentState)
		{
		default:
			this.State_Idle_Leave();
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Leave();
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Leave();
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Leave();
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Leave();
			return;
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000E634C File Offset: 0x000E454C
	public void ExitCurrentState()
	{
		this.OnCurrentStateExit();
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000E635B File Offset: 0x000E455B
	public float GetTime()
	{
		return UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000E6364 File Offset: 0x000E4564
	public void AIThink()
	{
		float time = this.GetTime();
		float num = time - this._lastThinkTime;
		this._lastThinkTime = time;
		switch (this._currentState)
		{
		default:
			this.State_Idle_Think(num);
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Think(num);
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Think(num);
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Think(num);
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Think(num);
			return;
		case PatrolHelicopterAI.aiState.DEATH:
			this.State_Death_Think(num);
			return;
		}
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000E63E0 File Offset: 0x000E45E0
	public Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		maxRange = Mathf.Max(minRange, maxRange);
		Vector3 vector = origin + onUnitSphere * UnityEngine.Random.Range(minRange, maxRange);
		return this.GetAppropriatePosition(vector, minHeight, maxHeight);
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x000E6430 File Offset: 0x000E4630
	public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
	{
		float num = 100f;
		Ray ray = new Ray(origin + new Vector3(0f, num, 0f), Vector3.down);
		float num2 = 5f;
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" });
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num2, out raycastHit, num * 2f - num2, mask))
		{
			origin = raycastHit.point;
		}
		origin.y += UnityEngine.Random.Range(minHeight, maxHeight);
		return origin;
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000E64C8 File Offset: 0x000E46C8
	public float GetThrottleForDistance(float distToTarget)
	{
		float num;
		if (distToTarget >= 75f)
		{
			num = 1f;
		}
		else if (distToTarget >= 50f)
		{
			num = 0.75f;
		}
		else if (distToTarget >= 25f)
		{
			num = 0.33f;
		}
		else if (distToTarget >= 5f)
		{
			num = 0.05f;
		}
		else
		{
			num = 0.05f * (1f - distToTarget / 5f);
		}
		return num;
	}

	// Token: 0x04001BDA RID: 7130
	public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();

	// Token: 0x04001BDB RID: 7131
	public Vector3 interestZoneOrigin;

	// Token: 0x04001BDC RID: 7132
	public Vector3 destination;

	// Token: 0x04001BDD RID: 7133
	public bool hasInterestZone;

	// Token: 0x04001BDE RID: 7134
	public float moveSpeed;

	// Token: 0x04001BDF RID: 7135
	public float maxSpeed = 25f;

	// Token: 0x04001BE0 RID: 7136
	public float courseAdjustLerpTime = 2f;

	// Token: 0x04001BE1 RID: 7137
	public Quaternion targetRotation;

	// Token: 0x04001BE2 RID: 7138
	public Vector3 windVec;

	// Token: 0x04001BE3 RID: 7139
	public Vector3 targetWindVec;

	// Token: 0x04001BE4 RID: 7140
	public float windForce = 5f;

	// Token: 0x04001BE5 RID: 7141
	public float windFrequency = 1f;

	// Token: 0x04001BE6 RID: 7142
	public float targetThrottleSpeed;

	// Token: 0x04001BE7 RID: 7143
	public float throttleSpeed;

	// Token: 0x04001BE8 RID: 7144
	public float maxRotationSpeed = 90f;

	// Token: 0x04001BE9 RID: 7145
	public float rotationSpeed;

	// Token: 0x04001BEA RID: 7146
	public float terrainPushForce = 100f;

	// Token: 0x04001BEB RID: 7147
	public float obstaclePushForce = 100f;

	// Token: 0x04001BEC RID: 7148
	public HelicopterTurret leftGun;

	// Token: 0x04001BED RID: 7149
	public HelicopterTurret rightGun;

	// Token: 0x04001BEE RID: 7150
	public static PatrolHelicopterAI heliInstance;

	// Token: 0x04001BEF RID: 7151
	public BaseHelicopter helicopterBase;

	// Token: 0x04001BF0 RID: 7152
	public PatrolHelicopterAI.aiState _currentState;

	// Token: 0x04001BF1 RID: 7153
	private Vector3 _aimTarget;

	// Token: 0x04001BF2 RID: 7154
	private bool movementLockingAiming;

	// Token: 0x04001BF3 RID: 7155
	private bool hasAimTarget;

	// Token: 0x04001BF4 RID: 7156
	private bool aimDoorSide;

	// Token: 0x04001BF5 RID: 7157
	private Vector3 pushVec = Vector3.zero;

	// Token: 0x04001BF6 RID: 7158
	private Vector3 _lastPos;

	// Token: 0x04001BF7 RID: 7159
	private Vector3 _lastMoveDir;

	// Token: 0x04001BF8 RID: 7160
	private bool isDead;

	// Token: 0x04001BF9 RID: 7161
	private bool isRetiring;

	// Token: 0x04001BFA RID: 7162
	private float spawnTime;

	// Token: 0x04001BFB RID: 7163
	private float lastDamageTime;

	// Token: 0x04001BFC RID: 7164
	private float deathTimeout;

	// Token: 0x04001BFD RID: 7165
	private float destination_min_dist = 2f;

	// Token: 0x04001BFE RID: 7166
	private float currentOrbitDistance;

	// Token: 0x04001BFF RID: 7167
	private float currentOrbitTime;

	// Token: 0x04001C00 RID: 7168
	private bool hasEnteredOrbit;

	// Token: 0x04001C01 RID: 7169
	private float orbitStartTime;

	// Token: 0x04001C02 RID: 7170
	private float maxOrbitDuration = 30f;

	// Token: 0x04001C03 RID: 7171
	private bool breakingOrbit;

	// Token: 0x04001C04 RID: 7172
	public List<MonumentInfo> _visitedMonuments;

	// Token: 0x04001C05 RID: 7173
	public float arrivalTime;

	// Token: 0x04001C06 RID: 7174
	public GameObjectRef rocketProjectile;

	// Token: 0x04001C07 RID: 7175
	public GameObjectRef rocketProjectile_Napalm;

	// Token: 0x04001C08 RID: 7176
	private bool leftTubeFiredLast;

	// Token: 0x04001C09 RID: 7177
	private float lastRocketTime;

	// Token: 0x04001C0A RID: 7178
	private float timeBetweenRockets = 0.2f;

	// Token: 0x04001C0B RID: 7179
	private int numRocketsLeft = 12;

	// Token: 0x04001C0C RID: 7180
	private const int maxRockets = 12;

	// Token: 0x04001C0D RID: 7181
	private Vector3 strafe_target_position;

	// Token: 0x04001C0E RID: 7182
	private bool puttingDistance;

	// Token: 0x04001C0F RID: 7183
	private const float strafe_approach_range = 175f;

	// Token: 0x04001C10 RID: 7184
	private const float strafe_firing_range = 150f;

	// Token: 0x04001C11 RID: 7185
	private bool useNapalm;

	// Token: 0x04001C12 RID: 7186
	[NonSerialized]
	private float lastNapalmTime = float.NegativeInfinity;

	// Token: 0x04001C13 RID: 7187
	[NonSerialized]
	private float lastStrafeTime = float.NegativeInfinity;

	// Token: 0x04001C14 RID: 7188
	private float _lastThinkTime;

	// Token: 0x02000CF6 RID: 3318
	public class targetinfo
	{
		// Token: 0x0600501E RID: 20510 RVA: 0x001A8261 File Offset: 0x001A6461
		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			this.ply = initPly;
			this.ent = initEnt;
			this.lastSeenTime = float.PositiveInfinity;
			this.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1.5f;
		}

		// Token: 0x0600501F RID: 20511 RVA: 0x001A829E File Offset: 0x001A649E
		public bool IsVisible()
		{
			return this.TimeSinceSeen() < 1.5f;
		}

		// Token: 0x06005020 RID: 20512 RVA: 0x001A82AD File Offset: 0x001A64AD
		public float TimeSinceSeen()
		{
			return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTime;
		}

		// Token: 0x04004617 RID: 17943
		public BasePlayer ply;

		// Token: 0x04004618 RID: 17944
		public BaseEntity ent;

		// Token: 0x04004619 RID: 17945
		public float lastSeenTime = float.PositiveInfinity;

		// Token: 0x0400461A RID: 17946
		public float visibleFor;

		// Token: 0x0400461B RID: 17947
		public float nextLOSCheck;
	}

	// Token: 0x02000CF7 RID: 3319
	public enum aiState
	{
		// Token: 0x0400461D RID: 17949
		IDLE,
		// Token: 0x0400461E RID: 17950
		MOVE,
		// Token: 0x0400461F RID: 17951
		ORBIT,
		// Token: 0x04004620 RID: 17952
		STRAFE,
		// Token: 0x04004621 RID: 17953
		PATROL,
		// Token: 0x04004622 RID: 17954
		GUARD,
		// Token: 0x04004623 RID: 17955
		DEATH
	}
}
