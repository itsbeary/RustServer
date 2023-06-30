using System;
using UnityEngine;

// Token: 0x02000765 RID: 1893
[CreateAssetMenu(menuName = "Rust/Steam DLC Item")]
public class SteamDLCItem : ScriptableObject
{
	// Token: 0x0600349C RID: 13468 RVA: 0x00144D94 File Offset: 0x00142F94
	public bool HasLicense(ulong steamid)
	{
		return this.bypassLicenseCheck || (PlatformService.Instance.IsValid && PlatformService.Instance.PlayerOwnsDownloadableContent(steamid, this.dlcAppID));
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x00144DBF File Offset: 0x00142FBF
	public bool CanUse(BasePlayer player)
	{
		return player.isServer && (this.HasLicense(player.userID) || player.userID < 10000000UL);
	}

	// Token: 0x04002B2B RID: 11051
	public int id;

	// Token: 0x04002B2C RID: 11052
	public Translate.Phrase dlcName;

	// Token: 0x04002B2D RID: 11053
	public int dlcAppID;

	// Token: 0x04002B2E RID: 11054
	public bool bypassLicenseCheck;
}
