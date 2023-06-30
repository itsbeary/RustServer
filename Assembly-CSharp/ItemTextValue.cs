using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000834 RID: 2100
public class ItemTextValue : MonoBehaviour
{
	// Token: 0x060035F1 RID: 13809 RVA: 0x001476E4 File Offset: 0x001458E4
	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		val *= this.multiplier;
		this.text.text = ((overrideText == "") ? string.Format("{0}{1:n" + numDecimals + "}", (val > 0f && this.signed) ? "+" : "", val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text += " %";
		}
		if (this.suffix != "" && !float.IsPositiveInfinity(val))
		{
			Text text2 = this.text;
			text2.text += this.suffix;
		}
		bool flag = val > 0f;
		if (this.negativestat)
		{
			flag = !flag;
		}
		if (this.useColors)
		{
			this.text.color = (flag ? this.good : this.bad);
		}
	}

	// Token: 0x04002F51 RID: 12113
	public Text text;

	// Token: 0x04002F52 RID: 12114
	public Color bad;

	// Token: 0x04002F53 RID: 12115
	public Color good;

	// Token: 0x04002F54 RID: 12116
	public bool negativestat;

	// Token: 0x04002F55 RID: 12117
	public bool asPercentage;

	// Token: 0x04002F56 RID: 12118
	public bool useColors = true;

	// Token: 0x04002F57 RID: 12119
	public bool signed = true;

	// Token: 0x04002F58 RID: 12120
	public string suffix;

	// Token: 0x04002F59 RID: 12121
	public float multiplier = 1f;
}
