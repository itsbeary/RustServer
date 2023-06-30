using System;
using UnityEngine.Playables;

// Token: 0x02000772 RID: 1906
[Serializable]
public class TimelineConvarPlayable : PlayableBehaviour
{
	// Token: 0x060034C1 RID: 13505 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
	}

	// Token: 0x04002B5A RID: 11098
	[NonSerialized]
	public string convar;

	// Token: 0x04002B5B RID: 11099
	public float ConvarValue;
}
