using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x020007D4 RID: 2004
public class PickAFriend : UIDialog
{
	// Token: 0x1700044C RID: 1100
	// (set) Token: 0x06003546 RID: 13638 RVA: 0x00145923 File Offset: 0x00143B23
	public Func<ulong, bool> shouldShowPlayer
	{
		set
		{
			if (this.friendsList != null)
			{
				this.friendsList.shouldShowPlayer = value;
			}
		}
	}

	// Token: 0x04002D13 RID: 11539
	public InputField input;

	// Token: 0x04002D14 RID: 11540
	public RustText headerText;

	// Token: 0x04002D15 RID: 11541
	public bool AutoSelectInputField;

	// Token: 0x04002D16 RID: 11542
	public bool AllowMultiple;

	// Token: 0x04002D17 RID: 11543
	public Action<ulong, string> onSelected;

	// Token: 0x04002D18 RID: 11544
	public Translate.Phrase sleepingBagHeaderPhrase = new Translate.Phrase("assign_to_friend", "Assign To a Friend");

	// Token: 0x04002D19 RID: 11545
	public Translate.Phrase turretHeaderPhrase = new Translate.Phrase("authorize_a_friend", "Authorize a Friend");

	// Token: 0x04002D1A RID: 11546
	public SteamFriendsList friendsList;
}
