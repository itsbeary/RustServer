using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200052C RID: 1324
public static class OnPostNetworkUpdateEx
{
	// Token: 0x06002A0C RID: 10764 RVA: 0x001019B8 File Offset: 0x000FFBB8
	public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponentsInChildren<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x001019F8 File Offset: 0x000FFBF8
	public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponents<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}
}
