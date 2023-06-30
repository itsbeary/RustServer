using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007A2 RID: 1954
public class ConsoleUI : SingletonComponent<ConsoleUI>
{
	// Token: 0x04002BA3 RID: 11171
	public RustText text;

	// Token: 0x04002BA4 RID: 11172
	public InputField outputField;

	// Token: 0x04002BA5 RID: 11173
	public InputField inputField;

	// Token: 0x04002BA6 RID: 11174
	public GameObject AutocompleteDropDown;

	// Token: 0x04002BA7 RID: 11175
	public GameObject ItemTemplate;

	// Token: 0x04002BA8 RID: 11176
	public Color errorColor;

	// Token: 0x04002BA9 RID: 11177
	public Color warningColor;

	// Token: 0x04002BAA RID: 11178
	public Color inputColor;
}
