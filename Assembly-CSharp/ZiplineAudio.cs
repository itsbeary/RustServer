using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class ZiplineAudio : MonoBehaviour
{
	// Token: 0x04000DA5 RID: 3493
	public ZiplineMountable zipline;

	// Token: 0x04000DA6 RID: 3494
	public SoundDefinition movementLoopDef;

	// Token: 0x04000DA7 RID: 3495
	public SoundDefinition frictionLoopDef;

	// Token: 0x04000DA8 RID: 3496
	public SoundDefinition sparksLoopDef;

	// Token: 0x04000DA9 RID: 3497
	public AnimationCurve movementGainCurve;

	// Token: 0x04000DAA RID: 3498
	public AnimationCurve movementPitchCurve;

	// Token: 0x04000DAB RID: 3499
	public AnimationCurve frictionGainCurve;

	// Token: 0x04000DAC RID: 3500
	public AnimationCurve sparksGainCurve;
}
