using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069D RID: 1693
public class TerrainCollision : TerrainExtension
{
	// Token: 0x06003036 RID: 12342 RVA: 0x00121F2A File Offset: 0x0012012A
	public override void Setup()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.terrainCollider = this.terrain.GetComponent<TerrainCollider>();
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x00121F48 File Offset: 0x00120148
	public void Clear()
	{
		if (!this.terrainCollider)
		{
			return;
		}
		foreach (Collider collider in this.ignoredColliders.Keys)
		{
			Physics.IgnoreCollision(collider, this.terrainCollider, false);
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00121FC0 File Offset: 0x001201C0
	public void Reset(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		Physics.IgnoreCollision(collider, this.terrainCollider, false);
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x00121FF2 File Offset: 0x001201F2
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x00122001 File Offset: 0x00120201
	public bool GetIgnore(RaycastHit hit)
	{
		return hit.collider is TerrainCollider && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x00122025 File Offset: 0x00120225
	public bool GetIgnore(Collider collider)
	{
		return this.terrainCollider && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x0012204C File Offset: 0x0012024C
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		if (!this.GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider> { trigger };
				Physics.IgnoreCollision(collider, this.terrainCollider, true);
				this.ignoredColliders.Add(collider, list);
				return;
			}
		}
		else
		{
			List<Collider> list2 = this.ignoredColliders[collider];
			if (ignore)
			{
				if (!list2.Contains(trigger))
				{
					list2.Add(trigger);
					return;
				}
			}
			else if (list2.Contains(trigger))
			{
				list2.Remove(trigger);
			}
		}
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x001220D8 File Offset: 0x001202D8
	protected void LateUpdate()
	{
		if (this.ignoredColliders == null)
		{
			return;
		}
		for (int i = 0; i < this.ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if (key == null)
			{
				this.ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				Physics.IgnoreCollision(key, this.terrainCollider, false);
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}

	// Token: 0x040027F5 RID: 10229
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x040027F6 RID: 10230
	private TerrainCollider terrainCollider;
}
