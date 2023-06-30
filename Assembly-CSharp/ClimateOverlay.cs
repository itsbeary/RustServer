using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200029B RID: 667
public class ClimateOverlay : MonoBehaviour
{
	// Token: 0x0400160F RID: 5647
	[Range(0f, 1f)]
	public float blendingSpeed = 1f;

	// Token: 0x04001610 RID: 5648
	public PostProcessVolume[] biomeVolumes;
}
