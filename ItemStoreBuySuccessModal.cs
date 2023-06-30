using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000878 RID: 2168
public class ItemStoreBuySuccessModal : MonoBehaviour
{
	// Token: 0x06003656 RID: 13910 RVA: 0x00148724 File Offset: 0x00146924
	public void Show(ulong orderId)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
		SingletonComponent<SteamInventoryManager>.Instance != null;
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x00148764 File Offset: 0x00146964
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}
}
