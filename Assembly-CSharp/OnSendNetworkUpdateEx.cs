using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200052E RID: 1326
public static class OnSendNetworkUpdateEx
{
	// Token: 0x06002A0F RID: 10767 RVA: 0x00101A38 File Offset: 0x000FFC38
	public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponentsInChildren<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x00101A78 File Offset: 0x000FFC78
	public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponents<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}
}
