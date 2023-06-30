using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class SoundModulation : MonoBehaviour, IClientComponent
{
	// Token: 0x040014E4 RID: 5348
	private const int parameterCount = 4;

	// Token: 0x02000C99 RID: 3225
	public enum Parameter
	{
		// Token: 0x0400446A RID: 17514
		Gain,
		// Token: 0x0400446B RID: 17515
		Pitch,
		// Token: 0x0400446C RID: 17516
		Spread,
		// Token: 0x0400446D RID: 17517
		MaxDistance
	}

	// Token: 0x02000C9A RID: 3226
	[Serializable]
	public class Modulator
	{
		// Token: 0x0400446E RID: 17518
		public SoundModulation.Parameter param;

		// Token: 0x0400446F RID: 17519
		public float value = 1f;
	}
}
