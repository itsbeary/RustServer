using System;

// Token: 0x02000260 RID: 608
public abstract class ModelConditionTest : PrefabAttribute
{
	// Token: 0x06001CA7 RID: 7335
	public abstract bool DoTest(BaseEntity ent);

	// Token: 0x06001CA8 RID: 7336 RVA: 0x000C74C1 File Offset: 0x000C56C1
	protected override Type GetIndexedType()
	{
		return typeof(ModelConditionTest);
	}
}
