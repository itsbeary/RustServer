using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008C0 RID: 2240
public class TeamMemberElement : MonoBehaviour
{
	// Token: 0x04003288 RID: 12936
	public RustText nameText;

	// Token: 0x04003289 RID: 12937
	public RawImage icon;

	// Token: 0x0400328A RID: 12938
	public Color onlineColor;

	// Token: 0x0400328B RID: 12939
	public Color offlineColor;

	// Token: 0x0400328C RID: 12940
	public Color deadColor;

	// Token: 0x0400328D RID: 12941
	public Color woundedColor;

	// Token: 0x0400328E RID: 12942
	public GameObject hoverOverlay;

	// Token: 0x0400328F RID: 12943
	public RawImage memberIcon;

	// Token: 0x04003290 RID: 12944
	public RawImage leaderIcon;

	// Token: 0x04003291 RID: 12945
	public RawImage deadIcon;

	// Token: 0x04003292 RID: 12946
	public RawImage woundedIcon;

	// Token: 0x04003293 RID: 12947
	public int teamIndex;
}
