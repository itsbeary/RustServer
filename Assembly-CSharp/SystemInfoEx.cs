using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000317 RID: 791
public static class SystemInfoEx
{
	// Token: 0x06001F02 RID: 7938
	[DllImport("RustNative")]
	private static extern ulong System_GetMemoryUsage();

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06001F03 RID: 7939 RVA: 0x000D2D13 File Offset: 0x000D0F13
	public static int systemMemoryUsed
	{
		get
		{
			return (int)(SystemInfoEx.System_GetMemoryUsage() / 1024UL / 1024UL);
		}
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x000D2D2C File Offset: 0x000D0F2C
	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (SystemInfoEx.supportedRenderTextureFormats == null)
		{
			Array values = Enum.GetValues(typeof(RenderTextureFormat));
			int num = (int)values.GetValue(values.Length - 1);
			SystemInfoEx.supportedRenderTextureFormats = new bool[num + 1];
			for (int i = 0; i <= num; i++)
			{
				bool flag = Enum.IsDefined(typeof(RenderTextureFormat), i);
				SystemInfoEx.supportedRenderTextureFormats[i] = flag && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)i);
			}
		}
		return SystemInfoEx.supportedRenderTextureFormats[(int)format];
	}

	// Token: 0x040017E6 RID: 6118
	private static bool[] supportedRenderTextureFormats;
}
