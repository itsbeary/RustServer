using System;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE5 RID: 2789
	[ConsoleSystem.Factory("stability")]
	public class Stability : ConsoleSystem
	{
		// Token: 0x0600431F RID: 17183 RVA: 0x0018C944 File Offset: 0x0018AB44
		[ServerVar]
		public static void refresh_stability(ConsoleSystem.Arg args)
		{
			StabilityEntity[] array = BaseNetworkable.serverEntities.OfType<StabilityEntity>().ToArray<StabilityEntity>();
			Debug.Log("Refreshing stability on " + array.Length + " entities...");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateStability();
			}
		}

		// Token: 0x04003C93 RID: 15507
		[ServerVar]
		public static int verbose = 0;

		// Token: 0x04003C94 RID: 15508
		[ServerVar]
		public static int strikes = 10;

		// Token: 0x04003C95 RID: 15509
		[ServerVar]
		public static float collapse = 0.05f;

		// Token: 0x04003C96 RID: 15510
		[ServerVar]
		public static float accuracy = 0.001f;

		// Token: 0x04003C97 RID: 15511
		[ServerVar]
		public static float stabilityqueue = 9f;

		// Token: 0x04003C98 RID: 15512
		[ServerVar]
		public static float surroundingsqueue = 3f;
	}
}
