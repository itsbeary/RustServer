using System;
using System.Collections.Generic;

// Token: 0x02000117 RID: 279
public class LaserDetector : BaseDetector
{
	// Token: 0x06001670 RID: 5744 RVA: 0x000AE368 File Offset: 0x000AC568
	public override void OnObjects()
	{
		using (HashSet<BaseEntity>.Enumerator enumerator = this.myTrigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsVisible(base.transform.position + base.transform.forward * 0.1f, 4f))
				{
					base.OnObjects();
					break;
				}
			}
		}
	}
}
