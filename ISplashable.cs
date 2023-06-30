using System;

// Token: 0x02000419 RID: 1049
public interface ISplashable
{
	// Token: 0x0600238C RID: 9100
	bool WantsSplash(ItemDefinition splashType, int amount);

	// Token: 0x0600238D RID: 9101
	int DoSplash(ItemDefinition splashType, int amount);
}
