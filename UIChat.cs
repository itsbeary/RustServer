using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079D RID: 1949
public class UIChat : PriorityListComponent<UIChat>
{
	// Token: 0x04002B8E RID: 11150
	public GameObject inputArea;

	// Token: 0x04002B8F RID: 11151
	public GameObject chatArea;

	// Token: 0x04002B90 RID: 11152
	public TMP_InputField inputField;

	// Token: 0x04002B91 RID: 11153
	public TextMeshProUGUI channelLabel;

	// Token: 0x04002B92 RID: 11154
	public ScrollRect scrollRect;

	// Token: 0x04002B93 RID: 11155
	public CanvasGroup canvasGroup;

	// Token: 0x04002B94 RID: 11156
	public GameObjectRef chatItemPlayer;

	// Token: 0x04002B95 RID: 11157
	public GameObject userPopup;

	// Token: 0x04002B96 RID: 11158
	public static bool isOpen;
}
