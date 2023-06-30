using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3B RID: 2619
	[AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06003E69 RID: 15977 RVA: 0x0016D4A2 File Offset: 0x0016B6A2
		// (set) Token: 0x06003E6A RID: 15978 RVA: 0x0016D4AA File Offset: 0x0016B6AA
		public int GridColumns
		{
			get
			{
				return this.m_GridColumns;
			}
			set
			{
				if (this.m_GridColumns == value)
				{
					return;
				}
				this.m_GridColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06003E6B RID: 15979 RVA: 0x0016D4C3 File Offset: 0x0016B6C3
		// (set) Token: 0x06003E6C RID: 15980 RVA: 0x0016D4CB File Offset: 0x0016B6CB
		public int GridRows
		{
			get
			{
				return this.m_GridRows;
			}
			set
			{
				if (this.m_GridRows == value)
				{
					return;
				}
				this.m_GridRows = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x0016D4E4 File Offset: 0x0016B6E4
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			this.relativeSize = true;
			int num = this.GridRows * 3 + 1;
			if (this.GridRows % 2 == 0)
			{
				num++;
			}
			num += this.GridColumns * 3 + 1;
			this.m_points = new Vector2[num];
			int num2 = 0;
			for (int i = 0; i < this.GridRows; i++)
			{
				float num3 = 1f;
				float num4 = 0f;
				if (i % 2 == 0)
				{
					num3 = 0f;
					num4 = 1f;
				}
				float num5 = (float)i / (float)this.GridRows;
				this.m_points[num2].x = num3;
				this.m_points[num2].y = num5;
				num2++;
				this.m_points[num2].x = num4;
				this.m_points[num2].y = num5;
				num2++;
				this.m_points[num2].x = num4;
				this.m_points[num2].y = (float)(i + 1) / (float)this.GridRows;
				num2++;
			}
			if (this.GridRows % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
				num2++;
			}
			this.m_points[num2].x = 0f;
			this.m_points[num2].y = 1f;
			num2++;
			for (int j = 0; j < this.GridColumns; j++)
			{
				float num6 = 1f;
				float num7 = 0f;
				if (j % 2 == 0)
				{
					num6 = 0f;
					num7 = 1f;
				}
				float num8 = (float)j / (float)this.GridColumns;
				this.m_points[num2].x = num8;
				this.m_points[num2].y = num6;
				num2++;
				this.m_points[num2].x = num8;
				this.m_points[num2].y = num7;
				num2++;
				this.m_points[num2].x = (float)(j + 1) / (float)this.GridColumns;
				this.m_points[num2].y = num7;
				num2++;
			}
			if (this.GridColumns % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
			}
			else
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 0f;
			}
			base.OnPopulateMesh(vh);
		}

		// Token: 0x0400381C RID: 14364
		[SerializeField]
		private int m_GridColumns = 10;

		// Token: 0x0400381D RID: 14365
		[SerializeField]
		private int m_GridRows = 10;
	}
}
