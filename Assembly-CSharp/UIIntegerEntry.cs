using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085A RID: 2138
public class UIIntegerEntry : MonoBehaviour
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06003624 RID: 13860 RVA: 0x001481F0 File Offset: 0x001463F0
	// (remove) Token: 0x06003625 RID: 13861 RVA: 0x00148228 File Offset: 0x00146428
	public event Action textChanged;

	// Token: 0x06003626 RID: 13862 RVA: 0x0014825D File Offset: 0x0014645D
	public void OnAmountTextChanged()
	{
		this.textChanged();
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x0014826A File Offset: 0x0014646A
	public void SetAmount(int amount)
	{
		if (amount == this.GetIntAmount())
		{
			return;
		}
		this.textEntry.text = amount.ToString();
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x00148288 File Offset: 0x00146488
	public int GetIntAmount()
	{
		int num = 0;
		int.TryParse(this.textEntry.text, out num);
		return num;
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x001482AB File Offset: 0x001464AB
	public void PlusMinus(int delta)
	{
		this.SetAmount(this.GetIntAmount() + delta);
	}

	// Token: 0x04003032 RID: 12338
	public InputField textEntry;
}
