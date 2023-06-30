using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200095C RID: 2396
[CreateAssetMenu(menuName = "Rust/Vehicles/WorldSpline Shared Data", fileName = "WorldSpline Prefab Shared Data")]
public class WorldSplineSharedData : ScriptableObject
{
	// Token: 0x06003988 RID: 14728 RVA: 0x001550D5 File Offset: 0x001532D5
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		WorldSplineSharedData.instance = Resources.Load<WorldSplineSharedData>("WorldSpline Prefab Shared Data");
	}

	// Token: 0x06003989 RID: 14729 RVA: 0x001550E8 File Offset: 0x001532E8
	public static bool TryGetDataFor(WorldSpline worldSpline, out WorldSplineData data)
	{
		if (WorldSplineSharedData.instance == null)
		{
			Debug.LogError("No instance of WorldSplineSharedData found.");
			data = null;
			return false;
		}
		if (worldSpline.dataIndex < 0 || worldSpline.dataIndex >= WorldSplineSharedData.instance.dataList.Count)
		{
			data = null;
			return false;
		}
		data = WorldSplineSharedData.instance.dataList[worldSpline.dataIndex];
		return true;
	}

	// Token: 0x040033FB RID: 13307
	[SerializeField]
	private List<WorldSplineData> dataList;

	// Token: 0x040033FC RID: 13308
	public static WorldSplineSharedData instance;

	// Token: 0x040033FD RID: 13309
	private static string[] worldSplineFolders = new string[] { "Assets/Content/Structures", "Assets/bundled/Prefabs/autospawn" };
}
