using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000918 RID: 2328
public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06003816 RID: 14358 RVA: 0x0014E1CE File Offset: 0x0014C3CE
	public bool isClient
	{
		get
		{
			return !this.isServer;
		}
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x0014E1DC File Offset: 0x0014C3DC
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.fullName = name;
		this.hierachyName = base.transform.GetRecursiveName("");
		this.prefabID = StringPool.Get(name);
		this.instanceID = base.GetInstanceID();
		this.worldPosition = base.transform.position;
		this.worldRotation = base.transform.rotation;
		this.worldForward = base.transform.forward;
		this.localPosition = base.transform.localPosition;
		this.localScale = base.transform.localScale;
		this.localRotation = base.transform.localRotation;
		if (serverside)
		{
			this.prefabAttribute = PrefabAttribute.server;
			this.gameManager = GameManager.server;
			this.isServer = true;
		}
		this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			PrefabAttribute.server.Add(this.prefabID, this);
		}
		preProcess.RemoveComponent(this);
		preProcess.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x06003819 RID: 14361
	protected abstract Type GetIndexedType();

	// Token: 0x0600381A RID: 14362 RVA: 0x0014E2E0 File Offset: 0x0014C4E0
	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x0600381B RID: 14363 RVA: 0x0014E2E9 File Offset: 0x0014C4E9
	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x0014E2F8 File Offset: 0x0014C4F8
	public override bool Equals(object o)
	{
		PrefabAttribute prefabAttribute;
		return (prefabAttribute = o as PrefabAttribute) != null && PrefabAttribute.ComparePrefabAttribute(this, prefabAttribute);
	}

	// Token: 0x0600381D RID: 14365 RVA: 0x0014E318 File Offset: 0x0014C518
	public override int GetHashCode()
	{
		if (this.hierachyName == null)
		{
			return base.GetHashCode();
		}
		return this.hierachyName.GetHashCode();
	}

	// Token: 0x0600381E RID: 14366 RVA: 0x0014DC88 File Offset: 0x0014BE88
	public static implicit operator bool(PrefabAttribute exists)
	{
		return exists != null;
	}

	// Token: 0x0600381F RID: 14367 RVA: 0x0014E334 File Offset: 0x0014C534
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = x == null;
		bool flag2 = y == null;
		return (flag && flag2) || (!flag && !flag2 && x.instanceID == y.instanceID);
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x0014E36A File Offset: 0x0014C56A
	public override string ToString()
	{
		if (this == null)
		{
			return "null";
		}
		return this.hierachyName;
	}

	// Token: 0x04003381 RID: 13185
	[NonSerialized]
	public Vector3 worldPosition;

	// Token: 0x04003382 RID: 13186
	[NonSerialized]
	public Quaternion worldRotation;

	// Token: 0x04003383 RID: 13187
	[NonSerialized]
	public Vector3 worldForward;

	// Token: 0x04003384 RID: 13188
	[NonSerialized]
	public Vector3 localPosition;

	// Token: 0x04003385 RID: 13189
	[NonSerialized]
	public Vector3 localScale;

	// Token: 0x04003386 RID: 13190
	[NonSerialized]
	public Quaternion localRotation;

	// Token: 0x04003387 RID: 13191
	[NonSerialized]
	public string fullName;

	// Token: 0x04003388 RID: 13192
	[NonSerialized]
	public string hierachyName;

	// Token: 0x04003389 RID: 13193
	[NonSerialized]
	public uint prefabID;

	// Token: 0x0400338A RID: 13194
	[NonSerialized]
	public int instanceID;

	// Token: 0x0400338B RID: 13195
	[NonSerialized]
	public PrefabAttribute.Library prefabAttribute;

	// Token: 0x0400338C RID: 13196
	[NonSerialized]
	public GameManager gameManager;

	// Token: 0x0400338D RID: 13197
	[NonSerialized]
	public bool isServer;

	// Token: 0x0400338E RID: 13198
	public static PrefabAttribute.Library server = new PrefabAttribute.Library(false, true);

	// Token: 0x02000ECB RID: 3787
	public class AttributeCollection
	{
		// Token: 0x0600535E RID: 21342 RVA: 0x001B24A8 File Offset: 0x001B06A8
		internal List<PrefabAttribute> Find(Type t)
		{
			List<PrefabAttribute> list;
			if (this.attributes.TryGetValue(t, out list))
			{
				return list;
			}
			list = new List<PrefabAttribute>();
			this.attributes.Add(t, list);
			return list;
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x001B24DC File Offset: 0x001B06DC
		public T[] Find<T>()
		{
			if (this.cache == null)
			{
				this.cache = new Dictionary<Type, object>();
			}
			object obj;
			if (this.cache.TryGetValue(typeof(T), out obj))
			{
				return (T[])obj;
			}
			obj = this.Find(typeof(T)).Cast<T>().ToArray<T>();
			this.cache.Add(typeof(T), obj);
			return (T[])obj;
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x001B2553 File Offset: 0x001B0753
		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> list = this.Find(attribute.GetIndexedType());
			Assert.IsTrue(!list.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			list.Add(attribute);
			this.cache = null;
		}

		// Token: 0x04004D5F RID: 19807
		private Dictionary<Type, List<PrefabAttribute>> attributes = new Dictionary<Type, List<PrefabAttribute>>();

		// Token: 0x04004D60 RID: 19808
		private Dictionary<Type, object> cache = new Dictionary<Type, object>();
	}

	// Token: 0x02000ECC RID: 3788
	public class Library
	{
		// Token: 0x06005362 RID: 21346 RVA: 0x001B25A0 File Offset: 0x001B07A0
		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x001B25C4 File Offset: 0x001B07C4
		public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			PrefabAttribute.AttributeCollection attributeCollection;
			if (this.prefabs.TryGetValue(prefabID, out attributeCollection))
			{
				return attributeCollection;
			}
			attributeCollection = new PrefabAttribute.AttributeCollection();
			this.prefabs.Add(prefabID, attributeCollection);
			if (warmup && (!this.clientside || this.serverside))
			{
				if (!this.clientside && this.serverside)
				{
					GameManager.server.FindPrefab(prefabID);
				}
				else if (this.clientside)
				{
					bool flag = this.serverside;
				}
			}
			return attributeCollection;
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x001B2638 File Offset: 0x001B0838
		public T Find<T>(uint prefabID) where T : PrefabAttribute
		{
			T[] array = this.Find(prefabID, true).Find<T>();
			if (array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		// Token: 0x06005365 RID: 21349 RVA: 0x001B2668 File Offset: 0x001B0868
		public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
		{
			return this.Find(prefabID, true).Find<T>();
		}

		// Token: 0x06005366 RID: 21350 RVA: 0x001B2677 File Offset: 0x001B0877
		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			this.Find(prefabID, false).Add(attribute);
		}

		// Token: 0x06005367 RID: 21351 RVA: 0x001B2687 File Offset: 0x001B0887
		public void Invalidate(uint prefabID)
		{
			this.prefabs.Remove(prefabID);
		}

		// Token: 0x04004D61 RID: 19809
		public bool clientside;

		// Token: 0x04004D62 RID: 19810
		public bool serverside;

		// Token: 0x04004D63 RID: 19811
		private Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs = new Dictionary<uint, PrefabAttribute.AttributeCollection>();
	}
}
