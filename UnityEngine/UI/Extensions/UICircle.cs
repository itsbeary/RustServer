using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A37 RID: 2615
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		// Token: 0x06003E39 RID: 15929 RVA: 0x0016BECC File Offset: 0x0016A0CC
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			int num = (this.ArcInvert ? (-1) : 1);
			float num2 = ((base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height) - (float)this.Padding;
			float num3 = -base.rectTransform.pivot.x * num2;
			float num4 = -base.rectTransform.pivot.x * num2 + this.Thickness;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int num5 = 0;
			int num6 = 1;
			float num7 = this.Arc * 360f / (float)this.ArcSteps;
			this._progress = (float)this.ArcSteps * this.Progress;
			float num8 = (float)num * 0.017453292f * (float)this.ArcRotation;
			float num9 = Mathf.Cos(num8);
			float num10 = Mathf.Sin(num8);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = ((this._progress > 0f) ? this.ProgressColor : this.color);
			simpleVert.position = new Vector2(num3 * num9, num3 * num10);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num4 * num9, num4 * num10);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float num11 = (float)num * 0.017453292f * ((float)i * num7 + (float)this.ArcRotation);
				num9 = Mathf.Cos(num11);
				num10 = Mathf.Sin(num11);
				simpleVert.color = (((float)i > this._progress) ? this.color : this.ProgressColor);
				simpleVert.position = new Vector2(num3 * num9, num3 * num10);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num4 * num9, num4 * num10);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
					this.vertices.Add(simpleVert);
					int num12 = num6;
					this.indices.Add(num5);
					this.indices.Add(num6 + 1);
					this.indices.Add(num6);
					num6++;
					num5 = num6;
					num6++;
					this.indices.Add(num5);
					this.indices.Add(num6);
					this.indices.Add(num12);
				}
				else
				{
					this.indices.Add(num5);
					this.indices.Add(num6 + 1);
					if ((float)i > this._progress)
					{
						this.indices.Add(this.ArcSteps + 2);
					}
					else
					{
						this.indices.Add(1);
					}
					num6++;
					num5 = num6;
				}
			}
			if (this.Fill)
			{
				simpleVert.position = zero;
				simpleVert.color = this.color;
				simpleVert.uv0 = this.uvCenter;
				this.vertices.Add(simpleVert);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003E3A RID: 15930 RVA: 0x0016C316 File Offset: 0x0016A516
		public void SetProgress(float progress)
		{
			this.Progress = progress;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E3B RID: 15931 RVA: 0x0016C325 File Offset: 0x0016A525
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E3C RID: 15932 RVA: 0x0016C334 File Offset: 0x0016A534
		public void SetInvertArc(bool invert)
		{
			this.ArcInvert = invert;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E3D RID: 15933 RVA: 0x0016C343 File Offset: 0x0016A543
		public void SetArcRotation(int rotation)
		{
			this.ArcRotation = rotation;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E3E RID: 15934 RVA: 0x0016C352 File Offset: 0x0016A552
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E3F RID: 15935 RVA: 0x0016C361 File Offset: 0x0016A561
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E40 RID: 15936 RVA: 0x0016C370 File Offset: 0x0016A570
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E41 RID: 15937 RVA: 0x0016C399 File Offset: 0x0016A599
		public void SetProgressColor(Color color)
		{
			this.ProgressColor = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E42 RID: 15938 RVA: 0x0016C3A8 File Offset: 0x0016A5A8
		public void UpdateProgressAlpha(float value)
		{
			this.ProgressColor.a = value;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E43 RID: 15939 RVA: 0x0016C3BC File Offset: 0x0016A5BC
		public void SetPadding(int padding)
		{
			this.Padding = padding;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E44 RID: 15940 RVA: 0x0016C3CB File Offset: 0x0016A5CB
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}

		// Token: 0x040037F9 RID: 14329
		[Tooltip("The Arc Invert property will invert the construction of the Arc.")]
		public bool ArcInvert = true;

		// Token: 0x040037FA RID: 14330
		[Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
		[Range(0f, 1f)]
		public float Arc = 1f;

		// Token: 0x040037FB RID: 14331
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x040037FC RID: 14332
		[Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
		[Range(0f, 360f)]
		public int ArcRotation;

		// Token: 0x040037FD RID: 14333
		[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
		[Range(0f, 1f)]
		public float Progress;

		// Token: 0x040037FE RID: 14334
		private float _progress;

		// Token: 0x040037FF RID: 14335
		public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

		// Token: 0x04003800 RID: 14336
		public bool Fill = true;

		// Token: 0x04003801 RID: 14337
		public float Thickness = 5f;

		// Token: 0x04003802 RID: 14338
		public int Padding;

		// Token: 0x04003803 RID: 14339
		private List<int> indices = new List<int>();

		// Token: 0x04003804 RID: 14340
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x04003805 RID: 14341
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);
	}
}
