using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D6 RID: 470
public class ToolgunScreen : MonoBehaviour
{
	// Token: 0x06001955 RID: 6485 RVA: 0x000BA078 File Offset: 0x000B8278
	public void SetScreenText(string newText)
	{
		bool flag = string.IsNullOrEmpty(newText);
		this.blockInfoText.gameObject.SetActive(!flag);
		this.noBlockText.gameObject.SetActive(flag);
		this.blockInfoText.text = newText;
	}

	// Token: 0x04001229 RID: 4649
	public Text blockInfoText;

	// Token: 0x0400122A RID: 4650
	public Text noBlockText;
}
