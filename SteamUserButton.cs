using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BE RID: 2238
public class SteamUserButton : MonoBehaviour
{
	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06003728 RID: 14120 RVA: 0x0014BC39 File Offset: 0x00149E39
	// (set) Token: 0x06003729 RID: 14121 RVA: 0x0014BC41 File Offset: 0x00149E41
	public ulong SteamId { get; private set; }

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x0600372A RID: 14122 RVA: 0x0014BC4A File Offset: 0x00149E4A
	// (set) Token: 0x0600372B RID: 14123 RVA: 0x0014BC52 File Offset: 0x00149E52
	public string Username { get; private set; }

	// Token: 0x0400327C RID: 12924
	public RustText steamName;

	// Token: 0x0400327D RID: 12925
	public RustText steamInfo;

	// Token: 0x0400327E RID: 12926
	public RawImage avatar;

	// Token: 0x0400327F RID: 12927
	public Color textColorInGame;

	// Token: 0x04003280 RID: 12928
	public Color textColorOnline;

	// Token: 0x04003281 RID: 12929
	public Color textColorNormal;
}
