using System;

// Token: 0x0200026D RID: 621
public class ModelConditionTest_True : ModelConditionTest
{
	// Token: 0x06001CD5 RID: 7381 RVA: 0x000C7FF1 File Offset: 0x000C61F1
	public override bool DoTest(BaseEntity ent)
	{
		return this.reference.RunTests(ent);
	}

	// Token: 0x04001568 RID: 5480
	public ConditionalModel reference;
}
