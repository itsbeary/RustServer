using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A43 RID: 2627
	[AddComponentMenu("UI/Extensions/Primitives/UI Ring")]
	public class UIRing : UIPrimitiveBase
	{
		// Token: 0x06003ED6 RID: 16086 RVA: 0x0017032C File Offset: 0x0016E52C
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = this.innerRadius * 2f;
			float num2 = this.outerRadius * 2f;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int num3 = 0;
			int num4 = 1;
			float num5 = 360f / (float)this.ArcSteps;
			float num6 = Mathf.Cos(0f);
			float num7 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num6, num2 * num7);
			this.vertices.Add(simpleVert);
			Vector2 vector = new Vector2(num * num6, num * num7);
			simpleVert.position = vector;
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float num8 = 0.017453292f * ((float)i * num5);
				num6 = Mathf.Cos(num8);
				num7 = Mathf.Sin(num8);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num6, num2 * num7);
				this.vertices.Add(simpleVert);
				simpleVert.position = new Vector2(num * num6, num * num7);
				this.vertices.Add(simpleVert);
				int num9 = num4;
				this.indices.Add(num3);
				this.indices.Add(num4 + 1);
				this.indices.Add(num4);
				num4++;
				num3 = num4;
				num4++;
				this.indices.Add(num3);
				this.indices.Add(num4);
				this.indices.Add(num9);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x00170501 File Offset: 0x0016E701
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x0016C361 File Offset: 0x0016A561
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x00170510 File Offset: 0x0016E710
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x04003865 RID: 14437
		public float innerRadius = 16f;

		// Token: 0x04003866 RID: 14438
		public float outerRadius = 32f;

		// Token: 0x04003867 RID: 14439
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x04003868 RID: 14440
		private List<int> indices = new List<int>();

		// Token: 0x04003869 RID: 14441
		private List<UIVertex> vertices = new List<UIVertex>();
	}
}
