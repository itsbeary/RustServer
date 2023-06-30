using System;
using ConVar;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class BuildingBlockDecay : global::Decay
{
	// Token: 0x06002264 RID: 8804 RVA: 0x000DE798 File Offset: 0x000DC998
	public override float GetDecayDelay(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum @enum = (buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs);
		return base.GetDecayDelay(@enum);
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000DE7C8 File Offset: 0x000DC9C8
	public override float GetDecayDuration(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum @enum = (buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs);
		return base.GetDecayDuration(@enum);
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000DE7F8 File Offset: 0x000DC9F8
	public override bool ShouldDecay(BaseEntity entity)
	{
		if (ConVar.Decay.upkeep)
		{
			return true;
		}
		if (this.isFoundation)
		{
			return true;
		}
		BuildingBlock buildingBlock = entity as BuildingBlock;
		return (buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs) == BuildingGrade.Enum.Twigs;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000DE833 File Offset: 0x000DCA33
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.isFoundation = name.Contains("foundation");
	}

	// Token: 0x04001A85 RID: 6789
	private bool isFoundation;
}
