using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200094E RID: 2382
public class PathInterpolator
{
	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x06003908 RID: 14600 RVA: 0x00152D63 File Offset: 0x00150F63
	// (set) Token: 0x06003909 RID: 14601 RVA: 0x00152D6B File Offset: 0x00150F6B
	public int MinIndex { get; set; }

	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x0600390A RID: 14602 RVA: 0x00152D74 File Offset: 0x00150F74
	// (set) Token: 0x0600390B RID: 14603 RVA: 0x00152D7C File Offset: 0x00150F7C
	public int MaxIndex { get; set; }

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x0600390C RID: 14604 RVA: 0x00152D85 File Offset: 0x00150F85
	// (set) Token: 0x0600390D RID: 14605 RVA: 0x00152D8D File Offset: 0x00150F8D
	public virtual float Length { get; private set; }

	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x0600390E RID: 14606 RVA: 0x00152D96 File Offset: 0x00150F96
	// (set) Token: 0x0600390F RID: 14607 RVA: 0x00152D9E File Offset: 0x00150F9E
	public virtual float StepSize { get; private set; }

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06003910 RID: 14608 RVA: 0x00152DA7 File Offset: 0x00150FA7
	// (set) Token: 0x06003911 RID: 14609 RVA: 0x00152DAF File Offset: 0x00150FAF
	public bool Circular { get; private set; }

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06003912 RID: 14610 RVA: 0x00007A44 File Offset: 0x00005C44
	public int DefaultMinIndex
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06003913 RID: 14611 RVA: 0x00152DB8 File Offset: 0x00150FB8
	public int DefaultMaxIndex
	{
		get
		{
			return this.Points.Length - 1;
		}
	}

	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x06003914 RID: 14612 RVA: 0x00152DC4 File Offset: 0x00150FC4
	public float StartOffset
	{
		get
		{
			return this.Length * (float)(this.MinIndex - this.DefaultMinIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x06003915 RID: 14613 RVA: 0x00152DEA File Offset: 0x00150FEA
	public float EndOffset
	{
		get
		{
			return this.Length * (float)(this.DefaultMaxIndex - this.MaxIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x00152E10 File Offset: 0x00151010
	public PathInterpolator(Vector3[] points)
	{
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		this.Points = points;
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.Circular = Vector3.Distance(points[0], points[points.Length - 1]) < 0.1f;
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00152E78 File Offset: 0x00151078
	public PathInterpolator(Vector3[] points, Vector3[] tangents)
		: this(points)
	{
		if (tangents.Length != points.Length)
		{
			throw new ArgumentException(string.Concat(new object[] { "Points and tangents lengths must match. Points: ", points.Length, " Tangents: ", tangents.Length }));
		}
		this.Tangents = tangents;
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x00152EE0 File Offset: 0x001510E0
	public void RecalculateTangents()
	{
		if (this.Tangents == null || this.Tangents.Length != this.Points.Length)
		{
			this.Tangents = new Vector3[this.Points.Length];
		}
		for (int i = 0; i < this.Points.Length; i++)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (num < 0)
			{
				num = (this.Circular ? (this.Points.Length - 2) : 0);
			}
			if (num2 > this.Points.Length - 1)
			{
				num2 = (this.Circular ? 1 : (this.Points.Length - 1));
			}
			Vector3 vector = this.Points[num];
			Vector3 vector2 = this.Points[num2];
			this.Tangents[i] = (vector2 - vector).normalized;
		}
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x00152FC0 File Offset: 0x001511C0
	public void RecalculateLength()
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 vector = this.Points[i];
			Vector3 vector2 = this.Points[i + 1];
			num += (vector2 - vector).magnitude;
		}
		this.Length = num;
		this.StepSize = num / (float)this.Points.Length;
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00153030 File Offset: 0x00151230
	public void Resample(float distance)
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 vector = this.Points[i];
			Vector3 vector2 = this.Points[i + 1];
			num += (vector2 - vector).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / distance);
		if (num2 < 2)
		{
			return;
		}
		distance = num / (float)(num2 - 1);
		List<Vector3> list = new List<Vector3>(num2);
		float num3 = 0f;
		for (int j = 0; j < this.Points.Length - 1; j++)
		{
			int num4 = j;
			int num5 = j + 1;
			Vector3 vector3 = this.Points[num4];
			Vector3 vector4 = this.Points[num5];
			float num6 = (vector4 - vector3).magnitude;
			if (num4 == 0)
			{
				list.Add(vector3);
			}
			while (num3 + num6 > distance)
			{
				float num7 = distance - num3;
				float num8 = num7 / num6;
				Vector3 vector5 = Vector3.Lerp(vector3, vector4, num8);
				list.Add(vector5);
				vector3 = vector5;
				num3 = 0f;
				num6 -= num7;
			}
			num3 += num6;
			if (num5 == this.Points.Length - 1 && num3 > distance * 0.5f)
			{
				list.Add(vector4);
			}
		}
		if (list.Count < 2)
		{
			return;
		}
		this.Points = list.ToArray();
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.initialized = false;
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x001531B0 File Offset: 0x001513B0
	public void Smoothen(int iterations, Func<int, float> filter = null)
	{
		this.Smoothen(iterations, Vector3.one, filter);
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x001531C0 File Offset: 0x001513C0
	public void Smoothen(int iterations, Vector3 multipliers, Func<int, float> filter = null)
	{
		for (int i = 0; i < iterations; i++)
		{
			for (int j = this.MinIndex + (this.Circular ? 0 : 1); j <= this.MaxIndex - 1; j += 2)
			{
				this.SmoothenIndex(j, multipliers, filter);
			}
			for (int k = this.MinIndex + (this.Circular ? 1 : 2); k <= this.MaxIndex - 1; k += 2)
			{
				this.SmoothenIndex(k, multipliers, filter);
			}
		}
		this.initialized = false;
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x0015323C File Offset: 0x0015143C
	private void SmoothenIndex(int i, Vector3 multipliers, Func<int, float> filter = null)
	{
		int num = i - 1;
		int num2 = i + 1;
		if (i == 0)
		{
			num = this.Points.Length - 2;
		}
		Vector3 vector = this.Points[num];
		Vector3 vector2 = this.Points[i];
		Vector3 vector3 = this.Points[num2];
		Vector3 vector4 = (vector + vector2 + vector2 + vector3) * 0.25f;
		if (filter != null)
		{
			multipliers *= filter(i);
		}
		if (multipliers != Vector3.one)
		{
			vector4.x = Mathf.LerpUnclamped(vector2.x, vector4.x, multipliers.x);
			vector4.y = Mathf.LerpUnclamped(vector2.y, vector4.y, multipliers.y);
			vector4.z = Mathf.LerpUnclamped(vector2.z, vector4.z, multipliers.z);
		}
		this.Points[i] = vector4;
		if (i == 0)
		{
			this.Points[this.Points.Length - 1] = this.Points[0];
		}
	}

	// Token: 0x0600391E RID: 14622 RVA: 0x00153355 File Offset: 0x00151555
	public Vector3 GetStartPoint()
	{
		return this.Points[this.MinIndex];
	}

	// Token: 0x0600391F RID: 14623 RVA: 0x00153368 File Offset: 0x00151568
	public Vector3 GetEndPoint()
	{
		return this.Points[this.MaxIndex];
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x0015337B File Offset: 0x0015157B
	public Vector3 GetStartTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MinIndex];
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x001533A1 File Offset: 0x001515A1
	public Vector3 GetEndTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MaxIndex];
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x001533C8 File Offset: 0x001515C8
	public Vector3 GetPoint(float distance)
	{
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 vector = this.Points[num2];
		Vector3 vector2 = this.Points[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Lerp(vector, vector2, num3);
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x0015344C File Offset: 0x0015164C
	public virtual Vector3 GetTangent(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Tangents.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartTangent();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndTangent();
		}
		Vector3 vector = this.Tangents[num2];
		Vector3 vector2 = this.Tangents[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Slerp(vector, vector2, num3);
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x001534E4 File Offset: 0x001516E4
	public virtual Vector3 GetPointCubicHermite(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 vector = this.Points[num2];
		Vector3 vector2 = this.Points[num2 + 1];
		Vector3 vector3 = this.Tangents[num2] * this.StepSize;
		Vector3 vector4 = this.Tangents[num2 + 1] * this.StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * vector + (num5 - 2f * num4 + num3) * vector3 + (-2f * num5 + 3f * num4) * vector2 + (num5 - num4) * vector4;
	}

	// Token: 0x040033CD RID: 13261
	public Vector3[] Points;

	// Token: 0x040033CE RID: 13262
	public Vector3[] Tangents;

	// Token: 0x040033D4 RID: 13268
	protected bool initialized;
}
