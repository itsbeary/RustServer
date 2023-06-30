using System;

// Token: 0x0200017C RID: 380
public class ItemModPhoto : ItemModAssociatedEntity<PhotoEntity>
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060017BD RID: 6077 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}
}
