using System;
using UnityEngine;

// Token: 0x020005FB RID: 1531
[RequireComponent(typeof(ItemModWearable))]
public class ItemModPaintable : ItemModAssociatedEntity<PaintedItemStorageEntity>
{
	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x06002DBF RID: 11711 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool OwnedByParentPlayer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0400256D RID: 9581
	public GameObjectRef ChangeSignTextDialog;

	// Token: 0x0400256E RID: 9582
	public MeshPaintableSource[] PaintableSources;
}
