using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06001E55 RID: 7765 RVA: 0x000CEAD6 File Offset: 0x000CCCD6
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		GameManager.Destroy(base.gameObject, (this.destroyAfterSeconds <= 0f) ? 1f : this.destroyAfterSeconds);
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x000CEB09 File Offset: 0x000CCD09
	protected void OnValidate()
	{
		if (this.destroyAfterSeconds <= 0f)
		{
			this.destroyAfterSeconds = 1f;
		}
	}

	// Token: 0x0400178F RID: 6031
	public float destroyAfterSeconds = 1f;
}
