using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012D RID: 301
public class AddSellOrderManager : MonoBehaviour
{
	// Token: 0x04000EE4 RID: 3812
	public VirtualItemIcon sellItemIcon;

	// Token: 0x04000EE5 RID: 3813
	public VirtualItemIcon currencyItemIcon;

	// Token: 0x04000EE6 RID: 3814
	public GameObject itemSearchParent;

	// Token: 0x04000EE7 RID: 3815
	public ItemSearchEntry itemSearchEntryPrefab;

	// Token: 0x04000EE8 RID: 3816
	public InputField sellItemInput;

	// Token: 0x04000EE9 RID: 3817
	public InputField sellItemAmount;

	// Token: 0x04000EEA RID: 3818
	public InputField currencyItemInput;

	// Token: 0x04000EEB RID: 3819
	public InputField currencyItemAmount;

	// Token: 0x04000EEC RID: 3820
	public VendingPanelAdmin adminPanel;
}
