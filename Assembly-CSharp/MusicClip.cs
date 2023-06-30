using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class MusicClip : ScriptableObject
{
	// Token: 0x06001C1F RID: 7199 RVA: 0x000C4EF8 File Offset: 0x000C30F8
	public float GetNextFadeInPoint(float currentClipTimeBars)
	{
		if (this.fadeInPoints.Count == 0)
		{
			return currentClipTimeBars + 0.125f;
		}
		float num = -1f;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < this.fadeInPoints.Count; i++)
		{
			float num3 = this.fadeInPoints[i];
			float num4 = num3 - currentClipTimeBars;
			if (num3 > 0.01f && num4 > 0f && num4 < num2)
			{
				num2 = num4;
				num = num3;
			}
		}
		return num;
	}

	// Token: 0x0400144D RID: 5197
	public AudioClip audioClip;

	// Token: 0x0400144E RID: 5198
	public int lengthInBars = 1;

	// Token: 0x0400144F RID: 5199
	public int lengthInBarsWithTail;

	// Token: 0x04001450 RID: 5200
	public List<float> fadeInPoints = new List<float>();
}
