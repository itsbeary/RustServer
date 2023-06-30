using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A45 RID: 2629
	[Serializable]
	public class CableCurve
	{
		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06003EE8 RID: 16104 RVA: 0x00170BBC File Offset: 0x0016EDBC
		// (set) Token: 0x06003EE9 RID: 16105 RVA: 0x00170BC4 File Offset: 0x0016EDC4
		public bool regenPoints
		{
			get
			{
				return this.m_regen;
			}
			set
			{
				this.m_regen = value;
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06003EEA RID: 16106 RVA: 0x00170BCD File Offset: 0x0016EDCD
		// (set) Token: 0x06003EEB RID: 16107 RVA: 0x00170BD5 File Offset: 0x0016EDD5
		public Vector2 start
		{
			get
			{
				return this.m_start;
			}
			set
			{
				if (value != this.m_start)
				{
					this.m_regen = true;
				}
				this.m_start = value;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06003EEC RID: 16108 RVA: 0x00170BF3 File Offset: 0x0016EDF3
		// (set) Token: 0x06003EED RID: 16109 RVA: 0x00170BFB File Offset: 0x0016EDFB
		public Vector2 end
		{
			get
			{
				return this.m_end;
			}
			set
			{
				if (value != this.m_end)
				{
					this.m_regen = true;
				}
				this.m_end = value;
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06003EEE RID: 16110 RVA: 0x00170C19 File Offset: 0x0016EE19
		// (set) Token: 0x06003EEF RID: 16111 RVA: 0x00170C21 File Offset: 0x0016EE21
		public float slack
		{
			get
			{
				return this.m_slack;
			}
			set
			{
				if (value != this.m_slack)
				{
					this.m_regen = true;
				}
				this.m_slack = Mathf.Max(0f, value);
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06003EF0 RID: 16112 RVA: 0x00170C44 File Offset: 0x0016EE44
		// (set) Token: 0x06003EF1 RID: 16113 RVA: 0x00170C4C File Offset: 0x0016EE4C
		public int steps
		{
			get
			{
				return this.m_steps;
			}
			set
			{
				if (value != this.m_steps)
				{
					this.m_regen = true;
				}
				this.m_steps = Mathf.Max(2, value);
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06003EF2 RID: 16114 RVA: 0x00170C6C File Offset: 0x0016EE6C
		public Vector2 midPoint
		{
			get
			{
				Vector2 vector = Vector2.zero;
				if (this.m_steps == 2)
				{
					return (this.points[0] + this.points[1]) * 0.5f;
				}
				if (this.m_steps > 2)
				{
					int num = this.m_steps / 2;
					if (this.m_steps % 2 == 0)
					{
						vector = (this.points[num] + this.points[num + 1]) * 0.5f;
					}
					else
					{
						vector = this.points[num];
					}
				}
				return vector;
			}
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x00170D08 File Offset: 0x0016EF08
		public CableCurve()
		{
			this.points = CableCurve.emptyCurve;
			this.m_start = Vector2.up;
			this.m_end = Vector2.up + Vector2.right;
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EF4 RID: 16116 RVA: 0x00170D60 File Offset: 0x0016EF60
		public CableCurve(Vector2[] inputPoints)
		{
			this.points = inputPoints;
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EF5 RID: 16117 RVA: 0x00170DB0 File Offset: 0x0016EFB0
		public CableCurve(List<Vector2> inputPoints)
		{
			this.points = inputPoints.ToArray();
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EF6 RID: 16118 RVA: 0x00170E04 File Offset: 0x0016F004
		public CableCurve(CableCurve v)
		{
			this.points = v.Points();
			this.m_start = v.start;
			this.m_end = v.end;
			this.m_slack = v.slack;
			this.m_steps = v.steps;
			this.m_regen = v.regenPoints;
		}

		// Token: 0x06003EF7 RID: 16119 RVA: 0x00170E60 File Offset: 0x0016F060
		public Vector2[] Points()
		{
			if (!this.m_regen)
			{
				return this.points;
			}
			if (this.m_steps < 2)
			{
				return CableCurve.emptyCurve;
			}
			float num = Vector2.Distance(this.m_end, this.m_start);
			float num2 = Vector2.Distance(new Vector2(this.m_end.x, this.m_start.y), this.m_start);
			float num3 = num + Mathf.Max(0.0001f, this.m_slack);
			float num4 = 0f;
			float y = this.m_start.y;
			float num5 = num2;
			float y2 = this.end.y;
			if (num5 - num4 == 0f)
			{
				return CableCurve.emptyCurve;
			}
			float num6 = Mathf.Sqrt(Mathf.Pow(num3, 2f) - Mathf.Pow(y2 - y, 2f)) / (num5 - num4);
			int num7 = 30;
			int num8 = 0;
			int num9 = num7 * 10;
			bool flag = false;
			float num10 = 0f;
			float num11 = 100f;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					num8++;
					float num12 = num10 + num11;
					float num13 = (float)Math.Sinh((double)num12) / num12;
					if (!float.IsInfinity(num13))
					{
						if (num13 == num6)
						{
							flag = true;
							num10 = num12;
							break;
						}
						if (num13 > num6)
						{
							break;
						}
						num10 = num12;
						if (num8 > num9)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				num11 *= 0.1f;
			}
			float num14 = (num5 - num4) / 2f / num10;
			float num15 = (num4 + num5 - num14 * Mathf.Log((num3 + y2 - y) / (num3 - y2 + y))) / 2f;
			float num16 = (y2 + y - num3 * (float)Math.Cosh((double)num10) / (float)Math.Sinh((double)num10)) / 2f;
			this.points = new Vector2[this.m_steps];
			float num17 = (float)(this.m_steps - 1);
			for (int k = 0; k < this.m_steps; k++)
			{
				float num18 = (float)k / num17;
				Vector2 zero = Vector2.zero;
				zero.x = Mathf.Lerp(this.start.x, this.end.x, num18);
				zero.y = num14 * (float)Math.Cosh((double)((num18 * num2 - num15) / num14)) + num16;
				this.points[k] = zero;
			}
			this.m_regen = false;
			return this.points;
		}

		// Token: 0x0400386F RID: 14447
		[SerializeField]
		private Vector2 m_start;

		// Token: 0x04003870 RID: 14448
		[SerializeField]
		private Vector2 m_end;

		// Token: 0x04003871 RID: 14449
		[SerializeField]
		private float m_slack;

		// Token: 0x04003872 RID: 14450
		[SerializeField]
		private int m_steps;

		// Token: 0x04003873 RID: 14451
		[SerializeField]
		private bool m_regen;

		// Token: 0x04003874 RID: 14452
		private static Vector2[] emptyCurve = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0f)
		};

		// Token: 0x04003875 RID: 14453
		[SerializeField]
		private Vector2[] points;
	}
}
