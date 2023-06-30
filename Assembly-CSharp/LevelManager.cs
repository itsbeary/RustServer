using System;
using System.Collections;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000554 RID: 1364
public static class LevelManager
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002A3A RID: 10810 RVA: 0x00101EC4 File Offset: 0x001000C4
	public static bool isLoaded
	{
		get
		{
			return LevelManager.CurrentLevelName != null && !(LevelManager.CurrentLevelName == "") && !(LevelManager.CurrentLevelName == "Empty") && !(LevelManager.CurrentLevelName == "MenuBackground");
		}
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x00101F14 File Offset: 0x00100114
	public static bool IsValid(string strName)
	{
		return Application.CanStreamedLevelBeLoaded(strName);
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x00101F1C File Offset: 0x0010011C
	public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		SceneManager.LoadScene(strName, LoadSceneMode.Single);
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x00101F49 File Offset: 0x00100149
	public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		yield return null;
		yield return SceneManager.LoadSceneAsync(strName, LoadSceneMode.Single);
		yield return null;
		yield return null;
		yield break;
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x00101F58 File Offset: 0x00100158
	public static void UnloadLevel(bool loadingScreen = true)
	{
		LevelManager.CurrentLevelName = null;
		SceneManager.LoadScene("Empty");
	}

	// Token: 0x04002291 RID: 8849
	public static string CurrentLevelName;
}
