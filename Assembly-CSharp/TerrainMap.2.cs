using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Token: 0x020006A8 RID: 1704
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	// Token: 0x060030C9 RID: 12489 RVA: 0x001253D1 File Offset: 0x001235D1
	public void Push()
	{
		if (this.src != this.dst)
		{
			return;
		}
		this.dst = (T[])this.src.Clone();
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x001253F8 File Offset: 0x001235F8
	public void Pop()
	{
		if (this.src == this.dst)
		{
			return;
		}
		Array.Copy(this.dst, this.src, this.src.Length);
		this.dst = this.src;
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x0012542E File Offset: 0x0012362E
	public IEnumerable<T> ToEnumerable()
	{
		return this.src.Cast<T>();
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x0012543B File Offset: 0x0012363B
	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0012544C File Offset: 0x0012364C
	public long GetMemoryUsage()
	{
		return (long)this.BytesPerElement() * (long)this.src.Length;
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x00125460 File Offset: 0x00123660
	public byte[] ToByteArray()
	{
		byte[] array = new byte[this.BytesPerElement() * this.src.Length];
		Buffer.BlockCopy(this.src, 0, array, 0, array.Length);
		return array;
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x00125494 File Offset: 0x00123694
	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, this.dst, 0, dat.Length);
	}

	// Token: 0x04002806 RID: 10246
	internal T[] src;

	// Token: 0x04002807 RID: 10247
	internal T[] dst;
}
