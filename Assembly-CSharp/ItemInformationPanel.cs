using System;
using UnityEngine;

// Token: 0x0200082B RID: 2091
public class ItemInformationPanel : MonoBehaviour
{
	// Token: 0x060035E6 RID: 13798 RVA: 0x001476B1 File Offset: 0x001458B1
	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning("ItemInformationPanel.EligableForDisplay");
		return false;
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x001476BE File Offset: 0x001458BE
	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning("ItemInformationPanel.SetupForItem");
	}
}
