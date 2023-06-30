using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class WeightedAnimationRandomiser : StateMachineBehaviour
{
	// Token: 0x040013B9 RID: 5049
	public int LoopRangeMin = 3;

	// Token: 0x040013BA RID: 5050
	public int LoopRangeMax = 5;

	// Token: 0x040013BB RID: 5051
	public float NormalizedTransitionDuration;

	// Token: 0x040013BC RID: 5052
	public WeightedAnimationRandomiser.IdleChance[] IdleTransitions = new WeightedAnimationRandomiser.IdleChance[0];

	// Token: 0x040013BD RID: 5053
	public bool AllowRepeats;

	// Token: 0x02000C84 RID: 3204
	[Serializable]
	public struct IdleChance
	{
		// Token: 0x040043FC RID: 17404
		public string StateName;

		// Token: 0x040043FD RID: 17405
		[Range(0f, 100f)]
		public int Chance;
	}
}
