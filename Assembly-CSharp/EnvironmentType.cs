using System;

// Token: 0x0200050C RID: 1292
[Flags]
public enum EnvironmentType
{
	// Token: 0x04002191 RID: 8593
	Underground = 1,
	// Token: 0x04002192 RID: 8594
	Building = 2,
	// Token: 0x04002193 RID: 8595
	Outdoor = 4,
	// Token: 0x04002194 RID: 8596
	Elevator = 8,
	// Token: 0x04002195 RID: 8597
	PlayerConstruction = 16,
	// Token: 0x04002196 RID: 8598
	TrainTunnels = 32,
	// Token: 0x04002197 RID: 8599
	UnderwaterLab = 64,
	// Token: 0x04002198 RID: 8600
	Submarine = 128,
	// Token: 0x04002199 RID: 8601
	BuildingDark = 256,
	// Token: 0x0400219A RID: 8602
	BuildingVeryDark = 512,
	// Token: 0x0400219B RID: 8603
	NoSunlight = 1024
}
