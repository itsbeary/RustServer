using System;

// Token: 0x02000201 RID: 513
public class Stag : BaseAnimalNPC
{
	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001AEA RID: 6890 RVA: 0x000C02D1 File Offset: 0x000BE4D1
	public override float RealisticMass
	{
		get
		{
			return 200f;
		}
	}

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06001AEB RID: 6891 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x000C02D8 File Offset: 0x000BE4D8
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

	// Token: 0x06001AED RID: 6893 RVA: 0x000C033A File Offset: 0x000BE53A
	public override string Categorize()
	{
		return "Stag";
	}

	// Token: 0x040012F0 RID: 4848
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;
}
