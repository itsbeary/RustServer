using System;

// Token: 0x02000103 RID: 259
[Flags]
public enum RemoteControllableControls
{
	// Token: 0x04000DF0 RID: 3568
	None = 0,
	// Token: 0x04000DF1 RID: 3569
	Movement = 1,
	// Token: 0x04000DF2 RID: 3570
	Mouse = 2,
	// Token: 0x04000DF3 RID: 3571
	SprintAndDuck = 4,
	// Token: 0x04000DF4 RID: 3572
	Fire = 8,
	// Token: 0x04000DF5 RID: 3573
	Reload = 16,
	// Token: 0x04000DF6 RID: 3574
	Crosshair = 32
}
