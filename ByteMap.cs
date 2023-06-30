using System;
using UnityEngine;

// Token: 0x02000656 RID: 1622
[Serializable]
public class ByteMap
{
	// Token: 0x06002EE6 RID: 12006 RVA: 0x0011A0D8 File Offset: 0x001182D8
	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = new byte[bytes * size * size];
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x0011A0FE File Offset: 0x001182FE
	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06002EE8 RID: 12008 RVA: 0x0011A11B File Offset: 0x0011831B
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x170003CB RID: 971
	public uint this[int x, int y]
	{
		get
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				return (uint)this.values[num];
			case 2:
			{
				uint num2 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				return (num2 << 8) | num3;
			}
			case 3:
			{
				uint num4 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				return (num4 << 16) | (num3 << 8) | num5;
			}
			default:
			{
				uint num6 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				uint num7 = (uint)this.values[num + 3];
				return (num6 << 24) | (num3 << 16) | (num5 << 8) | num7;
			}
			}
		}
		set
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				this.values[num] = (byte)(value & 255U);
				return;
			case 2:
				this.values[num] = (byte)((value >> 8) & 255U);
				this.values[num + 1] = (byte)(value & 255U);
				return;
			case 3:
				this.values[num] = (byte)((value >> 16) & 255U);
				this.values[num + 1] = (byte)((value >> 8) & 255U);
				this.values[num + 2] = (byte)(value & 255U);
				return;
			default:
				this.values[num] = (byte)((value >> 24) & 255U);
				this.values[num + 1] = (byte)((value >> 16) & 255U);
				this.values[num + 2] = (byte)((value >> 8) & 255U);
				this.values[num + 3] = (byte)(value & 255U);
				return;
			}
		}
	}

	// Token: 0x040026C7 RID: 9927
	[SerializeField]
	private int size;

	// Token: 0x040026C8 RID: 9928
	[SerializeField]
	private int bytes;

	// Token: 0x040026C9 RID: 9929
	[SerializeField]
	private byte[] values;
}
