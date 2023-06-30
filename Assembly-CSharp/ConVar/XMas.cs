using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AF4 RID: 2804
	[ConsoleSystem.Factory("xmas")]
	public class XMas : ConsoleSystem
	{
		// Token: 0x06004392 RID: 17298 RVA: 0x0018E174 File Offset: 0x0018C374
		[ServerVar]
		public static void refill(ConsoleSystem.Arg arg)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x04003CBB RID: 15547
		private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

		// Token: 0x04003CBC RID: 15548
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003CBD RID: 15549
		[ServerVar]
		public static float spawnRange = 40f;

		// Token: 0x04003CBE RID: 15550
		[ServerVar]
		public static int spawnAttempts = 5;

		// Token: 0x04003CBF RID: 15551
		[ServerVar]
		public static int giftsPerPlayer = 2;
	}
}
