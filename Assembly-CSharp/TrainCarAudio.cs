using System;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
public class TrainCarAudio : MonoBehaviour
{
	// Token: 0x0400200D RID: 8205
	[Header("Train Car Audio")]
	[SerializeField]
	private TrainCar trainCar;

	// Token: 0x0400200E RID: 8206
	[SerializeField]
	private SoundDefinition movementStartDef;

	// Token: 0x0400200F RID: 8207
	[SerializeField]
	private SoundDefinition movementStopDef;

	// Token: 0x04002010 RID: 8208
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04002011 RID: 8209
	[SerializeField]
	private AnimationCurve movementLoopGainCurve;

	// Token: 0x04002012 RID: 8210
	[SerializeField]
	private float movementChangeOneshotDebounce = 1f;

	// Token: 0x04002013 RID: 8211
	private Sound movementLoop;

	// Token: 0x04002014 RID: 8212
	private SoundModulation.Modulator movementLoopGain;

	// Token: 0x04002015 RID: 8213
	[SerializeField]
	private SoundDefinition turnLoopDef;

	// Token: 0x04002016 RID: 8214
	private Sound turnLoop;

	// Token: 0x04002017 RID: 8215
	[SerializeField]
	private SoundDefinition trackClatterLoopDef;

	// Token: 0x04002018 RID: 8216
	[SerializeField]
	private AnimationCurve trackClatterGainCurve;

	// Token: 0x04002019 RID: 8217
	[SerializeField]
	private AnimationCurve trackClatterPitchCurve;

	// Token: 0x0400201A RID: 8218
	private Sound trackClatterLoop;

	// Token: 0x0400201B RID: 8219
	private SoundModulation.Modulator trackClatterGain;

	// Token: 0x0400201C RID: 8220
	private SoundModulation.Modulator trackClatterPitch;
}
