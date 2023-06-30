using System;

namespace ConVar
{
	// Token: 0x02000AE6 RID: 2790
	public class Steam
	{
		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06004322 RID: 17186 RVA: 0x0018C9CA File Offset: 0x0018ABCA
		// (set) Token: 0x06004323 RID: 17187 RVA: 0x0018C9D1 File Offset: 0x0018ABD1
		[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
		public static bool server_allow_steam_nicknames { get; set; } = true;
	}
}
