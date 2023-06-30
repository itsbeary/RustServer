using System;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class PrefabInformation : PrefabAttribute
{
	// Token: 0x06002A73 RID: 10867 RVA: 0x00102A35 File Offset: 0x00100C35
	protected override Type GetIndexedType()
	{
		return typeof(PrefabInformation);
	}

	// Token: 0x040022B4 RID: 8884
	public ItemDefinition associatedItemDefinition;

	// Token: 0x040022B5 RID: 8885
	public Translate.Phrase title;

	// Token: 0x040022B6 RID: 8886
	public Translate.Phrase description;

	// Token: 0x040022B7 RID: 8887
	public Sprite sprite;

	// Token: 0x040022B8 RID: 8888
	public bool shownOnDeathScreen;
}
