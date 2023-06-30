using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200095B RID: 2395
[Serializable]
public class WorldSplineData
{
	// Token: 0x0600397A RID: 14714 RVA: 0x00154BB4 File Offset: 0x00152DB4
	public WorldSplineData(WorldSpline worldSpline)
	{
		worldSpline.CheckValidity();
		this.LUTValues = new List<WorldSplineData.LUTEntry>();
		this.inputPoints = new Vector3[worldSpline.points.Length];
		worldSpline.points.CopyTo(this.inputPoints, 0);
		this.inputTangents = new Vector3[worldSpline.tangents.Length];
		worldSpline.tangents.CopyTo(this.inputTangents, 0);
		this.inputLUTInterval = worldSpline.lutInterval;
		this.maxPointsIndex = this.inputPoints.Length - 1;
		this.CreateLookupTable(worldSpline);
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x00154C45 File Offset: 0x00152E45
	public bool IsSameAs(WorldSpline worldSpline)
	{
		return this.inputPoints.SequenceEqual(worldSpline.points) && this.inputTangents.SequenceEqual(worldSpline.tangents) && this.inputLUTInterval == worldSpline.lutInterval;
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x00154C7D File Offset: 0x00152E7D
	public bool IsDifferentTo(WorldSpline worldSpline)
	{
		return !this.IsSameAs(worldSpline);
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x00154C89 File Offset: 0x00152E89
	public Vector3 GetStartPoint()
	{
		return this.inputPoints[0];
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x00154C97 File Offset: 0x00152E97
	public Vector3 GetEndPoint()
	{
		return this.inputPoints[this.maxPointsIndex];
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x00154CAA File Offset: 0x00152EAA
	public Vector3 GetStartTangent()
	{
		return this.inputTangents[0];
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x00154CB8 File Offset: 0x00152EB8
	public Vector3 GetEndTangent()
	{
		return this.inputTangents[this.maxPointsIndex];
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x00154CCC File Offset: 0x00152ECC
	public Vector3 GetPointCubicHermite(float distance)
	{
		Vector3 vector;
		return this.GetPointAndTangentCubicHermite(distance, out vector);
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x00154CE4 File Offset: 0x00152EE4
	public Vector3 GetTangentCubicHermite(float distance)
	{
		Vector3 vector;
		this.GetPointAndTangentCubicHermite(distance, out vector);
		return vector;
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x00154CFC File Offset: 0x00152EFC
	public Vector3 GetPointAndTangentCubicHermite(float distance, out Vector3 tangent)
	{
		if (distance <= 0f)
		{
			tangent = this.GetStartTangent();
			return this.GetStartPoint();
		}
		if (distance >= this.Length)
		{
			tangent = this.GetEndTangent();
			return this.GetEndPoint();
		}
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count > num)
		{
			int num2 = -1;
			while (num2 < 0 && (float)num > 0f)
			{
				WorldSplineData.LUTEntry lutentry = this.LUTValues[num];
				int num3 = 0;
				while (num3 < lutentry.points.Count && lutentry.points[num3].distance <= distance)
				{
					num2 = num3;
					num3++;
				}
				if (num2 < 0)
				{
					num--;
				}
			}
			float num4;
			Vector3 vector;
			if (num2 < 0)
			{
				num4 = 0f;
				vector = this.GetStartPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = this.LUTValues[num].points[num2];
				num4 = lutpoint.distance;
				vector = lutpoint.pos;
			}
			num2 = -1;
			while (num2 < 0 && num < this.LUTValues.Count)
			{
				WorldSplineData.LUTEntry lutentry2 = this.LUTValues[num];
				for (int i = 0; i < lutentry2.points.Count; i++)
				{
					if (lutentry2.points[i].distance > distance)
					{
						num2 = i;
						break;
					}
				}
				if (num2 < 0)
				{
					num++;
				}
			}
			float num5;
			Vector3 vector2;
			if (num2 < 0)
			{
				num5 = this.Length;
				vector2 = this.GetEndPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint2 = this.LUTValues[num].points[num2];
				num5 = lutpoint2.distance;
				vector2 = lutpoint2.pos;
			}
			float num6 = Mathf.InverseLerp(num4, num5, distance);
			tangent = (vector2 - vector).normalized;
			return Vector3.Lerp(vector, vector2, num6);
		}
		tangent = this.GetEndTangent();
		return this.GetEndPoint();
	}

	// Token: 0x06003984 RID: 14724 RVA: 0x00154EC8 File Offset: 0x001530C8
	public void SetDefaultTangents(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		pathInterpolator.RecalculateTangents();
		worldSpline.tangents = pathInterpolator.Tangents;
	}

	// Token: 0x06003985 RID: 14725 RVA: 0x00154EFC File Offset: 0x001530FC
	public bool DetectSplineProblems(WorldSpline worldSpline)
	{
		bool flag = false;
		Vector3 vector = this.GetTangentCubicHermite(0f);
		for (float num = 0.05f; num <= this.Length; num += 0.05f)
		{
			Vector3 tangentCubicHermite = this.GetTangentCubicHermite(num);
			float num2 = Vector3.Angle(tangentCubicHermite, vector);
			if (num2 > 5f)
			{
				if (worldSpline != null)
				{
					Vector3 vector2;
					Vector3 pointAndTangentCubicHermiteWorld = worldSpline.GetPointAndTangentCubicHermiteWorld(num, out vector2);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, vector2, Color.red, 30f);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, Vector3.up, Color.red, 30f);
				}
				Debug.Log(string.Format("Spline may have a too-sharp bend at {0:P0}. Angle change: ", num / this.Length) + num2);
				flag = true;
			}
			vector = tangentCubicHermite;
		}
		return flag;
	}

	// Token: 0x06003986 RID: 14726 RVA: 0x00154FB0 File Offset: 0x001531B0
	private void CreateLookupTable(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		Vector3 vector = pathInterpolator.GetPointCubicHermite(0f);
		this.Length = 0f;
		this.AddEntry(0f, this.GetStartPoint());
		Vector3 vector2;
		for (float num = worldSpline.lutInterval; num < pathInterpolator.Length; num += worldSpline.lutInterval)
		{
			vector2 = pathInterpolator.GetPointCubicHermite(num);
			this.Length += Vector3.Distance(vector2, vector);
			this.AddEntry(this.Length, pathInterpolator.GetPointCubicHermite(num));
			vector = vector2;
		}
		vector2 = this.GetEndPoint();
		this.Length += Vector3.Distance(vector2, vector);
		this.AddEntry(this.Length, vector2);
	}

	// Token: 0x06003987 RID: 14727 RVA: 0x0015506C File Offset: 0x0015326C
	private void AddEntry(float distance, Vector3 pos)
	{
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count < num + 1)
		{
			for (int i = this.LUTValues.Count; i < num + 1; i++)
			{
				this.LUTValues.Add(new WorldSplineData.LUTEntry());
			}
		}
		this.LUTValues[num].points.Add(new WorldSplineData.LUTEntry.LUTPoint(distance, pos));
	}

	// Token: 0x040033F5 RID: 13301
	public Vector3[] inputPoints;

	// Token: 0x040033F6 RID: 13302
	public Vector3[] inputTangents;

	// Token: 0x040033F7 RID: 13303
	public float inputLUTInterval;

	// Token: 0x040033F8 RID: 13304
	public List<WorldSplineData.LUTEntry> LUTValues;

	// Token: 0x040033F9 RID: 13305
	public float Length;

	// Token: 0x040033FA RID: 13306
	[SerializeField]
	private int maxPointsIndex;

	// Token: 0x02000ED6 RID: 3798
	[Serializable]
	public class LUTEntry
	{
		// Token: 0x04004D81 RID: 19841
		public List<WorldSplineData.LUTEntry.LUTPoint> points = new List<WorldSplineData.LUTEntry.LUTPoint>();

		// Token: 0x02000FE9 RID: 4073
		[Serializable]
		public struct LUTPoint
		{
			// Token: 0x060055E0 RID: 21984 RVA: 0x001BB290 File Offset: 0x001B9490
			public LUTPoint(float distance, Vector3 pos)
			{
				this.distance = distance;
				this.pos = pos;
			}

			// Token: 0x040051AA RID: 20906
			public float distance;

			// Token: 0x040051AB RID: 20907
			public Vector3 pos;
		}
	}
}
