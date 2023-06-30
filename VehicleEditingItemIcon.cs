using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085F RID: 2143
[RequireComponent(typeof(ItemIcon))]
public class VehicleEditingItemIcon : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04003043 RID: 12355
	[SerializeField]
	private Image foregroundImage;

	// Token: 0x04003044 RID: 12356
	[SerializeField]
	private Image linkImage;
}
