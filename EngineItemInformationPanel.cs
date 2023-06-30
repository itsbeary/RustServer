using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000823 RID: 2083
public class EngineItemInformationPanel : ItemInformationPanel
{
	// Token: 0x04002EF7 RID: 12023
	[SerializeField]
	private Text tier;

	// Token: 0x04002EF8 RID: 12024
	[SerializeField]
	private Translate.Phrase low;

	// Token: 0x04002EF9 RID: 12025
	[SerializeField]
	private Translate.Phrase medium;

	// Token: 0x04002EFA RID: 12026
	[SerializeField]
	private Translate.Phrase high;

	// Token: 0x04002EFB RID: 12027
	[SerializeField]
	private GameObject accelerationRoot;

	// Token: 0x04002EFC RID: 12028
	[SerializeField]
	private GameObject topSpeedRoot;

	// Token: 0x04002EFD RID: 12029
	[SerializeField]
	private GameObject fuelEconomyRoot;
}
