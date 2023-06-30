using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class HideIfScoped : MonoBehaviour
{
	// Token: 0x0600193F RID: 6463 RVA: 0x000B9A04 File Offset: 0x000B7C04
	public void SetVisible(bool vis)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = vis;
		}
	}

	// Token: 0x040011FC RID: 4604
	public Renderer[] renderers;
}
