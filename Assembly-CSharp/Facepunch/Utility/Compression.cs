using System;
using Ionic.Zlib;

namespace Facepunch.Utility
{
	// Token: 0x02000B04 RID: 2820
	public class Compression
	{
		// Token: 0x060044BB RID: 17595 RVA: 0x00192E10 File Offset: 0x00191010
		public static byte[] Compress(byte[] data)
		{
			byte[] array;
			try
			{
				array = GZipStream.CompressBuffer(data);
			}
			catch (Exception)
			{
				array = null;
			}
			return array;
		}

		// Token: 0x060044BC RID: 17596 RVA: 0x00192E3C File Offset: 0x0019103C
		public static byte[] Uncompress(byte[] data)
		{
			return GZipStream.UncompressBuffer(data);
		}
	}
}
