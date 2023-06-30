using System;
using UnityEngine;

// Token: 0x02000877 RID: 2167
public class ItemStoreBuyFailedModal : MonoBehaviour
{
	// Token: 0x06003652 RID: 13906 RVA: 0x001486C7 File Offset: 0x001468C7
	public void Show(ulong orderid)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x001486FB File Offset: 0x001468FB
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}
}
