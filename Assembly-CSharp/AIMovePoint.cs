using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class AIMovePoint : AIPoint
{
	// Token: 0x060019A0 RID: 6560 RVA: 0x000BBFFC File Offset: 0x000BA1FC
	public void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCircleY(base.transform.position, this.radius);
		Gizmos.color = color;
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x000BC028 File Offset: 0x000BA228
	public void DrawLookAtPoints()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.gray;
		if (this.LookAtPoints != null)
		{
			foreach (Transform transform in this.LookAtPoints)
			{
				if (!(transform == null))
				{
					Gizmos.DrawSphere(transform.position, 0.2f);
					Gizmos.DrawLine(base.transform.position, transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x000BC0C4 File Offset: 0x000BA2C4
	public void Clear()
	{
		this.LookAtPoints = null;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x000BC0CD File Offset: 0x000BA2CD
	public void AddLookAtPoint(Transform transform)
	{
		if (this.LookAtPoints == null)
		{
			this.LookAtPoints = new List<Transform>();
		}
		this.LookAtPoints.Add(transform);
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x000BC0EE File Offset: 0x000BA2EE
	public bool HasLookAtPoints()
	{
		return this.LookAtPoints != null && this.LookAtPoints.Count > 0;
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x000BC108 File Offset: 0x000BA308
	public Transform GetRandomLookAtPoint()
	{
		if (this.LookAtPoints == null || this.LookAtPoints.Count == 0)
		{
			return null;
		}
		return this.LookAtPoints[UnityEngine.Random.Range(0, this.LookAtPoints.Count)];
	}

	// Token: 0x0400125F RID: 4703
	public ListDictionary<AIMovePoint, float> distances = new ListDictionary<AIMovePoint, float>();

	// Token: 0x04001260 RID: 4704
	public ListDictionary<AICoverPoint, float> distancesToCover = new ListDictionary<AICoverPoint, float>();

	// Token: 0x04001261 RID: 4705
	public float radius = 1f;

	// Token: 0x04001262 RID: 4706
	public float WaitTime;

	// Token: 0x04001263 RID: 4707
	public List<Transform> LookAtPoints;

	// Token: 0x02000C4E RID: 3150
	public class DistTo
	{
		// Token: 0x04004350 RID: 17232
		public float distance;

		// Token: 0x04004351 RID: 17233
		public AIMovePoint target;
	}
}
