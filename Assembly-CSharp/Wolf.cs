using System;

// Token: 0x02000202 RID: 514
public class Wolf : BaseAnimalNPC
{
	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x000C034D File Offset: 0x000BE54D
	public override float RealisticMass
	{
		get
		{
			return 45f;
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001AF1 RID: 6897 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000C0354 File Offset: 0x000BE554
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && (best.HasTrait(BaseEntity.TraitFlag.Meat) || base.WantsToEat(best));
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000C0374 File Offset: 0x000BE574
	public override string Categorize()
	{
		return "Wolf";
	}

	// Token: 0x040012F1 RID: 4849
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;
}
