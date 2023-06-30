using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000528 RID: 1320
public static class OnParentDestroyingEx
{
	// Token: 0x06002A06 RID: 10758 RVA: 0x001018B8 File Offset: 0x000FFAB8
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x001018F8 File Offset: 0x000FFAF8
	public static void SendOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}
}
