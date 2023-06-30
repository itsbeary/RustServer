using System;
using System.IO;

// Token: 0x0200092C RID: 2348
public static class StreamEx
{
	// Token: 0x06003856 RID: 14422 RVA: 0x0014F48C File Offset: 0x0014D68C
	public static void WriteToOtherStream(this Stream self, Stream target)
	{
		int num;
		while ((num = self.Read(StreamEx.StaticBuffer, 0, StreamEx.StaticBuffer.Length)) > 0)
		{
			target.Write(StreamEx.StaticBuffer, 0, num);
		}
	}

	// Token: 0x0400339A RID: 13210
	private static readonly byte[] StaticBuffer = new byte[16384];
}
