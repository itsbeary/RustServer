using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009C5 RID: 2501
	[Serializable]
	public class Preset
	{
		// Token: 0x04003690 RID: 13968
		public bool DiagDetection = true;

		// Token: 0x04003691 RID: 13969
		public bool CornerDetection = true;

		// Token: 0x04003692 RID: 13970
		[Range(0f, 0.5f)]
		public float Threshold = 0.1f;

		// Token: 0x04003693 RID: 13971
		[Min(0.0001f)]
		public float DepthThreshold = 0.01f;

		// Token: 0x04003694 RID: 13972
		[Range(0f, 112f)]
		public int MaxSearchSteps = 16;

		// Token: 0x04003695 RID: 13973
		[Range(0f, 20f)]
		public int MaxSearchStepsDiag = 8;

		// Token: 0x04003696 RID: 13974
		[Range(0f, 100f)]
		public int CornerRounding = 25;

		// Token: 0x04003697 RID: 13975
		[Min(0f)]
		public float LocalContrastAdaptationFactor = 2f;
	}
}
