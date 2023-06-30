using System;

// Token: 0x02000289 RID: 649
public class WeakpointProperties : PrefabAttribute
{
	// Token: 0x06001D3D RID: 7485 RVA: 0x000CA049 File Offset: 0x000C8249
	protected override Type GetIndexedType()
	{
		return typeof(WeakpointProperties);
	}

	// Token: 0x040015C6 RID: 5574
	public bool BlockWhenRoofAttached;
}
