using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A32 RID: 2610
	public static class TransformEx
	{
		// Token: 0x06003DC2 RID: 15810 RVA: 0x001699F4 File Offset: 0x00167BF4
		public static string GetRecursiveName(this Transform transform, string strEndName = "")
		{
			string text = transform.name;
			if (!string.IsNullOrEmpty(strEndName))
			{
				text = text + "/" + strEndName;
			}
			if (transform.parent != null)
			{
				text = transform.parent.GetRecursiveName(text);
			}
			return text;
		}

		// Token: 0x06003DC3 RID: 15811 RVA: 0x00169A3C File Offset: 0x00167C3C
		public static void RemoveComponent<T>(this Transform transform) where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			GameManager.Destroy(component, 0f);
		}

		// Token: 0x06003DC4 RID: 15812 RVA: 0x00169A70 File Offset: 0x00167C70
		public static void RetireAllChildren(this Transform transform, GameManager gameManager)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.CompareTag("persist"))
				{
					list.Add(transform2.gameObject);
				}
			}
			foreach (GameObject gameObject in list)
			{
				gameManager.Retire(gameObject);
			}
			Pool.FreeList<GameObject>(ref list);
		}

		// Token: 0x06003DC5 RID: 15813 RVA: 0x00169B24 File Offset: 0x00167D24
		public static List<Transform> GetChildren(this Transform transform)
		{
			return transform.Cast<Transform>().ToList<Transform>();
		}

		// Token: 0x06003DC6 RID: 15814 RVA: 0x00169B34 File Offset: 0x00167D34
		public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
		{
			foreach (Transform transform in tx.Cast<Transform>().OrderBy(selector))
			{
				transform.SetAsLastSibling();
			}
		}

		// Token: 0x06003DC7 RID: 15815 RVA: 0x00169B84 File Offset: 0x00167D84
		public static List<Transform> GetAllChildren(this Transform transform)
		{
			List<Transform> list = new List<Transform>();
			if (transform != null)
			{
				transform.AddAllChildren(list);
			}
			return list;
		}

		// Token: 0x06003DC8 RID: 15816 RVA: 0x00169BA8 File Offset: 0x00167DA8
		public static void AddAllChildren(this Transform transform, List<Transform> list)
		{
			list.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(child == null))
				{
					child.AddAllChildren(list);
				}
			}
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x00169BE8 File Offset: 0x00167DE8
		public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
		{
			return (from x in transform.GetAllChildren()
				where x.CompareTag(strTag)
				select x).ToArray<Transform>();
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x00169C1E File Offset: 0x00167E1E
		public static void Identity(this GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}

		// Token: 0x06003DCB RID: 15819 RVA: 0x00169C50 File Offset: 0x00167E50
		public static GameObject CreateChild(this GameObject go)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = go.transform;
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x00169C6E File Offset: 0x00167E6E
		public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
		{
			GameObject gameObject = Instantiate.GameObject(prefab, null);
			gameObject.transform.SetParent(go.transform, false);
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x00169C90 File Offset: 0x00167E90
		public static void SetLayerRecursive(this GameObject go, int Layer)
		{
			if (go.layer != Layer)
			{
				go.layer = Layer;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursive(Layer);
			}
		}

		// Token: 0x06003DCE RID: 15822 RVA: 0x00169CDC File Offset: 0x00167EDC
		public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
		{
			Vector3 vector;
			Vector3 vector2;
			if (transform.GetGroundInfo(out vector, out vector2, fRange))
			{
				transform.position = vector;
				if (alignToNormal)
				{
					transform.rotation = Quaternion.LookRotation(transform.forward, vector2);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x00169D15 File Offset: 0x00167F15
		public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
		}

		// Token: 0x06003DD0 RID: 15824 RVA: 0x00169D26 File Offset: 0x00167F26
		public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
		}

		// Token: 0x06003DD1 RID: 15825 RVA: 0x00169D38 File Offset: 0x00167F38
		public static Bounds WorkoutRenderBounds(this Transform tx)
		{
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach (Renderer renderer in tx.GetComponentsInChildren<Renderer>())
			{
				if (!(renderer is ParticleSystemRenderer))
				{
					if (bounds.center == Vector3.zero)
					{
						bounds = renderer.bounds;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
			return bounds;
		}

		// Token: 0x06003DD2 RID: 15826 RVA: 0x00169DA4 File Offset: 0x00167FA4
		public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
		{
			List<T> list = new List<T>();
			if (transform.parent == null)
			{
				return list;
			}
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				Transform child = transform.parent.GetChild(i);
				if (includeSelf || !(child == transform))
				{
					T component = child.GetComponent<T>();
					if (component != null)
					{
						list.Add(component);
					}
				}
			}
			return list;
		}

		// Token: 0x06003DD3 RID: 15827 RVA: 0x00169E10 File Offset: 0x00168010
		public static void DestroyChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				GameManager.Destroy(transform.GetChild(i).gameObject, 0f);
			}
		}

		// Token: 0x06003DD4 RID: 15828 RVA: 0x00169E44 File Offset: 0x00168044
		public static void SetChildrenActive(this Transform transform, bool b)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(b);
			}
		}

		// Token: 0x06003DD5 RID: 15829 RVA: 0x00169E74 File Offset: 0x00168074
		public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
		{
			Transform transform2 = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					transform2 = child;
					child.gameObject.SetActive(true);
				}
				else if (bDisableOthers)
				{
					child.gameObject.SetActive(false);
				}
			}
			return transform2;
		}

		// Token: 0x06003DD6 RID: 15830 RVA: 0x00169ECC File Offset: 0x001680CC
		public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			T t = ((list.Count > 0) ? list[0] : default(T));
			Pool.FreeList<T>(ref list);
			return t;
		}

		// Token: 0x06003DD7 RID: 15831 RVA: 0x00169F0C File Offset: 0x0016810C
		public static bool HasComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			bool flag = list.Count > 0;
			Pool.FreeList<T>(ref list);
			return flag;
		}

		// Token: 0x06003DD8 RID: 15832 RVA: 0x00169F37 File Offset: 0x00168137
		public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
		{
			transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003DD9 RID: 15833 RVA: 0x00169F50 File Offset: 0x00168150
		public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
		{
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			if (includeRenderers)
			{
				foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>(includeInactive))
				{
					if (meshFilter.sharedMesh)
					{
						Matrix4x4 matrix4x = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
						Bounds bounds2 = meshFilter.sharedMesh.bounds;
						bounds.Encapsulate(bounds2.Transform(matrix4x));
					}
				}
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive))
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						Matrix4x4 matrix4x2 = transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
						Bounds bounds3 = skinnedMeshRenderer.sharedMesh.bounds;
						bounds.Encapsulate(bounds3.Transform(matrix4x2));
					}
				}
			}
			if (includeColliders)
			{
				foreach (MeshCollider meshCollider in transform.GetComponentsInChildren<MeshCollider>(includeInactive))
				{
					if (meshCollider.sharedMesh && !meshCollider.isTrigger)
					{
						Matrix4x4 matrix4x3 = transform.worldToLocalMatrix * meshCollider.transform.localToWorldMatrix;
						Bounds bounds4 = meshCollider.sharedMesh.bounds;
						bounds.Encapsulate(bounds4.Transform(matrix4x3));
					}
				}
			}
			return bounds;
		}
	}
}
