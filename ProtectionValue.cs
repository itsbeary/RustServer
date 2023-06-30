using System;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x0200084F RID: 2127
public class ProtectionValue : MonoBehaviour, IClothingChanged
{
	// Token: 0x04002FE6 RID: 12262
	public CanvasGroup group;

	// Token: 0x04002FE7 RID: 12263
	public TextMeshProUGUI text;

	// Token: 0x04002FE8 RID: 12264
	public DamageType damageType;

	// Token: 0x04002FE9 RID: 12265
	public bool selectedItem;

	// Token: 0x04002FEA RID: 12266
	public bool displayBaseProtection;
}
