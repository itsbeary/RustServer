using System;
using UnityEngine;

// Token: 0x02000334 RID: 820
public class AnimationEvents : BaseMonoBehaviour
{
	// Token: 0x06001F40 RID: 8000 RVA: 0x000D4312 File Offset: 0x000D2512
	protected void OnEnable()
	{
		if (this.rootObject == null)
		{
			this.rootObject = base.transform;
		}
	}

	// Token: 0x0400182E RID: 6190
	public Transform rootObject;

	// Token: 0x0400182F RID: 6191
	public HeldEntity targetEntity;

	// Token: 0x04001830 RID: 6192
	[Tooltip("Path to the effect folder for these animations. Relative to this object.")]
	public string effectFolder;

	// Token: 0x04001831 RID: 6193
	public bool enforceClipWeights;

	// Token: 0x04001832 RID: 6194
	public string localFolder;

	// Token: 0x04001833 RID: 6195
	[Tooltip("If true the localFolder field won't update with manifest updates, use for custom paths")]
	public bool customLocalFolder;

	// Token: 0x04001834 RID: 6196
	public bool IsBusy;
}
