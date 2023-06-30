using System;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class Buttons
{
	// Token: 0x02000CBC RID: 3260
	public class ConButton : ConsoleSystem.IConsoleButton
	{
		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06004F9A RID: 20378 RVA: 0x001A70F7 File Offset: 0x001A52F7
		// (set) Token: 0x06004F9B RID: 20379 RVA: 0x001A70FF File Offset: 0x001A52FF
		public bool IsDown { get; set; }

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06004F9C RID: 20380 RVA: 0x001A7108 File Offset: 0x001A5308
		public bool JustPressed
		{
			get
			{
				return this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06004F9D RID: 20381 RVA: 0x001A7121 File Offset: 0x001A5321
		public bool JustReleased
		{
			get
			{
				return !this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x000063A5 File Offset: 0x000045A5
		public void Call(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06004F9F RID: 20383 RVA: 0x001A713A File Offset: 0x001A533A
		// (set) Token: 0x06004FA0 RID: 20384 RVA: 0x001A7142 File Offset: 0x001A5342
		public bool IsPressed
		{
			get
			{
				return this.IsDown;
			}
			set
			{
				if (value == this.IsDown)
				{
					return;
				}
				this.IsDown = value;
				this.frame = Time.frameCount;
			}
		}

		// Token: 0x0400450D RID: 17677
		private int frame;
	}
}
