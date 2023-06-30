using System;
using System.IO;
using System.Text;

// Token: 0x0200094B RID: 2379
public static class MurmurHashEx
{
	// Token: 0x060038F0 RID: 14576 RVA: 0x0015215F File Offset: 0x0015035F
	public static int MurmurHashSigned(this string str)
	{
		return MurmurHash.Signed(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x0015216C File Offset: 0x0015036C
	public static uint MurmurHashUnsigned(this string str)
	{
		return MurmurHash.Unsigned(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x00152179 File Offset: 0x00150379
	private static MemoryStream StringToStream(string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
	}
}
