using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007EA RID: 2026
public class GameStat : MonoBehaviour
{
	// Token: 0x04002D8C RID: 11660
	public float refreshTime = 5f;

	// Token: 0x04002D8D RID: 11661
	public Text title;

	// Token: 0x04002D8E RID: 11662
	public Text globalStat;

	// Token: 0x04002D8F RID: 11663
	public Text localStat;

	// Token: 0x04002D90 RID: 11664
	private long globalValue;

	// Token: 0x04002D91 RID: 11665
	private long localValue;

	// Token: 0x04002D92 RID: 11666
	private float secondsSinceRefresh;

	// Token: 0x04002D93 RID: 11667
	private float secondsUntilUpdate;

	// Token: 0x04002D94 RID: 11668
	private float secondsUntilChange;

	// Token: 0x04002D95 RID: 11669
	public GameStat.Stat[] stats;

	// Token: 0x02000E8E RID: 3726
	[Serializable]
	public struct Stat
	{
		// Token: 0x04004C7F RID: 19583
		public string statName;

		// Token: 0x04004C80 RID: 19584
		public string statTitle;
	}
}
