using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000959 RID: 2393
public static class TransformUtil
{
	// Token: 0x0600395A RID: 14682 RVA: 0x00154331 File Offset: 0x00152531
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x00154346 File Offset: 0x00152546
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, range, -1, ignoreTransform);
	}

	// Token: 0x0600395C RID: 14684 RVA: 0x00154358 File Offset: 0x00152558
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(new Ray(startPos, Vector3.down), 0f, out raycastHit, range, mask, QueryTriggerInteraction.UseGlobal, null))
		{
			return false;
		}
		if (ignoreTransform != null && (raycastHit.collider.transform == ignoreTransform || raycastHit.collider.transform.IsChildOf(ignoreTransform)))
		{
			return TransformUtil.GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
		}
		hitOut = raycastHit;
		return true;
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x0015440A File Offset: 0x0015260A
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x00154420 File Offset: 0x00152620
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, -1, ignoreTransform);
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x00154434 File Offset: 0x00152634
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(startPos, Vector3.down), 0f, list, range, mask, QueryTriggerInteraction.Ignore, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(ignoreTransform != null) || (!(raycastHit.collider.transform == ignoreTransform) && !raycastHit.collider.transform.IsChildOf(ignoreTransform)))
			{
				pos = raycastHit.point;
				normal = raycastHit.normal;
				Pool.FreeList<RaycastHit>(ref list);
				return true;
			}
		}
		pos = startPos;
		normal = Vector3.up;
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x00154530 File Offset: 0x00152730
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, -1);
	}

	// Token: 0x06003961 RID: 14689 RVA: 0x00154545 File Offset: 0x00152745
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, -1);
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x00154558 File Offset: 0x00152758
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask) && raycastHit.collider is TerrainCollider)
		{
			pos = raycastHit.point;
			normal = raycastHit.normal;
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x001545D7 File Offset: 0x001527D7
	public static Transform[] GetRootObjects()
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Transform>()
			where x.transform == x.transform.root
			select x).ToArray<Transform>();
	}
}
