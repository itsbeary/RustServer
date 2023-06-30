using System;
using ConVar;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000B12 RID: 2834
	internal static class GameInfo
	{
		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06004517 RID: 17687 RVA: 0x00194A5F File Offset: 0x00192C5F
		internal static bool IsOfficialServer
		{
			get
			{
				return Application.isEditor || Server.official;
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06004518 RID: 17688 RVA: 0x00194A6F File Offset: 0x00192C6F
		internal static bool HasAchievements
		{
			get
			{
				return GameInfo.IsOfficialServer;
			}
		}
	}
}
