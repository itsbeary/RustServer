using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class BlendShapeController : MonoBehaviour
{
	// Token: 0x040015DF RID: 5599
	public SkinnedMeshRenderer TargetRenderer;

	// Token: 0x040015E0 RID: 5600
	public BlendShapeController.BlendState[] States;

	// Token: 0x040015E1 RID: 5601
	public float LerpSpeed = 0.25f;

	// Token: 0x040015E2 RID: 5602
	public BlendShapeController.BlendMode CurrentMode;

	// Token: 0x02000CA7 RID: 3239
	public enum BlendMode
	{
		// Token: 0x040044B9 RID: 17593
		Idle,
		// Token: 0x040044BA RID: 17594
		Happy,
		// Token: 0x040044BB RID: 17595
		Angry
	}

	// Token: 0x02000CA8 RID: 3240
	[Serializable]
	public struct BlendState
	{
		// Token: 0x040044BC RID: 17596
		[Range(0f, 100f)]
		public float[] States;

		// Token: 0x040044BD RID: 17597
		public BlendShapeController.BlendMode Mode;
	}
}
