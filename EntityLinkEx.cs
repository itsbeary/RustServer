using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003F0 RID: 1008
public static class EntityLinkEx
{
	// Token: 0x060022B7 RID: 8887 RVA: 0x000DF864 File Offset: 0x000DDA64
	public static void FreeLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			EntityLink entityLink = links[i];
			entityLink.Clear();
			Pool.Free<EntityLink>(ref entityLink);
		}
		links.Clear();
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000DF8A0 File Offset: 0x000DDAA0
	public static void ClearLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			links[i].Clear();
		}
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000DF8CC File Offset: 0x000DDACC
	public static void AddLinks(this List<EntityLink> links, BaseEntity entity, Socket_Base[] sockets)
	{
		foreach (Socket_Base socket_Base in sockets)
		{
			EntityLink entityLink = Pool.Get<EntityLink>();
			entityLink.Setup(entity, socket_Base);
			links.Add(entityLink);
		}
	}
}
