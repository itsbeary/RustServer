using System;
using Rust;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public static class RaycastHitEx
{
	// Token: 0x0600384C RID: 14412 RVA: 0x0014F31D File Offset: 0x0014D51D
	public static Transform GetTransform(this RaycastHit hit)
	{
		return hit.transform;
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x0014F326 File Offset: 0x0014D526
	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		return hit.rigidbody;
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x0014F32F File Offset: 0x0014D52F
	public static Collider GetCollider(this RaycastHit hit)
	{
		return hit.collider;
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x0014F338 File Offset: 0x0014D538
	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		if (!(hit.collider != null))
		{
			return null;
		}
		return hit.collider.ToBaseEntity();
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x0014F357 File Offset: 0x0014D557
	public static bool IsOnLayer(this RaycastHit hit, Layer rustLayer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(rustLayer);
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x0014F37C File Offset: 0x0014D57C
	public static bool IsOnLayer(this RaycastHit hit, int layer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(layer);
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x0014F3A1 File Offset: 0x0014D5A1
	public static bool IsWaterHit(this RaycastHit hit)
	{
		return hit.collider == null || hit.collider.gameObject.IsOnLayer(Layer.Water);
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x0014F3C8 File Offset: 0x0014D5C8
	public static WaterBody GetWaterBody(this RaycastHit hit)
	{
		if (hit.collider == null)
		{
			return WaterSystem.Ocean;
		}
		Transform transform = hit.collider.transform;
		WaterBody waterBody;
		if (transform.TryGetComponent<WaterBody>(out waterBody))
		{
			return waterBody;
		}
		return transform.parent.GetComponentInChildren<WaterBody>();
	}
}
