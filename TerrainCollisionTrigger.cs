using System;
using UnityEngine;

// Token: 0x0200069F RID: 1695
public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x06003040 RID: 12352 RVA: 0x0012216B File Offset: 0x0012036B
	protected void OnTriggerEnter(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, true);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x0012218A File Offset: 0x0012038A
	protected void OnTriggerExit(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, false);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x001221AC File Offset: 0x001203AC
	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = other.GetComponent<TerrainCollisionProxy>();
		if (component)
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore(component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
