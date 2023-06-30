using System;
using UnityEngine;

// Token: 0x020008F8 RID: 2296
public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x060037BF RID: 14271 RVA: 0x0014D24D File Offset: 0x0014B44D
	public bool isServer
	{
		get
		{
			return !this.isClient;
		}
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x0014D258 File Offset: 0x0014B458
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.prefabID = StringPool.Get(name);
		this.isClient = clientside;
	}

	// Token: 0x0400331A RID: 13082
	[HideInInspector]
	public uint prefabID;

	// Token: 0x0400331B RID: 13083
	[HideInInspector]
	public bool isClient;
}
