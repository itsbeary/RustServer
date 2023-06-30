using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Windows
{
	// Token: 0x02000A23 RID: 2595
	[SuppressUnmanagedCodeSecurity]
	public class ConsoleWindow
	{
		// Token: 0x06003D8E RID: 15758 RVA: 0x00169244 File Offset: 0x00167444
		public void Initialize()
		{
			ConsoleWindow.FreeConsole();
			if (!ConsoleWindow.AttachConsole(4294967295U))
			{
				ConsoleWindow.AllocConsole();
			}
			this.oldOutput = Console.Out;
			try
			{
				Console.OutputEncoding = Encoding.UTF8;
				Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write), Encoding.UTF8)
				{
					AutoFlush = true
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Couldn't redirect output: " + ex.Message);
			}
		}

		// Token: 0x06003D8F RID: 15759 RVA: 0x001692D0 File Offset: 0x001674D0
		public void Shutdown()
		{
			Console.SetOut(this.oldOutput);
			ConsoleWindow.FreeConsole();
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x001692E3 File Offset: 0x001674E3
		public void SetTitle(string strName)
		{
			ConsoleWindow.SetConsoleTitleA(strName);
		}

		// Token: 0x06003D91 RID: 15761
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(uint dwProcessId);

		// Token: 0x06003D92 RID: 15762
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06003D93 RID: 15763
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		// Token: 0x06003D94 RID: 15764
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		// Token: 0x06003D95 RID: 15765
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleTitleA(string lpConsoleTitle);

		// Token: 0x040037C9 RID: 14281
		private TextWriter oldOutput;

		// Token: 0x040037CA RID: 14282
		private const int STD_INPUT_HANDLE = -10;

		// Token: 0x040037CB RID: 14283
		private const int STD_OUTPUT_HANDLE = -11;
	}
}
