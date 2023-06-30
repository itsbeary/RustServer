using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085B RID: 2139
public class UIInventory : SingletonComponent<UIInventory>
{
	// Token: 0x04003034 RID: 12340
	public TextMeshProUGUI PlayerName;

	// Token: 0x04003035 RID: 12341
	public static bool isOpen;

	// Token: 0x04003036 RID: 12342
	public static float LastOpened;

	// Token: 0x04003037 RID: 12343
	public VerticalLayoutGroup rightContents;

	// Token: 0x04003038 RID: 12344
	public GameObject QuickCraft;

	// Token: 0x04003039 RID: 12345
	public Transform InventoryIconContainer;

	// Token: 0x0400303A RID: 12346
	public ChangelogPanel ChangelogPanel;

	// Token: 0x0400303B RID: 12347
	public ContactsPanel contactsPanel;
}
