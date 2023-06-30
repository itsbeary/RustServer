using System;

// Token: 0x020001E8 RID: 488
public class Polarbear : BaseAnimalNPC
{
	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060019D5 RID: 6613 RVA: 0x000BC890 File Offset: 0x000BAA90
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x060019D6 RID: 6614 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000BC89B File Offset: 0x000BAA9B
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x000BC8CA File Offset: 0x000BAACA
	public override string Categorize()
	{
		return "Polarbear";
	}

	// Token: 0x04001272 RID: 4722
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 1f;
}
