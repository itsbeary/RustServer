using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200077E RID: 1918
public class EnumListItemUI : MonoBehaviour
{
	// Token: 0x060034CE RID: 13518 RVA: 0x001453F9 File Offset: 0x001435F9
	public void Init(object value, string valueText, EnumListUI list)
	{
		this.Value = value;
		this.list = list;
		this.TextValue.text = valueText;
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00145415 File Offset: 0x00143615
	public void Clicked()
	{
		this.list.ItemClicked(this.Value);
	}

	// Token: 0x04002B5C RID: 11100
	public object Value;

	// Token: 0x04002B5D RID: 11101
	public RustText TextValue;

	// Token: 0x04002B5E RID: 11102
	private EnumListUI list;
}
