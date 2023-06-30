using System;
using UnityEngine;

// Token: 0x02000970 RID: 2416
public class FishingRodViewmodel : MonoBehaviour
{
	// Token: 0x04003437 RID: 13367
	public Transform PitchTransform;

	// Token: 0x04003438 RID: 13368
	public Transform YawTransform;

	// Token: 0x04003439 RID: 13369
	public float YawLerpSpeed = 1f;

	// Token: 0x0400343A RID: 13370
	public float PitchLerpSpeed = 1f;

	// Token: 0x0400343B RID: 13371
	public Transform LineRendererStartPos;

	// Token: 0x0400343C RID: 13372
	public ParticleSystem[] StrainParticles;

	// Token: 0x0400343D RID: 13373
	public bool ApplyTransformRotation = true;

	// Token: 0x0400343E RID: 13374
	public GameObject CatchRoot;

	// Token: 0x0400343F RID: 13375
	public Transform CatchLinePoint;

	// Token: 0x04003440 RID: 13376
	public FishingRodViewmodel.FishViewmodel[] FishViewmodels;

	// Token: 0x04003441 RID: 13377
	public float ShakeMaxScale = 0.1f;

	// Token: 0x02000ED9 RID: 3801
	[Serializable]
	public struct FishViewmodel
	{
		// Token: 0x04004D8D RID: 19853
		public ItemDefinition Item;

		// Token: 0x04004D8E RID: 19854
		public GameObject Root;
	}
}
