using System;
using TMPro;
using UnityEngine;

// Token: 0x020008C1 RID: 2241
public class TeamUI : MonoBehaviour
{
	// Token: 0x04003294 RID: 12948
	public static Translate.Phrase invitePhrase = new Translate.Phrase("team.invited", "{0} has invited you to join a team");

	// Token: 0x04003295 RID: 12949
	public RectTransform MemberPanel;

	// Token: 0x04003296 RID: 12950
	public GameObject memberEntryPrefab;

	// Token: 0x04003297 RID: 12951
	public TeamMemberElement[] elements;

	// Token: 0x04003298 RID: 12952
	public GameObject NoTeamPanel;

	// Token: 0x04003299 RID: 12953
	public GameObject TeamPanel;

	// Token: 0x0400329A RID: 12954
	public GameObject LeaveTeamButton;

	// Token: 0x0400329B RID: 12955
	public GameObject InviteAcceptPanel;

	// Token: 0x0400329C RID: 12956
	public TextMeshProUGUI inviteText;

	// Token: 0x0400329D RID: 12957
	public static bool dirty = true;

	// Token: 0x0400329E RID: 12958
	[NonSerialized]
	public static ulong pendingTeamID;

	// Token: 0x0400329F RID: 12959
	[NonSerialized]
	public static string pendingTeamLeaderName;
}
