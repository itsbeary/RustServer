using System;

// Token: 0x0200051A RID: 1306
public class GameModeHardcore : GameModeVanilla
{
	// Token: 0x060029C1 RID: 10689 RVA: 0x00100105 File Offset: 0x000FE305
	protected override void OnCreated()
	{
		base.OnCreated();
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x00100110 File Offset: 0x000FE310
	public override BaseGameMode.ResearchCostResult GetScrapCostForResearch(ItemDefinition item, ResearchTable.ResearchType researchType)
	{
		ItemBlueprint blueprint = item.Blueprint;
		int? num = ((blueprint != null) ? new int?(blueprint.workbenchLevelRequired) : null);
		if (num != null)
		{
			switch (num.GetValueOrDefault())
			{
			case 1:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.2f)
				};
			case 2:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.4f)
				};
			case 3:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.6f)
				};
			}
		}
		return default(BaseGameMode.ResearchCostResult);
	}
}
