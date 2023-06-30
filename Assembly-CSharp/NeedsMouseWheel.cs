using System;

// Token: 0x020007A9 RID: 1961
public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	// Token: 0x06003510 RID: 13584 RVA: 0x00145845 File Offset: 0x00143A45
	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.Count > 0;
	}
}
