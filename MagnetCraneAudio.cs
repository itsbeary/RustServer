using System;
using UnityEngine;

// Token: 0x02000496 RID: 1174
public class MagnetCraneAudio : MonoBehaviour
{
	// Token: 0x04001F0B RID: 7947
	public MagnetCrane crane;

	// Token: 0x04001F0C RID: 7948
	[Header("Sound defs")]
	public SoundDefinition engineStartSoundDef;

	// Token: 0x04001F0D RID: 7949
	public SoundDefinition engineStopSoundDef;

	// Token: 0x04001F0E RID: 7950
	public BlendedLoopEngineSound engineLoops;

	// Token: 0x04001F0F RID: 7951
	public SoundDefinition cabinRotationStartDef;

	// Token: 0x04001F10 RID: 7952
	public SoundDefinition cabinRotationStopDef;

	// Token: 0x04001F11 RID: 7953
	public SoundDefinition cabinRotationLoopDef;

	// Token: 0x04001F12 RID: 7954
	private Sound cabinRotationLoop;

	// Token: 0x04001F13 RID: 7955
	public SoundDefinition turningLoopDef;

	// Token: 0x04001F14 RID: 7956
	private Sound turningLoop;

	// Token: 0x04001F15 RID: 7957
	public SoundDefinition trackMovementLoopDef;

	// Token: 0x04001F16 RID: 7958
	private Sound trackMovementLoop;

	// Token: 0x04001F17 RID: 7959
	private SoundModulation.Modulator trackGainMod;

	// Token: 0x04001F18 RID: 7960
	private SoundModulation.Modulator trackPitchMod;

	// Token: 0x04001F19 RID: 7961
	public SoundDefinition armMovementLoopDef;

	// Token: 0x04001F1A RID: 7962
	public SoundDefinition armMovementStartDef;

	// Token: 0x04001F1B RID: 7963
	public SoundDefinition armMovementStopDef;

	// Token: 0x04001F1C RID: 7964
	private Sound armMovementLoop01;

	// Token: 0x04001F1D RID: 7965
	private SoundModulation.Modulator armMovementLoop01PitchMod;

	// Token: 0x04001F1E RID: 7966
	public GameObject arm01SoundPosition;

	// Token: 0x04001F1F RID: 7967
	public GameObject arm02SoundPosition;

	// Token: 0x04001F20 RID: 7968
	private Sound armMovementLoop02;

	// Token: 0x04001F21 RID: 7969
	private SoundModulation.Modulator armMovementLoop02PitchMod;
}
