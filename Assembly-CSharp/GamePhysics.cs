using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020002FC RID: 764
public static class GamePhysics
{
	// Token: 0x06001E67 RID: 7783 RVA: 0x000CEEF6 File Offset: 0x000CD0F6
	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(position, layerMask);
		return UnityEngine.Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x000CEF0A File Offset: 0x000CD10A
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleIgnoreCollision((start + end) * 0.5f, layerMask);
		return UnityEngine.Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x000CEF30 File Offset: 0x000CD130
	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(obb.position, layerMask);
		return UnityEngine.Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x000CEF5C File Offset: 0x000CD15C
	public static bool CheckOBBAndEntity(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(obb.position, layerMask);
		int num = UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			BaseEntity baseEntity = GamePhysics.colBuffer[i].ToBaseEntity();
			if (!(baseEntity != null) || !(ignoreEntity != null) || (baseEntity.isServer == ignoreEntity.isServer && !(baseEntity == ignoreEntity)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x000CEFDB File Offset: 0x000CD1DB
	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(bounds.center, layerMask);
		return UnityEngine.Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x000CF008 File Offset: 0x000CD208
	public static bool CheckInsideNonConvexMesh(Vector3 point, int layerMask = -5)
	{
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		int num = UnityEngine.Physics.RaycastNonAlloc(point, Vector3.up, GamePhysics.hitBuffer, 100f, layerMask);
		int num2 = UnityEngine.Physics.RaycastNonAlloc(point, -Vector3.up, GamePhysics.hitBufferB, 100f, layerMask);
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBuffer length.");
			return false;
		}
		if (num2 > GamePhysics.hitBufferB.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBufferB length.");
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (GamePhysics.hitBuffer[i].collider == GamePhysics.hitBufferB[j].collider)
				{
					UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
					return true;
				}
			}
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x000CF0D3 File Offset: 0x000CD2D3
	public static bool CheckInsideAnyCollider(Vector3 point, int layerMask = -5)
	{
		return UnityEngine.Physics.CheckSphere(point, 0f, layerMask) || GamePhysics.CheckInsideNonConvexMesh(point, layerMask) || (TerrainMeta.HeightMap != null && TerrainMeta.HeightMap.GetHeight(point) > point.y);
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x000CF113 File Offset: 0x000CD313
	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x000CF133 File Offset: 0x000CD333
	public static void CapsuleSweep(Vector3 position0, Vector3 position1, float radius, Vector3 direction, float distance, List<RaycastHit> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(position1, layerMask);
		layerMask = GamePhysics.HandleIgnoreCollision(position1, layerMask);
		GamePhysics.HitBufferToList(UnityEngine.Physics.CapsuleCastNonAlloc(position0, position1, radius, direction, GamePhysics.hitBuffer, distance, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x000CF164 File Offset: 0x000CD364
	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(point0, layerMask);
		layerMask = GamePhysics.HandleIgnoreCollision(point1, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x000CF191 File Offset: 0x000CD391
	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(obb.position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x000CF1C5 File Offset: 0x000CD3C5
	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(bounds.center, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x000CF1FC File Offset: 0x000CD3FC
	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.colBuffer[i]);
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x000CF240 File Offset: 0x000CD440
	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x000CF26C File Offset: 0x000CD46C
	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000CF298 File Offset: 0x000CD498
	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000CF2C4 File Offset: 0x000CD4C4
	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool flag = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x000CF2F0 File Offset: 0x000CD4F0
	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000CF329 File Offset: 0x000CD529
	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleIgnoreCollision(position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000CF349 File Offset: 0x000CD549
	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleIgnoreCollision(point0, layerMask);
		layerMask = GamePhysics.HandleIgnoreCollision(point1, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000CF376 File Offset: 0x000CD576
	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleIgnoreCollision(obb.position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000CF3AA File Offset: 0x000CD5AA
	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleIgnoreCollision(bounds.center, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000CF3E0 File Offset: 0x000CD5E0
	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = GamePhysics.colBuffer[i].gameObject.GetComponent<T>();
			if (component)
			{
				list.Add(component);
			}
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000CF43C File Offset: 0x000CD63C
	private static void HitBufferToList(int count, List<RaycastHit> list)
	{
		if (count >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.hitBuffer[i]);
		}
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000CF47C File Offset: 0x000CD67C
	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		if (list.Count == 0)
		{
			hitInfo = default(RaycastHit);
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		GamePhysics.Sort(list);
		hitInfo = list[0];
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x000CF4D1 File Offset: 0x000CD6D1
	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		GamePhysics.Sort(hits);
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x000CF4E8 File Offset: 0x000CD6E8
	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		int num;
		if (radius == 0f)
		{
			num = UnityEngine.Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		else
		{
			num = UnityEngine.Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		Vector3 vector;
		Vector3 vector2;
		if (num < GamePhysics.hitBuffer.Length && (layerMask & 16) != 0 && WaterSystem.Trace(ray, out vector, out vector2, maxDistance))
		{
			RaycastHit raycastHit = new RaycastHit
			{
				point = vector,
				normal = vector2,
				distance = (vector - ray.origin).magnitude
			};
			GamePhysics.hitBuffer[num++] = raycastHit;
		}
		if (num == 0)
		{
			return;
		}
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit2 = GamePhysics.hitBuffer[i];
			if (GamePhysics.Verify(raycastHit2, ignoreEntity))
			{
				hits.Add(raycastHit2);
			}
		}
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x000CF5D3 File Offset: 0x000CD7D3
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000CF5E4 File Offset: 0x000CD7E4
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding, padding, ignoreEntity);
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000CF5F5 File Offset: 0x000CD7F5
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000CF60C File Offset: 0x000CD80C
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000CF620 File Offset: 0x000CD820
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding, padding, ignoreEntity);
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x000CF633 File Offset: 0x000CD833
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x000CF650 File Offset: 0x000CD850
	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		if (!ValidBounds.Test(p0))
		{
			return false;
		}
		if (!ValidBounds.Test(p1))
		{
			return false;
		}
		Vector3 vector = p1 - p0;
		float magnitude = vector.magnitude;
		if (magnitude <= padding0 + padding1)
		{
			return true;
		}
		Vector3 vector2 = vector / magnitude;
		Ray ray = new Ray(p0 + vector2 * padding0, vector2);
		float num = magnitude - padding0 - padding1;
		RaycastHit raycastHit;
		bool flag;
		if (!ignoreEntity.IsRealNull() || (layerMask & 8388608) != 0)
		{
			flag = GamePhysics.Trace(ray, 0f, out raycastHit, num, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			if (radius > 0f && !flag)
			{
				flag = GamePhysics.Trace(ray, radius, out raycastHit, num, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			}
		}
		else
		{
			flag = UnityEngine.Physics.Raycast(ray, out raycastHit, num, layerMask, QueryTriggerInteraction.Ignore);
			if (radius > 0f && !flag)
			{
				flag = UnityEngine.Physics.SphereCast(ray, radius, out raycastHit, num, layerMask, QueryTriggerInteraction.Ignore);
			}
		}
		if (!flag)
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
				{
					60f,
					Color.green,
					p0,
					p1
				});
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
			{
				60f,
				Color.red,
				p0,
				p1
			});
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
			{
				60f,
				Color.white,
				raycastHit.point,
				raycastHit.collider.name
			});
		}
		return false;
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000CF7FF File Offset: 0x000CD9FF
	public static bool Verify(RaycastHit hitInfo, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.Verify(hitInfo.collider, hitInfo.point, ignoreEntity);
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x000CF818 File Offset: 0x000CDA18
	public static bool Verify(Collider collider, Vector3 point, BaseEntity ignoreEntity = null)
	{
		if (collider == null)
		{
			return !WaterSystem.Collision || !WaterSystem.Collision.GetIgnore(point, 0.01f);
		}
		if (collider is TerrainCollider)
		{
			return !TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(point, 0.01f);
		}
		return !GamePhysics.CompareEntity(collider.ToBaseEntity(), ignoreEntity) && collider.enabled;
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x000CF890 File Offset: 0x000CDA90
	private static bool CompareEntity(BaseEntity a, BaseEntity b)
	{
		return !a.IsRealNull() && !b.IsRealNull() && a == b;
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000CF8B0 File Offset: 0x000CDAB0
	public static int HandleIgnoreCollision(Vector3 position, int layerMask)
	{
		int num = 8388608;
		if ((layerMask & num) != 0 && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num;
		}
		int num2 = 16;
		if ((layerMask & num2) != 0 && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num2;
		}
		return layerMask;
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x000CF919 File Offset: 0x000CDB19
	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000CF940 File Offset: 0x000CDB40
	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x0400179E RID: 6046
	public const int BufferLength = 8192;

	// Token: 0x0400179F RID: 6047
	private static RaycastHit[] hitBuffer = new RaycastHit[8192];

	// Token: 0x040017A0 RID: 6048
	private static RaycastHit[] hitBufferB = new RaycastHit[8192];

	// Token: 0x040017A1 RID: 6049
	private static Collider[] colBuffer = new Collider[8192];
}
