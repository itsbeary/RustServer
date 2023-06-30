using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class SoundFade : MonoBehaviour, IClientComponent
{
	// Token: 0x040014D6 RID: 5334
	public SoundFadeHQAudioFilter hqFadeFilter;

	// Token: 0x040014D7 RID: 5335
	public float currentGain = 1f;

	// Token: 0x040014D8 RID: 5336
	public float startingGain;

	// Token: 0x040014D9 RID: 5337
	public float finalGain = 1f;

	// Token: 0x040014DA RID: 5338
	public int sampleRate = 44100;

	// Token: 0x040014DB RID: 5339
	public bool highQualityFadeCompleted;

	// Token: 0x040014DC RID: 5340
	public float length;

	// Token: 0x040014DD RID: 5341
	public SoundFade.Direction currentDirection;

	// Token: 0x02000C98 RID: 3224
	public enum Direction
	{
		// Token: 0x04004467 RID: 17511
		In,
		// Token: 0x04004468 RID: 17512
		Out
	}
}
