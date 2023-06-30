using System;

// Token: 0x02000203 RID: 515
public class Zombie : BaseAnimalNPC
{
	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x000BC897 File Offset: 0x000BAA97
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x000BC89B File Offset: 0x000BAA9B
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x000C0387 File Offset: 0x000BE587
	protected override void TickSleep()
	{
		this.Sleep = 100f;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x000C0394 File Offset: 0x000BE594
	public override string Categorize()
	{
		return "Zombie";
	}

	// Token: 0x040012F2 RID: 4850
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;
}
