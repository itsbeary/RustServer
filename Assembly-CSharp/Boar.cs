using System;

// Token: 0x020001E9 RID: 489
public class Boar : BaseAnimalNPC
{
	// Token: 0x1700022A RID: 554
	// (get) Token: 0x060019DB RID: 6619 RVA: 0x000BC8DD File Offset: 0x000BAADD
	public override float RealisticMass
	{
		get
		{
			return 85f;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x060019DC RID: 6620 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x000BC8E4 File Offset: 0x000BAAE4
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

	// Token: 0x060019DE RID: 6622 RVA: 0x000BC946 File Offset: 0x000BAB46
	public override string Categorize()
	{
		return "Boar";
	}

	// Token: 0x04001273 RID: 4723
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 5f;
}
