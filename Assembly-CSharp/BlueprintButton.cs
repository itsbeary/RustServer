using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000816 RID: 2070
public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged
{
	// Token: 0x04002EAB RID: 11947
	public Image image;

	// Token: 0x04002EAC RID: 11948
	public Image imageFavourite;

	// Token: 0x04002EAD RID: 11949
	public Button button;

	// Token: 0x04002EAE RID: 11950
	public CanvasGroup group;

	// Token: 0x04002EAF RID: 11951
	public GameObject newNotification;

	// Token: 0x04002EB0 RID: 11952
	public GameObject lockedOverlay;

	// Token: 0x04002EB1 RID: 11953
	public Tooltip Tip;

	// Token: 0x04002EB2 RID: 11954
	public Image FavouriteIcon;
}
