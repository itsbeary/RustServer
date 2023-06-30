using System;
using System.Collections;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200051D RID: 1309
public class GameSetup : MonoBehaviour
{
	// Token: 0x060029E1 RID: 10721 RVA: 0x00100BFC File Offset: 0x000FEDFC
	protected void Awake()
	{
		if (GameSetup.RunOnce)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		GameSetup.RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		if (this.initializationCommands.Length > 0)
		{
			foreach (string text in this.initializationCommands.Split(new char[] { ';' }))
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, text.Trim(), Array.Empty<object>());
			}
		}
		base.StartCoroutine(this.DoGameSetup());
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x00100CA1 File Offset: 0x000FEEA1
	private IEnumerator DoGameSetup()
	{
		Rust.Application.isLoading = true;
		TerrainMeta.InitNoTerrain(false);
		ItemManager.Initialize();
		LevelManager.CurrentLevelName = SceneManager.GetActiveScene().name;
		if (this.loadLevel && !string.IsNullOrEmpty(this.loadLevelScene))
		{
			Network.Net.sv.Reset();
			ConVar.Server.level = this.loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			UnityEngine.Application.LoadLevelAdditive(this.loadLevelScene);
			LoadingScreen.Update(this.loadLevelScene.ToUpper() + " LOADED");
		}
		if (this.startServer)
		{
			yield return base.StartCoroutine(this.StartServer());
		}
		yield return null;
		Rust.Application.isLoading = false;
		yield break;
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x00100CB0 File Offset: 0x000FEEB0
	private IEnumerator StartServer()
	{
		ConVar.GC.collect();
		ConVar.GC.unload();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return base.StartCoroutine(Bootstrap.StartServer(this.loadSave, this.loadSaveFile, true));
		yield break;
	}

	// Token: 0x040021E9 RID: 8681
	public static bool RunOnce;

	// Token: 0x040021EA RID: 8682
	public bool startServer = true;

	// Token: 0x040021EB RID: 8683
	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	// Token: 0x040021EC RID: 8684
	public bool loadMenu = true;

	// Token: 0x040021ED RID: 8685
	public bool loadLevel;

	// Token: 0x040021EE RID: 8686
	public string loadLevelScene = "";

	// Token: 0x040021EF RID: 8687
	public bool loadSave;

	// Token: 0x040021F0 RID: 8688
	public string loadSaveFile = "";

	// Token: 0x040021F1 RID: 8689
	public string initializationCommands = "";
}
