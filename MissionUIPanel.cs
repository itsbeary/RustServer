using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200061D RID: 1565
public class MissionUIPanel : MonoBehaviour
{
	// Token: 0x0400260C RID: 9740
	public GameObject activeMissionParent;

	// Token: 0x0400260D RID: 9741
	public RustText missionTitleText;

	// Token: 0x0400260E RID: 9742
	public RustText missionDescText;

	// Token: 0x0400260F RID: 9743
	public VirtualItemIcon[] rewardIcons;

	// Token: 0x04002610 RID: 9744
	public Translate.Phrase noMissionText;
}
