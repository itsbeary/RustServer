using System;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class SubmarineAudio : MonoBehaviour
{
	// Token: 0x04001FAF RID: 8111
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001FB0 RID: 8112
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001FB1 RID: 8113
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001FB2 RID: 8114
	[SerializeField]
	private SoundDefinition engineLoopSound;

	// Token: 0x04001FB3 RID: 8115
	[SerializeField]
	private AnimationCurve engineLoopPitchCurve;

	// Token: 0x04001FB4 RID: 8116
	[Header("Water")]
	[SerializeField]
	private SoundDefinition underwaterLoopDef;

	// Token: 0x04001FB5 RID: 8117
	[SerializeField]
	private SoundDefinition underwaterMovementLoopDef;

	// Token: 0x04001FB6 RID: 8118
	[SerializeField]
	private BlendedSoundLoops surfaceWaterLoops;

	// Token: 0x04001FB7 RID: 8119
	[SerializeField]
	private float surfaceWaterSoundsMaxSpeed = 5f;

	// Token: 0x04001FB8 RID: 8120
	[SerializeField]
	private SoundDefinition waterEmergeSoundDef;

	// Token: 0x04001FB9 RID: 8121
	[SerializeField]
	private SoundDefinition waterSubmergeSoundDef;

	// Token: 0x04001FBA RID: 8122
	[Header("Interior")]
	[SerializeField]
	private SoundDefinition activeLoopDef;

	// Token: 0x04001FBB RID: 8123
	[SerializeField]
	private SoundDefinition footPedalSoundDef;

	// Token: 0x04001FBC RID: 8124
	[SerializeField]
	private Transform footPedalSoundPos;

	// Token: 0x04001FBD RID: 8125
	[SerializeField]
	private SoundDefinition steeringWheelSoundDef;

	// Token: 0x04001FBE RID: 8126
	[SerializeField]
	private Transform steeringWheelSoundPos;

	// Token: 0x04001FBF RID: 8127
	[SerializeField]
	private SoundDefinition heavyDamageSparksDef;

	// Token: 0x04001FC0 RID: 8128
	[SerializeField]
	private Transform heavyDamageSparksPos;

	// Token: 0x04001FC1 RID: 8129
	[SerializeField]
	private SoundDefinition flagRaise;

	// Token: 0x04001FC2 RID: 8130
	[SerializeField]
	private SoundDefinition flagLower;

	// Token: 0x04001FC3 RID: 8131
	[SerializeField]
	private GameObject dashboardAudioPoint;

	// Token: 0x04001FC4 RID: 8132
	[Header("Other")]
	[SerializeField]
	private SoundDefinition climbOrDiveLoopSound;

	// Token: 0x04001FC5 RID: 8133
	[SerializeField]
	private SoundDefinition torpedoFailedSound;
}
