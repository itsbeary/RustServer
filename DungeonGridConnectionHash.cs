using System;

// Token: 0x0200067B RID: 1659
public struct DungeonGridConnectionHash
{
	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06002FEB RID: 12267 RVA: 0x001206E2 File Offset: 0x0011E8E2
	public int Value
	{
		get
		{
			return (this.North ? 1 : 0) | (this.South ? 2 : 0) | (this.West ? 4 : 0) | (this.East ? 8 : 0);
		}
	}

	// Token: 0x04002776 RID: 10102
	public bool North;

	// Token: 0x04002777 RID: 10103
	public bool South;

	// Token: 0x04002778 RID: 10104
	public bool West;

	// Token: 0x04002779 RID: 10105
	public bool East;
}
