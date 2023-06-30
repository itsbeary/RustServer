using System;

// Token: 0x020003DB RID: 987
public class ItemModSign : ItemModAssociatedEntity<SignContent>
{
	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06002213 RID: 8723 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06002214 RID: 8724 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool ShouldAutoCreateEntity
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000DD7A0 File Offset: 0x000DB9A0
	public void OnSignPickedUp(ISignage s, IUGCBrowserEntity ugc, Item toItem)
	{
		SignContent signContent = base.CreateAssociatedEntity(toItem);
		if (signContent != null)
		{
			signContent.CopyInfoFromSign(s, ugc);
		}
	}
}
