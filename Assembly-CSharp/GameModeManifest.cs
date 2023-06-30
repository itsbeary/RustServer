using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000517 RID: 1303
[CreateAssetMenu(menuName = "Rust/Game Mode Manifest")]
public class GameModeManifest : ScriptableObject
{
	// Token: 0x060029B8 RID: 10680 RVA: 0x000FFFDB File Offset: 0x000FE1DB
	public static GameModeManifest Get()
	{
		if (GameModeManifest.instance == null)
		{
			GameModeManifest.instance = Resources.Load<GameModeManifest>("GameModeManifest");
		}
		return GameModeManifest.instance;
	}

	// Token: 0x040021D5 RID: 8661
	public static GameModeManifest instance;

	// Token: 0x040021D6 RID: 8662
	public List<GameObjectRef> gameModePrefabs;
}
