using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
	// Token: 0x040013FE RID: 5118
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x040013FF RID: 5119
	public float blendSmoothing = 1f;

	// Token: 0x04001400 RID: 5120
	public float loopFadeOutTime = 0.5f;

	// Token: 0x04001401 RID: 5121
	public float loopFadeInTime = 0.5f;

	// Token: 0x04001402 RID: 5122
	public float gainModSmoothing = 1f;

	// Token: 0x04001403 RID: 5123
	public float pitchModSmoothing = 1f;

	// Token: 0x04001404 RID: 5124
	public bool shouldPlay = true;

	// Token: 0x04001405 RID: 5125
	public float gain = 1f;

	// Token: 0x04001406 RID: 5126
	public List<BlendedSoundLoops.Loop> loops = new List<BlendedSoundLoops.Loop>();

	// Token: 0x04001407 RID: 5127
	public float maxDistance;

	// Token: 0x02000C8A RID: 3210
	[Serializable]
	public class Loop
	{
		// Token: 0x04004410 RID: 17424
		public SoundDefinition soundDef;

		// Token: 0x04004411 RID: 17425
		public AnimationCurve gainCurve;

		// Token: 0x04004412 RID: 17426
		public AnimationCurve pitchCurve;

		// Token: 0x04004413 RID: 17427
		[HideInInspector]
		public Sound sound;

		// Token: 0x04004414 RID: 17428
		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		// Token: 0x04004415 RID: 17429
		[HideInInspector]
		public SoundModulation.Modulator pitchMod;
	}
}
