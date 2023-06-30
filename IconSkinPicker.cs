using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C9 RID: 713
public class IconSkinPicker : MonoBehaviour
{
	// Token: 0x04001684 RID: 5764
	public GameObjectRef pickerIcon;

	// Token: 0x04001685 RID: 5765
	public GameObject container;

	// Token: 0x04001686 RID: 5766
	public Action skinChangedEvent;

	// Token: 0x04001687 RID: 5767
	public ScrollRect scroller;

	// Token: 0x04001688 RID: 5768
	public SearchFilterInput searchFilter;
}
