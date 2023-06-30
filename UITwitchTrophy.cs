using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class UITwitchTrophy : UIDialog
{
	// Token: 0x0400109E RID: 4254
	public HttpImage EventImage;

	// Token: 0x0400109F RID: 4255
	public RustText EventName;

	// Token: 0x040010A0 RID: 4256
	public RustText WinningTeamName;

	// Token: 0x040010A1 RID: 4257
	public RectTransform TeamMembersRoot;

	// Token: 0x040010A2 RID: 4258
	public GameObject TeamMemberNamePrefab;

	// Token: 0x040010A3 RID: 4259
	public GameObject MissingDataOverlay;
}
