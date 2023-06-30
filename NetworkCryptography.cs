using System;
using Network;

// Token: 0x02000629 RID: 1577
public abstract class NetworkCryptography : INetworkCryptography
{
	// Token: 0x06002E81 RID: 11905 RVA: 0x00117298 File Offset: 0x00115498
	public unsafe ArraySegment<byte> EncryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* ptr;
			if ((array = arraySegment2.Array) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte[] array2;
			byte* ptr2;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array2[0];
			}
			Buffer.MemoryCopy((void*)ptr2, (void*)ptr, (long)arraySegment2.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.EncryptionHandler(connection, arraySegment, ref arraySegment2);
		return arraySegment2;
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x0011735C File Offset: 0x0011555C
	public unsafe ArraySegment<byte> DecryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* ptr;
			if ((array = arraySegment2.Array) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte[] array2;
			byte* ptr2;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array2[0];
			}
			Buffer.MemoryCopy((void*)ptr2, (void*)ptr, (long)arraySegment2.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.DecryptionHandler(connection, arraySegment, ref arraySegment2);
		return arraySegment2;
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x00117420 File Offset: 0x00115620
	public void Encrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.EncryptionHandler(connection, arraySegment, ref arraySegment2);
		data = arraySegment2;
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x0011747C File Offset: 0x0011567C
	public void Decrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.DecryptionHandler(connection, arraySegment, ref arraySegment2);
		data = arraySegment2;
	}

	// Token: 0x06002E85 RID: 11909
	protected abstract void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);

	// Token: 0x06002E86 RID: 11910
	protected abstract void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);

	// Token: 0x04002638 RID: 9784
	private byte[] buffer = new byte[8388608];
}
