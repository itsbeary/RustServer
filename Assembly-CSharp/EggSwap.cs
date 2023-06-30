using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class EggSwap : MonoBehaviour
{
	// Token: 0x0600174D RID: 5965 RVA: 0x000B1248 File Offset: 0x000AF448
	public void Show(int index)
	{
		this.HideAll();
		this.eggRenderers[index].enabled = true;
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000B1260 File Offset: 0x000AF460
	public void HideAll()
	{
		Renderer[] array = this.eggRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x04000FEC RID: 4076
	public Renderer[] eggRenderers;
}
