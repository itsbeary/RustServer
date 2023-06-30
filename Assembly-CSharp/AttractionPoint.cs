using System;

// Token: 0x02000252 RID: 594
public class AttractionPoint : PrefabAttribute
{
	// Token: 0x06001C69 RID: 7273 RVA: 0x000C5B8F File Offset: 0x000C3D8F
	protected override Type GetIndexedType()
	{
		return typeof(AttractionPoint);
	}

	// Token: 0x040014FD RID: 5373
	public string groupName;
}
