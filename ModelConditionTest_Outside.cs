using System;

// Token: 0x02000264 RID: 612
public class ModelConditionTest_Outside : ModelConditionTest
{
	// Token: 0x06001CB2 RID: 7346 RVA: 0x000C76C6 File Offset: 0x000C58C6
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_Outside.CheckCondition(ent);
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x000C76D0 File Offset: 0x000C58D0
	public static bool CheckCondition(BaseEntity ent)
	{
		return ent.IsOutside(ent.WorldSpaceBounds().GetPoint(0f, 1f, 0f));
	}
}
