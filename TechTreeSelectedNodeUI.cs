using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007DF RID: 2015
public class TechTreeSelectedNodeUI : MonoBehaviour
{
	// Token: 0x04002D51 RID: 11601
	public RustText selectedTitle;

	// Token: 0x04002D52 RID: 11602
	public RawImage selectedIcon;

	// Token: 0x04002D53 RID: 11603
	public RustText selectedDescription;

	// Token: 0x04002D54 RID: 11604
	public RustText costText;

	// Token: 0x04002D55 RID: 11605
	public RustText craftingCostText;

	// Token: 0x04002D56 RID: 11606
	public GameObject costObject;

	// Token: 0x04002D57 RID: 11607
	public GameObject cantAffordObject;

	// Token: 0x04002D58 RID: 11608
	public GameObject unlockedObject;

	// Token: 0x04002D59 RID: 11609
	public GameObject unlockButton;

	// Token: 0x04002D5A RID: 11610
	public GameObject noPathObject;

	// Token: 0x04002D5B RID: 11611
	public TechTreeDialog dialog;

	// Token: 0x04002D5C RID: 11612
	public Color ColorAfford;

	// Token: 0x04002D5D RID: 11613
	public Color ColorCantAfford;

	// Token: 0x04002D5E RID: 11614
	public GameObject totalRequiredRoot;

	// Token: 0x04002D5F RID: 11615
	public RustText totalRequiredText;

	// Token: 0x04002D60 RID: 11616
	public ItemInformationPanel[] informationPanels;
}
