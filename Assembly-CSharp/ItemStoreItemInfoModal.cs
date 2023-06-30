using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x0200087B RID: 2171
public class ItemStoreItemInfoModal : MonoBehaviour
{
	// Token: 0x0600365E RID: 13918 RVA: 0x0014887C File Offset: 0x00146A7C
	public void Show(IPlayerItemDefinition item)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.text = item.Name;
		this.Description.text = item.Description.BBCodeToUnity();
		this.Price.text = item.LocalPriceFormatted;
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x0014890C File Offset: 0x00146B0C
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	// Token: 0x0400310D RID: 12557
	public HttpImage Icon;

	// Token: 0x0400310E RID: 12558
	public TextMeshProUGUI Name;

	// Token: 0x0400310F RID: 12559
	public TextMeshProUGUI Price;

	// Token: 0x04003110 RID: 12560
	public TextMeshProUGUI Description;

	// Token: 0x04003111 RID: 12561
	private IPlayerItemDefinition item;
}
