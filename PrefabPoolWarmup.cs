using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000967 RID: 2407
public class PrefabPoolWarmup
{
	// Token: 0x060039AF RID: 14767 RVA: 0x00155D88 File Offset: 0x00153F88
	public static void Run(string filter = null, int countOverride = 0)
	{
		if (Rust.Application.isLoadingPrefabs)
		{
			return;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] assetList = PrefabPoolWarmup.GetAssetList();
		if (string.IsNullOrEmpty(filter))
		{
			for (int i = 0; i < assetList.Length; i++)
			{
				PrefabPoolWarmup.PrefabWarmup(assetList[i], countOverride);
			}
		}
		else
		{
			foreach (string text in assetList)
			{
				if (text.Contains(filter, CompareOptions.IgnoreCase))
				{
					PrefabPoolWarmup.PrefabWarmup(text, countOverride);
				}
			}
		}
		Rust.Application.isLoadingPrefabs = false;
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x00155DF3 File Offset: 0x00153FF3
	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (UnityEngine.Application.isEditor)
		{
			yield break;
		}
		if (Rust.Application.isLoadingPrefabs)
		{
			yield break;
		}
		if (!Pool.prewarm)
		{
			yield break;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] prewarmAssets = PrefabPoolWarmup.GetAssetList();
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < prewarmAssets.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == prewarmAssets.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format((format != null) ? format : "{0}/{1}", i + 1, prewarmAssets.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			PrefabPoolWarmup.PrefabWarmup(prewarmAssets[i], 0);
			num = i;
		}
		Rust.Application.isLoadingPrefabs = false;
		yield break;
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x00155E10 File Offset: 0x00154010
	public static string[] GetAssetList()
	{
		return (from x in GameManifest.Current.prefabProperties
			where x.pool
			select x.name).ToArray<string>();
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x00155E74 File Offset: 0x00154074
	private static void PrefabWarmup(string path, int countOverride = 0)
	{
		if (!string.IsNullOrEmpty(path))
		{
			GameObject gameObject = GameManager.server.FindPrefab(path);
			if (gameObject != null && gameObject.SupportsPooling())
			{
				int num = gameObject.GetComponent<Poolable>().ServerCount;
				List<GameObject> list = new List<GameObject>();
				if (num > 0 && countOverride > 0)
				{
					num = countOverride;
				}
				for (int i = 0; i < num; i++)
				{
					list.Add(GameManager.server.CreatePrefab(path, true));
				}
				for (int j = 0; j < num; j++)
				{
					GameManager.server.Retire(list[j]);
				}
			}
		}
	}
}
