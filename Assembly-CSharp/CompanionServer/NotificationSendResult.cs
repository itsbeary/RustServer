using System;

namespace CompanionServer
{
	// Token: 0x020009F0 RID: 2544
	public enum NotificationSendResult
	{
		// Token: 0x04003720 RID: 14112
		Failed,
		// Token: 0x04003721 RID: 14113
		Sent,
		// Token: 0x04003722 RID: 14114
		Empty,
		// Token: 0x04003723 RID: 14115
		Disabled,
		// Token: 0x04003724 RID: 14116
		RateLimited,
		// Token: 0x04003725 RID: 14117
		ServerError,
		// Token: 0x04003726 RID: 14118
		NoTargetsFound,
		// Token: 0x04003727 RID: 14119
		TooManySubscribers
	}
}
