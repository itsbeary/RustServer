using System;
using UnityEngine;

// Token: 0x020008F3 RID: 2291
public class ForceChildSingletonSetup : MonoBehaviour
{
	// Token: 0x060037B5 RID: 14261 RVA: 0x0014D17C File Offset: 0x0014B37C
	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = base.GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SingletonSetup();
		}
	}
}
