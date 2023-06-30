using System;

// Token: 0x02000261 RID: 609
public class ModelConditionTest_False : ModelConditionTest
{
	// Token: 0x06001CAA RID: 7338 RVA: 0x000C74CD File Offset: 0x000C56CD
	public override bool DoTest(BaseEntity ent)
	{
		return !this.reference.RunTests(ent);
	}

	// Token: 0x0400153A RID: 5434
	public ConditionalModel reference;
}
