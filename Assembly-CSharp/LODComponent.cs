using System;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public abstract class LODComponent : BaseMonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x04002262 RID: 8802
	public LODDistanceMode DistanceMode;

	// Token: 0x04002263 RID: 8803
	public LODComponent.OccludeeParameters OccludeeParams = new LODComponent.OccludeeParameters
	{
		isDynamic = false,
		dynamicUpdateInterval = 0.2f,
		shadowRangeScale = 3f,
		showBounds = false,
		forceVisible = false
	};

	// Token: 0x02000D58 RID: 3416
	[Serializable]
	public struct OccludeeParameters
	{
		// Token: 0x04004799 RID: 18329
		[Tooltip("Is Occludee dynamic or static?")]
		public bool isDynamic;

		// Token: 0x0400479A RID: 18330
		[Tooltip("Dynamic occludee update interval in seconds; 0 = every frame")]
		public float dynamicUpdateInterval;

		// Token: 0x0400479B RID: 18331
		[Tooltip("Distance scale combined with occludee max bounds size at which culled occludee shadows are still visible")]
		public float shadowRangeScale;

		// Token: 0x0400479C RID: 18332
		[Tooltip("Show culling bounds via gizmos; editor only")]
		public bool showBounds;

		// Token: 0x0400479D RID: 18333
		[Tooltip("Force Occludee always visible?")]
		public bool forceVisible;
	}
}
