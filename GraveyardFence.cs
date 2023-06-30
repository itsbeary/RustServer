using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class GraveyardFence : SimpleBuildingBlock
{
	// Token: 0x06001772 RID: 6002 RVA: 0x000B1FF5 File Offset: 0x000B01F5
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdatePillars();
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x000B2004 File Offset: 0x000B0204
	public override void DestroyShared()
	{
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities<GraveyardFence>(base.transform.position, 5f, list, 2097152, QueryTriggerInteraction.Collide);
		foreach (GraveyardFence graveyardFence in list)
		{
			graveyardFence.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x000B2080 File Offset: 0x000B0280
	public virtual void UpdatePillars()
	{
		foreach (BoxCollider boxCollider in this.pillars)
		{
			boxCollider.gameObject.SetActive(true);
			foreach (Collider collider in Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, 2097152))
			{
				if (collider.CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
					if (!(baseEntity == null) && !base.EqualNetID(baseEntity) && collider != boxCollider)
					{
						boxCollider.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x04001015 RID: 4117
	public BoxCollider[] pillars;
}
