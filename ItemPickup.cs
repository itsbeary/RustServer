using System;
using Rust;

// Token: 0x020004C8 RID: 1224
public class ItemPickup : DroppedItem
{
	// Token: 0x06002808 RID: 10248 RVA: 0x0008207B File Offset: 0x0008027B
	public override float GetDespawnDuration()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000F9934 File Offset: 0x000F7B34
	public override void Spawn()
	{
		base.Spawn();
		if (Application.isLoadingSave)
		{
			return;
		}
		Item item = ItemManager.Create(this.itemDef, this.amount, this.skinOverride);
		base.InitializeItem(item);
		item.SetWorldEntity(this);
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000F9975 File Offset: 0x000F7B75
	internal override void DoServerDestroy()
	{
		if (this.item != null)
		{
			this.item.Remove(0f);
			this.item = null;
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000F999C File Offset: 0x000F7B9C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.IdleDestroy();
	}

	// Token: 0x04002098 RID: 8344
	public ItemDefinition itemDef;

	// Token: 0x04002099 RID: 8345
	public int amount = 1;

	// Token: 0x0400209A RID: 8346
	public ulong skinOverride;
}
