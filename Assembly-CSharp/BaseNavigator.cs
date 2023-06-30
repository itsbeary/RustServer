using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000204 RID: 516
public class BaseNavigator : BaseMonoBehaviour
{
	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001AFC RID: 6908 RVA: 0x000C039B File Offset: 0x000BE59B
	// (set) Token: 0x06001AFD RID: 6909 RVA: 0x000C03A3 File Offset: 0x000BE5A3
	public AIMovePointPath Path { get; set; }

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06001AFE RID: 6910 RVA: 0x000C03AC File Offset: 0x000BE5AC
	// (set) Token: 0x06001AFF RID: 6911 RVA: 0x000C03B4 File Offset: 0x000BE5B4
	public BasePath AStarGraph { get; set; }

	// Token: 0x06001B00 RID: 6912 RVA: 0x000C03BD File Offset: 0x000BE5BD
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x000C03C5 File Offset: 0x000BE5C5
	public int TopologyPrevent()
	{
		return (int)this.topologyPrevent;
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x000C03CD File Offset: 0x000BE5CD
	public int BiomeRequirement()
	{
		return (int)this.biomeRequirement;
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001B03 RID: 6915 RVA: 0x000C03D5 File Offset: 0x000BE5D5
	// (set) Token: 0x06001B04 RID: 6916 RVA: 0x000C03DD File Offset: 0x000BE5DD
	public NavMeshAgent Agent { get; private set; }

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06001B05 RID: 6917 RVA: 0x000C03E6 File Offset: 0x000BE5E6
	// (set) Token: 0x06001B06 RID: 6918 RVA: 0x000C03EE File Offset: 0x000BE5EE
	public BaseCombatEntity BaseEntity { get; private set; }

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06001B07 RID: 6919 RVA: 0x000C03F7 File Offset: 0x000BE5F7
	// (set) Token: 0x06001B08 RID: 6920 RVA: 0x000C03FF File Offset: 0x000BE5FF
	public Vector3 Destination { get; protected set; }

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06001B09 RID: 6921 RVA: 0x000C0408 File Offset: 0x000BE608
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.Agent.enabled && this.Agent.isOnOffMeshLink;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06001B0A RID: 6922 RVA: 0x000C0424 File Offset: 0x000BE624
	public bool Moving
	{
		get
		{
			return this.CurrentNavigationType > BaseNavigator.NavigationType.None;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001B0B RID: 6923 RVA: 0x000C042F File Offset: 0x000BE62F
	// (set) Token: 0x06001B0C RID: 6924 RVA: 0x000C0437 File Offset: 0x000BE637
	public BaseNavigator.NavigationType CurrentNavigationType { get; private set; }

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06001B0D RID: 6925 RVA: 0x000C0440 File Offset: 0x000BE640
	// (set) Token: 0x06001B0E RID: 6926 RVA: 0x000C0448 File Offset: 0x000BE648
	public BaseNavigator.NavigationType LastUsedNavigationType { get; private set; }

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001B0F RID: 6927 RVA: 0x000C0451 File Offset: 0x000BE651
	// (set) Token: 0x06001B10 RID: 6928 RVA: 0x000C0459 File Offset: 0x000BE659
	[HideInInspector]
	public bool StuckOffNavmesh { get; private set; }

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001B11 RID: 6929 RVA: 0x000C0462 File Offset: 0x000BE662
	public virtual bool HasPath
	{
		get
		{
			return !(this.Agent == null) && ((this.Agent.enabled && this.Agent.hasPath) || this.currentAStarPath != null);
		}
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x000C049C File Offset: 0x000BE69C
	public virtual void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		this.defaultAreaMask = 1 << NavMesh.GetAreaFromName(this.DefaultArea);
		this.BaseEntity = entity;
		this.Agent = agent;
		if (this.Agent != null)
		{
			this.Agent.acceleration = this.Acceleration;
			this.Agent.angularSpeed = this.TurnSpeed;
		}
		this.navMeshQueryFilter = default(NavMeshQueryFilter);
		this.navMeshQueryFilter.agentTypeID = this.Agent.agentTypeID;
		this.navMeshQueryFilter.areaMask = this.defaultAreaMask;
		this.path = new NavMeshPath();
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x000C0544 File Offset: 0x000BE744
	public void SetNavMeshEnabled(bool flag)
	{
		if (this.Agent == null)
		{
			return;
		}
		if (this.Agent.enabled == flag)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			this.Agent.enabled = false;
			return;
		}
		if (this.Agent.enabled)
		{
			if (flag)
			{
				if (this.Agent.isOnNavMesh)
				{
					this.Agent.isStopped = false;
				}
			}
			else if (this.Agent.isOnNavMesh)
			{
				this.Agent.isStopped = true;
			}
		}
		this.Agent.enabled = flag;
		if (flag)
		{
			if (!this.CanEnableNavMeshNavigation())
			{
				return;
			}
			this.PlaceOnNavMesh();
		}
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x000C05E6 File Offset: 0x000BE7E6
	protected virtual bool CanEnableNavMeshNavigation()
	{
		return this.CanUseNavMesh;
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x000C05F3 File Offset: 0x000BE7F3
	protected virtual bool CanUpdateMovement()
	{
		return !(this.BaseEntity != null) || this.BaseEntity.IsAlive();
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x000C0613 File Offset: 0x000BE813
	public void ForceToGround()
	{
		base.CancelInvoke(new Action(this.DelayedForceToGround));
		base.Invoke(new Action(this.DelayedForceToGround), 0.5f);
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x000C0640 File Offset: 0x000BE840
	private void DelayedForceToGround()
	{
		int num = 10551296;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(base.transform.position + Vector3.up * 0.5f, Vector3.down, out raycastHit, 1000f, num))
		{
			this.BaseEntity.ServerPosition = raycastHit.point;
		}
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x000C0698 File Offset: 0x000BE898
	public bool PlaceOnNavMesh()
	{
		if (this.Agent.isOnNavMesh)
		{
			return true;
		}
		bool flag = false;
		float num = (this.IsSwimming() ? 30f : 6f);
		Vector3 vector;
		if (this.GetNearestNavmeshPosition(base.transform.position + Vector3.one * 2f, out vector, num))
		{
			flag = this.Warp(vector);
			if (flag)
			{
				this.OnPlacedOnNavmesh();
			}
		}
		if (!flag)
		{
			this.StuckOffNavmesh = true;
			this.OnFailedToPlaceOnNavmesh();
		}
		return flag;
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPlacedOnNavmesh()
	{
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnFailedToPlaceOnNavmesh()
	{
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x000C0718 File Offset: 0x000BE918
	private bool Warp(Vector3 position)
	{
		this.Agent.Warp(position);
		this.Agent.enabled = true;
		base.transform.position = position;
		if (!this.Agent.isOnNavMesh)
		{
			Debug.LogWarning("Agent still not on navmesh after a warp. No navmesh areas matching agent type? Agent type: " + this.Agent.agentTypeID, base.gameObject);
			this.StuckOffNavmesh = true;
			return false;
		}
		this.StuckOffNavmesh = false;
		return true;
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x000C0790 File Offset: 0x000BE990
	public bool GetNearestNavmeshPosition(Vector3 target, out Vector3 position, float maxRange)
	{
		position = base.transform.position;
		bool flag = true;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(target, out navMeshHit, maxRange, this.defaultAreaMask))
		{
			position = navMeshHit.position;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x000C07D3 File Offset: 0x000BE9D3
	public bool SetBaseDestination(Vector3 pos, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.Destination = pos;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Base);
		return true;
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x000C0810 File Offset: 0x000BEA10
	public bool SetDestination(BasePath path, IAIPathNode newTargetNode, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		if (!this.CanUseAStar)
		{
			return false;
		}
		if (newTargetNode == this.targetNode && this.HasPath)
		{
			return true;
		}
		if (this.ReachedPosition(newTargetNode.Position))
		{
			return true;
		}
		IAIPathNode closestToPoint = path.GetClosestToPoint(base.transform.position);
		if (closestToPoint == null || !closestToPoint.IsValid())
		{
			return false;
		}
		float num;
		if (AStarPath.FindPath(closestToPoint, newTargetNode, out this.currentAStarPath, out num))
		{
			this.currentSpeedFraction = speedFraction;
			this.targetNode = newTargetNode;
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.AStar);
			this.Destination = newTargetNode.Position;
			return true;
		}
		return false;
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x000C08B7 File Offset: 0x000BEAB7
	public bool SetDestination(Vector3 pos, BaseNavigator.NavigationSpeed speed, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		return this.SetDestination(pos, this.GetSpeedFraction(speed), updateInterval, navmeshSampleDistance);
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x000C08CA File Offset: 0x000BEACA
	protected virtual bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (!this.CanUseCustomNav)
		{
			return false;
		}
		this.paused = false;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.currentSpeedFraction = speedFraction;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Custom);
		return true;
	}

	// Token: 0x06001B21 RID: 6945 RVA: 0x000C090C File Offset: 0x000BEB0C
	public bool SetDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (updateInterval > 0f && !this.UpdateIntervalElapsed(updateInterval))
		{
			return true;
		}
		this.lastSetDestinationTime = UnityEngine.Time.time;
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		BaseNavigator.NavigationType navigationType = BaseNavigator.NavigationType.NavMesh;
		if (this.CanUseBaseNav && this.CanUseNavMesh)
		{
			Vector3 vector;
			BaseNavigator.NavigationType navigationType2 = this.DetermineNavigationType(base.transform.position, out vector);
			Vector3 vector2;
			BaseNavigator.NavigationType navigationType3 = this.DetermineNavigationType(pos, out vector2);
			if (navigationType3 == BaseNavigator.NavigationType.NavMesh && navigationType2 == BaseNavigator.NavigationType.NavMesh && (this.CurrentNavigationType == BaseNavigator.NavigationType.None || this.CurrentNavigationType == BaseNavigator.NavigationType.Base))
			{
				this.Warp(vector);
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base && navigationType2 != BaseNavigator.NavigationType.Base)
			{
				BasePet basePet = this.BaseEntity as BasePet;
				if (basePet != null)
				{
					BasePlayer basePlayer = basePet.Brain.Events.Memory.Entity.Get(5) as BasePlayer;
					if (basePlayer != null)
					{
						BuildingPrivlidge buildingPrivilege = basePlayer.GetBuildingPrivilege(new OBB(pos, base.transform.rotation, this.BaseEntity.bounds));
						if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(basePlayer) && buildingPrivilege.AnyAuthed())
						{
							return false;
						}
					}
				}
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base)
			{
				if (navigationType2 != BaseNavigator.NavigationType.Base)
				{
					if (Vector3.Distance(this.BaseEntity.ServerPosition, pos) <= 10f && Mathf.Abs(this.BaseEntity.ServerPosition.y - pos.y) <= 3f)
					{
						navigationType = BaseNavigator.NavigationType.Base;
					}
					else
					{
						navigationType = BaseNavigator.NavigationType.NavMesh;
					}
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
			}
			else if (navigationType3 == BaseNavigator.NavigationType.NavMesh)
			{
				if (navigationType2 != BaseNavigator.NavigationType.NavMesh)
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.NavMesh;
				}
			}
		}
		else
		{
			navigationType = (this.CanUseNavMesh ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.AStar);
		}
		if (navigationType == BaseNavigator.NavigationType.Base)
		{
			return this.SetBaseDestination(pos, speedFraction);
		}
		if (navigationType == BaseNavigator.NavigationType.AStar)
		{
			if (this.AStarGraph != null)
			{
				return this.SetDestination(this.AStarGraph, this.AStarGraph.GetClosestToPoint(pos), speedFraction);
			}
			return this.CanUseCustomNav && this.SetCustomDestination(pos, speedFraction, updateInterval);
		}
		else
		{
			if (AiManager.nav_disable)
			{
				return false;
			}
			if (navmeshSampleDistance > 0f && AI.setdestinationsamplenavmesh)
			{
				NavMeshHit navMeshHit;
				if (!NavMesh.SamplePosition(pos, out navMeshHit, navmeshSampleDistance, this.defaultAreaMask))
				{
					return false;
				}
				pos = navMeshHit.position;
			}
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.NavMesh);
			if (!this.Agent.isOnNavMesh)
			{
				return false;
			}
			if (!this.Agent.isActiveAndEnabled)
			{
				return false;
			}
			this.Destination = pos;
			bool flag;
			if (AI.usecalculatepath)
			{
				flag = NavMesh.CalculatePath(base.transform.position, this.Destination, this.navMeshQueryFilter, this.path);
				if (flag)
				{
					this.Agent.SetPath(this.path);
				}
				else if (AI.usesetdestinationfallback)
				{
					flag = this.Agent.SetDestination(this.Destination);
				}
			}
			else
			{
				flag = this.Agent.SetDestination(this.Destination);
			}
			if (flag && this.SpeedBasedAvoidancePriority)
			{
				this.Agent.avoidancePriority = UnityEngine.Random.Range(0, 21) + Mathf.FloorToInt(speedFraction * 80f);
			}
			return flag;
		}
	}

	// Token: 0x06001B22 RID: 6946 RVA: 0x000C0C08 File Offset: 0x000BEE08
	private BaseNavigator.NavigationType DetermineNavigationType(Vector3 location, out Vector3 navMeshPos)
	{
		navMeshPos = location;
		int num = 2097152;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(location + Vector3.up * BaseNavigator.navTypeHeightOffset, Vector3.down, out raycastHit, BaseNavigator.navTypeDistance, num))
		{
			return BaseNavigator.NavigationType.Base;
		}
		Vector3 vector;
		BaseNavigator.NavigationType navigationType = (this.GetNearestNavmeshPosition(location + Vector3.up * BaseNavigator.navTypeHeightOffset, out vector, BaseNavigator.navTypeDistance) ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.Base);
		navMeshPos = vector;
		return navigationType;
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x000C0C7C File Offset: 0x000BEE7C
	public void SetCurrentSpeed(BaseNavigator.NavigationSpeed speed)
	{
		this.currentSpeedFraction = this.GetSpeedFraction(speed);
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x000C0C8B File Offset: 0x000BEE8B
	public bool UpdateIntervalElapsed(float updateInterval)
	{
		return updateInterval <= 0f || UnityEngine.Time.time - this.lastSetDestinationTime >= updateInterval;
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x000C0CA9 File Offset: 0x000BEEA9
	public float GetSpeedFraction(BaseNavigator.NavigationSpeed speed)
	{
		switch (speed)
		{
		case BaseNavigator.NavigationSpeed.Slowest:
			return this.SlowestSpeedFraction;
		case BaseNavigator.NavigationSpeed.Slow:
			return this.SlowSpeedFraction;
		case BaseNavigator.NavigationSpeed.Normal:
			return this.NormalSpeedFraction;
		case BaseNavigator.NavigationSpeed.Fast:
			return this.FastSpeedFraction;
		default:
			return 1f;
		}
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x000C0CE4 File Offset: 0x000BEEE4
	protected void SetCurrentNavigationType(BaseNavigator.NavigationType navType)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			this.stuckCheckPosition = base.transform.position;
			this.stuckTimer = 0f;
		}
		this.CurrentNavigationType = navType;
		if (this.CurrentNavigationType != BaseNavigator.NavigationType.None)
		{
			this.LastUsedNavigationType = this.CurrentNavigationType;
		}
		if (navType == BaseNavigator.NavigationType.None)
		{
			this.stuckTimer = 0f;
			return;
		}
		if (navType != BaseNavigator.NavigationType.NavMesh)
		{
			return;
		}
		this.SetNavMeshEnabled(true);
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x000C0D4B File Offset: 0x000BEF4B
	public void Pause()
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			return;
		}
		this.Stop();
		this.paused = true;
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x000C0D63 File Offset: 0x000BEF63
	public void Resume()
	{
		if (!this.paused)
		{
			return;
		}
		this.SetDestination(this.Destination, this.currentSpeedFraction, 0f, 0f);
		this.paused = false;
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x000C0D94 File Offset: 0x000BEF94
	public void Stop()
	{
		switch (this.CurrentNavigationType)
		{
		case BaseNavigator.NavigationType.NavMesh:
			this.StopNavMesh();
			break;
		case BaseNavigator.NavigationType.AStar:
			this.StopAStar();
			break;
		case BaseNavigator.NavigationType.Custom:
			this.StopCustom();
			break;
		}
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
		this.paused = false;
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x000C0DE2 File Offset: 0x000BEFE2
	private void StopNavMesh()
	{
		this.SetNavMeshEnabled(false);
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x000C0DEB File Offset: 0x000BEFEB
	private void StopAStar()
	{
		this.currentAStarPath = null;
		this.targetNode = null;
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void StopCustom()
	{
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x000C0DFB File Offset: 0x000BEFFB
	public void Think(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!AI.navthink)
		{
			return;
		}
		if (this.BaseEntity == null)
		{
			return;
		}
		this.UpdateNavigation(delta);
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x000C0E23 File Offset: 0x000BF023
	public void UpdateNavigation(float delta)
	{
		this.UpdateMovement(delta);
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x000C0E2C File Offset: 0x000BF02C
	private void UpdateMovement(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!this.CanUpdateMovement())
		{
			return;
		}
		Vector3 vector = base.transform.position;
		if (this.TriggerStuckEvent)
		{
			this.stuckTimer += delta;
			if (this.CurrentNavigationType != BaseNavigator.NavigationType.None && this.stuckTimer >= BaseNavigator.stuckTriggerDuration)
			{
				if (Vector3.Distance(base.transform.position, this.stuckCheckPosition) <= this.StuckDistance)
				{
					this.OnStuck();
				}
				this.stuckTimer = 0f;
				this.stuckCheckPosition = base.transform.position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			vector = this.Destination;
		}
		else if (this.IsOnNavMeshLink)
		{
			this.HandleNavMeshLinkTraversal(delta, ref vector);
		}
		else if (this.HasPath)
		{
			vector = this.GetNextPathPosition();
		}
		else if (this.CurrentNavigationType == BaseNavigator.NavigationType.Custom)
		{
			vector = this.Destination;
		}
		if (!this.ValidateNextPosition(ref vector))
		{
			return;
		}
		bool flag = this.IsSwimming();
		this.UpdateSpeed(delta, flag);
		this.UpdatePositionAndRotation(vector, delta);
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x000C0F2C File Offset: 0x000BF12C
	public virtual void OnStuck()
	{
		BasePet basePet = this.BaseEntity as BasePet;
		if (basePet != null && basePet.Brain != null)
		{
			basePet.Brain.LoadDefaultAIDesign();
		}
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsSwimming()
	{
		return false;
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x000C0F68 File Offset: 0x000BF168
	private Vector3 GetNextPathPosition()
	{
		if (this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			return this.currentAStarPath.Peek().Position;
		}
		return this.Agent.nextPosition;
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000C0F9C File Offset: 0x000BF19C
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		bool flag = ValidBounds.Test(moveToPosition);
		if (this.BaseEntity != null && !flag && base.transform != null && !this.BaseEntity.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			this.BaseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x000C102C File Offset: 0x000BF22C
	private void UpdateSpeed(float delta, bool swimming)
	{
		float num = this.GetTargetSpeed();
		if (this.LowHealthSpeedReductionTriggerFraction > 0f && this.BaseEntity.healthFraction <= this.LowHealthSpeedReductionTriggerFraction)
		{
			num = Mathf.Min(num, this.Speed * this.LowHealthMaxSpeedFraction);
		}
		this.Agent.speed = num * (swimming ? this.SwimmingSpeedMultiplier : 1f);
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x000C1091 File Offset: 0x000BF291
	protected virtual float GetTargetSpeed()
	{
		return this.Speed * this.currentSpeedFraction;
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x000C10A0 File Offset: 0x000BF2A0
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.AStar && this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.Agent.speed * delta);
			this.BaseEntity.ServerPosition = base.transform.localPosition;
			if (this.ReachedPosition(moveToPosition))
			{
				this.currentAStarPath.Pop();
				if (this.currentAStarPath.Count == 0)
				{
					this.Stop();
					return;
				}
				moveToPosition = this.currentAStarPath.Peek().Position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
		{
			if (this.ReachedPosition(this.Agent.destination))
			{
				this.Stop();
			}
			if (this.BaseEntity != null)
			{
				this.BaseEntity.ServerPosition = moveToPosition;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			this.frameCount++;
			this.accumDelta += delta;
			if (this.frameCount < BaseNavigator.baseNavMovementFrameInterval)
			{
				return;
			}
			this.frameCount = 0;
			delta = this.accumDelta;
			this.accumDelta = 0f;
			int num = 10551552;
			Vector3 vector = Vector3Ex.Direction2D(this.Destination, this.BaseEntity.ServerPosition);
			Vector3 vector2 = this.BaseEntity.ServerPosition + vector * delta * this.Agent.speed;
			Vector3 vector3 = this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance;
			Vector3 vector4 = Vector3Ex.Direction(vector2 + Vector3.up * BaseNavigator.maxStepUpDistance, this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance);
			float num2 = Vector3.Distance(vector3, vector2 + Vector3.up * BaseNavigator.maxStepUpDistance) + 0.25f;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(vector3, vector4, out raycastHit, num2, num))
			{
				return;
			}
			if (!UnityEngine.Physics.SphereCast(vector2 + Vector3.up * (BaseNavigator.maxStepUpDistance + 0.3f), 0.25f, Vector3.down, out raycastHit, 10f, num))
			{
				return;
			}
			Vector3 point = raycastHit.point;
			if (point.y - this.BaseEntity.ServerPosition.y > BaseNavigator.maxStepUpDistance)
			{
				return;
			}
			this.BaseEntity.ServerPosition = point;
			if (this.ReachedPosition(moveToPosition))
			{
				this.Stop();
			}
		}
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			this.ApplyFacingDirectionOverride();
		}
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ApplyFacingDirectionOverride()
	{
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x000C1334 File Offset: 0x000BF534
	public void SetFacingDirectionEntity(BaseEntity entity)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Entity;
		this.facingDirectionEntity = entity;
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x000C1344 File Offset: 0x000BF544
	public void SetFacingDirectionOverride(Vector3 direction)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Direction;
		this.overrideFacingDirection = true;
		this.facingDirectionOverride = direction;
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x000C135B File Offset: 0x000BF55B
	public void ClearFacingDirectionOverride()
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.None;
		this.overrideFacingDirection = false;
		this.facingDirectionEntity = null;
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06001B3B RID: 6971 RVA: 0x000C1372 File Offset: 0x000BF572
	public bool IsOverridingFacingDirection
	{
		get
		{
			return this.overrideFacingDirectionMode > BaseNavigator.OverrideFacingDirectionMode.None;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001B3C RID: 6972 RVA: 0x000C137D File Offset: 0x000BF57D
	public Vector3 FacingDirectionOverride
	{
		get
		{
			return this.facingDirectionOverride;
		}
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x000C1385 File Offset: 0x000BF585
	protected bool ReachedPosition(Vector3 position)
	{
		return Vector3.Distance(position, base.transform.position) <= this.StoppingDistance;
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x000C13A3 File Offset: 0x000BF5A3
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this.traversingNavMeshLink)
		{
			this.HandleNavMeshLinkTraversalStart(delta);
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this.CompleteNavMeshLink();
		}
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x000C13D0 File Offset: 0x000BF5D0
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.Agent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.Agent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this.currentNavMeshLinkName = currentOffMeshLinkData.linkType.ToString();
		Vector3 vector = ((this.BaseEntity != null) ? this.BaseEntity.ServerPosition : base.transform.position);
		if ((vector - currentOffMeshLinkData.startPos).sqrMagnitude > (vector - currentOffMeshLinkData.endPos).sqrMagnitude)
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
		}
		else
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
		}
		this.traversingNavMeshLink = true;
		this.Agent.ActivateCurrentOffMeshLink(false);
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		if (!(this.currentNavMeshLinkName == "OpenDoorLink") && !(this.currentNavMeshLinkName == "JumpRockLink"))
		{
			this.currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x000C1538 File Offset: 0x000BF738
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this.currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x000C1610 File Offset: 0x000BF810
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if ((moveToPosition - this.currentNavMeshLinkEndPos).sqrMagnitude < 0.01f)
		{
			moveToPosition = this.currentNavMeshLinkEndPos;
			this.traversingNavMeshLink = false;
			this.currentNavMeshLinkName = string.Empty;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x000C1664 File Offset: 0x000BF864
	private void CompleteNavMeshLink()
	{
		this.Agent.ActivateCurrentOffMeshLink(true);
		this.Agent.CompleteOffMeshLink();
		this.Agent.isStopped = false;
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x000C1698 File Offset: 0x000BF898
	public bool IsPositionATopologyPreference(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPreference() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x000C16CC File Offset: 0x000BF8CC
	public bool IsPositionPreventTopology(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPrevent() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x000C1700 File Offset: 0x000BF900
	public bool IsPositionABiomePreference(Vector3 position)
	{
		if (!this.UseBiomePreference)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int num = (int)this.biomePreference;
			if ((TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1) & num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x000C1740 File Offset: 0x000BF940
	public bool IsPositionABiomeRequirement(Vector3 position)
	{
		if (this.biomeRequirement == (TerrainBiome.Enum)0)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1);
			if ((this.BiomeRequirement() & biomeMaxType) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x000C177E File Offset: 0x000BF97E
	public bool IsAcceptableWaterDepth(Vector3 pos)
	{
		return WaterLevel.GetOverallWaterDepth(pos, true, false, null, false) <= this.MaxWaterDepth;
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x000C1795 File Offset: 0x000BF995
	public void SetBrakingEnabled(bool flag)
	{
		this.Agent.autoBraking = flag;
	}

	// Token: 0x040012F3 RID: 4851
	[ServerVar(Help = "The max step-up height difference for pet base navigation")]
	public static float maxStepUpDistance = 1.7f;

	// Token: 0x040012F4 RID: 4852
	[ServerVar(Help = "How many frames between base navigation movement updates")]
	public static int baseNavMovementFrameInterval = 2;

	// Token: 0x040012F5 RID: 4853
	[ServerVar(Help = "How long we are not moving for before trigger the stuck event")]
	public static float stuckTriggerDuration = 10f;

	// Token: 0x040012F6 RID: 4854
	[ServerVar]
	public static float navTypeHeightOffset = 0.5f;

	// Token: 0x040012F7 RID: 4855
	[ServerVar]
	public static float navTypeDistance = 1f;

	// Token: 0x040012F8 RID: 4856
	[Header("General")]
	public bool CanNavigateMounted;

	// Token: 0x040012F9 RID: 4857
	public bool CanUseNavMesh = true;

	// Token: 0x040012FA RID: 4858
	public bool CanUseAStar = true;

	// Token: 0x040012FB RID: 4859
	public bool CanUseBaseNav;

	// Token: 0x040012FC RID: 4860
	public bool CanUseCustomNav;

	// Token: 0x040012FD RID: 4861
	public float StoppingDistance = 0.5f;

	// Token: 0x040012FE RID: 4862
	public string DefaultArea = "Walkable";

	// Token: 0x040012FF RID: 4863
	[Header("Stuck Detection")]
	public bool TriggerStuckEvent;

	// Token: 0x04001300 RID: 4864
	public float StuckDistance = 1f;

	// Token: 0x04001301 RID: 4865
	[Header("Speed")]
	public float Speed = 5f;

	// Token: 0x04001302 RID: 4866
	public float Acceleration = 5f;

	// Token: 0x04001303 RID: 4867
	public float TurnSpeed = 10f;

	// Token: 0x04001304 RID: 4868
	public BaseNavigator.NavigationSpeed MoveTowardsSpeed = BaseNavigator.NavigationSpeed.Normal;

	// Token: 0x04001305 RID: 4869
	public bool FaceMoveTowardsTarget;

	// Token: 0x04001306 RID: 4870
	[Header("Speed Fractions")]
	public float SlowestSpeedFraction = 0.16f;

	// Token: 0x04001307 RID: 4871
	public float SlowSpeedFraction = 0.3f;

	// Token: 0x04001308 RID: 4872
	public float NormalSpeedFraction = 0.5f;

	// Token: 0x04001309 RID: 4873
	public float FastSpeedFraction = 1f;

	// Token: 0x0400130A RID: 4874
	public float LowHealthSpeedReductionTriggerFraction;

	// Token: 0x0400130B RID: 4875
	public float LowHealthMaxSpeedFraction = 0.5f;

	// Token: 0x0400130C RID: 4876
	public float SwimmingSpeedMultiplier = 0.25f;

	// Token: 0x0400130D RID: 4877
	[Header("AIPoint Usage")]
	public float BestMovementPointMaxDistance = 10f;

	// Token: 0x0400130E RID: 4878
	public float BestCoverPointMaxDistance = 20f;

	// Token: 0x0400130F RID: 4879
	public float BestRoamPointMaxDistance = 20f;

	// Token: 0x04001310 RID: 4880
	public float MaxRoamDistanceFromHome = -1f;

	// Token: 0x04001311 RID: 4881
	[Header("Misc")]
	public float MaxWaterDepth = 0.75f;

	// Token: 0x04001312 RID: 4882
	public bool SpeedBasedAvoidancePriority;

	// Token: 0x04001313 RID: 4883
	private NavMeshPath path;

	// Token: 0x04001314 RID: 4884
	private NavMeshQueryFilter navMeshQueryFilter;

	// Token: 0x04001317 RID: 4887
	private int defaultAreaMask;

	// Token: 0x04001318 RID: 4888
	[InspectorFlags]
	public TerrainBiome.Enum biomePreference = (TerrainBiome.Enum)12;

	// Token: 0x04001319 RID: 4889
	public bool UseBiomePreference;

	// Token: 0x0400131A RID: 4890
	[InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x0400131B RID: 4891
	[InspectorFlags]
	public TerrainTopology.Enum topologyPrevent;

	// Token: 0x0400131C RID: 4892
	[InspectorFlags]
	public TerrainBiome.Enum biomeRequirement;

	// Token: 0x04001322 RID: 4898
	private float stuckTimer;

	// Token: 0x04001323 RID: 4899
	private Vector3 stuckCheckPosition;

	// Token: 0x04001325 RID: 4901
	protected bool traversingNavMeshLink;

	// Token: 0x04001326 RID: 4902
	protected string currentNavMeshLinkName;

	// Token: 0x04001327 RID: 4903
	protected Vector3 currentNavMeshLinkEndPos;

	// Token: 0x04001328 RID: 4904
	protected Stack<IAIPathNode> currentAStarPath;

	// Token: 0x04001329 RID: 4905
	protected IAIPathNode targetNode;

	// Token: 0x0400132A RID: 4906
	protected float currentSpeedFraction = 1f;

	// Token: 0x0400132B RID: 4907
	private float lastSetDestinationTime;

	// Token: 0x0400132C RID: 4908
	protected BaseNavigator.OverrideFacingDirectionMode overrideFacingDirectionMode;

	// Token: 0x0400132D RID: 4909
	protected BaseEntity facingDirectionEntity;

	// Token: 0x0400132E RID: 4910
	protected bool overrideFacingDirection;

	// Token: 0x0400132F RID: 4911
	protected Vector3 facingDirectionOverride;

	// Token: 0x04001330 RID: 4912
	protected bool paused;

	// Token: 0x04001331 RID: 4913
	private int frameCount;

	// Token: 0x04001332 RID: 4914
	private float accumDelta;

	// Token: 0x02000C73 RID: 3187
	public enum NavigationType
	{
		// Token: 0x0400438F RID: 17295
		None,
		// Token: 0x04004390 RID: 17296
		NavMesh,
		// Token: 0x04004391 RID: 17297
		AStar,
		// Token: 0x04004392 RID: 17298
		Custom,
		// Token: 0x04004393 RID: 17299
		Base
	}

	// Token: 0x02000C74 RID: 3188
	public enum NavigationSpeed
	{
		// Token: 0x04004395 RID: 17301
		Slowest,
		// Token: 0x04004396 RID: 17302
		Slow,
		// Token: 0x04004397 RID: 17303
		Normal,
		// Token: 0x04004398 RID: 17304
		Fast
	}

	// Token: 0x02000C75 RID: 3189
	protected enum OverrideFacingDirectionMode
	{
		// Token: 0x0400439A RID: 17306
		None,
		// Token: 0x0400439B RID: 17307
		Direction,
		// Token: 0x0400439C RID: 17308
		Entity
	}
}
