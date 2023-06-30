using System;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class ParticleEmissionSound : FacepunchBehaviour, IClientComponent, ILOD
{
	// Token: 0x04001893 RID: 6291
	public ParticleSystem particleSystem;

	// Token: 0x04001894 RID: 6292
	public SoundDefinition soundDefinition;

	// Token: 0x04001895 RID: 6293
	public float soundCooldown;
}
