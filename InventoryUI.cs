using System;
using UnityEngine;

// Token: 0x02000864 RID: 2148
public class InventoryUI : MonoBehaviour
{
	// Token: 0x06003636 RID: 13878 RVA: 0x00148434 File Offset: 0x00146634
	private void Update()
	{
		if (this.ContactsButton != null && RelationshipManager.contacts != this.ContactsButton.activeSelf)
		{
			this.ContactsButton.SetActive(RelationshipManager.contacts);
		}
	}

	// Token: 0x04003084 RID: 12420
	public GameObject ContactsButton;
}
