using System;

namespace UnityEngine.UI
{
	// Token: 0x02000A35 RID: 2613
	public class ScrollRectSettable : ScrollRect
	{
		// Token: 0x06003E2C RID: 15916 RVA: 0x0016BCC8 File Offset: 0x00169EC8
		public void SetHorizNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x06003E2D RID: 15917 RVA: 0x0016BCD2 File Offset: 0x00169ED2
		public void SetVertNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}
	}
}
