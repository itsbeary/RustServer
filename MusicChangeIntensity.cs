using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class MusicChangeIntensity : MonoBehaviour
{
	// Token: 0x0400144A RID: 5194
	public float raiseTo;

	// Token: 0x0400144B RID: 5195
	public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities = new List<MusicChangeIntensity.DistanceIntensity>();

	// Token: 0x0400144C RID: 5196
	public float tickInterval = 0.2f;

	// Token: 0x02000C90 RID: 3216
	[Serializable]
	public class DistanceIntensity
	{
		// Token: 0x0400443B RID: 17467
		public float distance = 60f;

		// Token: 0x0400443C RID: 17468
		public float raiseTo;

		// Token: 0x0400443D RID: 17469
		public bool forceStartMusicInSuppressedMusicZones;
	}
}
