using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A36 RID: 2614
	[AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
	public class DiamondGraph : UIPrimitiveBase
	{
		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06003E2F RID: 15919 RVA: 0x0016BCE4 File Offset: 0x00169EE4
		// (set) Token: 0x06003E30 RID: 15920 RVA: 0x0016BCEC File Offset: 0x00169EEC
		public float A
		{
			get
			{
				return this.m_a;
			}
			set
			{
				this.m_a = value;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06003E31 RID: 15921 RVA: 0x0016BCF5 File Offset: 0x00169EF5
		// (set) Token: 0x06003E32 RID: 15922 RVA: 0x0016BCFD File Offset: 0x00169EFD
		public float B
		{
			get
			{
				return this.m_b;
			}
			set
			{
				this.m_b = value;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06003E33 RID: 15923 RVA: 0x0016BD06 File Offset: 0x00169F06
		// (set) Token: 0x06003E34 RID: 15924 RVA: 0x0016BD0E File Offset: 0x00169F0E
		public float C
		{
			get
			{
				return this.m_c;
			}
			set
			{
				this.m_c = value;
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06003E35 RID: 15925 RVA: 0x0016BD17 File Offset: 0x00169F17
		// (set) Token: 0x06003E36 RID: 15926 RVA: 0x0016BD1F File Offset: 0x00169F1F
		public float D
		{
			get
			{
				return this.m_d;
			}
			set
			{
				this.m_d = value;
			}
		}

		// Token: 0x06003E37 RID: 15927 RVA: 0x0016BD28 File Offset: 0x00169F28
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			float num = base.rectTransform.rect.width / 2f;
			this.m_a = Math.Min(1f, Math.Max(0f, this.m_a));
			this.m_b = Math.Min(1f, Math.Max(0f, this.m_b));
			this.m_c = Math.Min(1f, Math.Max(0f, this.m_c));
			this.m_d = Math.Min(1f, Math.Max(0f, this.m_d));
			Color32 color = this.color;
			vh.AddVert(new Vector3(-num * this.m_a, 0f), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(0f, num * this.m_b), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(num * this.m_c, 0f), color, new Vector2(1f, 1f));
			vh.AddVert(new Vector3(0f, -num * this.m_d), color, new Vector2(1f, 0f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		// Token: 0x040037F5 RID: 14325
		[SerializeField]
		private float m_a = 1f;

		// Token: 0x040037F6 RID: 14326
		[SerializeField]
		private float m_b = 1f;

		// Token: 0x040037F7 RID: 14327
		[SerializeField]
		private float m_c = 1f;

		// Token: 0x040037F8 RID: 14328
		[SerializeField]
		private float m_d = 1f;
	}
}
