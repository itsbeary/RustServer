using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009C4 RID: 2500
	[Serializable]
	public class PredicationPreset
	{
		// Token: 0x0400368D RID: 13965
		[Min(0.0001f)]
		public float Threshold = 0.01f;

		// Token: 0x0400368E RID: 13966
		[Range(1f, 5f)]
		public float Scale = 2f;

		// Token: 0x0400368F RID: 13967
		[Range(0f, 1f)]
		public float Strength = 0.4f;
	}
}
