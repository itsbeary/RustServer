using System;

// Token: 0x020001EE RID: 494
public class Horse : BaseAnimalNPC
{
	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001A0B RID: 6667 RVA: 0x000BD812 File Offset: 0x000BBA12
	public override float RealisticMass
	{
		get
		{
			return 500f;
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06001A0C RID: 6668 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x000BD819 File Offset: 0x000BBA19
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x000BD824 File Offset: 0x000BBA24
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

	// Token: 0x06001A0F RID: 6671 RVA: 0x000BD886 File Offset: 0x000BBA86
	public override string Categorize()
	{
		return "Horse";
	}

	// Token: 0x0400129D RID: 4765
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;
}
