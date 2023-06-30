using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class ConstructionGrade : PrefabAttribute
{
	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001C81 RID: 7297 RVA: 0x000C6985 File Offset: 0x000C4B85
	public float maxHealth
	{
		get
		{
			if (!this.gradeBase || !this.construction)
			{
				return 0f;
			}
			return this.gradeBase.baseHealth * this.construction.healthMultiplier;
		}
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x000C69C0 File Offset: 0x000C4BC0
	public List<ItemAmount> CostToBuild(BuildingGrade.Enum fromGrade = BuildingGrade.Enum.None)
	{
		if (this._costToBuild == null)
		{
			this._costToBuild = new List<ItemAmount>();
		}
		else
		{
			this._costToBuild.Clear();
		}
		float num = ((fromGrade == this.gradeBase.type) ? 0.2f : 1f);
		foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
		{
			this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier * num)));
		}
		return this._costToBuild;
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x000C6A84 File Offset: 0x000C4C84
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}

	// Token: 0x04001520 RID: 5408
	[NonSerialized]
	public Construction construction;

	// Token: 0x04001521 RID: 5409
	public BuildingGrade gradeBase;

	// Token: 0x04001522 RID: 5410
	public GameObjectRef skinObject;

	// Token: 0x04001523 RID: 5411
	internal List<ItemAmount> _costToBuild;
}
