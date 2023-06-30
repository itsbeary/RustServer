using System;
using System.Collections.Generic;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000881 RID: 2177
public class NewsParagraph : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06003677 RID: 13943 RVA: 0x00148B48 File Offset: 0x00146D48
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.Text == null || this.Links == null || eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.Text, eventData.position, eventData.pressEventCamera);
		if (num < 0 || num >= this.Text.textInfo.linkCount)
		{
			return;
		}
		TMP_LinkInfo tmp_LinkInfo = this.Text.textInfo.linkInfo[num];
		int num2;
		if (!int.TryParse(tmp_LinkInfo.GetLinkID(), out num2) || num2 < 0 || num2 >= this.Links.Count)
		{
			return;
		}
		string text = this.Links[num2];
		if (text.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
		{
			Application.OpenURL(text);
		}
	}

	// Token: 0x04003141 RID: 12609
	public RustText Text;

	// Token: 0x04003142 RID: 12610
	public List<string> Links;
}
