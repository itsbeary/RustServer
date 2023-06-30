using System;
using UnityEngine;

namespace Windows
{
	// Token: 0x02000A22 RID: 2594
	public class ConsoleInput
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06003D83 RID: 15747 RVA: 0x00168EDC File Offset: 0x001670DC
		// (remove) Token: 0x06003D84 RID: 15748 RVA: 0x00168F14 File Offset: 0x00167114
		public event Action<string> OnInputText;

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06003D85 RID: 15749 RVA: 0x00168F49 File Offset: 0x00167149
		public bool valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06003D86 RID: 15750 RVA: 0x00168F53 File Offset: 0x00167153
		public int lineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x00168F5A File Offset: 0x0016715A
		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.lineWidth * numLines));
			Console.CursorTop -= numLines;
			Console.CursorLeft = 0;
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x00168F88 File Offset: 0x00167188
		public void RedrawInputLine()
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.CursorTop++;
				for (int i = 0; i < this.statusText.Length; i++)
				{
					Console.CursorLeft = 0;
					Console.Write(this.statusText[i].PadRight(this.lineWidth));
				}
				Console.CursorTop -= this.statusText.Length + 1;
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length == 0)
				{
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
					return;
				}
				if (this.inputString.Length < this.lineWidth - 2)
				{
					Console.Write(this.inputString);
				}
				else
				{
					Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
				}
			}
			catch (Exception)
			{
			}
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x00169098 File Offset: 0x00167298
		internal void OnBackspace()
		{
			if (this.inputString.Length < 1)
			{
				return;
			}
			this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
			this.RedrawInputLine();
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x001690CE File Offset: 0x001672CE
		internal void OnEscape()
		{
			this.inputString = "";
			this.RedrawInputLine();
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x001690E4 File Offset: 0x001672E4
		internal void OnEnter()
		{
			this.ClearLine(this.statusText.Length);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("> " + this.inputString);
			string text = this.inputString;
			this.inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText(text);
			}
			this.RedrawInputLine();
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x00169148 File Offset: 0x00167348
		public void Update()
		{
			if (!this.valid)
			{
				return;
			}
			if (this.nextUpdate < Time.realtimeSinceStartup)
			{
				this.RedrawInputLine();
				this.nextUpdate = Time.realtimeSinceStartup + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key == ConsoleKey.Enter)
			{
				this.OnEnter();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Backspace)
			{
				this.OnBackspace();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Escape)
			{
				this.OnEscape();
				return;
			}
			if (consoleKeyInfo.KeyChar != '\0')
			{
				this.inputString += consoleKeyInfo.KeyChar.ToString();
				this.RedrawInputLine();
				return;
			}
		}

		// Token: 0x040037C6 RID: 14278
		public string inputString = "";

		// Token: 0x040037C7 RID: 14279
		public string[] statusText = new string[] { "", "", "" };

		// Token: 0x040037C8 RID: 14280
		internal float nextUpdate;
	}
}
