using System;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class ScaleTransform : ScaleRenderer
{
	// Token: 0x06001F78 RID: 8056 RVA: 0x000D4F4E File Offset: 0x000D314E
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.myRenderer.transform.localScale = this.initialScale * scale;
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000D4F73 File Offset: 0x000D3173
	public override void GatherInitialValues()
	{
		this.initialScale = this.myRenderer.transform.localScale;
		base.GatherInitialValues();
	}

	// Token: 0x040018C0 RID: 6336
	private Vector3 initialScale;
}
