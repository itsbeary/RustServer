using System;
using Rust;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class Stocking : LootContainer
{
	// Token: 0x06001817 RID: 6167 RVA: 0x000B5103 File Offset: 0x000B3303
	public override void ServerInit()
	{
		base.ServerInit();
		if (Stocking.stockings == null)
		{
			Stocking.stockings = new ListHashSet<Stocking>(8);
		}
		Stocking.stockings.Add(this);
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000B5128 File Offset: 0x000B3328
	internal override void DoServerDestroy()
	{
		Stocking.stockings.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x000B513C File Offset: 0x000B333C
	public bool IsEmpty()
	{
		if (base.inventory == null)
		{
			return false;
		}
		for (int i = base.inventory.itemList.Count - 1; i >= 0; i--)
		{
			if (base.inventory.itemList[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x000B5188 File Offset: 0x000B3388
	public override void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! Stocking::PopulateLoot has null inventory!!! " + base.name);
			return;
		}
		if (this.IsEmpty())
		{
			base.SpawnLoot();
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.Hurt(this.MaxHealth() * 0.1f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x000B51E0 File Offset: 0x000B33E0
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.IsEmpty() && base.healthFraction <= 0.1f)
		{
			base.Hurt(base.health, DamageType.Generic, this, false);
		}
	}

	// Token: 0x040010E9 RID: 4329
	public static ListHashSet<Stocking> stockings;
}
