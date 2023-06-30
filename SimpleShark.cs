using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class SimpleShark : BaseCombatEntity
{
	// Token: 0x1700022E RID: 558
	// (get) Token: 0x060019EB RID: 6635 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000BCA9C File Offset: 0x000BAC9C
	private void GenerateIdlePoints(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float num4 = 1f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, num4, num);
			Vector3 vector2 = Vector3Ex.Direction(vector, center);
			RaycastHit raycastHit;
			if (Physics.SphereCast(center, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, num3))
			{
				vector = center + vector2 * (raycastHit.distance - 6f);
			}
			else
			{
				vector = center + vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x000BCBC4 File Offset: 0x000BADC4
	private void GenerateIdlePoints_Shrinkwrap(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float num4 = radius * 2f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, num4, num);
			Vector3 vector2 = Vector3Ex.Direction(center, vector);
			RaycastHit raycastHit;
			if (Physics.SphereCast(vector, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, num3))
			{
				vector = raycastHit.point - vector2 * 6f;
			}
			else
			{
				vector += vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x000BCCF0 File Offset: 0x000BAEF0
	public override void ServerInit()
	{
		base.ServerInit();
		if (SimpleShark.disable)
		{
			base.Invoke(new Action(base.KillMessage), 0.01f);
			return;
		}
		base.transform.position = this.WaterClamp(base.transform.position);
		this.Init();
		base.InvokeRandomized(new Action(this.CheckSleepState), 0f, 1f, 0.5f);
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x000BCD68 File Offset: 0x000BAF68
	public void CheckSleepState()
	{
		bool flag = BaseNetworkable.HasCloseConnections(base.transform.position, 100f);
		this.sleeping = !flag;
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x000BCD98 File Offset: 0x000BAF98
	public void Init()
	{
		this.GenerateIdlePoints_Shrinkwrap(base.transform.position, 20f, 2f, 3f);
		this.states = new SimpleShark.SimpleState[2];
		this.states[0] = new SimpleShark.IdleState(this);
		this.states[1] = new SimpleShark.AttackState(this);
		base.transform.position = this.patrolPath[0];
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000BCE04 File Offset: 0x000BB004
	private void Think(float delta)
	{
		if (this.states == null)
		{
			return;
		}
		if (SimpleShark.disable)
		{
			if (!base.IsInvoking(new Action(base.KillMessage)))
			{
				base.Invoke(new Action(base.KillMessage), 0.01f);
			}
			return;
		}
		if (this.sleeping)
		{
			return;
		}
		SimpleShark.SimpleState simpleState = null;
		float num = -1f;
		foreach (SimpleShark.SimpleState simpleState2 in this.states)
		{
			float num2 = simpleState2.State_Weight();
			if (num2 > num)
			{
				simpleState = simpleState2;
				num = num2;
			}
		}
		if (simpleState != this._currentState && (this._currentState == null || this._currentState.CanInterrupt()))
		{
			if (this._currentState != null)
			{
				this._currentState.State_Exit();
			}
			simpleState.State_Enter();
			this._currentState = simpleState;
		}
		this.UpdateTarget(delta);
		this._currentState.State_Think(delta);
		this.UpdateObstacleAvoidance(delta);
		this.UpdateDirection(delta);
		this.UpdateSpeed(delta);
		this.UpdatePosition(delta);
		base.SetFlag(BaseEntity.Flags.Open, this.HasTarget() && this.CanAttack(), false, true);
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x000BCF14 File Offset: 0x000BB114
	public Vector3 WaterClamp(Vector3 point)
	{
		float height = WaterSystem.GetHeight(point);
		float num = TerrainMeta.HeightMap.GetHeight(point) + this.minFloorDist;
		float num2 = height - this.minSurfaceDist;
		if (SimpleShark.forceSurfaceAmount != 0f)
		{
			num2 = (num = WaterSystem.GetHeight(point) + SimpleShark.forceSurfaceAmount);
		}
		point.y = Mathf.Clamp(point.y, num, num2);
		return point;
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x000BCF74 File Offset: 0x000BB174
	public bool ValidTarget(BasePlayer newTarget)
	{
		float num = Vector3.Distance(newTarget.eyes.position, base.transform.position);
		Vector3 vector = Vector3Ex.Direction(newTarget.eyes.position, base.transform.position);
		int num2 = 10551552;
		if (Physics.Raycast(base.transform.position, vector, num, num2))
		{
			return false;
		}
		if (newTarget.isMounted)
		{
			if (newTarget.GetMountedVehicle())
			{
				return false;
			}
			if (!newTarget.GetMounted().GetComponent<WaterInflatable>().buoyancy.enabled)
			{
				return false;
			}
		}
		else if (!WaterLevel.Test(newTarget.CenterPoint(), true, false, newTarget))
		{
			return false;
		}
		return true;
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x000BD019 File Offset: 0x000BB219
	public void ClearTarget()
	{
		this.target = null;
		this.lastSeenTargetTime = 0f;
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000BD030 File Offset: 0x000BB230
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (base.isServer)
		{
			if (GameInfo.HasAchievements && hitInfo != null && hitInfo.InitiatorPlayer != null && !hitInfo.InitiatorPlayer.IsNpc && hitInfo.Weapon != null && hitInfo.Weapon.ShortPrefabName.Contains("speargun"))
			{
				hitInfo.InitiatorPlayer.stats.Add("shark_speargun_kills", 1, Stats.All);
				hitInfo.InitiatorPlayer.stats.Save(true);
			}
			BaseCorpse baseCorpse = base.DropCorpse(this.corpsePrefab.resourcePath);
			if (baseCorpse)
			{
				baseCorpse.Spawn();
				baseCorpse.TakeChildren(this);
			}
			base.Invoke(new Action(base.KillMessage), 0.5f);
		}
		base.OnKilled(hitInfo);
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000BD100 File Offset: 0x000BB300
	public void UpdateTarget(float delta)
	{
		if (this.target != null)
		{
			bool flag = Vector3.Distance(this.target.eyes.position, base.transform.position) > this.aggroRange * 2f;
			bool flag2 = Time.realtimeSinceStartup > this.lastSeenTargetTime + 4f;
			if (!this.ValidTarget(this.target) || flag || flag2)
			{
				this.ClearTarget();
			}
			else
			{
				this.lastSeenTargetTime = Time.realtimeSinceStartup;
			}
		}
		if (Time.realtimeSinceStartup < this.nextTargetSearchTime)
		{
			return;
		}
		if (this.target == null)
		{
			this.nextTargetSearchTime = Time.realtimeSinceStartup + 1f;
			if (BaseNetworkable.HasCloseConnections(base.transform.position, this.aggroRange))
			{
				int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(base.transform.position, this.aggroRange, SimpleShark.playerQueryResults, null);
				for (int i = 0; i < playersInSphere; i++)
				{
					BasePlayer basePlayer = SimpleShark.playerQueryResults[i];
					if (!basePlayer.isClient && this.ValidTarget(basePlayer))
					{
						this.target = basePlayer;
						this.lastSeenTargetTime = Time.realtimeSinceStartup;
						return;
					}
				}
			}
		}
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x000BD22C File Offset: 0x000BB42C
	public float TimeSinceAttacked()
	{
		return Time.realtimeSinceStartup - this.lastTimeAttacked;
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x000BD23C File Offset: 0x000BB43C
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.lastTimeAttacked = Time.realtimeSinceStartup;
		if (info.damageTypes.Total() > 20f)
		{
			this.Startle();
		}
		if (info.InitiatorPlayer != null && this.target == null && this.ValidTarget(info.InitiatorPlayer))
		{
			this.target = info.InitiatorPlayer;
			this.lastSeenTargetTime = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000BD2B4 File Offset: 0x000BB4B4
	public bool HasTarget()
	{
		return this.target != null;
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x000BD2C2 File Offset: 0x000BB4C2
	public BasePlayer GetTarget()
	{
		return this.target;
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000BD2CA File Offset: 0x000BB4CA
	public override string Categorize()
	{
		return "Shark";
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000BD2D1 File Offset: 0x000BB4D1
	public bool CanAttack()
	{
		return Time.realtimeSinceStartup > this.nextAttackTime;
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000BD2E0 File Offset: 0x000BB4E0
	public void DoAttack()
	{
		if (!this.HasTarget())
		{
			return;
		}
		this.GetTarget().Hurt(UnityEngine.Random.Range(30f, 70f), DamageType.Bite, this, true);
		Vector3 vector = this.WaterClamp(this.GetTarget().CenterPoint());
		Effect.server.Run(this.bloodCloud.resourcePath, vector, Vector3.forward, null, false);
		this.nextAttackTime = Time.realtimeSinceStartup + this.attackCooldown;
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x000BD350 File Offset: 0x000BB550
	public void Startle()
	{
		this.lastStartleTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x000BD35D File Offset: 0x000BB55D
	public bool IsStartled()
	{
		return this.lastStartleTime + this.startleDuration > Time.realtimeSinceStartup;
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000BD373 File Offset: 0x000BB573
	private float GetDesiredSpeed()
	{
		if (!this.IsStartled())
		{
			return this.minSpeed;
		}
		return this.maxSpeed;
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x000BD38A File Offset: 0x000BB58A
	public float GetTurnSpeed()
	{
		if (this.IsStartled())
		{
			return this.maxTurnSpeed;
		}
		if (this.obstacleAvoidanceScale != 0f)
		{
			return Mathf.Lerp(this.minTurnSpeed, this.maxTurnSpeed, this.obstacleAvoidanceScale);
		}
		return this.minTurnSpeed;
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000BD3C6 File Offset: 0x000BB5C6
	private float GetCurrentSpeed()
	{
		return this.currentSpeed;
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000BD3D0 File Offset: 0x000BB5D0
	private void UpdateObstacleAvoidance(float delta)
	{
		this.timeSinceLastObstacleCheck += delta;
		if (this.timeSinceLastObstacleCheck < 0.5f)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		Vector3 position = base.transform.position;
		int num = 1503764737;
		RaycastHit raycastHit;
		if (Physics.SphereCast(position, this.obstacleDetectionRadius, forward, out raycastHit, this.obstacleDetectionRange, num))
		{
			Vector3 point = raycastHit.point;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(position + Vector3.down * 0.25f + base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit2, this.obstacleDetectionRange, num))
			{
				vector = raycastHit2.point;
			}
			RaycastHit raycastHit3;
			if (Physics.SphereCast(position + Vector3.down * 0.25f - base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit3, this.obstacleDetectionRange, num))
			{
				vector2 = raycastHit3.point;
			}
			if (vector != Vector3.zero && vector2 != Vector3.zero)
			{
				Plane plane = new Plane(point, vector, vector2);
				Vector3 normal = plane.normal;
				if (normal != Vector3.zero)
				{
					raycastHit.normal = normal;
				}
			}
			this.cachedObstacleNormal = raycastHit.normal;
			this.cachedObstacleDistance = raycastHit.distance;
			this.obstacleAvoidanceScale = 1f - Mathf.InverseLerp(2f, this.obstacleDetectionRange * 0.75f, raycastHit.distance);
		}
		else
		{
			this.obstacleAvoidanceScale = Mathf.MoveTowards(this.obstacleAvoidanceScale, 0f, this.timeSinceLastObstacleCheck * 2f);
			if (this.obstacleAvoidanceScale == 0f)
			{
				this.cachedObstacleDistance = 0f;
			}
		}
		this.timeSinceLastObstacleCheck = 0f;
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x000BD5BC File Offset: 0x000BB7BC
	private void UpdateDirection(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = Vector3Ex.Direction(this.WaterClamp(this.destination), base.transform.position);
		if (this.obstacleAvoidanceScale != 0f)
		{
			Vector3 vector3;
			if (this.cachedObstacleNormal != Vector3.zero)
			{
				Vector3 vector2 = QuaternionEx.LookRotationForcedUp(this.cachedObstacleNormal, Vector3.up) * Vector3.forward;
				if (Vector3.Dot(vector2, base.transform.right) > Vector3.Dot(vector2, -base.transform.right))
				{
					vector3 = base.transform.right;
				}
				else
				{
					vector3 = -base.transform.right;
				}
			}
			else
			{
				vector3 = base.transform.right;
			}
			vector = vector3 * this.obstacleAvoidanceScale;
			vector.Normalize();
		}
		if (vector != Vector3.zero)
		{
			Quaternion quaternion = Quaternion.LookRotation(vector, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, delta * this.GetTurnSpeed());
		}
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x000BD6D8 File Offset: 0x000BB8D8
	private void UpdatePosition(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = base.transform.position + forward * this.GetCurrentSpeed() * delta;
		vector = this.WaterClamp(vector);
		base.transform.position = vector;
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000BD728 File Offset: 0x000BB928
	private void UpdateSpeed(float delta)
	{
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.GetDesiredSpeed(), delta * 4f);
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000BD748 File Offset: 0x000BB948
	public void Update()
	{
		if (base.isServer)
		{
			this.Think(Time.deltaTime);
		}
	}

	// Token: 0x04001276 RID: 4726
	public Vector3 destination;

	// Token: 0x04001277 RID: 4727
	public float minSpeed;

	// Token: 0x04001278 RID: 4728
	public float maxSpeed;

	// Token: 0x04001279 RID: 4729
	public float idealDepth;

	// Token: 0x0400127A RID: 4730
	public float minTurnSpeed = 0.25f;

	// Token: 0x0400127B RID: 4731
	public float maxTurnSpeed = 2f;

	// Token: 0x0400127C RID: 4732
	public float attackCooldown = 7f;

	// Token: 0x0400127D RID: 4733
	public float aggroRange = 15f;

	// Token: 0x0400127E RID: 4734
	public float obstacleDetectionRadius = 1f;

	// Token: 0x0400127F RID: 4735
	public Animator animator;

	// Token: 0x04001280 RID: 4736
	public GameObjectRef bloodCloud;

	// Token: 0x04001281 RID: 4737
	public GameObjectRef corpsePrefab;

	// Token: 0x04001282 RID: 4738
	private const string SPEARGUN_KILL_STAT = "shark_speargun_kills";

	// Token: 0x04001283 RID: 4739
	[ServerVar]
	public static float forceSurfaceAmount = 0f;

	// Token: 0x04001284 RID: 4740
	[ServerVar]
	public static bool disable = false;

	// Token: 0x04001285 RID: 4741
	private Vector3 spawnPos;

	// Token: 0x04001286 RID: 4742
	private float stoppingDistance = 3f;

	// Token: 0x04001287 RID: 4743
	private float currentSpeed;

	// Token: 0x04001288 RID: 4744
	private float lastStartleTime;

	// Token: 0x04001289 RID: 4745
	private float startleDuration = 1f;

	// Token: 0x0400128A RID: 4746
	private SimpleShark.SimpleState[] states;

	// Token: 0x0400128B RID: 4747
	private SimpleShark.SimpleState _currentState;

	// Token: 0x0400128C RID: 4748
	private bool sleeping;

	// Token: 0x0400128D RID: 4749
	public List<Vector3> patrolPath = new List<Vector3>();

	// Token: 0x0400128E RID: 4750
	private BasePlayer target;

	// Token: 0x0400128F RID: 4751
	private float lastSeenTargetTime;

	// Token: 0x04001290 RID: 4752
	private float nextTargetSearchTime;

	// Token: 0x04001291 RID: 4753
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x04001292 RID: 4754
	private float minFloorDist = 2f;

	// Token: 0x04001293 RID: 4755
	private float minSurfaceDist = 1f;

	// Token: 0x04001294 RID: 4756
	private float lastTimeAttacked;

	// Token: 0x04001295 RID: 4757
	private float nextAttackTime;

	// Token: 0x04001296 RID: 4758
	private Vector3 cachedObstacleNormal;

	// Token: 0x04001297 RID: 4759
	private float cachedObstacleDistance;

	// Token: 0x04001298 RID: 4760
	private float obstacleAvoidanceScale;

	// Token: 0x04001299 RID: 4761
	private float obstacleDetectionRange = 5f;

	// Token: 0x0400129A RID: 4762
	private float timeSinceLastObstacleCheck;

	// Token: 0x02000C5A RID: 3162
	public class SimpleState
	{
		// Token: 0x06004EA6 RID: 20134 RVA: 0x001A36D3 File Offset: 0x001A18D3
		public SimpleState(SimpleShark owner)
		{
			this.entity = owner;
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x00029EBC File Offset: 0x000280BC
		public virtual float State_Weight()
		{
			return 0f;
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x001A36E2 File Offset: 0x001A18E2
		public virtual void State_Enter()
		{
			this.stateEnterTime = Time.realtimeSinceStartup;
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void State_Think(float delta)
		{
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void State_Exit()
		{
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x001A36EF File Offset: 0x001A18EF
		public virtual float TimeInState()
		{
			return Time.realtimeSinceStartup - this.stateEnterTime;
		}

		// Token: 0x04004367 RID: 17255
		public SimpleShark entity;

		// Token: 0x04004368 RID: 17256
		private float stateEnterTime;
	}

	// Token: 0x02000C5B RID: 3163
	public class IdleState : SimpleShark.SimpleState
	{
		// Token: 0x06004EAD RID: 20141 RVA: 0x001A36FD File Offset: 0x001A18FD
		public IdleState(SimpleShark owner)
			: base(owner)
		{
		}

		// Token: 0x06004EAE RID: 20142 RVA: 0x001A3706 File Offset: 0x001A1906
		public Vector3 GetTargetPatrolPosition()
		{
			return this.entity.patrolPath[this.patrolTargetIndex];
		}

		// Token: 0x06004EAF RID: 20143 RVA: 0x00006CA5 File Offset: 0x00004EA5
		public override float State_Weight()
		{
			return 1f;
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x001A3720 File Offset: 0x001A1920
		public override void State_Enter()
		{
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < this.entity.patrolPath.Count; i++)
			{
				float num3 = Vector3.Distance(this.entity.patrolPath[i], this.entity.transform.position);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			this.patrolTargetIndex = num2;
			base.State_Enter();
		}

		// Token: 0x06004EB1 RID: 20145 RVA: 0x001A378C File Offset: 0x001A198C
		public override void State_Think(float delta)
		{
			if (Vector3.Distance(this.GetTargetPatrolPosition(), this.entity.transform.position) < this.entity.stoppingDistance)
			{
				this.patrolTargetIndex++;
				if (this.patrolTargetIndex >= this.entity.patrolPath.Count)
				{
					this.patrolTargetIndex = 0;
				}
			}
			if (this.entity.TimeSinceAttacked() >= 120f && this.entity.healthFraction < 1f)
			{
				this.entity.health = this.entity.MaxHealth();
			}
			this.entity.destination = this.entity.WaterClamp(this.GetTargetPatrolPosition());
		}

		// Token: 0x06004EB2 RID: 20146 RVA: 0x001A3844 File Offset: 0x001A1A44
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004EB3 RID: 20147 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x04004369 RID: 17257
		private int patrolTargetIndex;
	}

	// Token: 0x02000C5C RID: 3164
	public class AttackState : SimpleShark.SimpleState
	{
		// Token: 0x06004EB4 RID: 20148 RVA: 0x001A36FD File Offset: 0x001A18FD
		public AttackState(SimpleShark owner)
			: base(owner)
		{
		}

		// Token: 0x06004EB5 RID: 20149 RVA: 0x001A384C File Offset: 0x001A1A4C
		public override float State_Weight()
		{
			if (!this.entity.HasTarget() || !this.entity.CanAttack())
			{
				return 0f;
			}
			return 10f;
		}

		// Token: 0x06004EB6 RID: 20150 RVA: 0x001A3873 File Offset: 0x001A1A73
		public override void State_Enter()
		{
			base.State_Enter();
		}

		// Token: 0x06004EB7 RID: 20151 RVA: 0x001A387C File Offset: 0x001A1A7C
		public override void State_Think(float delta)
		{
			BasePlayer target = this.entity.GetTarget();
			if (target == null)
			{
				return;
			}
			if (this.TimeInState() >= 10f)
			{
				this.entity.nextAttackTime = Time.realtimeSinceStartup + 4f;
				this.entity.Startle();
				return;
			}
			if (this.entity.CanAttack())
			{
				this.entity.Startle();
			}
			float num = Vector3.Distance(this.entity.GetTarget().eyes.position, this.entity.transform.position);
			bool flag = num < 4f;
			if (this.entity.CanAttack() && num <= 2f)
			{
				this.entity.DoAttack();
			}
			if (!flag)
			{
				Vector3 vector = Vector3Ex.Direction(this.entity.GetTarget().eyes.position, this.entity.transform.position);
				Vector3 vector2 = target.eyes.position + vector * 10f;
				vector2 = this.entity.WaterClamp(vector2);
				this.entity.destination = vector2;
			}
		}

		// Token: 0x06004EB8 RID: 20152 RVA: 0x001A3844 File Offset: 0x001A1A44
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004EB9 RID: 20153 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}
	}
}
