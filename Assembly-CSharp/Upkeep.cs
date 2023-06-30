using System;

// Token: 0x020003EC RID: 1004
public class Upkeep : PrefabAttribute
{
	// Token: 0x06002293 RID: 8851 RVA: 0x000DF24D File Offset: 0x000DD44D
	protected override Type GetIndexedType()
	{
		return typeof(Upkeep);
	}

	// Token: 0x04001A94 RID: 6804
	public float upkeepMultiplier = 1f;
}
