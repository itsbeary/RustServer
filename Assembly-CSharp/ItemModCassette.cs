using System;

// Token: 0x020003A3 RID: 931
public class ItemModCassette : ItemModAssociatedEntity<Cassette>
{
	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x060020C8 RID: 8392 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x060020C9 RID: 8393 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowHeldEntityParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000D8993 File Offset: 0x000D6B93
	protected override void OnAssociatedItemCreated(Cassette ent)
	{
		base.OnAssociatedItemCreated(ent);
		ent.AssignPreloadContent();
	}

	// Token: 0x040019B2 RID: 6578
	public int noteSpriteIndex;

	// Token: 0x040019B3 RID: 6579
	public PreloadedCassetteContent PreloadedContent;
}
