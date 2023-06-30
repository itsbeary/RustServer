using System;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class FlameJet : MonoBehaviour
{
	// Token: 0x0600194A RID: 6474 RVA: 0x000B9D48 File Offset: 0x000B7F48
	private void Initialize()
	{
		this.currentColor = this.startColor;
		this.tesselation = 0.1f;
		this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
		this.spacing = this.maxLength / (float)this.numSegments;
		if (this.currentSegments.Length != this.numSegments)
		{
			this.currentSegments = new Vector3[this.numSegments];
		}
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000B9DB9 File Offset: 0x000B7FB9
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x000B9DC1 File Offset: 0x000B7FC1
	public void LateUpdate()
	{
		if (this.on || this.currentColor.a > 0f)
		{
			this.UpdateLine();
		}
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x000B9DE3 File Offset: 0x000B7FE3
	public void SetOn(bool isOn)
	{
		this.on = isOn;
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x000B9DEC File Offset: 0x000B7FEC
	private float curve(float x)
	{
		return x * x;
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x000B9DF4 File Offset: 0x000B7FF4
	private void UpdateLine()
	{
		this.currentColor.a = Mathf.Lerp(this.currentColor.a, this.on ? 1f : 0f, Time.deltaTime * 40f);
		this.line.SetColors(this.currentColor, this.endColor);
		if (this.lastWorldSegments == null)
		{
			this.lastWorldSegments = new Vector3[this.numSegments];
		}
		int num = this.currentSegments.Length;
		for (int i = 0; i < num; i++)
		{
			float num2 = 0f;
			float num3 = 0f;
			if (this.lastWorldSegments != null && this.lastWorldSegments[i] != Vector3.zero && i > 0)
			{
				Vector3 vector = base.transform.InverseTransformPoint(this.lastWorldSegments[i]);
				float num4 = (float)i / (float)this.currentSegments.Length;
				Vector3 vector2 = Vector3.Lerp(vector, Vector3.zero, Time.deltaTime * this.drag);
				vector2 = Vector3.Lerp(Vector3.zero, vector2, Mathf.Sqrt(num4));
				num2 = vector2.x;
				num3 = vector2.y;
			}
			if (i == 0)
			{
				num3 = (num2 = 0f);
			}
			Vector3 vector3 = new Vector3(num2, num3, (float)i * this.spacing);
			this.currentSegments[i] = vector3;
			this.lastWorldSegments[i] = base.transform.TransformPoint(vector3);
		}
		this.line.positionCount = this.numSegments;
		this.line.SetPositions(this.currentSegments);
	}

	// Token: 0x04001218 RID: 4632
	public LineRenderer line;

	// Token: 0x04001219 RID: 4633
	public float tesselation = 0.025f;

	// Token: 0x0400121A RID: 4634
	private float length;

	// Token: 0x0400121B RID: 4635
	public float maxLength = 2f;

	// Token: 0x0400121C RID: 4636
	public float drag;

	// Token: 0x0400121D RID: 4637
	private int numSegments;

	// Token: 0x0400121E RID: 4638
	private float spacing;

	// Token: 0x0400121F RID: 4639
	public bool on;

	// Token: 0x04001220 RID: 4640
	private Vector3[] lastWorldSegments;

	// Token: 0x04001221 RID: 4641
	private Vector3[] currentSegments = new Vector3[0];

	// Token: 0x04001222 RID: 4642
	public Color startColor;

	// Token: 0x04001223 RID: 4643
	public Color endColor;

	// Token: 0x04001224 RID: 4644
	public Color currentColor;
}
