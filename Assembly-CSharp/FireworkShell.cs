using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class FireworkShell : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400004C RID: 76
	public float fuseLengthMin;

	// Token: 0x0400004D RID: 77
	public float fuseLengthMax;

	// Token: 0x0400004E RID: 78
	public float speedMin;

	// Token: 0x0400004F RID: 79
	public float speedMax;

	// Token: 0x04000050 RID: 80
	public ParticleSystem explodePFX;

	// Token: 0x04000051 RID: 81
	public SoundPlayer explodeSound;

	// Token: 0x04000052 RID: 82
	public float inaccuracyDegrees;

	// Token: 0x04000053 RID: 83
	public LightEx explosionLight;

	// Token: 0x04000054 RID: 84
	public float lifetime = 8f;
}
