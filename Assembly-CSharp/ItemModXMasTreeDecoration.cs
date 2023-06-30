using System;

// Token: 0x0200018E RID: 398
public class ItemModXMasTreeDecoration : ItemMod
{
	// Token: 0x040010BC RID: 4284
	public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

	// Token: 0x02000C41 RID: 3137
	public enum xmasFlags
	{
		// Token: 0x04004321 RID: 17185
		pineCones = 128,
		// Token: 0x04004322 RID: 17186
		candyCanes = 256,
		// Token: 0x04004323 RID: 17187
		gingerbreadMen = 512,
		// Token: 0x04004324 RID: 17188
		Tinsel = 1024,
		// Token: 0x04004325 RID: 17189
		Balls = 2048,
		// Token: 0x04004326 RID: 17190
		Star = 16384,
		// Token: 0x04004327 RID: 17191
		Lights = 32768
	}
}
