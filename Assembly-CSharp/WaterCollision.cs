using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000708 RID: 1800
public class WaterCollision : MonoBehaviour
{
	// Token: 0x060032AF RID: 12975 RVA: 0x001385E0 File Offset: 0x001367E0
	private void Awake()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.waterColliders = new HashSet<Collider>();
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x001385F8 File Offset: 0x001367F8
	public void Clear()
	{
		if (this.waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (Collider collider in this.ignoredColliders.Keys)
			{
				Physics.IgnoreCollision(collider, enumerator.Current, false);
			}
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x00138688 File Offset: 0x00136888
	public void Reset(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		foreach (Collider collider2 in this.waterColliders)
		{
			Physics.IgnoreCollision(collider, collider2, false);
		}
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x001386DD File Offset: 0x001368DD
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x001386EC File Offset: 0x001368EC
	public bool GetIgnore(Bounds bounds)
	{
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x001386FA File Offset: 0x001368FA
	public bool GetIgnore(Vector3 start, Vector3 end, float radius)
	{
		return GamePhysics.CheckCapsule<WaterVisibilityTrigger>(start, end, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x0013870A File Offset: 0x0013690A
	public bool GetIgnore(RaycastHit hit)
	{
		return this.waterColliders.Contains(hit.collider) && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x00138734 File Offset: 0x00136934
	public bool GetIgnore(Collider collider)
	{
		return this.waterColliders.Count != 0 && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x060032B7 RID: 12983 RVA: 0x0013875C File Offset: 0x0013695C
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		if (!this.GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider> { trigger };
				foreach (Collider collider2 in this.waterColliders)
				{
					Physics.IgnoreCollision(collider, collider2, true);
				}
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

	// Token: 0x060032B8 RID: 12984 RVA: 0x00138800 File Offset: 0x00136A00
	protected void LateUpdate()
	{
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
				foreach (Collider collider in this.waterColliders)
				{
					Physics.IgnoreCollision(key, collider, false);
				}
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}

	// Token: 0x04002987 RID: 10631
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x04002988 RID: 10632
	private HashSet<Collider> waterColliders;
}
