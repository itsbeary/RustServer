using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001E0 RID: 480
public class AIInformationZone : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x06001978 RID: 6520 RVA: 0x000BAC9C File Offset: 0x000B8E9C
	public static AIInformationZone Merge(List<AIInformationZone> zones, GameObject newRoot)
	{
		if (zones == null)
		{
			return null;
		}
		AIInformationZone aiinformationZone = newRoot.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		foreach (AIInformationZone aiinformationZone2 in zones)
		{
			if (!(aiinformationZone2 == null))
			{
				foreach (AIMovePoint aimovePoint in aiinformationZone2.movePoints)
				{
					aiinformationZone.AddMovePoint(aimovePoint);
					aimovePoint.transform.SetParent(newRoot.transform);
				}
				foreach (AICoverPoint aicoverPoint in aiinformationZone2.coverPoints)
				{
					aiinformationZone.AddCoverPoint(aicoverPoint);
					aicoverPoint.transform.SetParent(newRoot.transform);
				}
			}
		}
		aiinformationZone.bounds = AIInformationZone.EncapsulateBounds(zones);
		AIInformationZone aiinformationZone3 = aiinformationZone;
		aiinformationZone3.bounds.extents = aiinformationZone3.bounds.extents + new Vector3(5f, 0f, 5f);
		AIInformationZone aiinformationZone4 = aiinformationZone;
		aiinformationZone4.bounds.center = aiinformationZone4.bounds.center - aiinformationZone.transform.position;
		for (int i = zones.Count - 1; i >= 0; i--)
		{
			AIInformationZone aiinformationZone5 = zones[i];
			if (!(aiinformationZone5 == null))
			{
				UnityEngine.Object.Destroy(aiinformationZone5);
			}
		}
		return aiinformationZone;
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x000BAE40 File Offset: 0x000B9040
	public static Bounds EncapsulateBounds(List<AIInformationZone> zones)
	{
		Bounds bounds = default(Bounds);
		bounds.center = zones[0].transform.position;
		foreach (AIInformationZone aiinformationZone in zones)
		{
			if (!(aiinformationZone == null))
			{
				Vector3 vector = aiinformationZone.bounds.center + aiinformationZone.transform.position;
				Bounds bounds2 = aiinformationZone.bounds;
				bounds2.center = vector;
				bounds.Encapsulate(bounds2);
			}
		}
		return bounds;
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x0600197A RID: 6522 RVA: 0x000BAEE8 File Offset: 0x000B90E8
	// (set) Token: 0x0600197B RID: 6523 RVA: 0x000BAEF0 File Offset: 0x000B90F0
	public bool Sleeping { get; private set; }

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600197C RID: 6524 RVA: 0x000BAEF9 File Offset: 0x000B90F9
	public int SleepingCount
	{
		get
		{
			if (!this.Sleeping)
			{
				return 0;
			}
			return this.sleepables.Count;
		}
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x000BAF10 File Offset: 0x000B9110
	public void Start()
	{
		this.Init();
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x000BAF18 File Offset: 0x000B9118
	public void Init()
	{
		if (this.initd)
		{
			return;
		}
		this.initd = true;
		this.AddInitialPoints();
		this.areaBox = new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
		AIInformationZone.zones.Add(this);
		this.grid = base.GetComponent<AIInformationGrid>();
		if (this.grid != null)
		{
			this.grid.Init();
		}
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x000BAF9D File Offset: 0x000B919D
	public void RegisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		if (!sleepable.AllowedToSleep())
		{
			return;
		}
		if (this.sleepables.Contains(sleepable))
		{
			return;
		}
		this.sleepables.Add(sleepable);
		if (this.Sleeping && sleepable.AllowedToSleep())
		{
			sleepable.SleepAI();
		}
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x000BAFDD File Offset: 0x000B91DD
	public void UnregisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		this.sleepables.Remove(sleepable);
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x000BAFF0 File Offset: 0x000B91F0
	public void SleepAI()
	{
		if (!AI.sleepwake)
		{
			return;
		}
		if (!this.ShouldSleepAI)
		{
			return;
		}
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.SleepAI();
			}
		}
		this.Sleeping = true;
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x000BB060 File Offset: 0x000B9260
	public void WakeAI()
	{
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.WakeAI();
			}
		}
		this.Sleeping = false;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000BB0BC File Offset: 0x000B92BC
	private void AddCoverPoint(AICoverPoint point)
	{
		if (this.coverPoints.Contains(point))
		{
			return;
		}
		this.coverPoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x000BB0E0 File Offset: 0x000B92E0
	private void RemoveCoverPoint(AICoverPoint point, bool markDirty = true)
	{
		this.coverPoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x000BB0F9 File Offset: 0x000B92F9
	private void AddMovePoint(AIMovePoint point)
	{
		if (this.movePoints.Contains(point))
		{
			return;
		}
		this.movePoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x000BB11D File Offset: 0x000B931D
	private void RemoveMovePoint(AIMovePoint point, bool markDirty = true)
	{
		this.movePoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000BB138 File Offset: 0x000B9338
	public void MarkDirty(bool completeRefresh = false)
	{
		this.isDirty = true;
		this.processIndex = 0;
		this.halfPaths = 0;
		this.pathSuccesses = 0;
		this.pathFails = 0;
		if (completeRefresh)
		{
			Debug.Log("AIInformationZone performing complete refresh, please wait...");
			foreach (AIMovePoint aimovePoint in this.movePoints)
			{
				aimovePoint.distances.Clear();
				aimovePoint.distancesToCover.Clear();
			}
		}
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x000BB1C8 File Offset: 0x000B93C8
	private bool PassesBudget(float startTime, float budgetSeconds)
	{
		return UnityEngine.Time.realtimeSinceStartup - startTime <= budgetSeconds;
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x0000441C File Offset: 0x0000261C
	public bool ProcessDistancesAttempt()
	{
		return true;
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x000BB1D8 File Offset: 0x000B93D8
	private bool ProcessDistances()
	{
		if (!this.UseCalculatedCoverDistances)
		{
			return true;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = AIThinkManager.framebudgetms / 1000f * 0.25f;
		if (realtimeSinceStartup < AIInformationZone.lastNavmeshBuildTime + 60f)
		{
			num = 0.1f;
		}
		int num2 = 1 << NavMesh.GetAreaFromName("HumanNPC");
		NavMeshPath navMeshPath = new NavMeshPath();
		while (this.PassesBudget(realtimeSinceStartup, num))
		{
			AIMovePoint aimovePoint = this.movePoints[this.processIndex];
			bool flag = true;
			int num3 = 0;
			for (int i = aimovePoint.distances.Keys.Count - 1; i >= 0; i--)
			{
				AIMovePoint aimovePoint2 = aimovePoint.distances.Keys[i];
				if (!this.movePoints.Contains(aimovePoint2))
				{
					aimovePoint.distances.Remove(aimovePoint2);
				}
			}
			for (int j = aimovePoint.distancesToCover.Keys.Count - 1; j >= 0; j--)
			{
				AICoverPoint aicoverPoint = aimovePoint.distancesToCover.Keys[j];
				if (!this.coverPoints.Contains(aicoverPoint))
				{
					num3++;
					aimovePoint.distancesToCover.Remove(aicoverPoint);
				}
			}
			foreach (AICoverPoint aicoverPoint2 in this.coverPoints)
			{
				if (!(aicoverPoint2 == null) && !aimovePoint.distancesToCover.Contains(aicoverPoint2))
				{
					float num4;
					if (Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position) > 40f)
					{
						num4 = -2f;
					}
					else if (NavMesh.CalculatePath(aimovePoint.transform.position, aicoverPoint2.transform.position, num2, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
					{
						int num5 = navMeshPath.corners.Length;
						if (num5 > 1)
						{
							Vector3 vector = navMeshPath.corners[0];
							float num6 = 0f;
							for (int k = 0; k < num5; k++)
							{
								Vector3 vector2 = navMeshPath.corners[k];
								num6 += Vector3.Distance(vector, vector2);
								vector = vector2;
							}
							num4 = num6;
							this.pathSuccesses++;
						}
						else
						{
							num4 = Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position);
							this.halfPaths++;
						}
					}
					else
					{
						this.pathFails++;
						num4 = -2f;
					}
					aimovePoint.distancesToCover.Add(aicoverPoint2, num4);
					if (!this.PassesBudget(realtimeSinceStartup, num))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.processIndex++;
			}
			if (this.processIndex >= this.movePoints.Count - 1)
			{
				break;
			}
		}
		return this.processIndex >= this.movePoints.Count - 1;
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000BB4F8 File Offset: 0x000B96F8
	public static void BudgetedTick()
	{
		if (!AI.move)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < AIInformationZone.buildTimeTest)
		{
			return;
		}
		bool flag = false;
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (aiinformationZone.isDirty)
			{
				flag = true;
				bool flag2 = aiinformationZone.isDirty;
				aiinformationZone.isDirty = !aiinformationZone.ProcessDistancesAttempt();
				break;
			}
		}
		if (Global.developer > 0)
		{
			if (flag && !AIInformationZone.lastFrameAnyDirty)
			{
				Debug.Log("AIInformationZones rebuilding...");
				AIInformationZone.rebuildStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (AIInformationZone.lastFrameAnyDirty && !flag)
			{
				Debug.Log("AIInformationZone rebuild complete! Duration : " + (UnityEngine.Time.realtimeSinceStartup - AIInformationZone.rebuildStartTime) + " seconds.");
			}
		}
		AIInformationZone.lastFrameAnyDirty = flag;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x000BB5D8 File Offset: 0x000B97D8
	public void NavmeshBuildingComplete()
	{
		AIInformationZone.lastNavmeshBuildTime = UnityEngine.Time.realtimeSinceStartup;
		AIInformationZone.buildTimeTest = UnityEngine.Time.realtimeSinceStartup + 15f;
		this.MarkDirty(true);
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x000BB5FB File Offset: 0x000B97FB
	public Vector3 ClosestPointTo(Vector3 target)
	{
		return this.areaBox.ClosestPoint(target);
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmos()
	{
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000BB609 File Offset: 0x000B9809
	public void OnDrawGizmosSelected()
	{
		this.DrawBounds();
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000BB614 File Offset: 0x000B9814
	private void DrawBounds()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x000BB66C File Offset: 0x000B986C
	public void AddInitialPoints()
	{
		foreach (AICoverPoint aicoverPoint in base.transform.GetComponentsInChildren<AICoverPoint>())
		{
			this.AddCoverPoint(aicoverPoint);
		}
		foreach (AIMovePoint aimovePoint in base.transform.GetComponentsInChildren<AIMovePoint>(true))
		{
			this.AddMovePoint(aimovePoint);
		}
		this.RefreshPointArrays();
		NavMeshLink[] componentsInChildren3 = base.transform.GetComponentsInChildren<NavMeshLink>(true);
		this.navMeshLinks.AddRange(componentsInChildren3);
		AIMovePointPath[] componentsInChildren4 = base.transform.GetComponentsInChildren<AIMovePointPath>();
		this.paths.AddRange(componentsInChildren4);
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x000BB702 File Offset: 0x000B9902
	private void RefreshPointArrays()
	{
		List<AIMovePoint> list = this.movePoints;
		this.movePointArray = ((list != null) ? list.ToArray() : null);
		List<AICoverPoint> list2 = this.coverPoints;
		this.coverPointArray = ((list2 != null) ? list2.ToArray() : null);
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x000BB734 File Offset: 0x000B9934
	public void AddDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints, Func<Vector3, bool> validatePoint = null)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aimovePoint.transform.position))))
				{
					this.AddMovePoint(aimovePoint);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aicoverPoint.transform.position))))
				{
					this.AddCoverPoint(aicoverPoint);
				}
			}
		}
		this.RefreshPointArrays();
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x000BB7CC File Offset: 0x000B99CC
	public void RemoveDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null))
				{
					this.RemoveMovePoint(aimovePoint, false);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null))
				{
					this.RemoveCoverPoint(aicoverPoint, false);
				}
			}
		}
		this.MarkDirty(false);
		this.RefreshPointArrays();
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x000BB83C File Offset: 0x000B9A3C
	public AIMovePointPath GetNearestPath(Vector3 position)
	{
		if (this.paths == null || this.paths.Count == 0)
		{
			return null;
		}
		float num = float.MaxValue;
		AIMovePointPath aimovePointPath = null;
		foreach (AIMovePointPath aimovePointPath2 in this.paths)
		{
			foreach (AIMovePoint aimovePoint in aimovePointPath2.Points)
			{
				float num2 = Vector3.SqrMagnitude(aimovePoint.transform.position - position);
				if (num2 < num)
				{
					num = num2;
					aimovePointPath = aimovePointPath2;
				}
			}
		}
		return aimovePointPath;
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x000BB904 File Offset: 0x000B9B04
	public static AIInformationZone GetForPoint(Vector3 point, bool fallBackToNearest = true)
	{
		if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (!(aiinformationZone == null) && !aiinformationZone.Virtual && aiinformationZone.areaBox.Contains(point))
			{
				return aiinformationZone;
			}
		}
		if (!fallBackToNearest)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		AIInformationZone aiinformationZone2 = AIInformationZone.zones[0];
		foreach (AIInformationZone aiinformationZone3 in AIInformationZone.zones)
		{
			if (!(aiinformationZone3 == null) && !(aiinformationZone3.transform == null) && !aiinformationZone3.Virtual)
			{
				float num2 = Vector3.Distance(aiinformationZone3.transform.position, point);
				if (num2 < num)
				{
					num = num2;
					aiinformationZone2 = aiinformationZone3;
				}
			}
		}
		if (aiinformationZone2.Virtual)
		{
			aiinformationZone2 = null;
		}
		return aiinformationZone2;
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x000BBA2C File Offset: 0x000B9C2C
	public bool PointInside(Vector3 point)
	{
		return this.areaBox.Contains(point);
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x000BBA3C File Offset: 0x000B9C3C
	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null, bool returnClosest = false)
	{
		AIPoint aipoint = null;
		AIPoint aipoint2 = null;
		float num = -1f;
		float num2 = float.PositiveInfinity;
		int num3;
		AIPoint[] movePointsInRange = this.GetMovePointsInRange(targetPosition, maxRange, out num3);
		if (movePointsInRange == null || num3 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num3; i++)
		{
			AIPoint aipoint3 = movePointsInRange[i];
			if (aipoint3.transform.parent.gameObject.activeSelf && (fromPosition.y < WaterSystem.OceanLevel || aipoint3.transform.position.y >= WaterSystem.OceanLevel))
			{
				float num4 = 0f;
				Vector3 position = aipoint3.transform.position;
				float num5 = Vector3.Distance(targetPosition, position);
				if (num5 < num2)
				{
					aipoint2 = aipoint3;
					num2 = num5;
				}
				if (num5 <= maxRange)
				{
					num4 += (aipoint3.CanBeUsedBy(forObject) ? 100f : 0f);
					num4 += (1f - Mathf.InverseLerp(minRange, maxRange, num5)) * 100f;
					if (num4 >= num && (!checkLOS || !UnityEngine.Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218519297, QueryTriggerInteraction.Ignore)) && num4 > num)
					{
						aipoint = aipoint3;
						num = num4;
					}
				}
			}
		}
		if (aipoint == null && returnClosest)
		{
			return aipoint2 as AIMovePoint;
		}
		return aipoint as AIMovePoint;
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x000BBBA8 File Offset: 0x000B9DA8
	public AIPoint[] GetMovePointsInRange(Vector3 currentPos, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AIMovePoint[] movePointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			movePointsInRange = this.grid.GetMovePointsInRange(currentPos, maxRange, out pointCount);
		}
		else
		{
			movePointsInRange = this.movePointArray;
			if (movePointsInRange != null)
			{
				pointCount = movePointsInRange.Length;
			}
		}
		return movePointsInRange;
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x000BBBF0 File Offset: 0x000B9DF0
	private AIMovePoint GetClosestRaw(Vector3 pos, bool onlyIncludeWithCover = false)
	{
		AIMovePoint aimovePoint = null;
		float num = float.PositiveInfinity;
		foreach (AIMovePoint aimovePoint2 in this.movePoints)
		{
			if (!onlyIncludeWithCover || aimovePoint2.distancesToCover.Count != 0)
			{
				float num2 = Vector3.Distance(aimovePoint2.transform.position, pos);
				if (num2 < num)
				{
					num = num2;
					aimovePoint = aimovePoint2;
				}
			}
		}
		return aimovePoint;
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x000BBC74 File Offset: 0x000B9E74
	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null, bool allowObjectToReuse = true)
	{
		AICoverPoint aicoverPoint = null;
		float num = 0f;
		AIMovePoint closestRaw = this.GetClosestRaw(currentPosition, true);
		int num2;
		AICoverPoint[] coverPointsInRange = this.GetCoverPointsInRange(currentPosition, maxRange, out num2);
		if (coverPointsInRange == null || num2 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num2; i++)
		{
			AICoverPoint aicoverPoint2 = coverPointsInRange[i];
			Vector3 position = aicoverPoint2.transform.position;
			Vector3 normalized = (hideFromPosition - position).normalized;
			float num3 = Vector3.Dot(aicoverPoint2.transform.forward, normalized);
			if (num3 >= 1f - aicoverPoint2.coverDot)
			{
				float num4;
				if (this.UseCalculatedCoverDistances && closestRaw != null && closestRaw.distancesToCover.Contains(aicoverPoint2) && !this.isDirty)
				{
					num4 = closestRaw.distancesToCover[aicoverPoint2];
					if (num4 == -2f)
					{
						goto IL_20D;
					}
				}
				else
				{
					num4 = Vector3.Distance(currentPosition, position);
				}
				float num5 = 0f;
				if (aicoverPoint2.InUse())
				{
					bool flag = aicoverPoint2.IsUsedBy(forObject);
					if (!allowObjectToReuse || !flag)
					{
						num5 -= 1000f;
					}
				}
				if (minRange > 0f)
				{
					num5 -= (1f - Mathf.InverseLerp(0f, minRange, num4)) * 100f;
				}
				float num6 = Mathf.Abs(position.y - currentPosition.y);
				num5 += (1f - Mathf.InverseLerp(1f, 5f, num6)) * 500f;
				num5 += Mathf.InverseLerp(1f - aicoverPoint2.coverDot, 1f, num3) * 50f;
				num5 += (1f - Mathf.InverseLerp(2f, maxRange, num4)) * 100f;
				float num7 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
				float num8 = Vector3.Dot((aicoverPoint2.transform.position - currentPosition).normalized, normalized);
				num5 -= Mathf.InverseLerp(-1f, 0.25f, num8) * 50f * num7;
				if (num5 > num)
				{
					aicoverPoint = aicoverPoint2;
					num = num5;
				}
			}
			IL_20D:;
		}
		if (aicoverPoint)
		{
			return aicoverPoint;
		}
		return null;
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x000BBEA8 File Offset: 0x000BA0A8
	private AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AICoverPoint[] coverPointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			coverPointsInRange = this.grid.GetCoverPointsInRange(position, maxRange, out pointCount);
		}
		else
		{
			coverPointsInRange = this.coverPointArray;
			if (coverPointsInRange != null)
			{
				pointCount = coverPointsInRange.Length;
			}
		}
		return coverPointsInRange;
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x000BBEF0 File Offset: 0x000BA0F0
	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		NavMeshLink navMeshLink = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink2 in this.navMeshLinks)
		{
			float num2 = Vector3.Distance(navMeshLink2.gameObject.transform.position, pos);
			if (num2 < num)
			{
				navMeshLink = navMeshLink2;
				num = num2;
				if (num2 < 0.25f)
				{
					break;
				}
			}
		}
		return navMeshLink;
	}

	// Token: 0x04001245 RID: 4677
	public bool RenderBounds;

	// Token: 0x04001246 RID: 4678
	public bool ShouldSleepAI;

	// Token: 0x04001247 RID: 4679
	public bool Virtual;

	// Token: 0x04001248 RID: 4680
	public bool UseCalculatedCoverDistances = true;

	// Token: 0x04001249 RID: 4681
	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x0400124A RID: 4682
	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	// Token: 0x0400124B RID: 4683
	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	// Token: 0x0400124C RID: 4684
	private AICoverPoint[] coverPointArray;

	// Token: 0x0400124D RID: 4685
	private AIMovePoint[] movePointArray;

	// Token: 0x0400124E RID: 4686
	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	// Token: 0x0400124F RID: 4687
	public List<AIMovePointPath> paths = new List<AIMovePointPath>();

	// Token: 0x04001250 RID: 4688
	public Bounds bounds;

	// Token: 0x04001251 RID: 4689
	private AIInformationGrid grid;

	// Token: 0x04001253 RID: 4691
	private List<IAISleepable> sleepables = new List<IAISleepable>();

	// Token: 0x04001254 RID: 4692
	private OBB areaBox;

	// Token: 0x04001255 RID: 4693
	private bool isDirty = true;

	// Token: 0x04001256 RID: 4694
	private int processIndex;

	// Token: 0x04001257 RID: 4695
	private int halfPaths;

	// Token: 0x04001258 RID: 4696
	private int pathSuccesses;

	// Token: 0x04001259 RID: 4697
	private int pathFails;

	// Token: 0x0400125A RID: 4698
	private bool initd;

	// Token: 0x0400125B RID: 4699
	private static bool lastFrameAnyDirty = false;

	// Token: 0x0400125C RID: 4700
	private static float rebuildStartTime = 0f;

	// Token: 0x0400125D RID: 4701
	public static float buildTimeTest = 0f;

	// Token: 0x0400125E RID: 4702
	private static float lastNavmeshBuildTime = 0f;
}
