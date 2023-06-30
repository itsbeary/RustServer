using System;

// Token: 0x020001E7 RID: 487
public class Bear : BaseAnimalNPC
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060019CF RID: 6607 RVA: 0x000BC890 File Offset: 0x000BAA90
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060019D0 RID: 6608 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000BC89B File Offset: 0x000BAA9B
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x000BC8AF File Offset: 0x000BAAAF
	public override string Categorize()
	{
		return "Bear";
	}

	// Token: 0x04001271 RID: 4721
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;
}
