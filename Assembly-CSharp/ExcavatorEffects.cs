using System;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public class ExcavatorEffects : MonoBehaviour
{
	// Token: 0x04001C7D RID: 7293
	public static ExcavatorEffects instance;

	// Token: 0x04001C7E RID: 7294
	public ParticleSystemContainer[] miningParticles;

	// Token: 0x04001C7F RID: 7295
	public SoundPlayer[] miningSounds;

	// Token: 0x04001C80 RID: 7296
	public SoundFollowCollider[] beltSounds;

	// Token: 0x04001C81 RID: 7297
	public SoundPlayer[] miningStartSounds;

	// Token: 0x04001C82 RID: 7298
	public GameObject[] ambientMetalRattles;

	// Token: 0x04001C83 RID: 7299
	public bool wasMining;
}
