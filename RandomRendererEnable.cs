using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class RandomRendererEnable : MonoBehaviour
{
	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x0600176B RID: 5995 RVA: 0x000B1FB3 File Offset: 0x000B01B3
	// (set) Token: 0x0600176C RID: 5996 RVA: 0x000B1FBB File Offset: 0x000B01BB
	public int EnabledIndex { get; private set; }

	// Token: 0x0600176D RID: 5997 RVA: 0x000B1FC4 File Offset: 0x000B01C4
	public void OnEnable()
	{
		int num = UnityEngine.Random.Range(0, this.randoms.Length);
		this.EnabledIndex = num;
		this.randoms[num].enabled = true;
	}

	// Token: 0x04001008 RID: 4104
	public Renderer[] randoms;
}
