using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB1 RID: 2737
	[ConsoleSystem.Factory("bradley")]
	public class Bradley : ConsoleSystem
	{
		// Token: 0x06004181 RID: 16769 RVA: 0x00183D78 File Offset: 0x00181F78
		[ServerVar]
		public static void quickrespawn(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			BradleySpawner singleton = BradleySpawner.singleton;
			if (singleton == null)
			{
				Debug.LogWarning("No Spawner");
				return;
			}
			if (singleton.spawned)
			{
				singleton.spawned.Kill(BaseNetworkable.DestroyMode.None);
			}
			singleton.spawned = null;
			singleton.DoRespawn();
		}

		// Token: 0x04003B72 RID: 15218
		[ServerVar]
		public static float respawnDelayMinutes = 60f;

		// Token: 0x04003B73 RID: 15219
		[ServerVar]
		public static float respawnDelayVariance = 1f;

		// Token: 0x04003B74 RID: 15220
		[ServerVar]
		public static bool enabled = true;
	}
}
