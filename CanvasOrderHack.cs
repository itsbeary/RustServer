using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class CanvasOrderHack : MonoBehaviour
{
	// Token: 0x06001D3F RID: 7487 RVA: 0x000CA058 File Offset: 0x000C8258
	private void OnEnable()
	{
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.overrideSorting)
			{
				Canvas canvas2 = canvas;
				int num = canvas2.sortingOrder;
				canvas2.sortingOrder = num + 1;
			}
		}
		foreach (Canvas canvas3 in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas3.overrideSorting)
			{
				Canvas canvas4 = canvas3;
				int num = canvas4.sortingOrder;
				canvas4.sortingOrder = num - 1;
			}
		}
	}
}
