using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000966 RID: 2406
public class PrefabPoolCollection
{
	// Token: 0x060039AB RID: 14763 RVA: 0x00155C48 File Offset: 0x00153E48
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		PrefabPool prefabPool;
		if (!this.storage.TryGetValue(component.prefabID, out prefabPool))
		{
			prefabPool = new PrefabPool();
			this.storage.Add(component.prefabID, prefabPool);
		}
		prefabPool.Push(component);
	}

	// Token: 0x060039AC RID: 14764 RVA: 0x00155C90 File Offset: 0x00153E90
	public GameObject Pop(uint id, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		PrefabPool prefabPool;
		if (this.storage.TryGetValue(id, out prefabPool))
		{
			return prefabPool.Pop(pos, rot);
		}
		return null;
	}

	// Token: 0x060039AD RID: 14765 RVA: 0x00155CB8 File Offset: 0x00153EB8
	public void Clear(string filter = null)
	{
		if (string.IsNullOrEmpty(filter))
		{
			using (Dictionary<uint, PrefabPool>.Enumerator enumerator = this.storage.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PrefabPool> keyValuePair = enumerator.Current;
					keyValuePair.Value.Clear();
				}
				return;
			}
		}
		foreach (KeyValuePair<uint, PrefabPool> keyValuePair2 in this.storage)
		{
			if (StringPool.Get(keyValuePair2.Key).Contains(filter, CompareOptions.IgnoreCase))
			{
				keyValuePair2.Value.Clear();
			}
		}
	}

	// Token: 0x04003425 RID: 13349
	public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();
}
