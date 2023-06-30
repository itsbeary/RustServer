using System;
using UnityEngine;

// Token: 0x02000955 RID: 2389
public class TickHistory
{
	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x06003945 RID: 14661 RVA: 0x00153EAE File Offset: 0x001520AE
	public int Count
	{
		get
		{
			return this.points.Count;
		}
	}

	// Token: 0x06003946 RID: 14662 RVA: 0x00153EBB File Offset: 0x001520BB
	public void Reset()
	{
		this.points.Clear();
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x00153EC8 File Offset: 0x001520C8
	public void Reset(Vector3 point)
	{
		this.Reset();
		this.AddPoint(point, -1);
	}

	// Token: 0x06003948 RID: 14664 RVA: 0x00153ED8 File Offset: 0x001520D8
	public float Distance(BasePlayer player, Vector3 point)
	{
		if (this.points.Count == 0)
		{
			return player.Distance(point);
		}
		Vector3 position = player.transform.position;
		Quaternion rotation = player.transform.rotation;
		Bounds bounds = player.bounds;
		Matrix4x4 tickHistoryMatrix = player.tickHistoryMatrix;
		float num = float.MaxValue;
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 vector = tickHistoryMatrix.MultiplyPoint3x4(this.points[i]);
			Vector3 vector2 = ((i == this.points.Count - 1) ? position : tickHistoryMatrix.MultiplyPoint3x4(this.points[i + 1]));
			Line line = new Line(vector, vector2);
			Vector3 vector3 = line.ClosestPoint(point);
			OBB obb = new OBB(vector3, rotation, bounds);
			num = Mathf.Min(num, obb.Distance(point));
		}
		return num;
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x00153FB9 File Offset: 0x001521B9
	public void AddPoint(Vector3 point, int limit = -1)
	{
		while (limit > 0 && this.points.Count >= limit)
		{
			this.points.PopFront();
		}
		this.points.PushBack(point);
	}

	// Token: 0x0600394A RID: 14666 RVA: 0x00153FE8 File Offset: 0x001521E8
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 vector = this.points[i];
			vector = matrix.MultiplyPoint3x4(vector);
			this.points[i] = vector;
		}
	}

	// Token: 0x040033DE RID: 13278
	private Deque<Vector3> points = new Deque<Vector3>(8);
}
