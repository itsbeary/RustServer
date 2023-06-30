using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A86 RID: 2694
	[Serializable]
	public sealed class FloatParameter : ParameterOverride<float>
	{
		// Token: 0x06004021 RID: 16417 RVA: 0x0017A5C8 File Offset: 0x001787C8
		public override void Interp(float from, float to, float t)
		{
			this.value = from + (to - from) * t;
		}
	}
}
