using System;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	// Token: 0x060021C7 RID: 8647 RVA: 0x000DC434 File Offset: 0x000DA634
	private void OnCollisionEnter(Collision collision)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (base.baseEntity.IsDestroyed)
		{
			return;
		}
		BaseEntity baseEntity = collision.GetEntity();
		if (baseEntity == base.baseEntity)
		{
			return;
		}
		if (baseEntity != null)
		{
			if (baseEntity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isServer)
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
			}
		}
		base.baseEntity.OnCollision(collision, baseEntity);
	}
}
