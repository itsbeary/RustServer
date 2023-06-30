using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9F RID: 2719
	public class HableCurve
	{
		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x060040A6 RID: 16550 RVA: 0x0017C95E File Offset: 0x0017AB5E
		// (set) Token: 0x060040A7 RID: 16551 RVA: 0x0017C966 File Offset: 0x0017AB66
		public float whitePoint { get; private set; }

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x060040A8 RID: 16552 RVA: 0x0017C96F File Offset: 0x0017AB6F
		// (set) Token: 0x060040A9 RID: 16553 RVA: 0x0017C977 File Offset: 0x0017AB77
		public float inverseWhitePoint { get; private set; }

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x060040AA RID: 16554 RVA: 0x0017C980 File Offset: 0x0017AB80
		// (set) Token: 0x060040AB RID: 16555 RVA: 0x0017C988 File Offset: 0x0017AB88
		internal float x0 { get; private set; }

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x060040AC RID: 16556 RVA: 0x0017C991 File Offset: 0x0017AB91
		// (set) Token: 0x060040AD RID: 16557 RVA: 0x0017C999 File Offset: 0x0017AB99
		internal float x1 { get; private set; }

		// Token: 0x060040AE RID: 16558 RVA: 0x0017C9A4 File Offset: 0x0017ABA4
		public HableCurve()
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_Segments[i] = new HableCurve.Segment();
			}
			this.uniforms = new HableCurve.Uniforms(this);
		}

		// Token: 0x060040AF RID: 16559 RVA: 0x0017C9E8 File Offset: 0x0017ABE8
		public float Eval(float x)
		{
			float num = x * this.inverseWhitePoint;
			int num2 = ((num < this.x0) ? 0 : ((num < this.x1) ? 1 : 2));
			return this.m_Segments[num2].Eval(num);
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x0017CA28 File Offset: 0x0017AC28
		public void Init(float toeStrength, float toeLength, float shoulderStrength, float shoulderLength, float shoulderAngle, float gamma)
		{
			HableCurve.DirectParams directParams = default(HableCurve.DirectParams);
			toeLength = Mathf.Pow(Mathf.Clamp01(toeLength), 2.2f);
			toeStrength = Mathf.Clamp01(toeStrength);
			shoulderAngle = Mathf.Clamp01(shoulderAngle);
			shoulderStrength = Mathf.Clamp(shoulderStrength, 1E-05f, 0.99999f);
			shoulderLength = Mathf.Max(0f, shoulderLength);
			gamma = Mathf.Max(1E-05f, gamma);
			float num = toeLength * 0.5f;
			float num2 = (1f - toeStrength) * num;
			float num3 = 1f - num2;
			float num4 = num + num3;
			float num5 = (1f - shoulderStrength) * num3;
			float num6 = num + num5;
			float num7 = num2 + num5;
			float num8 = RuntimeUtilities.Exp2(shoulderLength) - 1f;
			float num9 = num4 + num8;
			directParams.x0 = num;
			directParams.y0 = num2;
			directParams.x1 = num6;
			directParams.y1 = num7;
			directParams.W = num9;
			directParams.gamma = gamma;
			directParams.overshootX = directParams.W * 2f * shoulderAngle * shoulderLength;
			directParams.overshootY = 0.5f * shoulderAngle * shoulderLength;
			this.InitSegments(directParams);
		}

		// Token: 0x060040B1 RID: 16561 RVA: 0x0017CB3C File Offset: 0x0017AD3C
		private void InitSegments(HableCurve.DirectParams srcParams)
		{
			HableCurve.DirectParams directParams = srcParams;
			this.whitePoint = srcParams.W;
			this.inverseWhitePoint = 1f / srcParams.W;
			directParams.W = 1f;
			directParams.x0 /= srcParams.W;
			directParams.x1 /= srcParams.W;
			directParams.overshootX = srcParams.overshootX / srcParams.W;
			float num;
			float num2;
			this.AsSlopeIntercept(out num, out num2, directParams.x0, directParams.x1, directParams.y0, directParams.y1);
			float gamma = srcParams.gamma;
			HableCurve.Segment segment = this.m_Segments[1];
			segment.offsetX = -(num2 / num);
			segment.offsetY = 0f;
			segment.scaleX = 1f;
			segment.scaleY = 1f;
			segment.lnA = gamma * Mathf.Log(num);
			segment.B = gamma;
			float num3 = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x0);
			float num4 = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x1);
			directParams.y0 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y0, directParams.gamma));
			directParams.y1 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y1, directParams.gamma));
			directParams.overshootY = Mathf.Pow(1f + directParams.overshootY, directParams.gamma) - 1f;
			this.x0 = directParams.x0;
			this.x1 = directParams.x1;
			HableCurve.Segment segment2 = this.m_Segments[0];
			segment2.offsetX = 0f;
			segment2.offsetY = 0f;
			segment2.scaleX = 1f;
			segment2.scaleY = 1f;
			float num5;
			float num6;
			this.SolveAB(out num5, out num6, directParams.x0, directParams.y0, num3);
			segment2.lnA = num5;
			segment2.B = num6;
			HableCurve.Segment segment3 = this.m_Segments[2];
			float num7 = 1f + directParams.overshootX - directParams.x1;
			float num8 = 1f + directParams.overshootY - directParams.y1;
			float num9;
			float num10;
			this.SolveAB(out num9, out num10, num7, num8, num4);
			segment3.offsetX = 1f + directParams.overshootX;
			segment3.offsetY = 1f + directParams.overshootY;
			segment3.scaleX = -1f;
			segment3.scaleY = -1f;
			segment3.lnA = num9;
			segment3.B = num10;
			float num11 = this.m_Segments[2].Eval(1f);
			float num12 = 1f / num11;
			this.m_Segments[0].offsetY *= num12;
			this.m_Segments[0].scaleY *= num12;
			this.m_Segments[1].offsetY *= num12;
			this.m_Segments[1].scaleY *= num12;
			this.m_Segments[2].offsetY *= num12;
			this.m_Segments[2].scaleY *= num12;
		}

		// Token: 0x060040B2 RID: 16562 RVA: 0x0017CE55 File Offset: 0x0017B055
		private void SolveAB(out float lnA, out float B, float x0, float y0, float m)
		{
			B = m * x0 / y0;
			lnA = Mathf.Log(y0) - B * Mathf.Log(x0);
		}

		// Token: 0x060040B3 RID: 16563 RVA: 0x0017CE74 File Offset: 0x0017B074
		private void AsSlopeIntercept(out float m, out float b, float x0, float x1, float y0, float y1)
		{
			float num = y1 - y0;
			float num2 = x1 - x0;
			if (num2 == 0f)
			{
				m = 1f;
			}
			else
			{
				m = num / num2;
			}
			b = y0 - x0 * m;
		}

		// Token: 0x060040B4 RID: 16564 RVA: 0x0017CEAB File Offset: 0x0017B0AB
		private float EvalDerivativeLinearGamma(float m, float b, float g, float x)
		{
			return g * m * Mathf.Pow(m * x + b, g - 1f);
		}

		// Token: 0x04003A0C RID: 14860
		private readonly HableCurve.Segment[] m_Segments = new HableCurve.Segment[3];

		// Token: 0x04003A0D RID: 14861
		public readonly HableCurve.Uniforms uniforms;

		// Token: 0x02000F47 RID: 3911
		private class Segment
		{
			// Token: 0x06005455 RID: 21589 RVA: 0x001B5254 File Offset: 0x001B3454
			public float Eval(float x)
			{
				float num = (x - this.offsetX) * this.scaleX;
				float num2 = 0f;
				if (num > 0f)
				{
					num2 = Mathf.Exp(this.lnA + this.B * Mathf.Log(num));
				}
				return num2 * this.scaleY + this.offsetY;
			}

			// Token: 0x04004F91 RID: 20369
			public float offsetX;

			// Token: 0x04004F92 RID: 20370
			public float offsetY;

			// Token: 0x04004F93 RID: 20371
			public float scaleX;

			// Token: 0x04004F94 RID: 20372
			public float scaleY;

			// Token: 0x04004F95 RID: 20373
			public float lnA;

			// Token: 0x04004F96 RID: 20374
			public float B;
		}

		// Token: 0x02000F48 RID: 3912
		private struct DirectParams
		{
			// Token: 0x04004F97 RID: 20375
			internal float x0;

			// Token: 0x04004F98 RID: 20376
			internal float y0;

			// Token: 0x04004F99 RID: 20377
			internal float x1;

			// Token: 0x04004F9A RID: 20378
			internal float y1;

			// Token: 0x04004F9B RID: 20379
			internal float W;

			// Token: 0x04004F9C RID: 20380
			internal float overshootX;

			// Token: 0x04004F9D RID: 20381
			internal float overshootY;

			// Token: 0x04004F9E RID: 20382
			internal float gamma;
		}

		// Token: 0x02000F49 RID: 3913
		public class Uniforms
		{
			// Token: 0x06005457 RID: 21591 RVA: 0x001B52A8 File Offset: 0x001B34A8
			internal Uniforms(HableCurve parent)
			{
				this.parent = parent;
			}

			// Token: 0x17000729 RID: 1833
			// (get) Token: 0x06005458 RID: 21592 RVA: 0x001B52B7 File Offset: 0x001B34B7
			public Vector4 curve
			{
				get
				{
					return new Vector4(this.parent.inverseWhitePoint, this.parent.x0, this.parent.x1, 0f);
				}
			}

			// Token: 0x1700072A RID: 1834
			// (get) Token: 0x06005459 RID: 21593 RVA: 0x001B52E4 File Offset: 0x001B34E4
			public Vector4 toeSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x1700072B RID: 1835
			// (get) Token: 0x0600545A RID: 21594 RVA: 0x001B531C File Offset: 0x001B351C
			public Vector4 toeSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x1700072C RID: 1836
			// (get) Token: 0x0600545B RID: 21595 RVA: 0x001B5354 File Offset: 0x001B3554
			public Vector4 midSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x1700072D RID: 1837
			// (get) Token: 0x0600545C RID: 21596 RVA: 0x001B538C File Offset: 0x001B358C
			public Vector4 midSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x1700072E RID: 1838
			// (get) Token: 0x0600545D RID: 21597 RVA: 0x001B53C4 File Offset: 0x001B35C4
			public Vector4 shoSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x1700072F RID: 1839
			// (get) Token: 0x0600545E RID: 21598 RVA: 0x001B53FC File Offset: 0x001B35FC
			public Vector4 shoSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x04004F9F RID: 20383
			private HableCurve parent;
		}
	}
}
