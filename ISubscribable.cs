using System;

// Token: 0x020004CA RID: 1226
public interface ISubscribable
{
	// Token: 0x06002811 RID: 10257
	bool AddSubscription(ulong steamId);

	// Token: 0x06002812 RID: 10258
	bool RemoveSubscription(ulong steamId);

	// Token: 0x06002813 RID: 10259
	bool HasSubscription(ulong steamId);
}
