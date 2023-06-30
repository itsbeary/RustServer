using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class SellOrderEntry : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04000EFF RID: 3839
	public VirtualItemIcon MerchandiseIcon;

	// Token: 0x04000F00 RID: 3840
	public VirtualItemIcon CurrencyIcon;

	// Token: 0x04000F01 RID: 3841
	private ItemDefinition merchandiseInfo;

	// Token: 0x04000F02 RID: 3842
	private ItemDefinition currencyInfo;

	// Token: 0x04000F03 RID: 3843
	public GameObject buyButton;

	// Token: 0x04000F04 RID: 3844
	public GameObject cantaffordNotification;

	// Token: 0x04000F05 RID: 3845
	public GameObject outOfStockNotification;

	// Token: 0x04000F06 RID: 3846
	private IVendingMachineInterface vendingPanel;

	// Token: 0x04000F07 RID: 3847
	public UIIntegerEntry intEntry;
}
