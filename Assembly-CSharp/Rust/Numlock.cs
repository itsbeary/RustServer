using System;
using System.Runtime.InteropServices;

namespace Rust
{
	// Token: 0x02000B0C RID: 2828
	public static class Numlock
	{
		// Token: 0x060044FC RID: 17660
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		// Token: 0x060044FD RID: 17661
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x060044FE RID: 17662 RVA: 0x001947FB File Offset: 0x001929FB
		public static bool IsOn
		{
			get
			{
				return ((ushort)Numlock.GetKeyState(144) & ushort.MaxValue) > 0;
			}
		}

		// Token: 0x060044FF RID: 17663 RVA: 0x00194811 File Offset: 0x00192A11
		public static void TurnOn()
		{
			if (!Numlock.IsOn)
			{
				Numlock.keybd_event(144, 69, 1U, 0);
				Numlock.keybd_event(144, 69, 3U, 0);
			}
		}

		// Token: 0x04003D6E RID: 15726
		private const byte VK_NUMLOCK = 144;

		// Token: 0x04003D6F RID: 15727
		private const uint KEYEVENTF_EXTENDEDKEY = 1U;

		// Token: 0x04003D70 RID: 15728
		private const int KEYEVENTF_KEYUP = 2;

		// Token: 0x04003D71 RID: 15729
		private const int KEYEVENTF_KEYDOWN = 0;
	}
}
