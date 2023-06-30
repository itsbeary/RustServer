using System;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class TrainEngineAudio : TrainCarAudio
{
	// Token: 0x04002041 RID: 8257
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04002042 RID: 8258
	[SerializeField]
	private Transform cockpitSoundPosition;

	// Token: 0x04002043 RID: 8259
	[SerializeField]
	private Transform hornSoundPosition;

	// Token: 0x04002044 RID: 8260
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04002045 RID: 8261
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04002046 RID: 8262
	[SerializeField]
	private SoundDefinition engineActiveLoopDef;

	// Token: 0x04002047 RID: 8263
	[SerializeField]
	private AnimationCurve engineActiveLoopPitchCurve;

	// Token: 0x04002048 RID: 8264
	[SerializeField]
	private float engineActiveLoopChangeSpeed = 0.2f;

	// Token: 0x04002049 RID: 8265
	private Sound engineActiveLoop;

	// Token: 0x0400204A RID: 8266
	private SoundModulation.Modulator engineActiveLoopPitch;

	// Token: 0x0400204B RID: 8267
	[SerializeField]
	private BlendedLoopEngineSound engineLoops;

	// Token: 0x0400204C RID: 8268
	[SerializeField]
	private TrainEngineAudio.EngineReflection[] engineReflections;

	// Token: 0x0400204D RID: 8269
	[SerializeField]
	private LayerMask reflectionLayerMask;

	// Token: 0x0400204E RID: 8270
	[SerializeField]
	private float reflectionMaxDistance = 20f;

	// Token: 0x0400204F RID: 8271
	[SerializeField]
	private float reflectionGainChangeSpeed = 10f;

	// Token: 0x04002050 RID: 8272
	[SerializeField]
	private float reflectionPositionChangeSpeed = 10f;

	// Token: 0x04002051 RID: 8273
	[SerializeField]
	private float reflectionRayOffset = 0.5f;

	// Token: 0x04002052 RID: 8274
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04002053 RID: 8275
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04002054 RID: 8276
	[Header("Other")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;

	// Token: 0x04002055 RID: 8277
	[SerializeField]
	private SoundDefinition proximityAlertDef;

	// Token: 0x04002056 RID: 8278
	private Sound proximityAlertSound;

	// Token: 0x04002057 RID: 8279
	[SerializeField]
	private SoundDefinition damagedLoopDef;

	// Token: 0x04002058 RID: 8280
	private Sound damagedLoop;

	// Token: 0x04002059 RID: 8281
	[SerializeField]
	private SoundDefinition changeThrottleDef;

	// Token: 0x0400205A RID: 8282
	[SerializeField]
	private SoundDefinition changeCouplingDef;

	// Token: 0x0400205B RID: 8283
	[SerializeField]
	private SoundDefinition unloadableStartDef;

	// Token: 0x0400205C RID: 8284
	[SerializeField]
	private SoundDefinition unloadableEndDef;

	// Token: 0x0400205D RID: 8285
	[SerializeField]
	private GameObject bellObject;

	// Token: 0x0400205E RID: 8286
	[SerializeField]
	private SoundDefinition bellRingDef;

	// Token: 0x0400205F RID: 8287
	[SerializeField]
	private SoundPlayer brakeSound;

	// Token: 0x02000D29 RID: 3369
	[Serializable]
	public class EngineReflection
	{
		// Token: 0x040046ED RID: 18157
		public Vector3 direction;

		// Token: 0x040046EE RID: 18158
		public Vector3 offset;

		// Token: 0x040046EF RID: 18159
		public SoundDefinition soundDef;

		// Token: 0x040046F0 RID: 18160
		public Sound sound;

		// Token: 0x040046F1 RID: 18161
		public SoundModulation.Modulator pitchMod;

		// Token: 0x040046F2 RID: 18162
		public SoundModulation.Modulator gainMod;

		// Token: 0x040046F3 RID: 18163
		public float distance = 20f;
	}
}
