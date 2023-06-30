using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class SocketMod_BuildingBlock : SocketMod
{
	// Token: 0x06001CFF RID: 7423 RVA: 0x000C8B94 File Offset: 0x000C6D94
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000C8C04 File Offset: 0x000C6E04
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(vector, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		bool flag = list.Count > 0;
		if (flag && this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (flag && !this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x04001589 RID: 5513
	public float sphereRadius = 1f;

	// Token: 0x0400158A RID: 5514
	public LayerMask layerMask;

	// Token: 0x0400158B RID: 5515
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x0400158C RID: 5516
	public bool wantsCollide;
}
