using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A89 RID: 2697
	[Serializable]
	public sealed class ColorParameter : ParameterOverride<Color>
	{
		// Token: 0x06004026 RID: 16422 RVA: 0x0017A5FC File Offset: 0x001787FC
		public override void Interp(Color from, Color to, float t)
		{
			this.value.r = from.r + (to.r - from.r) * t;
			this.value.g = from.g + (to.g - from.g) * t;
			this.value.b = from.b + (to.b - from.b) * t;
			this.value.a = from.a + (to.a - from.a) * t;
		}

		// Token: 0x06004027 RID: 16423 RVA: 0x0017A68D File Offset: 0x0017888D
		public static implicit operator Vector4(ColorParameter prop)
		{
			return prop.value;
		}
	}
}
