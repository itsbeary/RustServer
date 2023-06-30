using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8D RID: 2701
	[Serializable]
	public sealed class SplineParameter : ParameterOverride<Spline>
	{
		// Token: 0x06004035 RID: 16437 RVA: 0x0017A85F File Offset: 0x00178A5F
		protected internal override void OnEnable()
		{
			if (this.value != null)
			{
				this.value.Cache(int.MinValue);
			}
		}

		// Token: 0x06004036 RID: 16438 RVA: 0x0017A879 File Offset: 0x00178A79
		internal override void SetValue(ParameterOverride parameter)
		{
			base.SetValue(parameter);
			if (this.value != null)
			{
				this.value.Cache(Time.renderedFrameCount);
			}
		}

		// Token: 0x06004037 RID: 16439 RVA: 0x0017A89C File Offset: 0x00178A9C
		public override void Interp(Spline from, Spline to, float t)
		{
			if (from == null || to == null)
			{
				base.Interp(from, to, t);
				return;
			}
			int renderedFrameCount = Time.renderedFrameCount;
			from.Cache(renderedFrameCount);
			to.Cache(renderedFrameCount);
			for (int i = 0; i < 128; i++)
			{
				float num = from.cachedData[i];
				float num2 = to.cachedData[i];
				this.value.cachedData[i] = num + (num2 - num) * t;
			}
		}
	}
}
