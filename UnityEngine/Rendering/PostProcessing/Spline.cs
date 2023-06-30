using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA7 RID: 2727
	[Serializable]
	public sealed class Spline
	{
		// Token: 0x06004104 RID: 16644 RVA: 0x0017E9BC File Offset: 0x0017CBBC
		public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
		{
			Assert.IsNotNull<AnimationCurve>(curve);
			this.curve = curve;
			this.m_ZeroValue = zeroValue;
			this.m_Loop = loop;
			this.m_Range = bounds.magnitude;
			this.cachedData = new float[128];
		}

		// Token: 0x06004105 RID: 16645 RVA: 0x0017EA10 File Offset: 0x0017CC10
		public void Cache(int frame)
		{
			if (frame == this.frameCount)
			{
				return;
			}
			int length = this.curve.length;
			if (this.m_Loop && length > 1)
			{
				if (this.m_InternalLoopingCurve == null)
				{
					this.m_InternalLoopingCurve = new AnimationCurve();
				}
				Keyframe keyframe = this.curve[length - 1];
				keyframe.time -= this.m_Range;
				Keyframe keyframe2 = this.curve[0];
				keyframe2.time += this.m_Range;
				this.m_InternalLoopingCurve.keys = this.curve.keys;
				this.m_InternalLoopingCurve.AddKey(keyframe);
				this.m_InternalLoopingCurve.AddKey(keyframe2);
			}
			for (int i = 0; i < 128; i++)
			{
				this.cachedData[i] = this.Evaluate((float)i * 0.0078125f, length);
			}
			this.frameCount = Time.renderedFrameCount;
		}

		// Token: 0x06004106 RID: 16646 RVA: 0x0017EAFE File Offset: 0x0017CCFE
		public float Evaluate(float t, int length)
		{
			if (length == 0)
			{
				return this.m_ZeroValue;
			}
			if (!this.m_Loop || length == 1)
			{
				return this.curve.Evaluate(t);
			}
			return this.m_InternalLoopingCurve.Evaluate(t);
		}

		// Token: 0x06004107 RID: 16647 RVA: 0x0017EB2F File Offset: 0x0017CD2F
		public float Evaluate(float t)
		{
			return this.Evaluate(t, this.curve.length);
		}

		// Token: 0x06004108 RID: 16648 RVA: 0x0017EB43 File Offset: 0x0017CD43
		public override int GetHashCode()
		{
			return 17 * 23 + this.curve.GetHashCode();
		}

		// Token: 0x04003AA9 RID: 15017
		public const int k_Precision = 128;

		// Token: 0x04003AAA RID: 15018
		public const float k_Step = 0.0078125f;

		// Token: 0x04003AAB RID: 15019
		public AnimationCurve curve;

		// Token: 0x04003AAC RID: 15020
		[SerializeField]
		private bool m_Loop;

		// Token: 0x04003AAD RID: 15021
		[SerializeField]
		private float m_ZeroValue;

		// Token: 0x04003AAE RID: 15022
		[SerializeField]
		private float m_Range;

		// Token: 0x04003AAF RID: 15023
		private AnimationCurve m_InternalLoopingCurve;

		// Token: 0x04003AB0 RID: 15024
		private int frameCount = -1;

		// Token: 0x04003AB1 RID: 15025
		public float[] cachedData;
	}
}
