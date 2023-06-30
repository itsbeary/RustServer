using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200052A RID: 1322
public static class OnParentSpawningEx
{
	// Token: 0x06002A09 RID: 10761 RVA: 0x00101938 File Offset: 0x000FFB38
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x00101978 File Offset: 0x000FFB78
	public static void SendOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}
}
