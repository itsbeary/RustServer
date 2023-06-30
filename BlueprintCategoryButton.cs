using System;
using TMPro;
using UnityEngine;

// Token: 0x02000817 RID: 2071
public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002EB3 RID: 11955
	public TextMeshProUGUI amountLabel;

	// Token: 0x04002EB4 RID: 11956
	public ItemCategory Category;

	// Token: 0x04002EB5 RID: 11957
	public bool AlwaysShow;

	// Token: 0x04002EB6 RID: 11958
	public bool ShowItemCount = true;

	// Token: 0x04002EB7 RID: 11959
	public GameObject BackgroundHighlight;

	// Token: 0x04002EB8 RID: 11960
	public SoundDefinition clickSound;

	// Token: 0x04002EB9 RID: 11961
	public SoundDefinition hoverSound;
}
