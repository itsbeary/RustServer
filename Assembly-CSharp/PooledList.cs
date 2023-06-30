using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x0200094F RID: 2383
public class PooledList<T>
{
	// Token: 0x06003925 RID: 14629 RVA: 0x0015361A File Offset: 0x0015181A
	public void Alloc()
	{
		if (this.data == null)
		{
			this.data = Pool.GetList<T>();
		}
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x0015362F File Offset: 0x0015182F
	public void Free()
	{
		if (this.data != null)
		{
			Pool.FreeList<T>(ref this.data);
		}
	}

	// Token: 0x06003927 RID: 14631 RVA: 0x00153644 File Offset: 0x00151844
	public void Clear()
	{
		if (this.data != null)
		{
			this.data.Clear();
		}
	}

	// Token: 0x040033D5 RID: 13269
	public List<T> data;
}
