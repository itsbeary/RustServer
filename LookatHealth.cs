using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086D RID: 2157
public class LookatHealth : MonoBehaviour
{
	// Token: 0x040030D1 RID: 12497
	public static bool Enabled = true;

	// Token: 0x040030D2 RID: 12498
	public GameObject container;

	// Token: 0x040030D3 RID: 12499
	public Text textHealth;

	// Token: 0x040030D4 RID: 12500
	public Text textStability;

	// Token: 0x040030D5 RID: 12501
	public Image healthBar;

	// Token: 0x040030D6 RID: 12502
	public Image healthBarBG;

	// Token: 0x040030D7 RID: 12503
	public Color barBGColorNormal;

	// Token: 0x040030D8 RID: 12504
	public Color barBGColorUnstable;
}
