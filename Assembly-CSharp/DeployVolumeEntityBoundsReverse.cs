using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	// Token: 0x06002960 RID: 10592 RVA: 0x000FE8A4 File Offset: 0x000FCAA4
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		OBB obb = new OBB(position, this.bounds.size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, obb.extents.magnitude, list, this.layers & mask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			DeployVolume[] array = PrefabAttribute.server.FindAll<DeployVolume>(baseEntity.prefabID);
			if (DeployVolume.Check(baseEntity.transform.position, baseEntity.transform.rotation, array, obb, 1 << this.layer))
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x000FE994 File Offset: 0x000FCB94
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.layer = rootObj.layer;
	}

	// Token: 0x04002176 RID: 8566
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x04002177 RID: 8567
	private int layer;
}
