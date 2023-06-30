using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008C2 RID: 2242
public class TextEntryCookie : MonoBehaviour
{
	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06003735 RID: 14133 RVA: 0x0014BE80 File Offset: 0x0014A080
	public InputField control
	{
		get
		{
			return base.GetComponent<InputField>();
		}
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x0014BE88 File Offset: 0x0014A088
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("TextEntryCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			this.control.text = @string;
		}
		this.control.onValueChanged.Invoke(this.control.text);
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x0014BEDA File Offset: 0x0014A0DA
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		PlayerPrefs.SetString("TextEntryCookie_" + base.name, this.control.text);
	}
}
