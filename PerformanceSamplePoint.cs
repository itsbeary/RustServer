using System;

// Token: 0x02000913 RID: 2323
public struct PerformanceSamplePoint
{
	// Token: 0x04003364 RID: 13156
	public int UpdateCount;

	// Token: 0x04003365 RID: 13157
	public int FixedUpdateCount;

	// Token: 0x04003366 RID: 13158
	public int RenderCount;

	// Token: 0x04003367 RID: 13159
	public TimeSpan PreCull;

	// Token: 0x04003368 RID: 13160
	public TimeSpan Update;

	// Token: 0x04003369 RID: 13161
	public TimeSpan LateUpdate;

	// Token: 0x0400336A RID: 13162
	public TimeSpan Render;

	// Token: 0x0400336B RID: 13163
	public TimeSpan FixedUpdate;

	// Token: 0x0400336C RID: 13164
	public TimeSpan NetworkMessage;

	// Token: 0x0400336D RID: 13165
	public TimeSpan TotalCPU;

	// Token: 0x0400336E RID: 13166
	public int CpuUpdateCount;
}
