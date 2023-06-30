using System;
using UnityEngine;

// Token: 0x020008CA RID: 2250
public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06003750 RID: 14160 RVA: 0x0014C384 File Offset: 0x0014A584
	public static float currentMax
	{
		get
		{
			if (ListComponent<UIBackgroundBlur>.InstanceList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < ListComponent<UIBackgroundBlur>.InstanceList.Count; i++)
			{
				num = Mathf.Max(ListComponent<UIBackgroundBlur>.InstanceList[i].amount, num);
			}
			return num;
		}
	}

	// Token: 0x040032AD RID: 12973
	public float amount = 1f;
}
