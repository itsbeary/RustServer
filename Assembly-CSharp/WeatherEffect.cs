using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public abstract class WeatherEffect : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04002401 RID: 9217
	public ParticleSystem[] emitOnStart;

	// Token: 0x04002402 RID: 9218
	public ParticleSystem[] emitOnStop;

	// Token: 0x04002403 RID: 9219
	public ParticleSystem[] emitOnLoop;
}
