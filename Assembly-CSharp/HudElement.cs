using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007EE RID: 2030
public class HudElement : MonoBehaviour
{
	// Token: 0x06003578 RID: 13688 RVA: 0x001462F0 File Offset: 0x001444F0
	public void SetValue(float value, float max = 1f)
	{
		using (TimeWarning.New("HudElement.SetValue", 0))
		{
			value = (float)Mathf.CeilToInt(value);
			if (value != this.lastValue || max != this.lastMax)
			{
				this.lastValue = value;
				this.lastMax = max;
				float num = value / max;
				this.SetText(value.ToString("0"));
				this.SetImage(num);
			}
		}
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00146370 File Offset: 0x00144570
	private void SetText(string v)
	{
		for (int i = 0; i < this.ValueText.Length; i++)
		{
			this.ValueText[i].text = v;
		}
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x001463A0 File Offset: 0x001445A0
	private void SetImage(float f)
	{
		for (int i = 0; i < this.FilledImage.Length; i++)
		{
			this.FilledImage[i].fillAmount = f;
		}
	}

	// Token: 0x04002DAB RID: 11691
	public Text[] ValueText;

	// Token: 0x04002DAC RID: 11692
	public Image[] FilledImage;

	// Token: 0x04002DAD RID: 11693
	private float lastValue;

	// Token: 0x04002DAE RID: 11694
	private float lastMax;
}
