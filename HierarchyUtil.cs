using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000945 RID: 2373
public static class HierarchyUtil
{
	// Token: 0x060038B4 RID: 14516 RVA: 0x0015179C File Offset: 0x0014F99C
	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		GameObject gameObject;
		if (HierarchyUtil.rootDict.TryGetValue(strName, out gameObject))
		{
			if (gameObject != null)
			{
				return gameObject;
			}
			HierarchyUtil.rootDict.Remove(strName);
		}
		gameObject = new GameObject(strName);
		gameObject.SetActive(groupActive);
		HierarchyUtil.rootDict.Add(strName, gameObject);
		if (persistant)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}

	// Token: 0x040033AF RID: 13231
	public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
}
