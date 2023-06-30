using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000137 RID: 311
public class VendingMachineScreen : MonoBehaviour
{
	// Token: 0x04000F0F RID: 3855
	public RawImage largeIcon;

	// Token: 0x04000F10 RID: 3856
	public RawImage blueprintIcon;

	// Token: 0x04000F11 RID: 3857
	public Text mainText;

	// Token: 0x04000F12 RID: 3858
	public Text lowerText;

	// Token: 0x04000F13 RID: 3859
	public Text centerText;

	// Token: 0x04000F14 RID: 3860
	public RawImage smallIcon;

	// Token: 0x04000F15 RID: 3861
	public VendingMachine vendingMachine;

	// Token: 0x04000F16 RID: 3862
	public Sprite outOfStockSprite;

	// Token: 0x04000F17 RID: 3863
	public Renderer fadeoutMesh;

	// Token: 0x04000F18 RID: 3864
	public CanvasGroup screenCanvas;

	// Token: 0x04000F19 RID: 3865
	public Renderer light1;

	// Token: 0x04000F1A RID: 3866
	public Renderer light2;

	// Token: 0x04000F1B RID: 3867
	public float nextImageTime;

	// Token: 0x04000F1C RID: 3868
	public int currentImageIndex;

	// Token: 0x02000C36 RID: 3126
	public enum vmScreenState
	{
		// Token: 0x040042F3 RID: 17139
		ItemScroll,
		// Token: 0x040042F4 RID: 17140
		Vending,
		// Token: 0x040042F5 RID: 17141
		Message,
		// Token: 0x040042F6 RID: 17142
		ShopName,
		// Token: 0x040042F7 RID: 17143
		OutOfStock
	}
}
