using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000876 RID: 2166
public class ItemStore : SingletonComponent<ItemStore>, VirtualScroll.IDataSource
{
	// Token: 0x0600364E RID: 13902 RVA: 0x0014864F File Offset: 0x0014684F
	public int GetItemCount()
	{
		return this.Cart.Count;
	}

	// Token: 0x0600364F RID: 13903 RVA: 0x0014865C File Offset: 0x0014685C
	public void SetItemData(int i, GameObject obj)
	{
		obj.GetComponent<ItemStoreCartItem>().Init(i, this.Cart[i]);
	}

	// Token: 0x040030F5 RID: 12533
	public static readonly Translate.Phrase CartEmptyPhrase = new Translate.Phrase("store.cart.empty", "Cart");

	// Token: 0x040030F6 RID: 12534
	public static readonly Translate.Phrase CartSingularPhrase = new Translate.Phrase("store.cart.singular", "1 item");

	// Token: 0x040030F7 RID: 12535
	public static readonly Translate.Phrase CartPluralPhrase = new Translate.Phrase("store.cart.plural", "{amount} items");

	// Token: 0x040030F8 RID: 12536
	public GameObject ItemPrefab;

	// Token: 0x040030F9 RID: 12537
	[FormerlySerializedAs("ItemParent")]
	public RectTransform LimitedItemParent;

	// Token: 0x040030FA RID: 12538
	public RectTransform GeneralItemParent;

	// Token: 0x040030FB RID: 12539
	public List<IPlayerItemDefinition> Cart = new List<IPlayerItemDefinition>();

	// Token: 0x040030FC RID: 12540
	public ItemStoreItemInfoModal ItemStoreInfoModal;

	// Token: 0x040030FD RID: 12541
	public GameObject BuyingModal;

	// Token: 0x040030FE RID: 12542
	public ItemStoreBuyFailedModal ItemStoreBuyFailedModal;

	// Token: 0x040030FF RID: 12543
	public ItemStoreBuySuccessModal ItemStoreBuySuccessModal;

	// Token: 0x04003100 RID: 12544
	public SoundDefinition AddToCartSound;

	// Token: 0x04003101 RID: 12545
	public RustText CartButtonLabel;

	// Token: 0x04003102 RID: 12546
	public RustText QuantityValue;

	// Token: 0x04003103 RID: 12547
	public RustText TotalValue;
}
