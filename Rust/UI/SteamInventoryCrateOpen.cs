using System;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B1A RID: 2842
	public class SteamInventoryCrateOpen : MonoBehaviour
	{
		// Token: 0x04003DC0 RID: 15808
		public TextMeshProUGUI Name;

		// Token: 0x04003DC1 RID: 15809
		public TextMeshProUGUI Requirements;

		// Token: 0x04003DC2 RID: 15810
		public TextMeshProUGUI Label;

		// Token: 0x04003DC3 RID: 15811
		public HttpImage IconImage;

		// Token: 0x04003DC4 RID: 15812
		public GameObject ErrorPanel;

		// Token: 0x04003DC5 RID: 15813
		public TextMeshProUGUI ErrorText;

		// Token: 0x04003DC6 RID: 15814
		public GameObject CraftButton;

		// Token: 0x04003DC7 RID: 15815
		public GameObject ProgressPanel;

		// Token: 0x04003DC8 RID: 15816
		public SteamInventoryNewItem NewItemModal;
	}
}
