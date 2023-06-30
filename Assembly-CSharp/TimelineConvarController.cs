using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Token: 0x02000771 RID: 1905
[Serializable]
public class TimelineConvarController : PlayableAsset, ITimelineClipAsset
{
	// Token: 0x060034BE RID: 13502 RVA: 0x001453A0 File Offset: 0x001435A0
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<TimelineConvarPlayable> scriptPlayable = ScriptPlayable<TimelineConvarPlayable>.Create(graph, this.template, 0);
		scriptPlayable.GetBehaviour().convar = this.convarName;
		return scriptPlayable;
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x060034BF RID: 13503 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.Extrapolation;
		}
	}

	// Token: 0x04002B58 RID: 11096
	public string convarName = string.Empty;

	// Token: 0x04002B59 RID: 11097
	public TimelineConvarPlayable template = new TimelineConvarPlayable();
}
