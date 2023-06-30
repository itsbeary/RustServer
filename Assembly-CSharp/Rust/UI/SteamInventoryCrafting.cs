using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000B19 RID: 2841
	public class SteamInventoryCrafting : MonoBehaviour
	{
		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06004520 RID: 17696 RVA: 0x00194B9E File Offset: 0x00192D9E
		// (set) Token: 0x06004521 RID: 17697 RVA: 0x00194BA6 File Offset: 0x00192DA6
		public IPlayerItemDefinition ResultItem { get; private set; }

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06004522 RID: 17698 RVA: 0x00194BAF File Offset: 0x00192DAF
		// (set) Token: 0x06004523 RID: 17699 RVA: 0x00194BB7 File Offset: 0x00192DB7
		public Coroutine MarketCoroutine { get; private set; }

		// Token: 0x04003DB3 RID: 15795
		public GameObject Container;

		// Token: 0x04003DB4 RID: 15796
		public ToggleGroup ToggleGroup;

		// Token: 0x04003DB5 RID: 15797
		public Button ConvertToItem;

		// Token: 0x04003DB6 RID: 15798
		public TextMeshProUGUI WoodAmount;

		// Token: 0x04003DB7 RID: 15799
		public TextMeshProUGUI ClothAmount;

		// Token: 0x04003DB8 RID: 15800
		public TextMeshProUGUI MetalAmount;

		// Token: 0x04003DB9 RID: 15801
		public TextMeshProUGUI InfoText;

		// Token: 0x04003DBC RID: 15804
		public SteamInventoryCrateOpen CraftModal;

		// Token: 0x04003DBD RID: 15805
		public GameObject CraftingContainer;

		// Token: 0x04003DBE RID: 15806
		public GameObject CraftingButton;

		// Token: 0x04003DBF RID: 15807
		public SteamInventoryNewItem NewItemModal;
	}
}
