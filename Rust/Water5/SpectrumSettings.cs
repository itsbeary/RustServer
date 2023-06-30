using System;
using UnityEngine;

namespace Rust.Water5
{
	// Token: 0x02000B2F RID: 2863
	[Serializable]
	public struct SpectrumSettings
	{
		// Token: 0x04003E3E RID: 15934
		[Range(0f, 1f)]
		public float scale;

		// Token: 0x04003E3F RID: 15935
		public float windSpeed;

		// Token: 0x04003E40 RID: 15936
		public float fetch;

		// Token: 0x04003E41 RID: 15937
		[Range(0f, 1f)]
		public float spreadBlend;

		// Token: 0x04003E42 RID: 15938
		[Range(0f, 1f)]
		public float swell;

		// Token: 0x04003E43 RID: 15939
		public float peakEnhancement;

		// Token: 0x04003E44 RID: 15940
		public float shortWavesFade;
	}
}
