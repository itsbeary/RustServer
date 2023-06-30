using System;
using Network;
using TMPro;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class NetworkInfoGeneralText : MonoBehaviour
{
	// Token: 0x06001EE8 RID: 7912 RVA: 0x000D2553 File Offset: 0x000D0753
	private void Update()
	{
		this.UpdateText();
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000D255C File Offset: 0x000D075C
	private void UpdateText()
	{
		string text = "";
		if (Net.sv != null)
		{
			text += "Server\n";
			text += Net.sv.GetDebug(null);
			text += "\n";
		}
		this.text.text = text;
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000D25AC File Offset: 0x000D07AC
	private static string ChannelStat(int window, int left)
	{
		return string.Format("{0}/{1}", left, window);
	}

	// Token: 0x040017D2 RID: 6098
	public TextMeshProUGUI text;
}
