using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B1D RID: 2845
	public class SteamInventoryManager : SingletonComponent<SteamInventoryManager>
	{
		// Token: 0x04003DCB RID: 15819
		public GameObject inventoryItemPrefab;

		// Token: 0x04003DCC RID: 15820
		public GameObject inventoryCanvas;

		// Token: 0x04003DCD RID: 15821
		public GameObject missingItems;

		// Token: 0x04003DCE RID: 15822
		public SteamInventoryCrafting CraftControl;

		// Token: 0x04003DCF RID: 15823
		public List<GameObject> items;

		// Token: 0x04003DD0 RID: 15824
		public GameObject LoadingOverlay;
	}
}
