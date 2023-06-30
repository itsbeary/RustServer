using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020006F5 RID: 1781
public class WorldSetup : SingletonComponent<WorldSetup>
{
	// Token: 0x06003268 RID: 12904 RVA: 0x00137058 File Offset: 0x00135258
	private void OnValidate()
	{
		if (this.terrain == null)
		{
			UnityEngine.Terrain terrain = UnityEngine.Object.FindObjectOfType<UnityEngine.Terrain>();
			if (terrain != null)
			{
				this.terrain = terrain.gameObject;
			}
		}
	}

	// Token: 0x06003269 RID: 12905 RVA: 0x00137090 File Offset: 0x00135290
	protected override void Awake()
	{
		base.Awake();
		foreach (Prefab prefab in Prefab.Load("assets/bundled/prefabs/world", null, null, true))
		{
			if (prefab.Object.GetComponent<BaseEntity>() != null)
			{
				prefab.SpawnEntity(Vector3.zero, Quaternion.identity, true).Spawn();
			}
			else
			{
				prefab.Spawn(Vector3.zero, Quaternion.identity, true);
			}
		}
		SingletonComponent[] array2 = UnityEngine.Object.FindObjectsOfType<SingletonComponent>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SingletonSetup();
		}
		if (this.terrain)
		{
			if (this.terrain.GetComponent<TerrainGenerator>())
			{
				global::World.Procedural = true;
			}
			else
			{
				global::World.Procedural = false;
				this.terrainMeta = this.terrain.GetComponent<TerrainMeta>();
				this.terrainMeta.Init(null, null);
				this.terrainMeta.SetupComponents();
				this.terrainMeta.BindShaderProperties();
				this.terrainMeta.PostSetupComponents();
				global::World.InitSize(Mathf.RoundToInt(TerrainMeta.Size.x));
				this.CreateObject(this.decorPrefab);
				this.CreateObject(this.grassPrefab);
				this.CreateObject(this.spawnPrefab);
			}
		}
		global::World.Serialization = new WorldSerialization();
		global::World.Cached = false;
		global::World.CleanupOldFiles();
		if (this.AutomaticallySetup)
		{
			base.StartCoroutine(this.InitCoroutine());
		}
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x001371F0 File Offset: 0x001353F0
	protected void CreateObject(GameObject prefab)
	{
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x0600326B RID: 12907 RVA: 0x0013721E File Offset: 0x0013541E
	public IEnumerator InitCoroutine()
	{
		if (global::World.CanLoadFromUrl())
		{
			Debug.Log("Loading custom map from " + global::World.Url);
		}
		else
		{
			Debug.Log(string.Concat(new object[]
			{
				"Generating procedural map of size ",
				global::World.Size,
				" with seed ",
				global::World.Seed
			}));
		}
		ProceduralComponent[] components = base.GetComponentsInChildren<ProceduralComponent>(true);
		Timing downloadTimer = Timing.Start("Downloading World");
		if (global::World.Procedural && !global::World.CanLoadFromDisk() && global::World.CanLoadFromUrl())
		{
			LoadingScreen.Update("DOWNLOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			UnityWebRequest request = UnityWebRequest.Get(global::World.Url);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.Send();
			while (!request.isDone)
			{
				LoadingScreen.Update("DOWNLOADING WORLD " + (request.downloadProgress * 100f).ToString("0.0") + "%");
				yield return CoroutineEx.waitForEndOfFrame;
			}
			if (!request.isHttpError && !request.isNetworkError)
			{
				File.WriteAllBytes(global::World.MapFolderName + "/" + global::World.MapFileName, request.downloadHandler.data);
			}
			else
			{
				this.CancelSetup(string.Concat(new string[]
				{
					"Couldn't Download Level: ",
					global::World.Name,
					" (",
					request.error,
					")"
				}));
			}
			request = null;
		}
		downloadTimer.End();
		Timing loadTimer = Timing.Start("Loading World");
		if (global::World.Procedural && global::World.CanLoadFromDisk())
		{
			LoadingScreen.Update("LOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.Load(global::World.MapFolderName + "/" + global::World.MapFileName);
			global::World.Cached = true;
		}
		loadTimer.End();
		if (global::World.Cached && 9U != global::World.Serialization.Version)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"World cache version mismatch: ",
				9U,
				" != ",
				global::World.Serialization.Version
			}));
			global::World.Serialization.Clear();
			global::World.Cached = false;
			if (global::World.CanLoadFromUrl())
			{
				this.CancelSetup("World File Outdated: " + global::World.Name);
			}
		}
		if (global::World.Cached && string.IsNullOrEmpty(global::World.Checksum))
		{
			global::World.Checksum = global::World.Serialization.Checksum;
		}
		if (global::World.Cached)
		{
			global::World.InitSize(global::World.Serialization.world.size);
		}
		if (this.terrain)
		{
			TerrainGenerator component2 = this.terrain.GetComponent<TerrainGenerator>();
			if (component2)
			{
				if (global::World.Cached)
				{
					int cachedHeightMapResolution = global::World.GetCachedHeightMapResolution();
					int cachedSplatMapResolution = global::World.GetCachedSplatMapResolution();
					this.terrain = component2.CreateTerrain(cachedHeightMapResolution, cachedSplatMapResolution);
				}
				else
				{
					this.terrain = component2.CreateTerrain();
				}
				this.terrainMeta = this.terrain.GetComponent<TerrainMeta>();
				this.terrainMeta.Init(null, null);
				this.terrainMeta.SetupComponents();
				this.CreateObject(this.decorPrefab);
				this.CreateObject(this.grassPrefab);
				this.CreateObject(this.spawnPrefab);
			}
		}
		Timing spawnTimer = Timing.Start("Spawning World");
		if (global::World.Cached)
		{
			LoadingScreen.Update("SPAWNING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			TerrainMeta.HeightMap.FromByteArray(global::World.GetMap("terrain"));
			TerrainMeta.SplatMap.FromByteArray(global::World.GetMap("splat"));
			TerrainMeta.BiomeMap.FromByteArray(global::World.GetMap("biome"));
			TerrainMeta.TopologyMap.FromByteArray(global::World.GetMap("topology"));
			TerrainMeta.AlphaMap.FromByteArray(global::World.GetMap("alpha"));
			TerrainMeta.WaterMap.FromByteArray(global::World.GetMap("water"));
			IEnumerator enumerator;
			if (ConVar.Global.preloadConcurrency <= 1)
			{
				enumerator = global::World.Spawn(0.2f, delegate(string str)
				{
					LoadingScreen.Update(str);
				});
			}
			else
			{
				enumerator = global::World.SpawnAsync(0.2f, delegate(string str)
				{
					LoadingScreen.Update(str);
				});
			}
			IEnumerator worldSpawn = enumerator;
			while (worldSpawn.MoveNext())
			{
				object obj = worldSpawn.Current;
				yield return obj;
			}
			TerrainMeta.Path.Clear();
			TerrainMeta.Path.Roads.AddRange(global::World.GetPaths("Road"));
			TerrainMeta.Path.Rivers.AddRange(global::World.GetPaths("River"));
			TerrainMeta.Path.Powerlines.AddRange(global::World.GetPaths("Powerline"));
			TerrainMeta.Path.Rails.AddRange(global::World.GetPaths("Rail"));
			worldSpawn = null;
		}
		spawnTimer.End();
		Timing procgenTimer = Timing.Start("Processing World");
		if (components.Length != 0)
		{
			int num;
			for (int i = 0; i < components.Length; i = num + 1)
			{
				ProceduralComponent component = components[i];
				if (component && component.ShouldRun())
				{
					uint seed = (uint)((ulong)global::World.Seed + (ulong)((long)i));
					LoadingScreen.Update(component.Description.ToUpper());
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					Timing timing = Timing.Start(component.Description);
					if (component)
					{
						component.Process(seed);
					}
					timing.End();
					component = null;
				}
				num = i;
			}
		}
		procgenTimer.End();
		Timing saveTimer = Timing.Start("Saving World");
		if (ConVar.World.cache && global::World.Procedural && !global::World.Cached)
		{
			LoadingScreen.Update("SAVING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.world.size = global::World.Size;
			global::World.AddPaths(TerrainMeta.Path.Roads);
			global::World.AddPaths(TerrainMeta.Path.Rivers);
			global::World.AddPaths(TerrainMeta.Path.Powerlines);
			global::World.AddPaths(TerrainMeta.Path.Rails);
			global::World.Serialization.Save(global::World.MapFolderName + "/" + global::World.MapFileName);
		}
		saveTimer.End();
		Timing checksumTimer = Timing.Start("Calculating Checksum");
		if (string.IsNullOrEmpty(global::World.Serialization.Checksum))
		{
			LoadingScreen.Update("CALCULATING CHECKSUM");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.CalculateChecksum();
		}
		checksumTimer.End();
		if (string.IsNullOrEmpty(global::World.Checksum))
		{
			global::World.Checksum = global::World.Serialization.Checksum;
		}
		Timing oceanTimer = Timing.Start("Ocean Patrol Paths");
		LoadingScreen.Update("OCEAN PATROL PATHS");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (BaseBoat.generate_paths && TerrainMeta.Path != null)
		{
			TerrainMeta.Path.OceanPatrolFar = BaseBoat.GenerateOceanPatrolPath(200f, 8f);
		}
		else
		{
			Debug.Log("Skipping ocean patrol paths, baseboat.generate_paths == false");
		}
		oceanTimer.End();
		Timing finalizeTimer = Timing.Start("Finalizing World");
		LoadingScreen.Update("FINALIZING WORLD");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (this.terrainMeta)
		{
			this.terrainMeta.BindShaderProperties();
			this.terrainMeta.PostSetupComponents();
			TerrainMargin.Create();
		}
		finalizeTimer.End();
		Timing cleaningTimer = Timing.Start("Cleaning Up");
		LoadingScreen.Update("CLEANING UP");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		foreach (string text in FileSystem.Backend.UnloadBundles("monuments"))
		{
			GameManager.server.preProcessed.Invalidate(text);
			GameManifest.Invalidate(text);
			PrefabAttribute.server.Invalidate(StringPool.Get(text));
		}
		Resources.UnloadUnusedAssets();
		cleaningTimer.End();
		LoadingScreen.Update("DONE");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (this)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
		yield break;
	}

	// Token: 0x0600326C RID: 12908 RVA: 0x0013722D File Offset: 0x0013542D
	private void CancelSetup(string msg)
	{
		Debug.LogError(msg);
		Rust.Application.Quit();
	}

	// Token: 0x04002949 RID: 10569
	public bool AutomaticallySetup;

	// Token: 0x0400294A RID: 10570
	public GameObject terrain;

	// Token: 0x0400294B RID: 10571
	public GameObject decorPrefab;

	// Token: 0x0400294C RID: 10572
	public GameObject grassPrefab;

	// Token: 0x0400294D RID: 10573
	public GameObject spawnPrefab;

	// Token: 0x0400294E RID: 10574
	private TerrainMeta terrainMeta;

	// Token: 0x0400294F RID: 10575
	public uint EditorSeed;

	// Token: 0x04002950 RID: 10576
	public uint EditorSalt;

	// Token: 0x04002951 RID: 10577
	public uint EditorSize;

	// Token: 0x04002952 RID: 10578
	public string EditorUrl = string.Empty;

	// Token: 0x04002953 RID: 10579
	internal List<ProceduralObject> ProceduralObjects = new List<ProceduralObject>();

	// Token: 0x04002954 RID: 10580
	internal List<MonumentNode> MonumentNodes = new List<MonumentNode>();
}
