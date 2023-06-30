using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200083C RID: 2108
public class LootPanelIndustrialCrafter : LootPanel
{
	// Token: 0x04002F73 RID: 12147
	public GameObject CraftingRoot;

	// Token: 0x04002F74 RID: 12148
	public RustSlider ProgressSlider;

	// Token: 0x04002F75 RID: 12149
	public Transform Spinner;

	// Token: 0x04002F76 RID: 12150
	public float SpinSpeed = 90f;

	// Token: 0x04002F77 RID: 12151
	public GameObject WorkbenchLevelRoot;
}
