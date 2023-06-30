using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000956 RID: 2390
public class TickInterpolator
{
	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x0600394C RID: 14668 RVA: 0x00154042 File Offset: 0x00152242
	public int Count
	{
		get
		{
			return this.points.Count;
		}
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x0015404F File Offset: 0x0015224F
	public void Reset()
	{
		this.index = 0;
		this.CurrentPoint = this.StartPoint;
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x00154064 File Offset: 0x00152264
	public void Reset(Vector3 point)
	{
		this.points.Clear();
		this.index = 0;
		this.Length = 0f;
		this.EndPoint = point;
		this.StartPoint = point;
		this.CurrentPoint = point;
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x001540A8 File Offset: 0x001522A8
	public void AddPoint(Vector3 point)
	{
		TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
		this.points.Add(segment);
		this.Length += segment.length;
		this.EndPoint = segment.point;
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x001540F0 File Offset: 0x001522F0
	public bool MoveNext(float distance)
	{
		float num = 0f;
		while (num < distance && this.index < this.points.Count)
		{
			TickInterpolator.Segment segment = this.points[this.index];
			this.CurrentPoint = segment.point;
			num += segment.length;
			this.index++;
		}
		return num > 0f;
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x00154159 File Offset: 0x00152359
	public bool HasNext()
	{
		return this.index < this.points.Count;
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x00154170 File Offset: 0x00152370
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			TickInterpolator.Segment segment = this.points[i];
			segment.point = matrix.MultiplyPoint3x4(segment.point);
			this.points[i] = segment;
		}
		this.CurrentPoint = matrix.MultiplyPoint3x4(this.CurrentPoint);
		this.StartPoint = matrix.MultiplyPoint3x4(this.StartPoint);
		this.EndPoint = matrix.MultiplyPoint3x4(this.EndPoint);
	}

	// Token: 0x040033DF RID: 13279
	private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();

	// Token: 0x040033E0 RID: 13280
	private int index;

	// Token: 0x040033E1 RID: 13281
	public float Length;

	// Token: 0x040033E2 RID: 13282
	public Vector3 CurrentPoint;

	// Token: 0x040033E3 RID: 13283
	public Vector3 StartPoint;

	// Token: 0x040033E4 RID: 13284
	public Vector3 EndPoint;

	// Token: 0x02000ED4 RID: 3796
	private struct Segment
	{
		// Token: 0x0600537D RID: 21373 RVA: 0x001B28BA File Offset: 0x001B0ABA
		public Segment(Vector3 a, Vector3 b)
		{
			this.point = b;
			this.length = Vector3.Distance(a, b);
		}

		// Token: 0x04004D7D RID: 19837
		public Vector3 point;

		// Token: 0x04004D7E RID: 19838
		public float length;
	}
}
