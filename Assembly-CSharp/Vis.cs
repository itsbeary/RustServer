using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public static class Vis
{
	// Token: 0x06002C11 RID: 11281 RVA: 0x0010AA6C File Offset: 0x00108C6C
	private static void Buffer(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x0010AAD7 File Offset: 0x00108CD7
	public static bool AnyColliders(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		return Vis.colCount > 0;
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x0010AAEC File Offset: 0x00108CEC
	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x0010AB48 File Offset: 0x00108D48
	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x0010ABA4 File Offset: 0x00108DA4
	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : class
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (t != null)
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x0010AC04 File Offset: 0x00108E04
	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x0010AC70 File Offset: 0x00108E70
	private static void Buffer(OBB bounds, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(bounds.position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, Vis.colBuffer, bounds.rotation, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x0010ACF0 File Offset: 0x00108EF0
	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x0010AD4C File Offset: 0x00108F4C
	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x0010ADA8 File Offset: 0x00108FA8
	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x0010AE0C File Offset: 0x0010900C
	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x0010AE78 File Offset: 0x00109078
	private static void Buffer(Vector3 startPosition, Vector3 endPosition, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleIgnoreCollision(startPosition, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapCapsuleNonAlloc(startPosition, endPosition, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x0010AEE8 File Offset: 0x001090E8
	public static void Colliders<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x0010AF48 File Offset: 0x00109148
	public static void Components<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x0010AFA8 File Offset: 0x001091A8
	public static void Entities<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x0010B010 File Offset: 0x00109210
	public static void EntityComponents<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x040023CD RID: 9165
	private static int colCount = 0;

	// Token: 0x040023CE RID: 9166
	private static Collider[] colBuffer = new Collider[8192];
}
