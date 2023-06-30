using System;

// Token: 0x0200026E RID: 622
public class ModelConditionTest_Wall : ModelConditionTest
{
	// Token: 0x06001CD7 RID: 7383 RVA: 0x000C7FFF File Offset: 0x000C61FF
	public override bool DoTest(BaseEntity ent)
	{
		return !ModelConditionTest_WallTriangleLeft.CheckCondition(ent) && !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
