using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A39 RID: 2617
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06003E4D RID: 15949 RVA: 0x0016C918 File Offset: 0x0016AB18
		// (set) Token: 0x06003E4E RID: 15950 RVA: 0x0016C920 File Offset: 0x0016AB20
		public bool CutUL
		{
			get
			{
				return this.m_cutUL;
			}
			set
			{
				this.m_cutUL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06003E4F RID: 15951 RVA: 0x0016C92F File Offset: 0x0016AB2F
		// (set) Token: 0x06003E50 RID: 15952 RVA: 0x0016C937 File Offset: 0x0016AB37
		public bool CutUR
		{
			get
			{
				return this.m_cutUR;
			}
			set
			{
				this.m_cutUR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06003E51 RID: 15953 RVA: 0x0016C946 File Offset: 0x0016AB46
		// (set) Token: 0x06003E52 RID: 15954 RVA: 0x0016C94E File Offset: 0x0016AB4E
		public bool CutLL
		{
			get
			{
				return this.m_cutLL;
			}
			set
			{
				this.m_cutLL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06003E53 RID: 15955 RVA: 0x0016C95D File Offset: 0x0016AB5D
		// (set) Token: 0x06003E54 RID: 15956 RVA: 0x0016C965 File Offset: 0x0016AB65
		public bool CutLR
		{
			get
			{
				return this.m_cutLR;
			}
			set
			{
				this.m_cutLR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06003E55 RID: 15957 RVA: 0x0016C974 File Offset: 0x0016AB74
		// (set) Token: 0x06003E56 RID: 15958 RVA: 0x0016C97C File Offset: 0x0016AB7C
		public bool MakeColumns
		{
			get
			{
				return this.m_makeColumns;
			}
			set
			{
				this.m_makeColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06003E57 RID: 15959 RVA: 0x0016C98B File Offset: 0x0016AB8B
		// (set) Token: 0x06003E58 RID: 15960 RVA: 0x0016C993 File Offset: 0x0016AB93
		public bool UseColorUp
		{
			get
			{
				return this.m_useColorUp;
			}
			set
			{
				this.m_useColorUp = value;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06003E59 RID: 15961 RVA: 0x0016C99C File Offset: 0x0016AB9C
		// (set) Token: 0x06003E5A RID: 15962 RVA: 0x0016C9A4 File Offset: 0x0016ABA4
		public Color32 ColorUp
		{
			get
			{
				return this.m_colorUp;
			}
			set
			{
				this.m_colorUp = value;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06003E5B RID: 15963 RVA: 0x0016C9AD File Offset: 0x0016ABAD
		// (set) Token: 0x06003E5C RID: 15964 RVA: 0x0016C9B5 File Offset: 0x0016ABB5
		public bool UseColorDown
		{
			get
			{
				return this.m_useColorDown;
			}
			set
			{
				this.m_useColorDown = value;
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06003E5D RID: 15965 RVA: 0x0016C9BE File Offset: 0x0016ABBE
		// (set) Token: 0x06003E5E RID: 15966 RVA: 0x0016C9C6 File Offset: 0x0016ABC6
		public Color32 ColorDown
		{
			get
			{
				return this.m_colorDown;
			}
			set
			{
				this.m_colorDown = value;
			}
		}

		// Token: 0x06003E5F RID: 15967 RVA: 0x0016C9D0 File Offset: 0x0016ABD0
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			Rect rect2 = rect;
			Color32 color = this.color;
			bool flag = this.m_cutUL | this.m_cutUR;
			bool flag2 = this.m_cutLL | this.m_cutLR;
			bool flag3 = this.m_cutLL | this.m_cutUL;
			bool flag4 = this.m_cutLR | this.m_cutUR;
			if ((flag || flag2) && this.cornerSize.sqrMagnitude > 0f)
			{
				vh.Clear();
				if (flag3)
				{
					rect2.xMin += this.cornerSize.x;
				}
				if (flag2)
				{
					rect2.yMin += this.cornerSize.y;
				}
				if (flag)
				{
					rect2.yMax -= this.cornerSize.y;
				}
				if (flag4)
				{
					rect2.xMax -= this.cornerSize.x;
				}
				if (this.m_makeColumns)
				{
					Vector2 vector = new Vector2(rect.xMin, this.m_cutUL ? rect2.yMax : rect.yMax);
					Vector2 vector2 = new Vector2(rect.xMax, this.m_cutUR ? rect2.yMax : rect.yMax);
					Vector2 vector3 = new Vector2(rect.xMin, this.m_cutLL ? rect2.yMin : rect.yMin);
					Vector2 vector4 = new Vector2(rect.xMax, this.m_cutLR ? rect2.yMin : rect.yMin);
					if (flag3)
					{
						UICornerCut.AddSquare(vector3, vector, new Vector2(rect2.xMin, rect.yMax), new Vector2(rect2.xMin, rect.yMin), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
					if (flag4)
					{
						UICornerCut.AddSquare(vector2, vector4, new Vector2(rect2.xMax, rect.yMin), new Vector2(rect2.xMax, rect.yMax), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
				}
				else
				{
					Vector2 vector = new Vector2(this.m_cutUL ? rect2.xMin : rect.xMin, rect.yMax);
					Vector2 vector2 = new Vector2(this.m_cutUR ? rect2.xMax : rect.xMax, rect.yMax);
					Vector2 vector3 = new Vector2(this.m_cutLL ? rect2.xMin : rect.xMin, rect.yMin);
					Vector2 vector4 = new Vector2(this.m_cutLR ? rect2.xMax : rect.xMax, rect.yMin);
					if (flag2)
					{
						UICornerCut.AddSquare(vector4, vector3, new Vector2(rect.xMin, rect2.yMin), new Vector2(rect.xMax, rect2.yMin), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
					if (flag)
					{
						UICornerCut.AddSquare(vector, vector2, new Vector2(rect.xMax, rect2.yMax), new Vector2(rect.xMin, rect2.yMax), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
				}
				if (this.m_makeColumns)
				{
					UICornerCut.AddSquare(new Rect(rect2.xMin, rect.yMin, rect2.width, rect.height), rect, color, vh);
					return;
				}
				UICornerCut.AddSquare(new Rect(rect.xMin, rect2.yMin, rect.width, rect2.height), rect, color, vh);
			}
		}

		// Token: 0x06003E60 RID: 15968 RVA: 0x0016CD80 File Offset: 0x0016AF80
		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
			int num3 = UICornerCut.AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
			int num4 = UICornerCut.AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
			vh.AddTriangle(num, num2, num3);
			vh.AddTriangle(num3, num4, num);
		}

		// Token: 0x06003E61 RID: 15969 RVA: 0x0016CDFC File Offset: 0x0016AFFC
		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(a.x, a.y, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(b.x, b.y, rectUV, color32, vh);
			int num3 = UICornerCut.AddVert(c.x, c.y, rectUV, color32, vh);
			int num4 = UICornerCut.AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, num2, num3);
			vh.AddTriangle(num3, num4, num);
		}

		// Token: 0x06003E62 RID: 15970 RVA: 0x0016CE80 File Offset: 0x0016B080
		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			Vector2 vector = new Vector2(Mathf.InverseLerp(area.xMin, area.xMax, x), Mathf.InverseLerp(area.yMin, area.yMax, y));
			vh.AddVert(new Vector3(x, y), color32, vector);
			return vh.currentVertCount - 1;
		}

		// Token: 0x0400380D RID: 14349
		public Vector2 cornerSize = new Vector2(16f, 16f);

		// Token: 0x0400380E RID: 14350
		[Header("Corners to cut")]
		[SerializeField]
		private bool m_cutUL = true;

		// Token: 0x0400380F RID: 14351
		[SerializeField]
		private bool m_cutUR;

		// Token: 0x04003810 RID: 14352
		[SerializeField]
		private bool m_cutLL;

		// Token: 0x04003811 RID: 14353
		[SerializeField]
		private bool m_cutLR;

		// Token: 0x04003812 RID: 14354
		[Tooltip("Up-Down colors become Left-Right colors")]
		[SerializeField]
		private bool m_makeColumns;

		// Token: 0x04003813 RID: 14355
		[Header("Color the cut bars differently")]
		[SerializeField]
		private bool m_useColorUp;

		// Token: 0x04003814 RID: 14356
		[SerializeField]
		private Color32 m_colorUp;

		// Token: 0x04003815 RID: 14357
		[SerializeField]
		private bool m_useColorDown;

		// Token: 0x04003816 RID: 14358
		[SerializeField]
		private Color32 m_colorDown;
	}
}
