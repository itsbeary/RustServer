using System;
using UnityEngine.UI;

// Token: 0x020004E4 RID: 1252
public class FrequencyConfig : UIDialog
{
	// Token: 0x040020F8 RID: 8440
	[NonSerialized]
	private IRFObject rfObject;

	// Token: 0x040020F9 RID: 8441
	public InputField input;

	// Token: 0x040020FA RID: 8442
	public int target;

	// Token: 0x040020FB RID: 8443
	private ItemContainer tempContainer;

	// Token: 0x040020FC RID: 8444
	private ItemId tempItemID;
}
