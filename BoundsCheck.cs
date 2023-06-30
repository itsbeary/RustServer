using System;

// Token: 0x02000653 RID: 1619
public class BoundsCheck : PrefabAttribute
{
	// Token: 0x06002EE1 RID: 12001 RVA: 0x00119EE3 File Offset: 0x001180E3
	protected override Type GetIndexedType()
	{
		return typeof(BoundsCheck);
	}

	// Token: 0x040026C4 RID: 9924
	public BoundsCheck.BlockType IsType;

	// Token: 0x02000DB5 RID: 3509
	public enum BlockType
	{
		// Token: 0x0400490F RID: 18703
		Tree
	}
}
