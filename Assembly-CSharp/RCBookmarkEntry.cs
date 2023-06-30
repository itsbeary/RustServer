using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000109 RID: 265
public class RCBookmarkEntry : MonoBehaviour
{
	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060015DC RID: 5596 RVA: 0x000ABD7C File Offset: 0x000A9F7C
	// (set) Token: 0x060015DD RID: 5597 RVA: 0x000ABD84 File Offset: 0x000A9F84
	public string identifier { get; private set; }

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x060015DE RID: 5598 RVA: 0x000ABD8D File Offset: 0x000A9F8D
	// (set) Token: 0x060015DF RID: 5599 RVA: 0x000ABD95 File Offset: 0x000A9F95
	public bool isSelected { get; private set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x060015E0 RID: 5600 RVA: 0x000ABD9E File Offset: 0x000A9F9E
	// (set) Token: 0x060015E1 RID: 5601 RVA: 0x000ABDA6 File Offset: 0x000A9FA6
	public bool isControlling { get; private set; }

	// Token: 0x04000E03 RID: 3587
	private ComputerMenu owner;

	// Token: 0x04000E04 RID: 3588
	public RectTransform connectButton;

	// Token: 0x04000E05 RID: 3589
	public RectTransform disconnectButton;

	// Token: 0x04000E06 RID: 3590
	public RawImage onlineIndicator;

	// Token: 0x04000E07 RID: 3591
	public RawImage offlineIndicator;

	// Token: 0x04000E08 RID: 3592
	public GameObject selectedindicator;

	// Token: 0x04000E09 RID: 3593
	public Image backgroundImage;

	// Token: 0x04000E0A RID: 3594
	public Color selectedColor;

	// Token: 0x04000E0B RID: 3595
	public Color activeColor;

	// Token: 0x04000E0C RID: 3596
	public Color inactiveColor;

	// Token: 0x04000E0D RID: 3597
	public Text nameLabel;

	// Token: 0x04000E10 RID: 3600
	public EventTrigger eventTrigger;
}
