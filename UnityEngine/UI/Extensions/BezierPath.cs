using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A44 RID: 2628
	public class BezierPath
	{
		// Token: 0x06003EDB RID: 16091 RVA: 0x00170575 File Offset: 0x0016E775
		public BezierPath()
		{
			this.controlPoints = new List<Vector2>();
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x001705A6 File Offset: 0x0016E7A6
		public void SetControlPoints(List<Vector2> newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x001705A6 File Offset: 0x0016E7A6
		public void SetControlPoints(Vector2[] newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003EDE RID: 16094 RVA: 0x001705D4 File Offset: 0x0016E7D4
		public List<Vector2> GetControlPoints()
		{
			return this.controlPoints;
		}

		// Token: 0x06003EDF RID: 16095 RVA: 0x001705DC File Offset: 0x0016E7DC
		public void Interpolate(List<Vector2> segmentPoints, float scale)
		{
			this.controlPoints.Clear();
			if (segmentPoints.Count < 2)
			{
				return;
			}
			for (int i = 0; i < segmentPoints.Count; i++)
			{
				if (i == 0)
				{
					Vector2 vector = segmentPoints[i];
					Vector2 vector2 = segmentPoints[i + 1] - vector;
					Vector2 vector3 = vector + scale * vector2;
					this.controlPoints.Add(vector);
					this.controlPoints.Add(vector3);
				}
				else if (i == segmentPoints.Count - 1)
				{
					Vector2 vector4 = segmentPoints[i - 1];
					Vector2 vector5 = segmentPoints[i];
					Vector2 vector6 = vector5 - vector4;
					Vector2 vector7 = vector5 - scale * vector6;
					this.controlPoints.Add(vector7);
					this.controlPoints.Add(vector5);
				}
				else
				{
					Vector2 vector8 = segmentPoints[i - 1];
					Vector2 vector9 = segmentPoints[i];
					Vector2 vector10 = segmentPoints[i + 1];
					Vector2 normalized = (vector10 - vector8).normalized;
					Vector2 vector11 = vector9 - scale * normalized * (vector9 - vector8).magnitude;
					Vector2 vector12 = vector9 + scale * normalized * (vector10 - vector9).magnitude;
					this.controlPoints.Add(vector11);
					this.controlPoints.Add(vector9);
					this.controlPoints.Add(vector12);
				}
			}
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003EE0 RID: 16096 RVA: 0x00170774 File Offset: 0x0016E974
		public void SamplePoints(List<Vector2> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
		{
			if (sourcePoints.Count < 2)
			{
				return;
			}
			Stack<Vector2> stack = new Stack<Vector2>();
			stack.Push(sourcePoints[0]);
			Vector2 vector = sourcePoints[1];
			for (int i = 2; i < sourcePoints.Count; i++)
			{
				if ((vector - sourcePoints[i]).sqrMagnitude > minSqrDistance && (stack.Peek() - sourcePoints[i]).sqrMagnitude > maxSqrDistance)
				{
					stack.Push(vector);
				}
				vector = sourcePoints[i];
			}
			Vector2 vector2 = stack.Pop();
			Vector2 vector3 = stack.Peek();
			Vector2 normalized = (vector3 - vector).normalized;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			vector2 += normalized * ((magnitude2 - magnitude) / 2f);
			stack.Push(vector2);
			stack.Push(vector);
			this.Interpolate(new List<Vector2>(stack), scale);
		}

		// Token: 0x06003EE1 RID: 16097 RVA: 0x0017087C File Offset: 0x0016EA7C
		public Vector2 CalculateBezierPoint(int curveIndex, float t)
		{
			int num = curveIndex * 3;
			Vector2 vector = this.controlPoints[num];
			Vector2 vector2 = this.controlPoints[num + 1];
			Vector2 vector3 = this.controlPoints[num + 2];
			Vector2 vector4 = this.controlPoints[num + 3];
			return this.CalculateBezierPoint(t, vector, vector2, vector3, vector4);
		}

		// Token: 0x06003EE2 RID: 16098 RVA: 0x001708D4 File Offset: 0x0016EAD4
		public List<Vector2> GetDrawingPoints0()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(i, 0f));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float num = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(i, num));
				}
			}
			return list;
		}

		// Token: 0x06003EE3 RID: 16099 RVA: 0x00170938 File Offset: 0x0016EB38
		public List<Vector2> GetDrawingPoints1()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.controlPoints.Count - 3; i += 3)
			{
				Vector2 vector = this.controlPoints[i];
				Vector2 vector2 = this.controlPoints[i + 1];
				Vector2 vector3 = this.controlPoints[i + 2];
				Vector2 vector4 = this.controlPoints[i + 3];
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(0f, vector, vector2, vector3, vector4));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float num = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(num, vector, vector2, vector3, vector4));
				}
			}
			return list;
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x001709F8 File Offset: 0x0016EBF8
		public List<Vector2> GetDrawingPoints2()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				List<Vector2> list2 = this.FindDrawingPoints(i);
				if (i != 0)
				{
					list2.RemoveAt(0);
				}
				list.AddRange(list2);
			}
			return list;
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x00170A38 File Offset: 0x0016EC38
		private List<Vector2> FindDrawingPoints(int curveIndex)
		{
			List<Vector2> list = new List<Vector2>();
			Vector2 vector = this.CalculateBezierPoint(curveIndex, 0f);
			Vector2 vector2 = this.CalculateBezierPoint(curveIndex, 1f);
			list.Add(vector);
			list.Add(vector2);
			this.FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
			return list;
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x00170A88 File Offset: 0x0016EC88
		private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector2> pointList, int insertionIndex)
		{
			Vector2 vector = this.CalculateBezierPoint(curveIndex, t0);
			Vector2 vector2 = this.CalculateBezierPoint(curveIndex, t1);
			if ((vector - vector2).sqrMagnitude < this.MINIMUM_SQR_DISTANCE)
			{
				return 0;
			}
			float num = (t0 + t1) / 2f;
			Vector2 vector3 = this.CalculateBezierPoint(curveIndex, num);
			Vector2 normalized = (vector - vector3).normalized;
			Vector2 normalized2 = (vector2 - vector3).normalized;
			if (Vector2.Dot(normalized, normalized2) > this.DIVISION_THRESHOLD || Mathf.Abs(num - 0.5f) < 0.0001f)
			{
				int num2 = 0;
				num2 += this.FindDrawingPoints(curveIndex, t0, num, pointList, insertionIndex);
				pointList.Insert(insertionIndex + num2, vector3);
				num2++;
				return num2 + this.FindDrawingPoints(curveIndex, num, t1, pointList, insertionIndex + num2);
			}
			return 0;
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x00170B5C File Offset: 0x0016ED5C
		private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float num4 = num3 * num;
			float num5 = num2 * t;
			return num4 * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + num5 * p3;
		}

		// Token: 0x0400386A RID: 14442
		public int SegmentsPerCurve = 10;

		// Token: 0x0400386B RID: 14443
		public float MINIMUM_SQR_DISTANCE = 0.01f;

		// Token: 0x0400386C RID: 14444
		public float DIVISION_THRESHOLD = -0.99f;

		// Token: 0x0400386D RID: 14445
		private List<Vector2> controlPoints;

		// Token: 0x0400386E RID: 14446
		private int curveCount;
	}
}
