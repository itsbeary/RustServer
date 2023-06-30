using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000655 RID: 1621
public class SpawnableBoundsBlocker : MonoBehaviour
{
	// Token: 0x06002EE4 RID: 12004 RVA: 0x0011A000 File Offset: 0x00118200
	[Button("Clear Trees")]
	public void ClearTrees()
	{
		List<TreeEntity> list = Pool.GetList<TreeEntity>();
		if (this.BoxCollider != null)
		{
			GamePhysics.OverlapOBB<TreeEntity>(new OBB(base.transform.TransformPoint(this.BoxCollider.center), this.BoxCollider.size + Vector3.one, base.transform.rotation), list, 1073741824, QueryTriggerInteraction.Collide);
		}
		foreach (TreeEntity treeEntity in list)
		{
			BoundsCheck boundsCheck = PrefabAttribute.server.Find<BoundsCheck>(treeEntity.prefabID);
			if (boundsCheck != null && boundsCheck.IsType == this.BlockType)
			{
				treeEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<TreeEntity>(ref list);
	}

	// Token: 0x040026C5 RID: 9925
	public BoundsCheck.BlockType BlockType;

	// Token: 0x040026C6 RID: 9926
	public BoxCollider BoxCollider;
}
