using System;
using System.Linq;
using Facepunch;
using Facepunch.Models;

// Token: 0x02000326 RID: 806
public static class DeveloperList
{
	// Token: 0x06001F26 RID: 7974 RVA: 0x000D3CD4 File Offset: 0x000D1ED4
	public static bool Contains(string steamid)
	{
		return Application.Manifest != null && Application.Manifest.Administrators != null && Application.Manifest.Administrators.Any((Facepunch.Models.Manifest.Administrator x) => x.UserId == steamid);
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000D3D20 File Offset: 0x000D1F20
	public static bool Contains(ulong steamid)
	{
		return DeveloperList.Contains(steamid.ToString());
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000D3D2E File Offset: 0x000D1F2E
	public static bool IsDeveloper(BasePlayer ply)
	{
		return ply != null && DeveloperList.Contains(ply.UserIDString);
	}
}
