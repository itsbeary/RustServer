using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000862 RID: 2146
public class VirtualItemIcon : MonoBehaviour
{
	// Token: 0x0400306E RID: 12398
	public ItemDefinition itemDef;

	// Token: 0x0400306F RID: 12399
	public int itemAmount;

	// Token: 0x04003070 RID: 12400
	public bool asBlueprint;

	// Token: 0x04003071 RID: 12401
	public Image iconImage;

	// Token: 0x04003072 RID: 12402
	public Image bpUnderlay;

	// Token: 0x04003073 RID: 12403
	public Text amountText;

	// Token: 0x04003074 RID: 12404
	public Text hoverText;

	// Token: 0x04003075 RID: 12405
	public CanvasGroup iconContents;

	// Token: 0x04003076 RID: 12406
	public Tooltip ToolTip;

	// Token: 0x04003077 RID: 12407
	public CanvasGroup conditionObject;

	// Token: 0x04003078 RID: 12408
	public Image conditionFill;

	// Token: 0x04003079 RID: 12409
	public Image maxConditionFill;

	// Token: 0x0400307A RID: 12410
	public Image cornerIcon;
}
