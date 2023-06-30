using System;
using UnityEngine.Serialization;

// Token: 0x02000508 RID: 1288
public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle
{
	// Token: 0x0400218A RID: 8586
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float detachTime;

	// Token: 0x0400218B RID: 8587
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float recycleTime;

	// Token: 0x0400218C RID: 8588
	public EffectRecycle.PlayMode playMode;

	// Token: 0x0400218D RID: 8589
	public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

	// Token: 0x02000D4C RID: 3404
	public enum PlayMode
	{
		// Token: 0x04004765 RID: 18277
		Once,
		// Token: 0x04004766 RID: 18278
		Looped
	}

	// Token: 0x02000D4D RID: 3405
	public enum ParentDestroyBehaviour
	{
		// Token: 0x04004768 RID: 18280
		Detach,
		// Token: 0x04004769 RID: 18281
		Destroy,
		// Token: 0x0400476A RID: 18282
		DetachWaitDestroy
	}
}
