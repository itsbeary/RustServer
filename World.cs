using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000662 RID: 1634
public static class World
{
	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x06002F76 RID: 12150 RVA: 0x0011F064 File Offset: 0x0011D264
	// (set) Token: 0x06002F77 RID: 12151 RVA: 0x0011F06B File Offset: 0x0011D26B
	public static uint Seed { get; set; }

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x06002F78 RID: 12152 RVA: 0x0011F073 File Offset: 0x0011D273
	// (set) Token: 0x06002F79 RID: 12153 RVA: 0x0011F07A File Offset: 0x0011D27A
	public static uint Salt { get; set; }

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002F7A RID: 12154 RVA: 0x0011F082 File Offset: 0x0011D282
	// (set) Token: 0x06002F7B RID: 12155 RVA: 0x0011F089 File Offset: 0x0011D289
	public static uint Size { get; set; }

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06002F7C RID: 12156 RVA: 0x0011F091 File Offset: 0x0011D291
	// (set) Token: 0x06002F7D RID: 12157 RVA: 0x0011F098 File Offset: 0x0011D298
	public static string Checksum { get; set; }

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06002F7E RID: 12158 RVA: 0x0011F0A0 File Offset: 0x0011D2A0
	// (set) Token: 0x06002F7F RID: 12159 RVA: 0x0011F0A7 File Offset: 0x0011D2A7
	public static string Url { get; set; }

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002F80 RID: 12160 RVA: 0x0011F0AF File Offset: 0x0011D2AF
	// (set) Token: 0x06002F81 RID: 12161 RVA: 0x0011F0B6 File Offset: 0x0011D2B6
	public static bool Procedural { get; set; }

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06002F82 RID: 12162 RVA: 0x0011F0BE File Offset: 0x0011D2BE
	// (set) Token: 0x06002F83 RID: 12163 RVA: 0x0011F0C5 File Offset: 0x0011D2C5
	public static bool Cached { get; set; }

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06002F84 RID: 12164 RVA: 0x0011F0CD File Offset: 0x0011D2CD
	// (set) Token: 0x06002F85 RID: 12165 RVA: 0x0011F0D4 File Offset: 0x0011D2D4
	public static bool Networked { get; set; }

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06002F86 RID: 12166 RVA: 0x0011F0DC File Offset: 0x0011D2DC
	// (set) Token: 0x06002F87 RID: 12167 RVA: 0x0011F0E3 File Offset: 0x0011D2E3
	public static bool Receiving { get; set; }

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002F88 RID: 12168 RVA: 0x0011F0EB File Offset: 0x0011D2EB
	// (set) Token: 0x06002F89 RID: 12169 RVA: 0x0011F0F2 File Offset: 0x0011D2F2
	public static bool Transfer { get; set; }

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06002F8A RID: 12170 RVA: 0x0011F0FA File Offset: 0x0011D2FA
	// (set) Token: 0x06002F8B RID: 12171 RVA: 0x0011F101 File Offset: 0x0011D301
	public static bool LoadedFromSave { get; set; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06002F8C RID: 12172 RVA: 0x0011F109 File Offset: 0x0011D309
	// (set) Token: 0x06002F8D RID: 12173 RVA: 0x0011F110 File Offset: 0x0011D310
	public static int SpawnIndex { get; set; }

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x06002F8E RID: 12174 RVA: 0x0011F118 File Offset: 0x0011D318
	// (set) Token: 0x06002F8F RID: 12175 RVA: 0x0011F11F File Offset: 0x0011D31F
	public static WorldSerialization Serialization { get; set; }

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x06002F90 RID: 12176 RVA: 0x0011F127 File Offset: 0x0011D327
	public static string Name
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(global::World.Url));
			}
			return Application.loadedLevelName;
		}
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x0011F145 File Offset: 0x0011D345
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetServerBrowserMapName()
	{
		if (!global::World.CanLoadFromUrl())
		{
			return global::World.Name;
		}
		if (global::World.Name.StartsWith("proceduralmap."))
		{
			return "Procedural Map";
		}
		return "Custom Map";
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x0011F170 File Offset: 0x0011D370
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(global::World.Url);
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x0011F17F File Offset: 0x0011D37F
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromDisk()
	{
		return File.Exists(global::World.MapFolderName + "/" + global::World.MapFileName);
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x0011F19C File Offset: 0x0011D39C
	public static void CleanupOldFiles()
	{
		Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + 239 + "\\.map");
		foreach (string text in from path in Directory.GetFiles(global::World.MapFolderName, "*.map")
			where regex1.IsMatch(path) && !regex2.IsMatch(path)
			select path)
		{
			try
			{
				File.Delete(text);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
		}
	}

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002F95 RID: 12181 RVA: 0x0011F258 File Offset: 0x0011D458
	public static string MapFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return global::World.Name + ".map";
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				239,
				".map"
			});
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002F96 RID: 12182 RVA: 0x0011F2EA File Offset: 0x0011D4EA
	public static string MapFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002F97 RID: 12183 RVA: 0x0011F2F4 File Offset: 0x0011D4F4
	public static string SaveFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return string.Concat(new object[]
				{
					global::World.Name,
					".",
					239,
					".sav"
				});
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				239,
				".sav"
			});
		}
	}

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002F98 RID: 12184 RVA: 0x0011F2EA File Offset: 0x0011D4EA
	public static string SaveFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x0011F3A7 File Offset: 0x0011D5A7
	public static void InitSeed(int seed)
	{
		global::World.InitSeed((uint)seed);
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x0011F3AF File Offset: 0x0011D5AF
	public static void InitSeed(uint seed)
	{
		if (seed == 0U)
		{
			seed = global::World.SeedIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (seed == 0U)
		{
			seed = 123456U;
		}
		global::World.Seed = seed;
		Server.seed = (int)seed;
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x0011F3DC File Offset: 0x0011D5DC
	private static string SeedIdentifier()
	{
		return string.Concat(new object[]
		{
			SystemInfo.deviceUniqueIdentifier,
			"_",
			239,
			"_",
			Server.identity
		});
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x0011F416 File Offset: 0x0011D616
	public static void InitSalt(int salt)
	{
		global::World.InitSalt((uint)salt);
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x0011F41E File Offset: 0x0011D61E
	public static void InitSalt(uint salt)
	{
		if (salt == 0U)
		{
			salt = global::World.SaltIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (salt == 0U)
		{
			salt = 654321U;
		}
		global::World.Salt = salt;
		Server.salt = (int)salt;
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x0011F44B File Offset: 0x0011D64B
	private static string SaltIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier + "_salt";
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x0011F45C File Offset: 0x0011D65C
	public static void InitSize(int size)
	{
		global::World.InitSize((uint)size);
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x0011F464 File Offset: 0x0011D664
	public static void InitSize(uint size)
	{
		if (size == 0U)
		{
			size = 4500U;
		}
		if (size < 1000U)
		{
			size = 1000U;
		}
		if (size > 6000U)
		{
			size = 6000U;
		}
		global::World.Size = size;
		Server.worldsize = (int)size;
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x0011F49C File Offset: 0x0011D69C
	public static byte[] GetMap(string name)
	{
		MapData map = global::World.Serialization.GetMap(name);
		if (map == null)
		{
			return null;
		}
		return map.data;
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x0011F4C0 File Offset: 0x0011D6C0
	public static int GetCachedHeightMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("height").Length / 2)));
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x0011F4DB File Offset: 0x0011D6DB
	public static int GetCachedSplatMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("splat").Length / 8)));
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x0011F4F6 File Offset: 0x0011D6F6
	public static void AddMap(string name, byte[] data)
	{
		global::World.Serialization.AddMap(name, data);
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x0011F504 File Offset: 0x0011D704
	public static void AddPrefab(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		global::World.Serialization.AddPrefab(category, prefab.ID, position, rotation, scale);
		if (!global::World.Cached)
		{
			rotation = Quaternion.Euler(rotation.eulerAngles);
			global::World.Spawn(category, prefab, position, rotation, scale);
		}
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x0011F53C File Offset: 0x0011D73C
	public static PathData PathListToPathData(PathList src)
	{
		return new PathData
		{
			name = src.Name,
			spline = src.Spline,
			start = src.Start,
			end = src.End,
			width = src.Width,
			innerPadding = src.InnerPadding,
			outerPadding = src.OuterPadding,
			innerFade = src.InnerFade,
			outerFade = src.OuterFade,
			randomScale = src.RandomScale,
			meshOffset = src.MeshOffset,
			terrainOffset = src.TerrainOffset,
			splat = src.Splat,
			topology = src.Topology,
			hierarchy = src.Hierarchy,
			nodes = global::World.VectorArrayToList(src.Path.Points)
		};
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x0011F618 File Offset: 0x0011D818
	public static PathList PathDataToPathList(PathData src)
	{
		PathList pathList = new PathList(src.name, global::World.VectorListToArray(src.nodes));
		pathList.Spline = src.spline;
		pathList.Start = src.start;
		pathList.End = src.end;
		pathList.Width = src.width;
		pathList.InnerPadding = src.innerPadding;
		pathList.OuterPadding = src.outerPadding;
		pathList.InnerFade = src.innerFade;
		pathList.OuterFade = src.outerFade;
		pathList.RandomScale = src.randomScale;
		pathList.MeshOffset = src.meshOffset;
		pathList.TerrainOffset = src.terrainOffset;
		pathList.Splat = src.splat;
		pathList.Topology = src.topology;
		pathList.Hierarchy = src.hierarchy;
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x0011F6F0 File Offset: 0x0011D8F0
	public static Vector3[] VectorListToArray(List<VectorData> src)
	{
		Vector3[] array = new Vector3[src.Count];
		for (int i = 0; i < array.Length; i++)
		{
			VectorData vectorData = src[i];
			array[i] = new Vector3
			{
				x = vectorData.x,
				y = vectorData.y,
				z = vectorData.z
			};
		}
		return array;
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x0011F758 File Offset: 0x0011D958
	public static List<VectorData> VectorArrayToList(Vector3[] src)
	{
		List<VectorData> list = new List<VectorData>(src.Length);
		foreach (Vector3 vector in src)
		{
			list.Add(new VectorData
			{
				x = vector.x,
				y = vector.y,
				z = vector.z
			});
		}
		return list;
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x0011F7BB File Offset: 0x0011D9BB
	public static IEnumerable<PathList> GetPaths(string name)
	{
		return from p in global::World.Serialization.GetPaths(name)
			select global::World.PathDataToPathList(p);
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x0011F7EC File Offset: 0x0011D9EC
	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList pathList in paths)
		{
			global::World.AddPath(pathList);
		}
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x0011F834 File Offset: 0x0011DA34
	public static void AddPath(PathList path)
	{
		global::World.Serialization.AddPath(global::World.PathListToPathData(path));
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x0011F846 File Offset: 0x0011DA46
	public static IEnumerator SpawnAsync(float deltaTime, Action<string> statusFunction = null)
	{
		int totalCount = 0;
		Dictionary<string, List<PrefabData>> assetGroups = new Dictionary<string, List<PrefabData>>(StringComparer.InvariantCultureIgnoreCase);
		foreach (PrefabData prefabData in global::World.Serialization.world.prefabs)
		{
			string text = StringPool.Get(prefabData.id);
			if (string.IsNullOrWhiteSpace(text))
			{
				UnityEngine.Debug.LogWarning(string.Format("Could not find path for prefab ID {0}, skipping spawn", prefabData.id));
			}
			else
			{
				List<PrefabData> list;
				if (!assetGroups.TryGetValue(text, out list))
				{
					list = new List<PrefabData>();
					assetGroups.Add(text, list);
				}
				list.Add(prefabData);
				int num = totalCount;
				totalCount = num + 1;
			}
		}
		int spawnedCount = 0;
		int resultIndex = 0;
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult load = FileSystem.PreloadAssets(assetGroups.Keys, Global.preloadConcurrency, 10);
		while (load != null && (load.MoveNext() || assetGroups.Count > 0))
		{
			while (resultIndex < load.Results.Count && sw.Elapsed.TotalSeconds < (double)deltaTime)
			{
				string item = load.Results[resultIndex].Item1;
				List<PrefabData> list2;
				if (!assetGroups.TryGetValue(item, out list2))
				{
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else if (list2.Count == 0)
				{
					assetGroups.Remove(item);
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else
				{
					int num2 = list2.Count - 1;
					PrefabData prefabData2 = list2[num2];
					list2.RemoveAt(num2);
					global::World.Spawn(prefabData2);
					int num = spawnedCount;
					spawnedCount = num + 1;
				}
			}
			global::World.Status(statusFunction, "Spawning World ({0}/{1})", spawnedCount, totalCount);
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		yield break;
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x0011F85C File Offset: 0x0011DA5C
	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == global::World.Serialization.world.prefabs.Count - 1)
			{
				global::World.Status(statusFunction, "Spawning World ({0}/{1})", i + 1, global::World.Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
			num = i;
		}
		yield break;
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x0011F874 File Offset: 0x0011DA74
	public static void Spawn()
	{
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
		}
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x0011F8BC File Offset: 0x0011DABC
	public static void Spawn(string category, string folder = null)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string text = StringPool.Get(prefabData.id);
			if (!string.IsNullOrEmpty(folder) && !text.StartsWith(folder))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x0011F940 File Offset: 0x0011DB40
	public static void Spawn(string category, string[] folders)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string text = StringPool.Get(prefabData.id);
			if (folders != null && !text.StartsWithAny(folders))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x0011F9BE File Offset: 0x0011DBBE
	private static void Spawn(PrefabData prefab)
	{
		global::World.Spawn(prefab.category, Prefab.Load(prefab.id, null, null), prefab.position, prefab.rotation, prefab.scale);
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x0011F9FC File Offset: 0x0011DBFC
	private static void Spawn(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (prefab == null || !prefab.Object)
		{
			return;
		}
		if (!global::World.Cached)
		{
			prefab.ApplyTerrainPlacements(position, rotation, scale);
			prefab.ApplyTerrainModifiers(position, rotation, scale);
		}
		GameObject gameObject = prefab.Spawn(position, rotation, scale, true);
		if (gameObject)
		{
			gameObject.SetHierarchyGroup(category, true, false);
		}
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x0011FA52 File Offset: 0x0011DC52
	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1));
		}
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x0011FA64 File Offset: 0x0011DC64
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2));
		}
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x0011FA77 File Offset: 0x0011DC77
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2, obj3));
		}
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x0011FA8C File Offset: 0x0011DC8C
	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, objs));
		}
	}
}
