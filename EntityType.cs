using System;

// Token: 0x02000392 RID: 914
[Flags]
public enum EntityType
{
	// Token: 0x04001977 RID: 6519
	Player = 1,
	// Token: 0x04001978 RID: 6520
	NPC = 2,
	// Token: 0x04001979 RID: 6521
	WorldItem = 4,
	// Token: 0x0400197A RID: 6522
	Corpse = 8,
	// Token: 0x0400197B RID: 6523
	TimedExplosive = 16,
	// Token: 0x0400197C RID: 6524
	Chair = 32,
	// Token: 0x0400197D RID: 6525
	BasePlayerNPC = 64
}
