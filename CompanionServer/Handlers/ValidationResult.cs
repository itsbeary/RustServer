using System;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A03 RID: 2563
	public enum ValidationResult
	{
		// Token: 0x04003753 RID: 14163
		Success,
		// Token: 0x04003754 RID: 14164
		NotFound,
		// Token: 0x04003755 RID: 14165
		RateLimit,
		// Token: 0x04003756 RID: 14166
		Banned,
		// Token: 0x04003757 RID: 14167
		Rejected
	}
}
