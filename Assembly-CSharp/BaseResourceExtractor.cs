using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class BaseResourceExtractor : BaseCombatEntity
{
	// Token: 0x06002339 RID: 9017 RVA: 0x000E14EC File Offset: 0x000DF6EC
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 3f, list, 1, QueryTriggerInteraction.Collide);
		foreach (SurveyCrater surveyCrater in list)
		{
			if (surveyCrater.isServer)
			{
				surveyCrater.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<SurveyCrater>(ref list);
	}

	// Token: 0x04001B19 RID: 6937
	public bool canExtractLiquid;

	// Token: 0x04001B1A RID: 6938
	public bool canExtractSolid = true;
}
