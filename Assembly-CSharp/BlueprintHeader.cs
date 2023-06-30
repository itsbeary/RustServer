using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000819 RID: 2073
public class BlueprintHeader : MonoBehaviour
{
	// Token: 0x060035CE RID: 13774 RVA: 0x0014752C File Offset: 0x0014572C
	public void Setup(ItemCategory name, int unlocked, int total)
	{
		this.categoryName.text = name.ToString().ToUpper();
		this.unlockCount.text = string.Format("UNLOCKED {0}/{1}", unlocked, total);
	}

	// Token: 0x04002EC0 RID: 11968
	public Text categoryName;

	// Token: 0x04002EC1 RID: 11969
	public Text unlockCount;
}
