using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A38 RID: 2616
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle Simple")]
	public class UICircleSimple : UIPrimitiveBase
	{
		// Token: 0x06003E46 RID: 15942 RVA: 0x0016C468 File Offset: 0x0016A668
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = ((base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height);
			float num2 = (this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num - this.Thickness) : (-base.rectTransform.pivot.x * num));
			float num3 = (this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num) : (-base.rectTransform.pivot.x * num + this.Thickness));
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int num4 = 0;
			int num5 = 1;
			float num6 = 360f / (float)this.ArcSteps;
			float num7 = Mathf.Cos(0f);
			float num8 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num7, num2 * num8);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num3 * num7, num3 * num8);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float num9 = 0.017453292f * ((float)i * num6);
				num7 = Mathf.Cos(num9);
				num8 = Mathf.Sin(num9);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num7, num2 * num8);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num3 * num7, num3 * num8);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
					this.vertices.Add(simpleVert);
					int num10 = num5;
					this.indices.Add(num4);
					this.indices.Add(num5 + 1);
					this.indices.Add(num5);
					num5++;
					num4 = num5;
					num5++;
					this.indices.Add(num4);
					this.indices.Add(num5);
					this.indices.Add(num10);
				}
				else
				{
					this.indices.Add(num4);
					this.indices.Add(num5 + 1);
					this.indices.Add(1);
					num5++;
					num4 = num5;
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

		// Token: 0x06003E47 RID: 15943 RVA: 0x0016C863 File Offset: 0x0016AA63
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E48 RID: 15944 RVA: 0x0016C872 File Offset: 0x0016AA72
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E49 RID: 15945 RVA: 0x0016C361 File Offset: 0x0016A561
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E4A RID: 15946 RVA: 0x0016C884 File Offset: 0x0016AA84
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E4B RID: 15947 RVA: 0x0016C8AD File Offset: 0x0016AAAD
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}

		// Token: 0x04003806 RID: 14342
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x04003807 RID: 14343
		public bool Fill = true;

		// Token: 0x04003808 RID: 14344
		public float Thickness = 5f;

		// Token: 0x04003809 RID: 14345
		public bool ThicknessIsOutside;

		// Token: 0x0400380A RID: 14346
		private List<int> indices = new List<int>();

		// Token: 0x0400380B RID: 14347
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x0400380C RID: 14348
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);
	}
}
