using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class CargoShipSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x040000F0 RID: 240
	public SoundDefinition waveSoundDef;

	// Token: 0x040000F1 RID: 241
	public AnimationCurve waveSoundYGainCurve;

	// Token: 0x040000F2 RID: 242
	public AnimationCurve waveSoundEdgeDistanceGainCurve;

	// Token: 0x040000F3 RID: 243
	private Sound waveSoundL;

	// Token: 0x040000F4 RID: 244
	private Sound waveSoundR;

	// Token: 0x040000F5 RID: 245
	private SoundModulation.Modulator waveSoundLGainMod;

	// Token: 0x040000F6 RID: 246
	private SoundModulation.Modulator waveSoundRGainMod;

	// Token: 0x040000F7 RID: 247
	public SoundDefinition sternWakeSoundDef;

	// Token: 0x040000F8 RID: 248
	private Sound sternWakeSound;

	// Token: 0x040000F9 RID: 249
	private SoundModulation.Modulator sternWakeSoundGainMod;

	// Token: 0x040000FA RID: 250
	public SoundDefinition engineHumSoundDef;

	// Token: 0x040000FB RID: 251
	private Sound engineHumSound;

	// Token: 0x040000FC RID: 252
	public GameObject engineHumTarget;

	// Token: 0x040000FD RID: 253
	public SoundDefinition hugeRumbleSoundDef;

	// Token: 0x040000FE RID: 254
	public AnimationCurve hugeRumbleYDiffCurve;

	// Token: 0x040000FF RID: 255
	public AnimationCurve hugeRumbleRelativeSpeedCurve;

	// Token: 0x04000100 RID: 256
	private Sound hugeRumbleSound;

	// Token: 0x04000101 RID: 257
	private SoundModulation.Modulator hugeRumbleGainMod;

	// Token: 0x04000102 RID: 258
	private Vector3 lastCameraPos;

	// Token: 0x04000103 RID: 259
	private Vector3 lastRumblePos;

	// Token: 0x04000104 RID: 260
	private Vector3 lastRumbleLocalPos;

	// Token: 0x04000105 RID: 261
	public Collider soundFollowCollider;

	// Token: 0x04000106 RID: 262
	public Collider soundFollowColliderL;

	// Token: 0x04000107 RID: 263
	public Collider soundFollowColliderR;

	// Token: 0x04000108 RID: 264
	public Collider sternSoundFollowCollider;
}
