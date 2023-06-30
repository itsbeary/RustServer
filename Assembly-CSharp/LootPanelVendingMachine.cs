using System;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class LootPanelVendingMachine : LootPanel, IVendingMachineInterface
{
	// Token: 0x04000EFB RID: 3835
	public GameObjectRef sellOrderPrefab;

	// Token: 0x04000EFC RID: 3836
	public GameObject sellOrderContainer;

	// Token: 0x04000EFD RID: 3837
	public GameObject busyOverlayPrefab;

	// Token: 0x04000EFE RID: 3838
	private GameObject busyOverlayInstance;
}
