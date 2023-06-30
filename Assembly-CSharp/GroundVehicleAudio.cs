using System;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public abstract class GroundVehicleAudio : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E80 RID: 7808
	[SerializeField]
	protected GroundVehicle groundVehicle;

	// Token: 0x04001E81 RID: 7809
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001E82 RID: 7810
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001E83 RID: 7811
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001E84 RID: 7812
	[SerializeField]
	private BlendedLoopEngineSound blendedEngineLoops;

	// Token: 0x04001E85 RID: 7813
	[SerializeField]
	private float wheelRatioMultiplier = 600f;

	// Token: 0x04001E86 RID: 7814
	[Header("Water")]
	[SerializeField]
	private SoundDefinition waterSplashSoundDef;

	// Token: 0x04001E87 RID: 7815
	[SerializeField]
	private BlendedSoundLoops waterLoops;

	// Token: 0x04001E88 RID: 7816
	[SerializeField]
	private float waterSoundsMaxSpeed = 10f;

	// Token: 0x04001E89 RID: 7817
	[Header("Brakes")]
	[SerializeField]
	private SoundDefinition brakeSoundDef;

	// Token: 0x04001E8A RID: 7818
	[Header("Lights")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;
}
