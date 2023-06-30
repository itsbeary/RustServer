using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3F RID: 2623
	[AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
	public class UIPolygon : UIPrimitiveBase
	{
		// Token: 0x06003EA5 RID: 16037 RVA: 0x0016F78C File Offset: 0x0016D98C
		public void DrawPolygon(int _sides)
		{
			this.sides = _sides;
			this.VerticesDistances = new float[_sides + 1];
			for (int i = 0; i < _sides; i++)
			{
				this.VerticesDistances[i] = 1f;
			}
			this.rotation = 0f;
			this.SetAllDirty();
		}

		// Token: 0x06003EA6 RID: 16038 RVA: 0x0016F7D8 File Offset: 0x0016D9D8
		public void DrawPolygon(int _sides, float[] _VerticesDistances)
		{
			this.sides = _sides;
			this.VerticesDistances = _VerticesDistances;
			this.rotation = 0f;
			this.SetAllDirty();
		}

		// Token: 0x06003EA7 RID: 16039 RVA: 0x0016F7F9 File Offset: 0x0016D9F9
		public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
		{
			this.sides = _sides;
			this.VerticesDistances = _VerticesDistances;
			this.rotation = _rotation;
			this.SetAllDirty();
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x0016F818 File Offset: 0x0016DA18
		private void Update()
		{
			this.size = base.rectTransform.rect.width;
			if (base.rectTransform.rect.width > base.rectTransform.rect.height)
			{
				this.size = base.rectTransform.rect.height;
			}
			else
			{
				this.size = base.rectTransform.rect.width;
			}
			this.thickness = Mathf.Clamp(this.thickness, 0f, this.size / 2f);
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x0016F8C0 File Offset: 0x0016DAC0
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = new Vector2(0f, 0f);
			Vector2 vector4 = new Vector2(0f, 1f);
			Vector2 vector5 = new Vector2(1f, 1f);
			Vector2 vector6 = new Vector2(1f, 0f);
			float num = 360f / (float)this.sides;
			int num2 = this.sides + 1;
			if (this.VerticesDistances.Length != num2)
			{
				this.VerticesDistances = new float[num2];
				for (int i = 0; i < num2 - 1; i++)
				{
					this.VerticesDistances[i] = 1f;
				}
			}
			this.VerticesDistances[num2 - 1] = this.VerticesDistances[0];
			for (int j = 0; j < num2; j++)
			{
				float num3 = -base.rectTransform.pivot.x * this.size * this.VerticesDistances[j];
				float num4 = -base.rectTransform.pivot.x * this.size * this.VerticesDistances[j] + this.thickness;
				float num5 = 0.017453292f * ((float)j * num + this.rotation);
				float num6 = Mathf.Cos(num5);
				float num7 = Mathf.Sin(num5);
				vector3 = new Vector2(0f, 1f);
				vector4 = new Vector2(1f, 1f);
				vector5 = new Vector2(1f, 0f);
				vector6 = new Vector2(0f, 0f);
				Vector2 vector7 = vector;
				Vector2 vector8 = new Vector2(num3 * num6, num3 * num7);
				Vector2 zero;
				Vector2 vector9;
				if (this.fill)
				{
					zero = Vector2.zero;
					vector9 = Vector2.zero;
				}
				else
				{
					zero = new Vector2(num4 * num6, num4 * num7);
					vector9 = vector2;
				}
				vector = vector8;
				vector2 = zero;
				vh.AddUIVertexQuad(base.SetVbo(new Vector2[] { vector7, vector8, zero, vector9 }, new Vector2[] { vector3, vector4, vector5, vector6 }));
			}
		}

		// Token: 0x04003853 RID: 14419
		public bool fill = true;

		// Token: 0x04003854 RID: 14420
		public float thickness = 5f;

		// Token: 0x04003855 RID: 14421
		[Range(3f, 360f)]
		public int sides = 3;

		// Token: 0x04003856 RID: 14422
		[Range(0f, 360f)]
		public float rotation;

		// Token: 0x04003857 RID: 14423
		[Range(0f, 1f)]
		public float[] VerticesDistances = new float[3];

		// Token: 0x04003858 RID: 14424
		private float size;
	}
}
