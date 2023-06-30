using System;

namespace CompanionServer
{
	// Token: 0x020009F9 RID: 2553
	public interface ITokenBucketSettings
	{
		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06003CF4 RID: 15604
		double MaxTokens { get; }

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06003CF5 RID: 15605
		double TokensPerSec { get; }
	}
}
