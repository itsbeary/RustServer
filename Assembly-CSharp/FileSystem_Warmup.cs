using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200035F RID: 863
public class FileSystem_Warmup : MonoBehaviour
{
	// Token: 0x06001F96 RID: 8086 RVA: 0x000D56E0 File Offset: 0x000D38E0
	public static void Run()
	{
		if (Global.skipAssetWarmup_crashes)
		{
			return;
		}
		if (!FileSystem_Warmup.run)
		{
			return;
		}
		string[] assetList = FileSystem_Warmup.GetAssetList(null);
		for (int i = 0; i < assetList.Length; i++)
		{
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		FileSystem_Warmup.run = false;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000D5728 File Offset: 0x000D3928
	public static IEnumerator Run(string[] assetList, Action<string> statusFunction = null, string format = null, int priority = 0)
	{
		if (Global.warmupConcurrency <= 1)
		{
			return FileSystem_Warmup.RunImpl(assetList, statusFunction, format);
		}
		return FileSystem_Warmup.RunAsyncImpl(assetList, statusFunction, format, priority);
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x000D5744 File Offset: 0x000D3944
	private static IEnumerator RunAsyncImpl(string[] assetList, Action<string> statusFunction, string format, int priority)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch statusSw = Stopwatch.StartNew();
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult preload = FileSystem.PreloadAssets(assetList, Global.warmupConcurrency, priority);
		int warmupIndex = 0;
		while (preload.MoveNext() || warmupIndex < preload.TotalCount)
		{
			float num = FileSystem_Warmup.CalculateFrameBudget();
			if (num > 0f)
			{
				while (warmupIndex < preload.Results.Count && sw.Elapsed.TotalSeconds < (double)num)
				{
					IReadOnlyList<ValueTuple<string, UnityEngine.Object>> results = preload.Results;
					int num2 = warmupIndex;
					warmupIndex = num2 + 1;
					FileSystem_Warmup.PrefabWarmup(results[num2].Item1);
				}
			}
			if (warmupIndex == 0 || warmupIndex == preload.TotalCount || statusSw.Elapsed.TotalSeconds > 1.0)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format(format ?? "{0}/{1}", warmupIndex, preload.TotalCount));
				}
				statusSw.Restart();
			}
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x000D5768 File Offset: 0x000D3968
	private static IEnumerator RunImpl(string[] assetList, Action<string> statusFunction = null, string format = null)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < assetList.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)FileSystem_Warmup.CalculateFrameBudget() || i == 0 || i == assetList.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format((format != null) ? format : "{0}/{1}", i + 1, assetList.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
			num = i;
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x00032D46 File Offset: 0x00030F46
	private static float CalculateFrameBudget()
	{
		return 2f;
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000D5788 File Offset: 0x000D3988
	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < FileSystem_Warmup.ExcludeFilter.Length; i++)
		{
			if (path.Contains(FileSystem_Warmup.ExcludeFilter[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000D57BC File Offset: 0x000D39BC
	public static string[] GetAssetList(bool? poolFilter = null)
	{
		if (poolFilter == null)
		{
			return (from x in GameManifest.Current.prefabProperties
				select x.name into x
				where !FileSystem_Warmup.ShouldIgnore(x)
				select x).ToArray<string>();
		}
		return (from x in GameManifest.Current.prefabProperties.Where(delegate(GameManifest.PrefabProperties x)
			{
				if (!FileSystem_Warmup.ShouldIgnore(x.name))
				{
					bool pool = x.pool;
					bool? poolFilter2 = poolFilter;
					return (pool == poolFilter2.GetValueOrDefault()) & (poolFilter2 != null);
				}
				return false;
			})
			select x.name).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray<string>();
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000D5889 File Offset: 0x000D3A89
	private static void PrefabWarmup(string path)
	{
		GameManager.server.FindPrefab(path);
	}

	// Token: 0x040018E1 RID: 6369
	public static bool ranInBackground = false;

	// Token: 0x040018E2 RID: 6370
	public static Coroutine warmupTask;

	// Token: 0x040018E3 RID: 6371
	private static bool run = true;

	// Token: 0x040018E4 RID: 6372
	public static string[] ExcludeFilter = new string[]
	{
		"/bundled/prefabs/autospawn/monument", "/bundled/prefabs/autospawn/mountain", "/bundled/prefabs/autospawn/canyon", "/bundled/prefabs/autospawn/decor", "/bundled/prefabs/navmesh", "/content/ui/", "/prefabs/ui/", "/prefabs/world/", "/prefabs/system/", "/standard assets/",
		"/third party/"
	};
}
