using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008BD RID: 2237
public class SteamFriendsList : MonoBehaviour
{
	// Token: 0x04003271 RID: 12913
	public RectTransform targetPanel;

	// Token: 0x04003272 RID: 12914
	public SteamUserButton userButton;

	// Token: 0x04003273 RID: 12915
	public bool IncludeFriendsList = true;

	// Token: 0x04003274 RID: 12916
	public bool IncludeRecentlySeen;

	// Token: 0x04003275 RID: 12917
	public bool IncludeLastAttacker;

	// Token: 0x04003276 RID: 12918
	public bool IncludeRecentlyPlayedWith;

	// Token: 0x04003277 RID: 12919
	public bool ShowTeamFirst;

	// Token: 0x04003278 RID: 12920
	public bool HideSteamIdsInStreamerMode;

	// Token: 0x04003279 RID: 12921
	public bool RefreshOnEnable = true;

	// Token: 0x0400327A RID: 12922
	public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

	// Token: 0x0400327B RID: 12923
	public Func<ulong, bool> shouldShowPlayer;

	// Token: 0x02000EB6 RID: 3766
	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong, string>
	{
	}
}
