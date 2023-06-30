using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3E RID: 2622
	[AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
	public class UILineTextureRenderer : UIPrimitiveBase
	{
		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003E9E RID: 16030 RVA: 0x0016F142 File Offset: 0x0016D342
		// (set) Token: 0x06003E9F RID: 16031 RVA: 0x0016F14A File Offset: 0x0016D34A
		public Rect uvRect
		{
			get
			{
				return this.m_UVRect;
			}
			set
			{
				if (this.m_UVRect == value)
				{
					return;
				}
				this.m_UVRect = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06003EA0 RID: 16032 RVA: 0x0016F168 File Offset: 0x0016D368
		// (set) Token: 0x06003EA1 RID: 16033 RVA: 0x0016F170 File Offset: 0x0016D370
		public Vector2[] Points
		{
			get
			{
				return this.m_points;
			}
			set
			{
				if (this.m_points == value)
				{
					return;
				}
				this.m_points = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x0016F18C File Offset: 0x0016D38C
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points == null || this.m_points.Length < 2)
			{
				this.m_points = new Vector2[]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}
			int num = 24;
			float num2 = base.rectTransform.rect.width;
			float num3 = base.rectTransform.rect.height;
			float num4 = -base.rectTransform.pivot.x * base.rectTransform.rect.width;
			float num5 = -base.rectTransform.pivot.y * base.rectTransform.rect.height;
			if (!this.relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}
			List<Vector2> list = new List<Vector2>();
			list.Add(this.m_points[0]);
			Vector2 vector = this.m_points[0] + (this.m_points[1] - this.m_points[0]).normalized * (float)num;
			list.Add(vector);
			for (int i = 1; i < this.m_points.Length - 1; i++)
			{
				list.Add(this.m_points[i]);
			}
			vector = this.m_points[this.m_points.Length - 1] - (this.m_points[this.m_points.Length - 1] - this.m_points[this.m_points.Length - 2]).normalized * (float)num;
			list.Add(vector);
			list.Add(this.m_points[this.m_points.Length - 1]);
			Vector2[] array = list.ToArray();
			if (this.UseMargins)
			{
				num2 -= this.Margin.x;
				num3 -= this.Margin.y;
				num4 += this.Margin.x / 2f;
				num5 += this.Margin.y / 2f;
			}
			vh.Clear();
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = Vector2.zero;
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 vector4 = array[j - 1];
				Vector2 vector5 = array[j];
				vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
				vector5 = new Vector2(vector5.x * num2 + num4, vector5.y * num3 + num5);
				float num6 = Mathf.Atan2(vector5.y - vector4.y, vector5.x - vector4.x) * 180f / 3.1415927f;
				Vector2 vector6 = vector4 + new Vector2(0f, -this.LineThickness / 2f);
				Vector2 vector7 = vector4 + new Vector2(0f, this.LineThickness / 2f);
				Vector2 vector8 = vector5 + new Vector2(0f, this.LineThickness / 2f);
				Vector2 vector9 = vector5 + new Vector2(0f, -this.LineThickness / 2f);
				vector6 = this.RotatePointAroundPivot(vector6, vector4, new Vector3(0f, 0f, num6));
				vector7 = this.RotatePointAroundPivot(vector7, vector4, new Vector3(0f, 0f, num6));
				vector8 = this.RotatePointAroundPivot(vector8, vector5, new Vector3(0f, 0f, num6));
				vector9 = this.RotatePointAroundPivot(vector9, vector5, new Vector3(0f, 0f, num6));
				Vector2 zero = Vector2.zero;
				Vector2 vector10 = new Vector2(0f, 1f);
				Vector2 vector11 = new Vector2(0.5f, 0f);
				Vector2 vector12 = new Vector2(0.5f, 1f);
				Vector2 vector13 = new Vector2(1f, 0f);
				Vector2 vector14 = new Vector2(1f, 1f);
				Vector2[] array2 = new Vector2[] { vector11, vector12, vector12, vector11 };
				if (j > 1)
				{
					vh.AddUIVertexQuad(base.SetVbo(new Vector2[] { vector2, vector3, vector6, vector7 }, array2));
				}
				if (j == 1)
				{
					array2 = new Vector2[] { zero, vector10, vector12, vector11 };
				}
				else if (j == array.Length - 1)
				{
					array2 = new Vector2[] { vector11, vector12, vector14, vector13 };
				}
				vh.AddUIVertexQuad(base.SetVbo(new Vector2[] { vector6, vector7, vector8, vector9 }, array2));
				vector2 = vector8;
				vector3 = vector9;
			}
		}

		// Token: 0x06003EA3 RID: 16035 RVA: 0x0016F72C File Offset: 0x0016D92C
		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 vector = point - pivot;
			vector = Quaternion.Euler(angles) * vector;
			point = vector + pivot;
			return point;
		}

		// Token: 0x0400384D RID: 14413
		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x0400384E RID: 14414
		[SerializeField]
		private Vector2[] m_points;

		// Token: 0x0400384F RID: 14415
		public float LineThickness = 2f;

		// Token: 0x04003850 RID: 14416
		public bool UseMargins;

		// Token: 0x04003851 RID: 14417
		public Vector2 Margin;

		// Token: 0x04003852 RID: 14418
		public bool relativeSize;
	}
}
