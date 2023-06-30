using System;
using UnityEngine;

// Token: 0x020004AC RID: 1196
public class SnowmobileAudio : GroundVehicleAudio
{
	// Token: 0x04001F8D RID: 8077
	[Header("Engine")]
	[SerializeField]
	private EngineAudioSet engineAudioSet;

	// Token: 0x04001F8E RID: 8078
	[Header("Skis")]
	[SerializeField]
	private AnimationCurve skiGainCurve;

	// Token: 0x04001F8F RID: 8079
	[SerializeField]
	private SoundDefinition skiSlideSoundDef;

	// Token: 0x04001F90 RID: 8080
	[SerializeField]
	private SoundDefinition skiSlideSnowSoundDef;

	// Token: 0x04001F91 RID: 8081
	[SerializeField]
	private SoundDefinition skiSlideSandSoundDef;

	// Token: 0x04001F92 RID: 8082
	[SerializeField]
	private SoundDefinition skiSlideGrassSoundDef;

	// Token: 0x04001F93 RID: 8083
	[SerializeField]
	private SoundDefinition skiSlideWaterSoundDef;

	// Token: 0x04001F94 RID: 8084
	[Header("Movement")]
	[SerializeField]
	private AnimationCurve movementGainCurve;

	// Token: 0x04001F95 RID: 8085
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04001F96 RID: 8086
	[SerializeField]
	private SoundDefinition suspensionLurchSoundDef;

	// Token: 0x04001F97 RID: 8087
	[SerializeField]
	private float suspensionLurchMinExtensionDelta = 0.4f;

	// Token: 0x04001F98 RID: 8088
	[SerializeField]
	private float suspensionLurchMinTimeBetweenSounds = 0.25f;
}
