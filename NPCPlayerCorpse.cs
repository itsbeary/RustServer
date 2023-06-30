using System;

// Token: 0x020001B5 RID: 437
public class NPCPlayerCorpse : PlayerCorpse
{
	// Token: 0x06001904 RID: 6404 RVA: 0x000B8C72 File Offset: 0x000B6E72
	public override float GetRemovalTime()
	{
		return 600f;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000B8C79 File Offset: 0x000B6E79
	public override bool CanLoot()
	{
		return this.lootEnabled;
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000B8C81 File Offset: 0x000B6E81
	public void SetLootableIn(float when)
	{
		base.Invoke(new Action(this.EnableLooting), when);
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000B8C96 File Offset: 0x000B6E96
	public void EnableLooting()
	{
		this.lootEnabled = true;
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x000B8C9F File Offset: 0x000B6E9F
	protected override bool CanLootContainer(ItemContainer c, int index)
	{
		return index != 1 && index != 2 && base.CanLootContainer(c, index);
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x000B8CB3 File Offset: 0x000B6EB3
	protected override void PreDropItems()
	{
		base.PreDropItems();
		if (this.containers != null && this.containers.Length >= 2)
		{
			this.containers[1].Clear();
			ItemManager.DoRemoves();
		}
	}

	// Token: 0x04001195 RID: 4501
	private bool lootEnabled;
}
