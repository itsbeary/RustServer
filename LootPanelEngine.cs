using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000839 RID: 2105
public class LootPanelEngine : LootPanel
{
	// Token: 0x04002F64 RID: 12132
	[SerializeField]
	private Image engineImage;

	// Token: 0x04002F65 RID: 12133
	[SerializeField]
	private ItemIcon[] icons;

	// Token: 0x04002F66 RID: 12134
	[SerializeField]
	private GameObject warning;

	// Token: 0x04002F67 RID: 12135
	[SerializeField]
	private RustText hp;

	// Token: 0x04002F68 RID: 12136
	[SerializeField]
	private RustText power;

	// Token: 0x04002F69 RID: 12137
	[SerializeField]
	private RustText acceleration;

	// Token: 0x04002F6A RID: 12138
	[SerializeField]
	private RustText topSpeed;

	// Token: 0x04002F6B RID: 12139
	[SerializeField]
	private RustText fuelEconomy;
}
