using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class AIMovePointPath : MonoBehaviour
{
	// Token: 0x060019A7 RID: 6567 RVA: 0x000BC166 File Offset: 0x000BA366
	public void Clear()
	{
		this.Points.Clear();
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x000BC173 File Offset: 0x000BA373
	public void AddPoint(AIMovePoint point)
	{
		this.Points.Add(point);
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x000BC181 File Offset: 0x000BA381
	public AIMovePoint FindNearestPoint(Vector3 position)
	{
		return this.Points[this.FindNearestPointIndex(position)];
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x000BC198 File Offset: 0x000BA398
	public int FindNearestPointIndex(Vector3 position)
	{
		float num = float.MaxValue;
		int num2 = 0;
		int num3 = 0;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			float num4 = Vector3.SqrMagnitude(position - aimovePoint.transform.position);
			if (num4 < num)
			{
				num = num4;
				num2 = num3;
			}
			num3++;
		}
		return num2;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x000BC218 File Offset: 0x000BA418
	public AIMovePoint GetPointAtIndex(int index)
	{
		if (index < 0 || index >= this.Points.Count)
		{
			return null;
		}
		return this.Points[index];
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x000BC23C File Offset: 0x000BA43C
	public int GetNextPointIndex(int currentPointIndex, ref AIMovePointPath.PathDirection pathDirection)
	{
		int num = currentPointIndex + ((pathDirection == AIMovePointPath.PathDirection.Forwards) ? 1 : (-1));
		if (num < 0)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = this.Points.Count - 1;
			}
			else
			{
				num = 1;
				pathDirection = AIMovePointPath.PathDirection.Forwards;
			}
		}
		else if (num >= this.Points.Count)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = 0;
			}
			else
			{
				num = this.Points.Count - 2;
				pathDirection = AIMovePointPath.PathDirection.Backwards;
			}
		}
		return num;
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x000BC2A4 File Offset: 0x000BA4A4
	private void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = this.DebugPathColor;
		int num = -1;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			num++;
			if (!(aimovePoint == null))
			{
				if (num + 1 < this.Points.Count)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[num + 1].transform.position);
				}
				else if (this.LoopMode == AIMovePointPath.Mode.Loop)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[0].transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000BC384 File Offset: 0x000BA584
	private void OnDrawGizmosSelected()
	{
		if (this.Points == null)
		{
			return;
		}
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			aimovePoint.DrawLookAtPoints();
		}
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x000BC3E0 File Offset: 0x000BA5E0
	[ContextMenu("Add Child Points")]
	public void AddChildPoints()
	{
		this.Points = new List<AIMovePoint>();
		this.Points.AddRange(base.GetComponentsInChildren<AIMovePoint>());
	}

	// Token: 0x04001264 RID: 4708
	public Color DebugPathColor = Color.green;

	// Token: 0x04001265 RID: 4709
	public AIMovePointPath.Mode LoopMode;

	// Token: 0x04001266 RID: 4710
	public List<AIMovePoint> Points = new List<AIMovePoint>();

	// Token: 0x02000C4F RID: 3151
	public enum Mode
	{
		// Token: 0x04004353 RID: 17235
		Loop,
		// Token: 0x04004354 RID: 17236
		Reverse
	}

	// Token: 0x02000C50 RID: 3152
	public enum PathDirection
	{
		// Token: 0x04004356 RID: 17238
		Forwards,
		// Token: 0x04004357 RID: 17239
		Backwards
	}
}
