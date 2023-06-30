using System;
using UnityEngine;

// Token: 0x0200099B RID: 2459
public class ExplosionsScaleCurves : MonoBehaviour
{
	// Token: 0x06003A5A RID: 14938 RVA: 0x001585F1 File Offset: 0x001567F1
	private void Awake()
	{
		this.t = base.transform;
	}

	// Token: 0x06003A5B RID: 14939 RVA: 0x001585FF File Offset: 0x001567FF
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.evalX = 0f;
		this.evalY = 0f;
		this.evalZ = 0f;
	}

	// Token: 0x06003A5C RID: 14940 RVA: 0x00158630 File Offset: 0x00156830
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphTimeMultiplier.x)
		{
			this.evalX = this.ScaleCurveX.Evaluate(num / this.GraphTimeMultiplier.x) * this.GraphScaleMultiplier.x;
		}
		if (num <= this.GraphTimeMultiplier.y)
		{
			this.evalY = this.ScaleCurveY.Evaluate(num / this.GraphTimeMultiplier.y) * this.GraphScaleMultiplier.y;
		}
		if (num <= this.GraphTimeMultiplier.z)
		{
			this.evalZ = this.ScaleCurveZ.Evaluate(num / this.GraphTimeMultiplier.z) * this.GraphScaleMultiplier.z;
		}
		this.t.localScale = new Vector3(this.evalX, this.evalY, this.evalZ);
	}

	// Token: 0x04003505 RID: 13573
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04003506 RID: 13574
	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04003507 RID: 13575
	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04003508 RID: 13576
	public Vector3 GraphTimeMultiplier = Vector3.one;

	// Token: 0x04003509 RID: 13577
	public Vector3 GraphScaleMultiplier = Vector3.one;

	// Token: 0x0400350A RID: 13578
	private float startTime;

	// Token: 0x0400350B RID: 13579
	private Transform t;

	// Token: 0x0400350C RID: 13580
	private float evalX;

	// Token: 0x0400350D RID: 13581
	private float evalY;

	// Token: 0x0400350E RID: 13582
	private float evalZ;
}
