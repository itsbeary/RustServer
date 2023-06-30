using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class MiniCopterSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x04000139 RID: 313
	public MiniCopter miniCopter;

	// Token: 0x0400013A RID: 314
	public GameObject soundAttachPoint;

	// Token: 0x0400013B RID: 315
	public SoundDefinition engineStartDef;

	// Token: 0x0400013C RID: 316
	public SoundDefinition engineLoopDef;

	// Token: 0x0400013D RID: 317
	public SoundDefinition engineStopDef;

	// Token: 0x0400013E RID: 318
	public SoundDefinition rotorLoopDef;

	// Token: 0x0400013F RID: 319
	public float engineStartFadeOutTime = 1f;

	// Token: 0x04000140 RID: 320
	public float engineLoopFadeInTime = 0.7f;

	// Token: 0x04000141 RID: 321
	public float engineLoopFadeOutTime = 0.25f;

	// Token: 0x04000142 RID: 322
	public float engineStopFadeOutTime = 0.25f;

	// Token: 0x04000143 RID: 323
	public float rotorLoopFadeInTime = 0.7f;

	// Token: 0x04000144 RID: 324
	public float rotorLoopFadeOutTime = 0.25f;

	// Token: 0x04000145 RID: 325
	public float enginePitchInterpRate = 0.5f;

	// Token: 0x04000146 RID: 326
	public float rotorPitchInterpRate = 1f;

	// Token: 0x04000147 RID: 327
	public float rotorGainInterpRate = 0.5f;

	// Token: 0x04000148 RID: 328
	public float rotorStartStopPitchRateUp = 7f;

	// Token: 0x04000149 RID: 329
	public float rotorStartStopPitchRateDown = 9f;

	// Token: 0x0400014A RID: 330
	public float rotorStartStopGainRateUp = 5f;

	// Token: 0x0400014B RID: 331
	public float rotorStartStopGainRateDown = 4f;

	// Token: 0x0400014C RID: 332
	public AnimationCurve engineUpDotPitchCurve;

	// Token: 0x0400014D RID: 333
	public AnimationCurve rotorUpDotPitchCurve;
}
