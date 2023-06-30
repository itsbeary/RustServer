using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Network;
using Facepunch.Network.Raknet;
using Facepunch.Rust;
using Facepunch.Utility;
using Network;
using Rust;
using Rust.Ai;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020004EE RID: 1262
public class Bootstrap : SingletonComponent<Bootstrap>
{
	// Token: 0x17000371 RID: 881
	// (get) Token: 0x060028D7 RID: 10455 RVA: 0x000FC214 File Offset: 0x000FA414
	public static bool needsSetup
	{
		get
		{
			return !Bootstrap.bootstrapInitRun;
		}
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x060028D8 RID: 10456 RVA: 0x000FC21E File Offset: 0x000FA41E
	public static bool isPresent
	{
		get
		{
			return Bootstrap.bootstrapInitRun || UnityEngine.Object.FindObjectsOfType<GameSetup>().Count<GameSetup>() > 0;
		}
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000FC239 File Offset: 0x000FA439
	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		UnityEngine.Application.targetFrameRate = 256;
		UnityEngine.Time.fixedDeltaTime = 0.0625f;
		UnityEngine.Time.maximumDeltaTime = 0.125f;
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000FC278 File Offset: 0x000FA478
	public static void Init_Tier0()
	{
		Bootstrap.RunDefaults();
		GameSetup.RunOnce = true;
		Bootstrap.bootstrapInitRun = true;
		ConsoleSystem.Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Facepunch.Pool.ResizeBuffer<NetRead>(16384);
		Facepunch.Pool.ResizeBuffer<NetWrite>(16384);
		Facepunch.Pool.ResizeBuffer<Networkable>(65536);
		Facepunch.Pool.ResizeBuffer<EntityLink>(65536);
		Facepunch.Pool.FillBuffer<Networkable>();
		Facepunch.Pool.FillBuffer<EntityLink>();
		if (Facepunch.CommandLine.HasSwitch("-nonetworkthread"))
		{
			BaseNetwork.Multithreading = false;
		}
		SteamNetworking.SetDebugFunction();
		if (Facepunch.CommandLine.HasSwitch("-swnet"))
		{
			Bootstrap.NetworkInitSteamworks(false);
		}
		else if (Facepunch.CommandLine.HasSwitch("-sdrnet"))
		{
			Bootstrap.NetworkInitSteamworks(true);
		}
		else if (Facepunch.CommandLine.HasSwitch("-raknet"))
		{
			Bootstrap.NetworkInitRaknet();
		}
		else
		{
			Bootstrap.NetworkInitRaknet();
		}
		if (!UnityEngine.Application.isEditor)
		{
			string text = Facepunch.CommandLine.Full.Replace(Facepunch.CommandLine.GetSwitch("-rcon.password", Facepunch.CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
			Bootstrap.WriteToLog("Command Line: " + text);
		}
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000FC374 File Offset: 0x000FA574
	public static void Init_Systems()
	{
		Rust.Global.Init();
		if (GameInfo.IsOfficialServer && ConVar.Server.stats)
		{
			GA.Logging = false;
			GA.Build = BuildInfo.Current.Scm.ChangeId;
			GA.Initialize("218faecaf1ad400a4e15c53392ebeebc", "0c9803ce52c38671278899538b9c54c8d4e19849");
			Analytics.Server.Enabled = true;
		}
		Facepunch.Application.Initialize(new Integration());
		Facepunch.Performance.GetMemoryUsage = () => SystemInfoEx.systemMemoryUsed;
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000FC3F4 File Offset: 0x000FA5F4
	public static void Init_Config()
	{
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000FC41A File Offset: 0x000FA61A
	public static void NetworkInitRaknet()
	{
		Network.Net.sv = new Facepunch.Network.Raknet.Server();
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000FC426 File Offset: 0x000FA626
	public static void NetworkInitSteamworks(bool enableSteamDatagramRelay)
	{
		Network.Net.sv = new SteamNetworking.Server(enableSteamDatagramRelay);
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000FC433 File Offset: 0x000FA633
	private IEnumerator Start()
	{
		Bootstrap.WriteToLog("Bootstrap Startup");
		BenchmarkTimer.Enabled = Facepunch.Utility.CommandLine.Full.Contains("+autobench");
		BenchmarkTimer timer = BenchmarkTimer.New("bootstrap");
		if (!UnityEngine.Application.isEditor)
		{
			BuildInfo buildInfo = BuildInfo.Current;
			if ((buildInfo.Scm.Branch == null || !(buildInfo.Scm.Branch == "experimental/release")) && !(buildInfo.Scm.Branch == "release"))
			{
				ExceptionReporter.InitializeFromUrl("https://0654eb77d1e04d6babad83201b6b6b95:d2098f1d15834cae90501548bd5dbd0d@sentry.io/1836389");
			}
			else
			{
				ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
			}
			bool flag = Facepunch.Utility.CommandLine.Full.Contains("-official") || Facepunch.Utility.CommandLine.Full.Contains("-server.official") || Facepunch.Utility.CommandLine.Full.Contains("+official") || Facepunch.Utility.CommandLine.Full.Contains("+server.official");
			bool flag2 = Facepunch.Utility.CommandLine.Full.Contains("-stats") || Facepunch.Utility.CommandLine.Full.Contains("-server.stats") || Facepunch.Utility.CommandLine.Full.Contains("+stats") || Facepunch.Utility.CommandLine.Full.Contains("+server.stats");
			ExceptionReporter.Disabled = !flag || !flag2;
		}
		BenchmarkTimer benchmarkTimer;
		if (AssetBundleBackend.Enabled)
		{
			AssetBundleBackend newBackend = new AssetBundleBackend();
			using (BenchmarkTimer.New("bootstrap;bundles"))
			{
				yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Opening Bundles"));
				newBackend.Load("Bundles/Bundles");
				FileSystem.Backend = newBackend;
			}
			benchmarkTimer = null;
			if (FileSystem.Backend.isError)
			{
				this.ThrowError(FileSystem.Backend.loadingError);
				yield break;
			}
			using (BenchmarkTimer.New("bootstrap;bundlesindex"))
			{
				newBackend.BuildFileIndex();
			}
			newBackend = null;
		}
		if (FileSystem.Backend.isError)
		{
			this.ThrowError(FileSystem.Backend.loadingError);
			yield break;
		}
		if (!UnityEngine.Application.isEditor)
		{
			Bootstrap.WriteToLog(SystemInfoGeneralText.currentInfo);
		}
		UnityEngine.Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		using (BenchmarkTimer.New("bootstrap;gamemanifest"))
		{
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Game Manifest"));
			GameManifest.Load();
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("DONE!"));
		}
		benchmarkTimer = null;
		using (BenchmarkTimer.New("bootstrap;selfcheck"))
		{
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Running Self Check"));
			SelfCheck.Run();
		}
		benchmarkTimer = null;
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Tier0"));
		using (BenchmarkTimer.New("bootstrap;tier0"))
		{
			Bootstrap.Init_Tier0();
		}
		using (BenchmarkTimer.New("bootstrap;commandlinevalues"))
		{
			ConsoleSystem.UpdateValuesFromCommandLine();
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Systems"));
		using (BenchmarkTimer.New("bootstrap;init_systems"))
		{
			Bootstrap.Init_Systems();
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Config"));
		using (BenchmarkTimer.New("bootstrap;init_config"))
		{
			Bootstrap.Init_Config();
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Items"));
		using (BenchmarkTimer.New("bootstrap;itemmanager"))
		{
			ItemManager.Initialize();
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(this.DedicatedServerStartup());
		BenchmarkTimer benchmarkTimer3 = timer;
		if (benchmarkTimer3 != null)
		{
			benchmarkTimer3.Dispose();
		}
		GameManager.Destroy(base.gameObject, 0f);
		yield break;
		yield break;
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000FC442 File Offset: 0x000FA642
	private IEnumerator DedicatedServerStartup()
	{
		Rust.Application.isLoading = true;
		UnityEngine.Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;
		Bootstrap.WriteToLog("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		Bootstrap.WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		UnityEngine.Physics.solverIterationCount = 3;
		int @int = PlayerPrefs.GetInt("UnityGraphicsQuality");
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("UnityGraphicsQuality", @int);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab", true));
		this.StartupShared();
		global::World.InitSize(ConVar.Server.worldsize);
		global::World.InitSeed(ConVar.Server.seed);
		global::World.InitSalt(ConVar.Server.salt);
		global::World.Url = ConVar.Server.levelurl;
		global::World.Transfer = ConVar.Server.leveltransfer;
		LevelManager.LoadLevel(ConVar.Server.level, true);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		string[] assetList = FileSystem_Warmup.GetAssetList(null);
		yield return base.StartCoroutine(FileSystem_Warmup.Run(assetList, new Action<string>(Bootstrap.WriteToLog), "Asset Warmup ({0}/{1})", 0));
		yield return base.StartCoroutine(Bootstrap.StartServer(!Facepunch.CommandLine.HasSwitch("-skipload"), "", false));
		if (!UnityEngine.Object.FindObjectOfType<global::Performance>())
		{
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
		}
		Rust.GC.Collect();
		Rust.Application.isLoading = false;
		yield break;
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000FC451 File Offset: 0x000FA651
	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float timeScale = UnityEngine.Time.timeScale;
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = 0f;
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
		if (SingletonComponent<WorldSetup>.Instance)
		{
			yield return SingletonComponent<WorldSetup>.Instance.StartCoroutine(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		if (SingletonComponent<DynamicNavMesh>.Instance && SingletonComponent<DynamicNavMesh>.Instance.enabled && !AiManager.nav_disable)
		{
			yield return SingletonComponent<DynamicNavMesh>.Instance.StartCoroutine(SingletonComponent<DynamicNavMesh>.Instance.UpdateNavMeshAndWait());
		}
		if (SingletonComponent<AiManager>.Instance && SingletonComponent<AiManager>.Instance.enabled)
		{
			SingletonComponent<AiManager>.Instance.Initialize();
			if (!AiManager.nav_disable && AI.npc_enable && TerrainMeta.Path != null)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.HasNavmesh)
					{
						yield return monumentInfo.StartCoroutine(monumentInfo.GetMonumentNavMesh().UpdateNavMeshAndWait());
					}
				}
				List<MonumentInfo>.Enumerator enumerator = default(List<MonumentInfo>.Enumerator);
				if (TerrainMeta.Path && TerrainMeta.Path.DungeonGridRoot)
				{
					DungeonNavmesh dungeonNavmesh = TerrainMeta.Path.DungeonGridRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
					dungeonNavmesh.LayerMask = 65537;
					yield return dungeonNavmesh.StartCoroutine(dungeonNavmesh.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError("Failed to find DungeonGridRoot, NOT generating Dungeon navmesh");
				}
				if (TerrainMeta.Path && TerrainMeta.Path.DungeonBaseRoot)
				{
					DungeonNavmesh dungeonNavmesh2 = TerrainMeta.Path.DungeonBaseRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh2.NavmeshResolutionModifier = 0.3f;
					dungeonNavmesh2.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
					dungeonNavmesh2.LayerMask = 65537;
					yield return dungeonNavmesh2.StartCoroutine(dungeonNavmesh2.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError("Failed to find DungeonBaseRoot , NOT generating Dungeon navmesh");
				}
				GenerateDungeonBase.SetupAI();
			}
		}
		GameObject gameObject = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		ServerMgr serverMgr = gameObject.GetComponent<ServerMgr>();
		serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves, false);
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityLinks();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntitySupports();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityConditionals();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.GetSaveCache();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		BaseGameMode.CreateGameMode("");
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		MissionManifest.Get();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		serverMgr.OpenConnection();
		CompanionServer.Server.Initialize();
		using (BenchmarkTimer.New("Boombox.LoadStations"))
		{
			BoomBox.LoadStations();
		}
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = timeScale;
		}
		Bootstrap.WriteToLog("Server startup complete");
		yield break;
		yield break;
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000FC46E File Offset: 0x000FA66E
	private void StartupShared()
	{
		ItemManager.Initialize();
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000FC475 File Offset: 0x000FA675
	public void ThrowError(string error)
	{
		Debug.Log("ThrowError: " + error);
		this.errorPanel.SetActive(true);
		this.errorText.text = error;
		Bootstrap.isErrored = true;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000FC4A5 File Offset: 0x000FA6A5
	public void ExitGame()
	{
		Debug.Log("Exiting due to Exit Game button on bootstrap error panel");
		Rust.Application.Quit();
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000FC4B6 File Offset: 0x000FA6B6
	public static IEnumerator LoadingUpdate(string str)
	{
		if (!SingletonComponent<Bootstrap>.Instance)
		{
			yield break;
		}
		SingletonComponent<Bootstrap>.Instance.messageString = str;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield break;
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000FC4C5 File Offset: 0x000FA6C5
	public static void WriteToLog(string str)
	{
		if (Bootstrap.lastWrittenValue == str)
		{
			return;
		}
		DebugEx.Log(str, StackTraceLogType.None);
		Bootstrap.lastWrittenValue = str;
	}

	// Token: 0x04002117 RID: 8471
	internal static bool bootstrapInitRun;

	// Token: 0x04002118 RID: 8472
	public static bool isErrored;

	// Token: 0x04002119 RID: 8473
	public string messageString = "Loading...";

	// Token: 0x0400211A RID: 8474
	public CanvasGroup BootstrapUiCanvas;

	// Token: 0x0400211B RID: 8475
	public GameObject errorPanel;

	// Token: 0x0400211C RID: 8476
	public TextMeshProUGUI errorText;

	// Token: 0x0400211D RID: 8477
	public TextMeshProUGUI statusText;

	// Token: 0x0400211E RID: 8478
	private static string lastWrittenValue;
}
