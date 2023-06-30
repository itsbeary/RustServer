using System;

// Token: 0x020001EA RID: 490
public class Chicken : BaseAnimalNPC
{
	// Token: 0x1700022C RID: 556
	// (get) Token: 0x060019E1 RID: 6625 RVA: 0x000ACE07 File Offset: 0x000AB007
	public override float RealisticMass
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x060019E2 RID: 6626 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x000BC95C File Offset: 0x000BAB5C
	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Meat))
		{
			return false;
		}
		CollectibleEntity collectibleEntity = best as CollectibleEntity;
		if (collectibleEntity != null)
		{
			ItemAmount[] itemList = collectibleEntity.itemList;
			for (int i = 0; i < itemList.Length; i++)
			{
				if (itemList[i].itemDef.category == ItemCategory.Food)
				{
					return true;
				}
			}
		}
		return base.WantsToEat(best);
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x000BC9BE File Offset: 0x000BABBE
	public override string Categorize()
	{
		return "Chicken";
	}

	// Token: 0x04001274 RID: 4724
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;
}
