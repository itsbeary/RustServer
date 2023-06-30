using System;
using System.Collections;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Token: 0x02000044 RID: 68
public class BaseNpc : BaseCombatEntity
{
	// Token: 0x06000460 RID: 1120 RVA: 0x00035560 File Offset: 0x00033760
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseNpc.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000355A0 File Offset: 0x000337A0
	public void UpdateDestination(Vector3 position)
	{
		if (this.IsStopped)
		{
			this.IsStopped = false;
		}
		if ((this.Destination - position).sqrMagnitude > 0.010000001f)
		{
			this.Destination = position;
		}
		this.ChaseTransform = null;
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x000355E5 File Offset: 0x000337E5
	public void UpdateDestination(Transform tx)
	{
		this.IsStopped = false;
		this.ChaseTransform = tx;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x000355F5 File Offset: 0x000337F5
	public void StopMoving()
	{
		this.IsStopped = true;
		this.ChaseTransform = null;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, 0, true, true);
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00035610 File Offset: 0x00033810
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00035620 File Offset: 0x00033820
	public static Vector3 GetNewNavPosWithVelocity(global::BaseEntity ent, Vector3 velocity)
	{
		global::BaseEntity parentEntity = ent.GetParentEntity();
		if (parentEntity != null)
		{
			velocity = parentEntity.transform.InverseTransformDirection(velocity);
		}
		Vector3 vector = ent.ServerPosition + velocity * UnityEngine.Time.fixedDeltaTime;
		NavMeshHit navMeshHit;
		NavMesh.Raycast(ent.ServerPosition, vector, out navMeshHit, -1);
		if (!navMeshHit.position.IsNaNOrInfinity())
		{
			return navMeshHit.position;
		}
		return ent.ServerPosition;
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x00035690 File Offset: 0x00033890
	// (set) Token: 0x06000467 RID: 1127 RVA: 0x00035698 File Offset: 0x00033898
	public int AgentTypeIndex
	{
		get
		{
			return this.agentTypeIndex;
		}
		set
		{
			this.agentTypeIndex = value;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000468 RID: 1128 RVA: 0x000356A1 File Offset: 0x000338A1
	// (set) Token: 0x06000469 RID: 1129 RVA: 0x000356A9 File Offset: 0x000338A9
	public bool IsStuck { get; set; }

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x0600046A RID: 1130 RVA: 0x000356B2 File Offset: 0x000338B2
	// (set) Token: 0x0600046B RID: 1131 RVA: 0x000356BA File Offset: 0x000338BA
	public bool AgencyUpdateRequired { get; set; }

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600046C RID: 1132 RVA: 0x000356C3 File Offset: 0x000338C3
	// (set) Token: 0x0600046D RID: 1133 RVA: 0x000356CB File Offset: 0x000338CB
	public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

	// Token: 0x0600046E RID: 1134 RVA: 0x000356D4 File Offset: 0x000338D4
	public override string DebugText()
	{
		return base.DebugText() + string.Format("\nBehaviour: {0}", this.CurrentBehaviour) + string.Format("\nAttackTarget: {0}", this.AttackTarget) + string.Format("\nFoodTarget: {0}", this.FoodTarget) + string.Format("\nSleep: {0:0.00}", this.Sleep);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00035748 File Offset: 0x00033948
	public void TickAi()
	{
		if (!AI.think)
		{
			return;
		}
		if (TerrainMeta.WaterMap != null)
		{
			this.waterDepth = TerrainMeta.WaterMap.GetDepth(this.ServerPosition);
			this.wasSwimming = this.swimming;
			this.swimming = this.waterDepth > this.Stats.WaterLevelNeck * 0.25f;
		}
		else
		{
			this.wasSwimming = false;
			this.swimming = false;
			this.waterDepth = 0f;
		}
		using (TimeWarning.New("TickNavigation", 0))
		{
			this.TickNavigation();
		}
		if (!AiManager.ai_dormant || this.GetNavAgent.enabled || this.CurrentBehaviour == BaseNpc.Behaviour.Sleep || this.NewAI)
		{
			using (TimeWarning.New("TickMetabolism", 0))
			{
				this.TickSleep();
				this.TickMetabolism();
				this.TickSpeed();
			}
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00035850 File Offset: 0x00033A50
	private void TickSpeed()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		float num = this.Stats.Speed;
		if (this.NewAI)
		{
			num = (this.swimming ? this.ToSpeed(BaseNpc.SpeedEnum.Walk) : this.TargetSpeed);
			num *= 0.5f + base.healthFraction * 0.5f;
			this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
			this.NavAgent.angularSpeed = this.Stats.TurnSpeed;
			this.NavAgent.acceleration = this.Stats.Acceleration;
			return;
		}
		num *= 0.5f + base.healthFraction * 0.5f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			num *= 0.2f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Eat)
		{
			num *= 0.3f;
		}
		float num2 = Mathf.Min(this.NavAgent.speed / this.Stats.Speed, 1f);
		num2 = BaseNpc.speedFractionResponse.Evaluate(num2);
		float num3 = 1f - 0.9f * Vector3.Angle(base.transform.forward, (this.NavAgent.nextPosition - this.ServerPosition).normalized) / 180f * num2 * num2;
		num *= num3;
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
		this.NavAgent.angularSpeed = this.Stats.TurnSpeed * (1.1f - num2);
		this.NavAgent.acceleration = this.Stats.Acceleration;
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000359F8 File Offset: 0x00033BF8
	protected virtual void TickMetabolism()
	{
		float num = 0.00016666666f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			num *= 0.01f;
		}
		if (this.NavAgent.desiredVelocity.sqrMagnitude > 0.1f)
		{
			num *= 2f;
		}
		this.Energy.Add(num * 0.1f * -1f);
		if (this.Stamina.TimeSinceUsed > 5f)
		{
			float num2 = 0.06666667f;
			this.Stamina.Add(0.1f * num2);
		}
		float secondsSinceAttacked = base.SecondsSinceAttacked;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00035A8D File Offset: 0x00033C8D
	public virtual bool WantsToEat(global::BaseEntity best)
	{
		return best.HasTrait(global::BaseEntity.TraitFlag.Food) && !best.HasTrait(global::BaseEntity.TraitFlag.Alive);
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00035AA8 File Offset: 0x00033CA8
	public virtual float FearLevel(global::BaseEntity ent)
	{
		float num = 0f;
		BaseNpc baseNpc = ent as BaseNpc;
		if (baseNpc != null && baseNpc.Stats.Size > this.Stats.Size)
		{
			if (baseNpc.WantsToAttack(this) > 0.25f)
			{
				num += 0.2f;
			}
			if (baseNpc.AttackTarget == this)
			{
				num += 0.3f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Attack)
			{
				num *= 1.5f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
			{
				num *= 0.1f;
			}
		}
		if (ent as global::BasePlayer != null)
		{
			num += 1f;
		}
		return num;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float HateLevel(global::BaseEntity ent)
	{
		return 0f;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00035B48 File Offset: 0x00033D48
	protected virtual void TickSleep()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			this.IsSleeping = true;
			this.Sleep += 0.00033333336f;
		}
		else
		{
			this.IsSleeping = false;
			this.Sleep -= 2.7777778E-05f;
		}
		this.Sleep = Mathf.Clamp01(this.Sleep);
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00035BA4 File Offset: 0x00033DA4
	public void TickNavigationWater()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		position.y = 0f - this.Stats.WaterLevelNeck;
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00035C74 File Offset: 0x00033E74
	public void TickNavigation()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00035D2C File Offset: 0x00033F2C
	private void TickChase()
	{
		Vector3 vector = this.ChaseTransform.position;
		Vector3 vector2 = base.transform.position - vector;
		if ((double)vector2.magnitude < 5.0)
		{
			vector += vector2.normalized * this.AttackOffset.z;
		}
		if ((this.NavAgent.destination - vector).sqrMagnitude > 0.010000001f)
		{
			this.NavAgent.SetDestination(vector);
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00035DB5 File Offset: 0x00033FB5
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this._traversingNavMeshLink && !this.HandleNavMeshLinkTraversalStart(delta))
		{
			return;
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (!this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this._currentNavMeshLinkTraversalTimeDelta += delta;
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00035DEC File Offset: 0x00033FEC
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.NavAgent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated || currentOffMeshLinkData.offMeshLink == null)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.NavAgent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this._currentNavMeshLink = currentOffMeshLinkData;
		this._currentNavMeshLinkName = this._currentNavMeshLink.linkType.ToString();
		if (currentOffMeshLinkData.offMeshLink.biDirectional)
		{
			if ((currentOffMeshLinkData.endPos - this.ServerPosition).sqrMagnitude < 0.05f)
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.startPos + Vector3.up * (currentOffMeshLinkData.endPos.y - currentOffMeshLinkData.startPos.y) - currentOffMeshLinkData.endPos);
			}
			else
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
			}
		}
		else
		{
			this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
		}
		this._traversingNavMeshLink = true;
		this.NavAgent.ActivateCurrentOffMeshLink(false);
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		float num = Mathf.Max(this.NavAgent.speed, 2.8f);
		float magnitude = (this._currentNavMeshLink.startPos - this._currentNavMeshLink.endPos).magnitude;
		this._currentNavMeshLinkTraversalTime = magnitude / num;
		this._currentNavMeshLinkTraversalTimeDelta = 0f;
		if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
		{
			this._currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00036080 File Offset: 0x00034280
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00036164 File Offset: 0x00034364
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkTraversalTimeDelta >= this._currentNavMeshLinkTraversalTime)
		{
			moveToPosition = this._currentNavMeshLink.endPos;
			this._traversingNavMeshLink = false;
			this._currentNavMeshLink = default(OffMeshLinkData);
			this._currentNavMeshLinkTraversalTime = 0f;
			this._currentNavMeshLinkTraversalTimeDelta = 0f;
			this._currentNavMeshLinkName = string.Empty;
			this._currentNavMeshLinkOrientation = Quaternion.identity;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x000361D8 File Offset: 0x000343D8
	private void CompleteNavMeshLink()
	{
		this.NavAgent.ActivateCurrentOffMeshLink(true);
		this.NavAgent.CompleteOffMeshLink();
		this.NavAgent.isStopped = false;
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0003620C File Offset: 0x0003440C
	private void TickFollowPath(ref Vector3 moveToPosition)
	{
		moveToPosition = this.NavAgent.nextPosition;
		this.stepDirection = this.NavAgent.desiredVelocity.normalized;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00036244 File Offset: 0x00034444
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[] { "Invalid NavAgent Position: ", this, " ", moveToPosition, " (destroying)" }));
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x000362B8 File Offset: 0x000344B8
	private void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.ServerPosition = moveToPosition;
		this.UpdateAiRotation();
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x000362C7 File Offset: 0x000344C7
	private void TickIdle()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			this.idleDuration += 0.1f;
			return;
		}
		this.idleDuration = 0f;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x000362F0 File Offset: 0x000344F0
	public void TickStuck()
	{
		if (this.IsNavRunning() && !this.NavAgent.isStopped && (this.lastStuckPos - this.ServerPosition).sqrMagnitude < 0.0625f && this.AttackReady())
		{
			this.stuckDuration += 0.1f;
			if (this.stuckDuration >= 5f && Mathf.Approximately(this.lastStuckTime, 0f))
			{
				this.lastStuckTime = UnityEngine.Time.time;
				this.OnBecomeStuck();
				return;
			}
		}
		else
		{
			this.stuckDuration = 0f;
			this.lastStuckPos = this.ServerPosition;
			if (UnityEngine.Time.time - this.lastStuckTime > 5f)
			{
				this.lastStuckTime = 0f;
				this.OnBecomeUnStuck();
			}
		}
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000363B8 File Offset: 0x000345B8
	public void OnBecomeStuck()
	{
		this.IsStuck = true;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000363C1 File Offset: 0x000345C1
	public void OnBecomeUnStuck()
	{
		this.IsStuck = false;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x000363CC File Offset: 0x000345CC
	public void UpdateAiRotation()
	{
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return;
		}
		if (this._traversingNavMeshLink)
		{
			Vector3 vector;
			if (this.ChaseTransform != null)
			{
				vector = this.ChaseTransform.localPosition - this.ServerPosition;
			}
			else if (this.AttackTarget != null)
			{
				vector = this.AttackTarget.ServerPosition - this.ServerPosition;
			}
			else
			{
				vector = this.NavAgent.destination - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 1f)
			{
				vector = this._currentNavMeshLinkEndPos - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = this._currentNavMeshLinkOrientation;
				return;
			}
		}
		else if ((this.NavAgent.destination - this.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 vector2 = this.stepDirection;
			if (vector2.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector2);
				return;
			}
		}
		if (this.ChaseTransform && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector3 = this.ChaseTransform.localPosition - this.ServerPosition;
			float sqrMagnitude = vector3.sqrMagnitude;
			if (sqrMagnitude < 9f && sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector3.normalized);
				return;
			}
		}
		else if (this.AttackTarget && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector4 = this.AttackTarget.ServerPosition - this.ServerPosition;
			float sqrMagnitude2 = vector4.sqrMagnitude;
			if (sqrMagnitude2 < 9f && sqrMagnitude2 > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector4.normalized);
				return;
			}
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000486 RID: 1158 RVA: 0x00036595 File Offset: 0x00034795
	public float GetAttackRate
	{
		get
		{
			return this.AttackRate;
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0003659D File Offset: 0x0003479D
	public bool AttackReady()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextAttackTime;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x000365B0 File Offset: 0x000347B0
	public virtual void StartAttack()
	{
		if (!this.AttackTarget)
		{
			return;
		}
		if (!this.AttackReady())
		{
			return;
		}
		if ((this.AttackTarget.ServerPosition - this.ServerPosition).magnitude > this.AttackRange)
		{
			return;
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		BaseCombatEntity combatTarget = this.CombatTarget;
		if (!combatTarget)
		{
			return;
		}
		combatTarget.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		this.BusyTimer.Activate(0.5f, null);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", this.AttackTarget.ServerPosition);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00036674 File Offset: 0x00034874
	public void Attack(BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.ServerPosition - this.ServerPosition;
		if (vector.magnitude > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(vector.normalized);
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		target.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", target.ServerPosition);
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00036710 File Offset: 0x00034910
	public virtual void Eat()
	{
		if (!this.FoodTarget)
		{
			return;
		}
		this.BusyTimer.Activate(0.5f, null);
		this.FoodTarget.Eat(this, 0.5f);
		this.StartEating(UnityEngine.Random.value * 5f + 0.5f);
		base.ClientRPC<Vector3>(null, "Eat", this.FoodTarget.transform.position);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00036781 File Offset: 0x00034981
	public virtual void AddCalories(float amount)
	{
		this.Energy.Add(amount / 1000f);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00036795 File Offset: 0x00034995
	public virtual void Startled()
	{
		base.ClientRPC<Vector3>(null, "Startled", base.transform.position);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x000367AE File Offset: 0x000349AE
	private bool IsAfraid()
	{
		this.SetFact(BaseNpc.Facts.IsAfraid, 0, true, true);
		return false;
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x000367BC File Offset: 0x000349BC
	protected bool IsAfraidOf(BaseNpc.AiStatistics.FamilyEnum family)
	{
		foreach (BaseNpc.AiStatistics.FamilyEnum familyEnum in this.Stats.IsAfraidOf)
		{
			if (family == familyEnum)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x000367F0 File Offset: 0x000349F0
	private bool CheckHealthThresholdToFlee()
	{
		if (base.healthFraction > this.Stats.HealthThresholdForFleeing)
		{
			if (this.Stats.HealthThresholdForFleeing < 1f)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
			if (this.GetFact(BaseNpc.Facts.HasEnemy) == 1)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
		}
		bool flag = UnityEngine.Random.value < this.Stats.HealthThresholdFleeChance;
		this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, flag ? 1 : 0, true, true);
		return flag;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0003686C File Offset: 0x00034A6C
	private void TickBehaviourState()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 && this.IsNavRunning() && this.NavAgent.pathStatus == NavMeshPathStatus.PathComplete && UnityEngine.Time.realtimeSinceStartup - (this.maxFleeTime - this.Stats.MaxFleeTime) > 0.5f)
		{
			this.TickFlee();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			this.TickBlockEnemyTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.TickBlockFoodTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			this.TickAggro();
		}
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			this.TickEating();
		}
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			this.TickWakeUpBlockMove();
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00036910 File Offset: 0x00034B10
	private void WantsToFlee()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 || !this.IsNavRunning())
		{
			return;
		}
		this.SetFact(BaseNpc.Facts.WantsToFlee, 1, true, true);
		this.maxFleeTime = UnityEngine.Time.realtimeSinceStartup + this.Stats.MaxFleeTime;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickFlee()
	{
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00036948 File Offset: 0x00034B48
	public bool BlockEnemyTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, true, true);
		this.blockEnemyTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		this.blockTargetingThisEnemy = this.AttackTarget;
		return true;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00036979 File Offset: 0x00034B79
	private void TickBlockEnemyTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockEnemyTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0003699D File Offset: 0x00034B9D
	public bool BlockFoodTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
		this.blockFoodTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x000369C4 File Offset: 0x00034BC4
	private void TickBlockFoodTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockFoodTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x000369EC File Offset: 0x00034BEC
	public bool TryAggro(BaseNpc.EnemyRangeEnum range)
	{
		if (Mathf.Approximately(this.Stats.Hostility, 0f) && Mathf.Approximately(this.Stats.Defensiveness, 0f))
		{
			return false;
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 0 && (range == BaseNpc.EnemyRangeEnum.AggroRange || range == BaseNpc.EnemyRangeEnum.AttackRange))
		{
			float num = ((range == BaseNpc.EnemyRangeEnum.AttackRange) ? 1f : this.Stats.Defensiveness);
			num = Mathf.Max(num, this.Stats.Hostility);
			if (UnityEngine.Time.realtimeSinceStartup > this.lastAggroChanceCalcTime + 5f)
			{
				this.lastAggroChanceResult = UnityEngine.Random.value;
				this.lastAggroChanceCalcTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (this.lastAggroChanceResult < num)
			{
				return this.StartAggro(this.Stats.DeaggroChaseTime);
			}
		}
		return false;
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00036AA7 File Offset: 0x00034CA7
	public bool StartAggro(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsAggro, 1, true, true);
		this.aggroTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickAggro()
	{
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00036ACF File Offset: 0x00034CCF
	public bool StartEating(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsEating, 1, true, true);
		this.eatTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00036AF7 File Offset: 0x00034CF7
	private void TickEating()
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.eatTimeout)
		{
			this.SetFact(BaseNpc.Facts.IsEating, 0, true, true);
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00036B1C File Offset: 0x00034D1C
	public bool WakeUpBlockMove(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanNotMove, 1, true, true);
		this.wakeUpBlockMoveTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00036B44 File Offset: 0x00034D44
	private void TickWakeUpBlockMove()
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.wakeUpBlockMoveTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanNotMove, 0, true, true);
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00036B6C File Offset: 0x00034D6C
	private void OnFactChanged(BaseNpc.Facts fact, byte oldValue, byte newValue)
	{
		if (fact <= BaseNpc.Facts.IsAggro)
		{
			switch (fact)
			{
			case BaseNpc.Facts.CanTargetEnemies:
				if (newValue == 1)
				{
					this.blockTargetingThisEnemy = null;
				}
				break;
			case BaseNpc.Facts.Health:
			case BaseNpc.Facts.IsTired:
				break;
			case BaseNpc.Facts.Speed:
				if (newValue == 0)
				{
					this.StopMoving();
					this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
					return;
				}
				if (newValue != 1)
				{
					this.IsStopped = false;
					return;
				}
				this.IsStopped = false;
				this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
				return;
			case BaseNpc.Facts.IsSleeping:
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Sleep;
					this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, false, true);
					this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
					return;
				}
				this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
				this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
				this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
				this.WakeUpBlockMove(this.Stats.WakeupBlockMoveTime);
				this.TickSenses();
				return;
			default:
				if (fact != BaseNpc.Facts.IsAggro)
				{
					return;
				}
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
					return;
				}
				this.BlockEnemyTargeting(this.Stats.DeaggroCooldown);
				return;
			}
		}
		else if (fact != BaseNpc.Facts.FoodRange)
		{
			if (fact != BaseNpc.Facts.IsEating)
			{
				return;
			}
			if (newValue == 0)
			{
				this.FoodTarget = null;
				return;
			}
		}
		else if (newValue == 0)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Eat;
			return;
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00036C74 File Offset: 0x00034E74
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00036C7C File Offset: 0x00034E7C
	public bool HasAiFlag(BaseNpc.AiFlags f)
	{
		return (this.aiFlags & f) == f;
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x00036C8C File Offset: 0x00034E8C
	public void SetAiFlag(BaseNpc.AiFlags f, bool set)
	{
		BaseNpc.AiFlags aiFlags = this.aiFlags;
		if (set)
		{
			this.aiFlags |= f;
		}
		else
		{
			this.aiFlags &= ~f;
		}
		if (aiFlags != this.aiFlags && base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060004A2 RID: 1186 RVA: 0x00036CD8 File Offset: 0x00034ED8
	// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00036CE1 File Offset: 0x00034EE1
	public bool IsSitting
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060004A4 RID: 1188 RVA: 0x00036CEB File Offset: 0x00034EEB
	// (set) Token: 0x060004A5 RID: 1189 RVA: 0x00036CF4 File Offset: 0x00034EF4
	public bool IsChasing
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00036CFE File Offset: 0x00034EFE
	// (set) Token: 0x060004A7 RID: 1191 RVA: 0x00036D07 File Offset: 0x00034F07
	public bool IsSleeping
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00036D11 File Offset: 0x00034F11
	public void InitFacts()
	{
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00036D28 File Offset: 0x00034F28
	public byte GetFact(BaseNpc.Facts fact)
	{
		return this.CurrentFacts[(int)fact];
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00036D34 File Offset: 0x00034F34
	public void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte b = this.CurrentFacts[(int)fact];
		this.CurrentFacts[(int)fact] = value;
		if (triggerCallback && value != b)
		{
			this.OnFactChanged(fact, b, value);
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00036D64 File Offset: 0x00034F64
	public BaseNpc.EnemyRangeEnum ToEnemyRangeEnum(float range)
	{
		if (range <= this.AttackRange)
		{
			return BaseNpc.EnemyRangeEnum.AttackRange;
		}
		if (range <= this.Stats.AggressionRange)
		{
			return BaseNpc.EnemyRangeEnum.AggroRange;
		}
		if (range >= this.Stats.DeaggroRange && this.GetFact(BaseNpc.Facts.IsAggro) > 0)
		{
			return BaseNpc.EnemyRangeEnum.OutOfRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.EnemyRangeEnum.AwareRange;
		}
		return BaseNpc.EnemyRangeEnum.OutOfRange;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00036DB8 File Offset: 0x00034FB8
	public float GetActiveAggressionRangeSqr()
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
		}
		return this.Stats.AggressionRange * this.Stats.AggressionRange;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00036DF4 File Offset: 0x00034FF4
	public BaseNpc.FoodRangeEnum ToFoodRangeEnum(float range)
	{
		if (range <= 0.5f)
		{
			return BaseNpc.FoodRangeEnum.EatRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.FoodRangeEnum.AwareRange;
		}
		return BaseNpc.FoodRangeEnum.OutOfRange;
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00036E11 File Offset: 0x00035011
	public BaseNpc.AfraidRangeEnum ToAfraidRangeEnum(float range)
	{
		if (range <= this.Stats.AfraidRange)
		{
			return BaseNpc.AfraidRangeEnum.InAfraidRange;
		}
		return BaseNpc.AfraidRangeEnum.OutOfRange;
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00036E24 File Offset: 0x00035024
	public BaseNpc.HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return BaseNpc.HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return BaseNpc.HealthEnum.Medium;
		}
		return BaseNpc.HealthEnum.Low;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00036E3C File Offset: 0x0003503C
	public byte ToIsTired(float energyNormalized)
	{
		bool flag = this.GetFact(BaseNpc.Facts.IsSleeping) == 1;
		if (!flag && energyNormalized < 0.1f)
		{
			return 1;
		}
		if (flag && energyNormalized < 0.5f)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00036E6F File Offset: 0x0003506F
	public BaseNpc.SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return BaseNpc.SpeedEnum.StandStill;
		}
		if (speed <= 0.18f)
		{
			return BaseNpc.SpeedEnum.Walk;
		}
		return BaseNpc.SpeedEnum.Run;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00036E86 File Offset: 0x00035086
	public float ToSpeed(BaseNpc.SpeedEnum speed)
	{
		if (speed == BaseNpc.SpeedEnum.StandStill)
		{
			return 0f;
		}
		if (speed != BaseNpc.SpeedEnum.Walk)
		{
			return this.Stats.Speed;
		}
		return 0.18f * this.Stats.Speed;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00036EB4 File Offset: 0x000350B4
	public byte GetPathStatus()
	{
		if (!this.IsNavRunning())
		{
			return 2;
		}
		return (byte)this.NavAgent.pathStatus;
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00036ECC File Offset: 0x000350CC
	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00036ED0 File Offset: 0x000350D0
	private void TickSenses()
	{
		if (global::BaseEntity.Query.Server == null || this.IsDormant)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.lastTickTime + this.SensesTickRate)
		{
			this.TickHearing();
			this.TickSmell();
			this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		}
		if (!AI.animal_ignore_food)
		{
			this.TickFoodAwareness();
		}
		this.UpdateSelfFacts();
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00036F2B File Offset: 0x0003512B
	private void TickHearing()
	{
		this.SetFact(BaseNpc.Facts.LoudNoiseNearby, 0, true, true);
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickSmell()
	{
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00036F38 File Offset: 0x00035138
	private float DecisionMomentumPlayerTarget()
	{
		float num = UnityEngine.Time.time - this.playerTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00036F64 File Offset: 0x00035164
	private float DecisionMomentumAnimalTarget()
	{
		float num = UnityEngine.Time.time - this.animalTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00036F8D File Offset: 0x0003518D
	private void TickFoodAwareness()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.FoodTarget = null;
			this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
			return;
		}
		this.SelectFood();
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x000063A5 File Offset: 0x000045A5
	private void SelectFood()
	{
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x000063A5 File Offset: 0x000045A5
	private void SelectClosestFood()
	{
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateSelfFacts()
	{
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00036FB4 File Offset: 0x000351B4
	private byte IsMoving()
	{
		return (this.IsNavRunning() && this.NavAgent.hasPath && this.NavAgent.remainingDistance > this.NavAgent.stoppingDistance && !this.IsStuck && this.GetFact(BaseNpc.Facts.Speed) != 0) ? 1 : 0;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00037004 File Offset: 0x00035204
	private static bool AiCaresAbout(global::BaseEntity ent)
	{
		if (ent is global::BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (!AI.animal_ignore_food)
		{
			if (ent is global::WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			if (ent is CollectibleEntity)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x00037040 File Offset: 0x00035240
	private static bool WithinVisionCone(BaseNpc npc, global::BaseEntity other)
	{
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 normalized = (other.ServerPosition - npc.ServerPosition).normalized;
		return Vector3.Dot(npc.transform.forward, normalized) >= npc.Stats.VisionCone;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x000370A4 File Offset: 0x000352A4
	public void SetTargetPathStatus(float pendingDelay = 0.05f)
	{
		if (this.isAlreadyCheckingPathPending)
		{
			return;
		}
		if (this.NavAgent.pathPending && this.numPathPendingAttempts < 10)
		{
			this.isAlreadyCheckingPathPending = true;
			base.Invoke(new Action(this.DelayedTargetPathStatus), pendingDelay);
			return;
		}
		this.numPathPendingAttempts = 0;
		this.accumPathPendingDelay = 0f;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0003710E File Offset: 0x0003530E
	private void DelayedTargetPathStatus()
	{
		this.accumPathPendingDelay += 0.1f;
		this.isAlreadyCheckingPathPending = false;
		this.SetTargetPathStatus(this.accumPathPendingDelay);
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x00037138 File Offset: 0x00035338
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent != null)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		this.IsStuck = false;
		this.AgencyUpdateRequired = false;
		this.IsOnOffmeshLinkAndReachedNewCoord = false;
		base.InvokeRandomized(new Action(this.TickAi), 0.1f, 0.1f, 0.0050000004f);
		this.Sleep = UnityEngine.Random.Range(0.5f, 1f);
		this.Stamina.Level = UnityEngine.Random.Range(0.1f, 1f);
		this.Energy.Level = UnityEngine.Random.Range(0.5f, 1f);
		this.Hydration.Level = UnityEngine.Random.Range(0.5f, 1f);
		if (this.NewAI)
		{
			this.InitFacts();
			this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00037267 File Offset: 0x00035467
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0003726F File Offset: 0x0003546F
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00037278 File Offset: 0x00035478
	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(base.KillMessage), 0.5f);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x000372D4 File Offset: 0x000354D4
	protected virtual void OnSenseGunshot(Sensation sensation)
	{
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		this.LastHeardGunshotDirection = (sensation.Position - base.transform.localPosition).normalized;
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Flee;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00037320 File Offset: 0x00035520
	public float SecondsSinceLastHeardGunshot
	{
		get
		{
			return UnityEngine.Time.time - this._lastHeardGunshotTime;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060004CA RID: 1226 RVA: 0x0003732E File Offset: 0x0003552E
	// (set) Token: 0x060004CB RID: 1227 RVA: 0x00037336 File Offset: 0x00035536
	public Vector3 LastHeardGunshotDirection { get; set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060004CC RID: 1228 RVA: 0x0003733F File Offset: 0x0003553F
	// (set) Token: 0x060004CD RID: 1229 RVA: 0x00037347 File Offset: 0x00035547
	public float TargetSpeed { get; set; }

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060004CE RID: 1230 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060004CF RID: 1231 RVA: 0x00037350 File Offset: 0x00035550
	// (set) Token: 0x060004D0 RID: 1232 RVA: 0x00037358 File Offset: 0x00035558
	public bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			if (this._isDormant)
			{
				this.StopMoving();
				this.Pause();
				return;
			}
			if (this.GetNavAgent == null || AiManager.nav_disable)
			{
				this.IsDormant = true;
				return;
			}
			this.Resume();
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060004D1 RID: 1233 RVA: 0x000373A4 File Offset: 0x000355A4
	public float SecondsSinceLastSetDestination
	{
		get
		{
			return UnityEngine.Time.time - this.lastSetDestinationTime;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060004D2 RID: 1234 RVA: 0x000373B2 File Offset: 0x000355B2
	public float LastSetDestinationTime
	{
		get
		{
			return this.lastSetDestinationTime;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060004D3 RID: 1235 RVA: 0x000373BA File Offset: 0x000355BA
	// (set) Token: 0x060004D4 RID: 1236 RVA: 0x000373DB File Offset: 0x000355DB
	public Vector3 Destination
	{
		get
		{
			if (this.IsNavRunning())
			{
				return this.GetNavAgent.destination;
			}
			return this.Entity.ServerPosition;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.destination = value;
				this.lastSetDestinationTime = UnityEngine.Time.time;
			}
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060004D5 RID: 1237 RVA: 0x000373FC File Offset: 0x000355FC
	// (set) Token: 0x060004D6 RID: 1238 RVA: 0x00037413 File Offset: 0x00035613
	public bool IsStopped
	{
		get
		{
			return !this.IsNavRunning() || this.GetNavAgent.isStopped;
		}
		set
		{
			if (this.IsNavRunning())
			{
				if (value)
				{
					this.GetNavAgent.destination = this.ServerPosition;
				}
				this.GetNavAgent.isStopped = value;
			}
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0003743D File Offset: 0x0003563D
	// (set) Token: 0x060004D8 RID: 1240 RVA: 0x00037454 File Offset: 0x00035654
	public bool AutoBraking
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.autoBraking;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.autoBraking = value;
			}
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0003746A File Offset: 0x0003566A
	public bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.hasPath;
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00037481 File Offset: 0x00035681
	public bool IsNavRunning()
	{
		return this.GetNavAgent != null && this.GetNavAgent.enabled && this.GetNavAgent.isOnNavMesh;
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x000374AB File Offset: 0x000356AB
	public void Pause()
	{
		if (this.GetNavAgent != null && this.GetNavAgent.enabled)
		{
			this.GetNavAgent.enabled = false;
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x000374D4 File Offset: 0x000356D4
	public void Resume()
	{
		if (!this.GetNavAgent.isOnNavMesh)
		{
			base.StartCoroutine(this.TryForceToNavmesh());
			return;
		}
		this.GetNavAgent.enabled = true;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x000374FD File Offset: 0x000356FD
	private IEnumerator TryForceToNavmesh()
	{
		yield return null;
		int numTries = 0;
		float waitForRetryTime = 1f;
		float maxDistanceMultiplier = 2f;
		if (SingletonComponent<DynamicNavMesh>.Instance != null)
		{
			while (SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
			{
				yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
				waitForRetryTime += 0.5f;
			}
		}
		waitForRetryTime = 1f;
		while (numTries < 4)
		{
			if (this.GetNavAgent.isOnNavMesh)
			{
				this.GetNavAgent.enabled = true;
				yield break;
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.ServerPosition, out navMeshHit, this.GetNavAgent.height * maxDistanceMultiplier, this.GetNavAgent.areaMask))
			{
				this.ServerPosition = navMeshHit.position;
				this.GetNavAgent.Warp(this.ServerPosition);
				this.GetNavAgent.enabled = true;
				yield break;
			}
			yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
			maxDistanceMultiplier *= 1.5f;
			waitForRetryTime *= 1.5f;
			int num = numTries;
			numTries = num + 1;
		}
		Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { base.name });
		base.DieInstantly();
		yield break;
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060004DE RID: 1246 RVA: 0x0003750C File Offset: 0x0003570C
	// (set) Token: 0x060004DF RID: 1247 RVA: 0x00037514 File Offset: 0x00035714
	public global::BaseEntity AttackTarget { get; set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060004E0 RID: 1248 RVA: 0x0003751D File Offset: 0x0003571D
	// (set) Token: 0x060004E1 RID: 1249 RVA: 0x00037525 File Offset: 0x00035725
	public Memory.SeenInfo AttackTargetMemory { get; set; }

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0003752E File Offset: 0x0003572E
	// (set) Token: 0x060004E3 RID: 1251 RVA: 0x00037536 File Offset: 0x00035736
	public global::BaseEntity FoodTarget { get; set; }

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0003753F File Offset: 0x0003573F
	public BaseCombatEntity CombatTarget
	{
		get
		{
			return this.AttackTarget as BaseCombatEntity;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060004E5 RID: 1253 RVA: 0x0003754C File Offset: 0x0003574C
	// (set) Token: 0x060004E6 RID: 1254 RVA: 0x00037554 File Offset: 0x00035754
	public Vector3 SpawnPosition { get; set; }

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060004E7 RID: 1255 RVA: 0x00029EBC File Offset: 0x000280BC
	public float AttackTargetVisibleFor
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060004E8 RID: 1256 RVA: 0x00029EBC File Offset: 0x000280BC
	public float TimeAtDestination
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseCombatEntity Entity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060004EA RID: 1258 RVA: 0x00037560 File Offset: 0x00035760
	public NavMeshAgent GetNavAgent
	{
		get
		{
			if (base.isClient)
			{
				return null;
			}
			if (this.NavAgent == null)
			{
				this.NavAgent = base.GetComponent<NavMeshAgent>();
				if (this.NavAgent == null)
				{
					Debug.LogErrorFormat("{0} has no nav agent!", new object[] { base.name });
				}
			}
			return this.NavAgent;
		}
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x000375BE File Offset: 0x000357BE
	public float GetWantsToAttack(global::BaseEntity target)
	{
		return this.WantsToAttack(target);
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060004EC RID: 1260 RVA: 0x000375C7 File Offset: 0x000357C7
	public BaseNpc.AiStatistics GetStats
	{
		get
		{
			return this.Stats;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060004ED RID: 1261 RVA: 0x000375CF File Offset: 0x000357CF
	public float GetAttackRange
	{
		get
		{
			return this.AttackRange;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x060004EE RID: 1262 RVA: 0x000375D7 File Offset: 0x000357D7
	public Vector3 GetAttackOffset
	{
		get
		{
			return this.AttackOffset;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x060004EF RID: 1263 RVA: 0x000375DF File Offset: 0x000357DF
	public float GetStamina
	{
		get
		{
			return this.Stamina.Level;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060004F0 RID: 1264 RVA: 0x000375EC File Offset: 0x000357EC
	public float GetEnergy
	{
		get
		{
			return this.Energy.Level;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060004F1 RID: 1265 RVA: 0x000375F9 File Offset: 0x000357F9
	public float GetAttackCost
	{
		get
		{
			return this.AttackCost;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00037601 File Offset: 0x00035801
	public float GetSleep
	{
		get
		{
			return this.Sleep;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060004F3 RID: 1267 RVA: 0x00037609 File Offset: 0x00035809
	public Vector3 CurrentAimAngles
	{
		get
		{
			return base.transform.forward;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00037616 File Offset: 0x00035816
	public float GetStuckDuration
	{
		get
		{
			return this.stuckDuration;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060004F5 RID: 1269 RVA: 0x0003761E File Offset: 0x0003581E
	public float GetLastStuckTime
	{
		get
		{
			return this.lastStuckTime;
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00037626 File Offset: 0x00035826
	public bool BusyTimerActive()
	{
		return this.BusyTimer.IsActive;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00037633 File Offset: 0x00035833
	public void SetBusyFor(float dur)
	{
		this.BusyTimer.Activate(dur, null);
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00037642 File Offset: 0x00035842
	public Vector3 AttackPosition
	{
		get
		{
			return this.ServerPosition + base.transform.TransformDirection(this.AttackOffset);
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00037660 File Offset: 0x00035860
	public Vector3 CrouchedAttackPosition
	{
		get
		{
			return this.AttackPosition;
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00037668 File Offset: 0x00035868
	internal float WantsToAttack(global::BaseEntity target)
	{
		if (target == null)
		{
			return 0f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return 0f;
		}
		if (!target.HasAnyTrait(global::BaseEntity.TraitFlag.Animal | global::BaseEntity.TraitFlag.Human))
		{
			return 0f;
		}
		if (target.GetType() == base.GetType())
		{
			return 1f - this.Stats.Tolerance;
		}
		return 1f;
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x060004FB RID: 1275 RVA: 0x00029EBC File Offset: 0x000280BC
	public float currentBehaviorDuration
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060004FC RID: 1276 RVA: 0x000376CC File Offset: 0x000358CC
	// (set) Token: 0x060004FD RID: 1277 RVA: 0x000376D4 File Offset: 0x000358D4
	public BaseNpc.Behaviour CurrentBehaviour { get; set; }

	// Token: 0x060004FE RID: 1278 RVA: 0x000376DD File Offset: 0x000358DD
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseNPC = Facepunch.Pool.Get<BaseNPC>();
		info.msg.baseNPC.flags = (int)this.aiFlags;
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0003770C File Offset: 0x0003590C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseNPC != null)
		{
			this.aiFlags = (BaseNpc.AiFlags)info.msg.baseNPC.flags;
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00037738 File Offset: 0x00035938
	public override float MaxVelocity()
	{
		return this.Stats.Speed;
	}

	// Token: 0x04000350 RID: 848
	[NonSerialized]
	public Transform ChaseTransform;

	// Token: 0x04000351 RID: 849
	public int agentTypeIndex;

	// Token: 0x04000352 RID: 850
	public bool NewAI;

	// Token: 0x04000353 RID: 851
	public bool LegacyNavigation = true;

	// Token: 0x04000355 RID: 853
	private Vector3 stepDirection;

	// Token: 0x04000358 RID: 856
	private float maxFleeTime;

	// Token: 0x04000359 RID: 857
	private float fleeHealthThresholdPercentage = 1f;

	// Token: 0x0400035A RID: 858
	private float blockEnemyTargetingTimeout = float.NegativeInfinity;

	// Token: 0x0400035B RID: 859
	private float blockFoodTargetingTimeout = float.NegativeInfinity;

	// Token: 0x0400035C RID: 860
	private float aggroTimeout = float.NegativeInfinity;

	// Token: 0x0400035D RID: 861
	private float lastAggroChanceResult;

	// Token: 0x0400035E RID: 862
	private float lastAggroChanceCalcTime;

	// Token: 0x0400035F RID: 863
	private const float aggroChanceRecalcTimeout = 5f;

	// Token: 0x04000360 RID: 864
	private float eatTimeout = float.NegativeInfinity;

	// Token: 0x04000361 RID: 865
	private float wakeUpBlockMoveTimeout = float.NegativeInfinity;

	// Token: 0x04000362 RID: 866
	private global::BaseEntity blockTargetingThisEnemy;

	// Token: 0x04000363 RID: 867
	[NonSerialized]
	public float waterDepth;

	// Token: 0x04000364 RID: 868
	[NonSerialized]
	public bool swimming;

	// Token: 0x04000365 RID: 869
	[NonSerialized]
	public bool wasSwimming;

	// Token: 0x04000366 RID: 870
	private static readonly AnimationCurve speedFractionResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000367 RID: 871
	private bool _traversingNavMeshLink;

	// Token: 0x04000368 RID: 872
	private OffMeshLinkData _currentNavMeshLink;

	// Token: 0x04000369 RID: 873
	private string _currentNavMeshLinkName;

	// Token: 0x0400036A RID: 874
	private float _currentNavMeshLinkTraversalTime;

	// Token: 0x0400036B RID: 875
	private float _currentNavMeshLinkTraversalTimeDelta;

	// Token: 0x0400036C RID: 876
	private Quaternion _currentNavMeshLinkOrientation;

	// Token: 0x0400036D RID: 877
	private Vector3 _currentNavMeshLinkEndPos;

	// Token: 0x0400036E RID: 878
	private float nextAttackTime;

	// Token: 0x0400036F RID: 879
	[SerializeField]
	[global::InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x04000370 RID: 880
	[global::InspectorFlags]
	public BaseNpc.AiFlags aiFlags;

	// Token: 0x04000371 RID: 881
	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(BaseNpc.Facts)).Length];

	// Token: 0x04000372 RID: 882
	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	// Token: 0x04000373 RID: 883
	public float SensesTickRate = 0.5f;

	// Token: 0x04000374 RID: 884
	[NonSerialized]
	public global::BaseEntity[] SensesResults = new global::BaseEntity[64];

	// Token: 0x04000375 RID: 885
	private float lastTickTime;

	// Token: 0x04000376 RID: 886
	private float playerTargetDecisionStartTime;

	// Token: 0x04000377 RID: 887
	private float animalTargetDecisionStartTime;

	// Token: 0x04000378 RID: 888
	private bool isAlreadyCheckingPathPending;

	// Token: 0x04000379 RID: 889
	private int numPathPendingAttempts;

	// Token: 0x0400037A RID: 890
	private float accumPathPendingDelay;

	// Token: 0x0400037B RID: 891
	public const float TickRate = 0.1f;

	// Token: 0x0400037C RID: 892
	private Vector3 lastStuckPos;

	// Token: 0x0400037D RID: 893
	private float nextFlinchTime;

	// Token: 0x0400037E RID: 894
	private float _lastHeardGunshotTime = float.NegativeInfinity;

	// Token: 0x04000381 RID: 897
	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	// Token: 0x04000382 RID: 898
	public BaseNpc.AiStatistics Stats;

	// Token: 0x04000383 RID: 899
	public Vector3 AttackOffset;

	// Token: 0x04000384 RID: 900
	public float AttackDamage = 20f;

	// Token: 0x04000385 RID: 901
	public DamageType AttackDamageType = DamageType.Bite;

	// Token: 0x04000386 RID: 902
	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	// Token: 0x04000387 RID: 903
	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	// Token: 0x04000388 RID: 904
	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	// Token: 0x04000389 RID: 905
	public NavMeshAgent NavAgent;

	// Token: 0x0400038A RID: 906
	public LayerMask movementMask = 429990145;

	// Token: 0x0400038B RID: 907
	public float stuckDuration;

	// Token: 0x0400038C RID: 908
	public float lastStuckTime;

	// Token: 0x0400038D RID: 909
	public float idleDuration;

	// Token: 0x0400038E RID: 910
	private bool _isDormant;

	// Token: 0x0400038F RID: 911
	private float lastSetDestinationTime;

	// Token: 0x04000394 RID: 916
	[NonSerialized]
	public StateTimer BusyTimer;

	// Token: 0x04000395 RID: 917
	[NonSerialized]
	public float Sleep;

	// Token: 0x04000396 RID: 918
	[NonSerialized]
	public VitalLevel Stamina;

	// Token: 0x04000397 RID: 919
	[NonSerialized]
	public VitalLevel Energy;

	// Token: 0x04000398 RID: 920
	[NonSerialized]
	public VitalLevel Hydration;

	// Token: 0x02000B9D RID: 2973
	[Flags]
	public enum AiFlags
	{
		// Token: 0x04004044 RID: 16452
		Sitting = 2,
		// Token: 0x04004045 RID: 16453
		Chasing = 4,
		// Token: 0x04004046 RID: 16454
		Sleeping = 8
	}

	// Token: 0x02000B9E RID: 2974
	public enum Facts
	{
		// Token: 0x04004048 RID: 16456
		HasEnemy,
		// Token: 0x04004049 RID: 16457
		EnemyRange,
		// Token: 0x0400404A RID: 16458
		CanTargetEnemies,
		// Token: 0x0400404B RID: 16459
		Health,
		// Token: 0x0400404C RID: 16460
		Speed,
		// Token: 0x0400404D RID: 16461
		IsTired,
		// Token: 0x0400404E RID: 16462
		IsSleeping,
		// Token: 0x0400404F RID: 16463
		IsAttackReady,
		// Token: 0x04004050 RID: 16464
		IsRoamReady,
		// Token: 0x04004051 RID: 16465
		IsAggro,
		// Token: 0x04004052 RID: 16466
		WantsToFlee,
		// Token: 0x04004053 RID: 16467
		IsHungry,
		// Token: 0x04004054 RID: 16468
		FoodRange,
		// Token: 0x04004055 RID: 16469
		AttackedLately,
		// Token: 0x04004056 RID: 16470
		LoudNoiseNearby,
		// Token: 0x04004057 RID: 16471
		CanTargetFood,
		// Token: 0x04004058 RID: 16472
		IsMoving,
		// Token: 0x04004059 RID: 16473
		IsFleeing,
		// Token: 0x0400405A RID: 16474
		IsEating,
		// Token: 0x0400405B RID: 16475
		IsAfraid,
		// Token: 0x0400405C RID: 16476
		AfraidRange,
		// Token: 0x0400405D RID: 16477
		IsUnderHealthThreshold,
		// Token: 0x0400405E RID: 16478
		CanNotMove,
		// Token: 0x0400405F RID: 16479
		PathToTargetStatus
	}

	// Token: 0x02000B9F RID: 2975
	public enum EnemyRangeEnum : byte
	{
		// Token: 0x04004061 RID: 16481
		AttackRange,
		// Token: 0x04004062 RID: 16482
		AggroRange,
		// Token: 0x04004063 RID: 16483
		AwareRange,
		// Token: 0x04004064 RID: 16484
		OutOfRange
	}

	// Token: 0x02000BA0 RID: 2976
	public enum FoodRangeEnum : byte
	{
		// Token: 0x04004066 RID: 16486
		EatRange,
		// Token: 0x04004067 RID: 16487
		AwareRange,
		// Token: 0x04004068 RID: 16488
		OutOfRange
	}

	// Token: 0x02000BA1 RID: 2977
	public enum AfraidRangeEnum : byte
	{
		// Token: 0x0400406A RID: 16490
		InAfraidRange,
		// Token: 0x0400406B RID: 16491
		OutOfRange
	}

	// Token: 0x02000BA2 RID: 2978
	public enum HealthEnum : byte
	{
		// Token: 0x0400406D RID: 16493
		Fine,
		// Token: 0x0400406E RID: 16494
		Medium,
		// Token: 0x0400406F RID: 16495
		Low
	}

	// Token: 0x02000BA3 RID: 2979
	public enum SpeedEnum : byte
	{
		// Token: 0x04004071 RID: 16497
		StandStill,
		// Token: 0x04004072 RID: 16498
		Walk,
		// Token: 0x04004073 RID: 16499
		Run
	}

	// Token: 0x02000BA4 RID: 2980
	[Serializable]
	public struct AiStatistics
	{
		// Token: 0x04004074 RID: 16500
		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		[Range(0f, 1f)]
		public float Size;

		// Token: 0x04004075 RID: 16501
		[Tooltip("How fast we can move")]
		public float Speed;

		// Token: 0x04004076 RID: 16502
		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		// Token: 0x04004077 RID: 16503
		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		// Token: 0x04004078 RID: 16504
		[Tooltip("Determines things like how near we'll allow other species to get")]
		[Range(0f, 1f)]
		public float Tolerance;

		// Token: 0x04004079 RID: 16505
		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		// Token: 0x0400407A RID: 16506
		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		// Token: 0x0400407B RID: 16507
		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		// Token: 0x0400407C RID: 16508
		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		// Token: 0x0400407D RID: 16509
		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		// Token: 0x0400407E RID: 16510
		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		// Token: 0x0400407F RID: 16511
		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		// Token: 0x04004080 RID: 16512
		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		// Token: 0x04004081 RID: 16513
		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		// Token: 0x04004082 RID: 16514
		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		// Token: 0x04004083 RID: 16515
		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		// Token: 0x04004084 RID: 16516
		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		// Token: 0x04004085 RID: 16517
		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		// Token: 0x04004086 RID: 16518
		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		// Token: 0x04004087 RID: 16519
		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		// Token: 0x04004088 RID: 16520
		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public BaseNpc.AiStatistics.FamilyEnum Family;

		// Token: 0x04004089 RID: 16521
		[Tooltip("List of the types of Npc that we are afraid of.")]
		public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;

		// Token: 0x0400408A RID: 16522
		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		// Token: 0x0400408B RID: 16523
		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		// Token: 0x0400408C RID: 16524
		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		// Token: 0x0400408D RID: 16525
		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		// Token: 0x0400408E RID: 16526
		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		// Token: 0x0400408F RID: 16527
		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		// Token: 0x04004090 RID: 16528
		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		// Token: 0x04004091 RID: 16529
		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		// Token: 0x04004092 RID: 16530
		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		// Token: 0x04004093 RID: 16531
		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		// Token: 0x04004094 RID: 16532
		public float WaterLevelNeckOffset;

		// Token: 0x04004095 RID: 16533
		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		// Token: 0x04004096 RID: 16534
		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		// Token: 0x04004097 RID: 16535
		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		// Token: 0x04004098 RID: 16536
		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		// Token: 0x04004099 RID: 16537
		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;

		// Token: 0x02000FD5 RID: 4053
		public enum FamilyEnum
		{
			// Token: 0x04005156 RID: 20822
			Bear,
			// Token: 0x04005157 RID: 20823
			Wolf,
			// Token: 0x04005158 RID: 20824
			Deer,
			// Token: 0x04005159 RID: 20825
			Boar,
			// Token: 0x0400515A RID: 20826
			Chicken,
			// Token: 0x0400515B RID: 20827
			Horse,
			// Token: 0x0400515C RID: 20828
			Zombie,
			// Token: 0x0400515D RID: 20829
			Scientist,
			// Token: 0x0400515E RID: 20830
			Murderer,
			// Token: 0x0400515F RID: 20831
			Player
		}
	}

	// Token: 0x02000BA5 RID: 2981
	public enum Behaviour
	{
		// Token: 0x0400409B RID: 16539
		Idle,
		// Token: 0x0400409C RID: 16540
		Wander,
		// Token: 0x0400409D RID: 16541
		Attack,
		// Token: 0x0400409E RID: 16542
		Flee,
		// Token: 0x0400409F RID: 16543
		Eat,
		// Token: 0x040040A0 RID: 16544
		Sleep,
		// Token: 0x040040A1 RID: 16545
		RetreatingToCover
	}
}
