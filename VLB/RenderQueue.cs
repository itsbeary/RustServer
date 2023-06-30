using System;

namespace VLB
{
	// Token: 0x020009B8 RID: 2488
	public enum RenderQueue
	{
		// Token: 0x0400363D RID: 13885
		Custom,
		// Token: 0x0400363E RID: 13886
		Background = 1000,
		// Token: 0x0400363F RID: 13887
		Geometry = 2000,
		// Token: 0x04003640 RID: 13888
		AlphaTest = 2450,
		// Token: 0x04003641 RID: 13889
		GeometryLast = 2500,
		// Token: 0x04003642 RID: 13890
		Transparent = 3000,
		// Token: 0x04003643 RID: 13891
		Overlay = 4000
	}
}
