using System;

// Token: 0x02000263 RID: 611
public class ModelConditionTest_Inside : ModelConditionTest
{
	// Token: 0x06001CB0 RID: 7344 RVA: 0x000C76BB File Offset: 0x000C58BB
	public override bool DoTest(BaseEntity ent)
	{
		return !ModelConditionTest_Outside.CheckCondition(ent);
	}
}
