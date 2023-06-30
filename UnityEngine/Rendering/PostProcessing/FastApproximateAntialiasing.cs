using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A68 RID: 2664
	[Preserve]
	[Serializable]
	public sealed class FastApproximateAntialiasing
	{
		// Token: 0x0400393F RID: 14655
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performances by lowering the effect quality. This setting is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public bool fastMode;

		// Token: 0x04003940 RID: 14656
		[Tooltip("Keep alpha channel. This will slightly lower the effect quality but allows rendering against a transparent background.")]
		public bool keepAlpha;
	}
}
