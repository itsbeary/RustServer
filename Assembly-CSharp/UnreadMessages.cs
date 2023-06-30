using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200089C RID: 2204
public class UnreadMessages : SingletonComponent<UnreadMessages>
{
	// Token: 0x040031A8 RID: 12712
	public StyleAsset AllRead;

	// Token: 0x040031A9 RID: 12713
	public StyleAsset Unread;

	// Token: 0x040031AA RID: 12714
	public RustButton Button;

	// Token: 0x040031AB RID: 12715
	public GameObject UnreadTextObject;

	// Token: 0x040031AC RID: 12716
	public RustText UnreadText;

	// Token: 0x040031AD RID: 12717
	public GameObject MessageList;

	// Token: 0x040031AE RID: 12718
	public GameObject MessageListContainer;

	// Token: 0x040031AF RID: 12719
	public GameObject MessageListEmpty;
}
