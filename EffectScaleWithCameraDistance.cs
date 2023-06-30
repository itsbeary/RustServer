using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class EffectScaleWithCameraDistance : MonoBehaviour, IEffect
{
	// Token: 0x04001854 RID: 6228
	public float minScale = 1f;

	// Token: 0x04001855 RID: 6229
	public float maxScale = 2.5f;

	// Token: 0x04001856 RID: 6230
	public float scaleStartDistance = 50f;

	// Token: 0x04001857 RID: 6231
	public float scaleEndDistance = 150f;
}
