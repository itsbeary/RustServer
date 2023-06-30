using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020000FF RID: 255
public class NVGCamera : FacepunchBehaviour, IClothingChanged
{
	// Token: 0x04000DE2 RID: 3554
	public static NVGCamera instance;

	// Token: 0x04000DE3 RID: 3555
	public PostProcessVolume postProcessVolume;

	// Token: 0x04000DE4 RID: 3556
	public GameObject lights;
}
