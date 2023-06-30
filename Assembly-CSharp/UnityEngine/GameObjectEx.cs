using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Registry;

namespace UnityEngine
{
	// Token: 0x02000A2B RID: 2603
	public static class GameObjectEx
	{
		// Token: 0x06003DAC RID: 15788 RVA: 0x001695FD File Offset: 0x001677FD
		public static BaseEntity ToBaseEntity(this GameObject go)
		{
			return go.transform.ToBaseEntity();
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x0016960A File Offset: 0x0016780A
		public static BaseEntity ToBaseEntity(this Collider collider)
		{
			return collider.transform.ToBaseEntity();
		}

		// Token: 0x06003DAE RID: 15790 RVA: 0x00169618 File Offset: 0x00167818
		public static BaseEntity ToBaseEntity(this Transform transform)
		{
			IEntity entity = GameObjectEx.GetEntityFromRegistry(transform);
			if (entity == null && !transform.gameObject.activeInHierarchy)
			{
				entity = GameObjectEx.GetEntityFromComponent(transform);
			}
			return entity as BaseEntity;
		}

		// Token: 0x06003DAF RID: 15791 RVA: 0x00169649 File Offset: 0x00167849
		public static bool IsOnLayer(this GameObject go, Layer rustLayer)
		{
			return go.IsOnLayer((int)rustLayer);
		}

		// Token: 0x06003DB0 RID: 15792 RVA: 0x00169652 File Offset: 0x00167852
		public static bool IsOnLayer(this GameObject go, int layer)
		{
			return go != null && go.layer == layer;
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x00169668 File Offset: 0x00167868
		private static IEntity GetEntityFromRegistry(Transform transform)
		{
			Transform transform2 = transform;
			IEntity entity = Entity.Get(transform2);
			while (entity == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				entity = Entity.Get(transform2);
			}
			if (entity != null && !entity.IsDestroyed)
			{
				return entity;
			}
			return null;
		}

		// Token: 0x06003DB2 RID: 15794 RVA: 0x001696B0 File Offset: 0x001678B0
		private static IEntity GetEntityFromComponent(Transform transform)
		{
			Transform transform2 = transform;
			IEntity entity = transform2.GetComponent<IEntity>();
			while (entity == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				entity = transform2.GetComponent<IEntity>();
			}
			if (entity != null && !entity.IsDestroyed)
			{
				return entity;
			}
			return null;
		}

		// Token: 0x06003DB3 RID: 15795 RVA: 0x001696F5 File Offset: 0x001678F5
		public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003DB4 RID: 15796 RVA: 0x00169710 File Offset: 0x00167910
		public static bool HasComponent<T>(this GameObject obj) where T : Component
		{
			return obj.GetComponent<T>() != null;
		}

		// Token: 0x06003DB5 RID: 15797 RVA: 0x00169724 File Offset: 0x00167924
		public static void SetChildComponentsEnabled<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
		{
			List<T> list = Pool.GetList<T>();
			gameObject.GetComponentsInChildren<T>(true, list);
			foreach (T t in list)
			{
				t.enabled = enabled;
			}
			Pool.FreeList<T>(ref list);
		}
	}
}
