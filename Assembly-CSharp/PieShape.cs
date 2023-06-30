using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A9 RID: 2217
[ExecuteInEditMode]
public class PieShape : Graphic
{
	// Token: 0x0600370A RID: 14090 RVA: 0x0014B784 File Offset: 0x00149984
	protected override void OnPopulateMesh(VertexHelper vbo)
	{
		vbo.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		float num = this.startRadius;
		float num2 = this.endRadius;
		if (this.startRadius > this.endRadius)
		{
			num2 = this.endRadius + 360f;
		}
		float num3 = Mathf.Floor((num2 - num) / 6f);
		if (num3 <= 1f)
		{
			return;
		}
		float num4 = (num2 - num) / num3;
		float num5 = num + (num2 - num) * 0.5f;
		Color color = this.color;
		float num6 = base.rectTransform.rect.height * 0.5f;
		Vector2 vector = new Vector2(Mathf.Sin(num5 * 0.017453292f), Mathf.Cos(num5 * 0.017453292f)) * this.border;
		int num7 = 0;
		for (float num8 = num; num8 < num2; num8 += num4)
		{
			if (this.debugDrawing)
			{
				if (color == Color.red)
				{
					color = Color.white;
				}
				else
				{
					color = Color.red;
				}
			}
			simpleVert.color = color;
			float num9 = Mathf.Sin(num8 * 0.017453292f);
			float num10 = Mathf.Cos(num8 * 0.017453292f);
			float num11 = num8 + num4;
			if (num11 > num2)
			{
				num11 = num2;
			}
			float num12 = Mathf.Sin(num11 * 0.017453292f);
			float num13 = Mathf.Cos(num11 * 0.017453292f);
			simpleVert.position = new Vector2(num9 * this.outerSize * num6, num10 * this.outerSize * num6) + vector;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num12 * this.outerSize * num6, num13 * this.outerSize * num6) + vector;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num12 * this.innerSize * num6, num13 * this.innerSize * num6) + vector;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num9 * this.innerSize * num6, num10 * this.innerSize * num6) + vector;
			vbo.AddVert(simpleVert);
			vbo.AddTriangle(num7, num7 + 1, num7 + 2);
			vbo.AddTriangle(num7 + 2, num7 + 3, num7);
			num7 += 4;
		}
	}

	// Token: 0x04003203 RID: 12803
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x04003204 RID: 12804
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x04003205 RID: 12805
	public float startRadius = -45f;

	// Token: 0x04003206 RID: 12806
	public float endRadius = 45f;

	// Token: 0x04003207 RID: 12807
	public float border;

	// Token: 0x04003208 RID: 12808
	public bool debugDrawing;
}
