using System;
using System.Runtime.InteropServices;

// Token: 0x02000303 RID: 771
public static class WindowsAffinityShim
{
	// Token: 0x06001EA4 RID: 7844
	[DllImport("kernel32.dll")]
	public static extern bool SetProcessAffinityMask(IntPtr process, IntPtr mask);

	// Token: 0x06001EA5 RID: 7845
	[DllImport("kernel32.dll")]
	public static extern bool SetPriorityClass(IntPtr process, uint mask);
}
