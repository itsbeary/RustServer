using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009F3 RID: 2547
	public class PushRequest : Pool.IPooled
	{
		// Token: 0x06003CCA RID: 15562 RVA: 0x00164A72 File Offset: 0x00162C72
		public void EnterPool()
		{
			Pool.FreeList<ulong>(ref this.SteamIds);
			this.Channel = (NotificationChannel)0;
			this.Title = null;
			this.Body = null;
			if (this.Data != null)
			{
				this.Data.Clear();
				Pool.Free<Dictionary<string, string>>(ref this.Data);
			}
		}

		// Token: 0x06003CCB RID: 15563 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x0400372D RID: 14125
		public string ServerToken;

		// Token: 0x0400372E RID: 14126
		public List<ulong> SteamIds;

		// Token: 0x0400372F RID: 14127
		public NotificationChannel Channel;

		// Token: 0x04003730 RID: 14128
		public string Title;

		// Token: 0x04003731 RID: 14129
		public string Body;

		// Token: 0x04003732 RID: 14130
		public Dictionary<string, string> Data;
	}
}
