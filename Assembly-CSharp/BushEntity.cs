using System;
using ConVar;
using UnityEngine;

// Token: 0x0200045F RID: 1119
public class BushEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x0600252A RID: 9514 RVA: 0x000EB364 File Offset: 0x000E9564
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] array = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(array);
		}
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000EB39C File Offset: 0x000E959C
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeSpawned(this);
		}
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000EB3B2 File Offset: 0x000E95B2
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeDestroyed(this);
		}
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000A1365 File Offset: 0x0009F565
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}

	// Token: 0x04001D6A RID: 7530
	public GameObjectRef prefab;

	// Token: 0x04001D6B RID: 7531
	public bool globalBillboard = true;
}
