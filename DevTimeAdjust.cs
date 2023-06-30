using System;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class DevTimeAdjust : MonoBehaviour
{
	// Token: 0x06001F20 RID: 7968 RVA: 0x000D3A1D File Offset: 0x000D1C1D
	private void Start()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000D3A48 File Offset: 0x000D1C48
	private void OnGUI()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		float num = (float)Screen.width * 0.2f;
		Rect rect = new Rect((float)Screen.width - (num + 20f), (float)Screen.height - 30f, num, 20f);
		float num2 = TOD_Sky.Instance.Cycle.Hour;
		num2 = GUI.HorizontalSlider(rect, num2, 0f, 24f);
		rect.y -= 20f;
		GUI.Label(rect, "Time Of Day");
		if (num2 != TOD_Sky.Instance.Cycle.Hour)
		{
			TOD_Sky.Instance.Cycle.Hour = num2;
			PlayerPrefs.SetFloat("DevTime", num2);
		}
	}
}
