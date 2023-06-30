using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x0200087A RID: 2170
public class ItemStoreItem : MonoBehaviour
{
	// Token: 0x0600365C RID: 13916 RVA: 0x001487B8 File Offset: 0x001469B8
	internal void Init(IPlayerItemDefinition item, bool inCart)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.SetText(item.Name);
		this.Price.text = item.LocalPriceFormatted;
		this.InCartTag.SetActive(inCart);
		if (string.IsNullOrWhiteSpace(item.ItemShortName))
		{
			this.ItemName.SetText("");
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.ItemShortName);
		if (itemDefinition != null && !string.Equals(itemDefinition.displayName.english, item.Name, StringComparison.InvariantCultureIgnoreCase))
		{
			this.ItemName.SetPhrase(itemDefinition.displayName);
			return;
		}
		this.ItemName.SetText("");
	}

	// Token: 0x04003107 RID: 12551
	public HttpImage Icon;

	// Token: 0x04003108 RID: 12552
	public RustText Name;

	// Token: 0x04003109 RID: 12553
	public TextMeshProUGUI Price;

	// Token: 0x0400310A RID: 12554
	public RustText ItemName;

	// Token: 0x0400310B RID: 12555
	public GameObject InCartTag;

	// Token: 0x0400310C RID: 12556
	private IPlayerItemDefinition item;
}
