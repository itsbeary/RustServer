using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B51 RID: 2897
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06004618 RID: 17944 RVA: 0x0019886D File Offset: 0x00196A6D
		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if (this.MainSpawner != null)
				{
					return this.MainSpawner.Location;
				}
				return this.LocationWhenMainSpawnerIsNull;
			}
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x00198890 File Offset: 0x00196A90
		private void Awake()
		{
			AiLocationManager.Managers.Add(this);
			if (this.SnapCoverPointsToGround)
			{
				foreach (AICoverPoint aicoverPoint in this.CoverPointGroup.GetComponentsInChildren<AICoverPoint>())
				{
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(aicoverPoint.transform.position, out navMeshHit, 4f, -1))
					{
						aicoverPoint.transform.position = navMeshHit.position;
					}
				}
			}
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x001988FA File Offset: 0x00196AFA
		private void OnDestroy()
		{
			AiLocationManager.Managers.Remove(this);
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x00198908 File Offset: 0x00196B08
		public PathInterestNode GetFirstPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			foreach (PathInterestNode pathInterestNode in this.patrolPoints)
			{
				float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
				if (sqrMagnitude >= num && sqrMagnitude <= num2)
				{
					return pathInterestNode;
				}
			}
			return null;
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x001989C8 File Offset: 0x00196BC8
		public PathInterestNode GetRandomPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f, PathInterestNode currentPatrolPoint = null)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			for (int i = 0; i < 20; i++)
			{
				PathInterestNode pathInterestNode = this.patrolPoints[UnityEngine.Random.Range(0, this.patrolPoints.Count)];
				if (UnityEngine.Time.time < pathInterestNode.NextVisitTime)
				{
					if (pathInterestNode == currentPatrolPoint)
					{
						return null;
					}
				}
				else
				{
					float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2)
					{
						pathInterestNode.NextVisitTime = UnityEngine.Time.time + AI.npc_patrol_point_cooldown;
						return pathInterestNode;
					}
				}
			}
			return null;
		}

		// Token: 0x04003F0A RID: 16138
		public static List<AiLocationManager> Managers = new List<AiLocationManager>();

		// Token: 0x04003F0B RID: 16139
		[SerializeField]
		public AiLocationSpawner MainSpawner;

		// Token: 0x04003F0C RID: 16140
		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		// Token: 0x04003F0D RID: 16141
		public Transform CoverPointGroup;

		// Token: 0x04003F0E RID: 16142
		public Transform PatrolPointGroup;

		// Token: 0x04003F0F RID: 16143
		public CoverPointVolume DynamicCoverPointVolume;

		// Token: 0x04003F10 RID: 16144
		public bool SnapCoverPointsToGround;

		// Token: 0x04003F11 RID: 16145
		private List<PathInterestNode> patrolPoints;
	}
}
