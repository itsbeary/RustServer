using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A87 RID: 2695
	[Serializable]
	public sealed class IntParameter : ParameterOverride<int>
	{
		// Token: 0x06004023 RID: 16419 RVA: 0x0017A5DF File Offset: 0x001787DF
		public override void Interp(int from, int to, float t)
		{
			this.value = (int)((float)from + (float)(to - from) * t);
		}
	}
}
