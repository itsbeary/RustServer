using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class SoundFollowCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x040014DE RID: 5342
	public SoundDefinition soundDefinition;

	// Token: 0x040014DF RID: 5343
	public Sound sound;

	// Token: 0x040014E0 RID: 5344
	public Bounds soundFollowBounds;

	// Token: 0x040014E1 RID: 5345
	public bool startImmediately;
}
