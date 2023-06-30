using System;

namespace GameTips
{
	// Token: 0x020009E5 RID: 2533
	public abstract class BaseTip
	{
		// Token: 0x06003C65 RID: 15461
		public abstract Translate.Phrase GetPhrase();

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06003C66 RID: 15462
		public abstract bool ShouldShow { get; }

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06003C67 RID: 15463 RVA: 0x00162E2C File Offset: 0x0016102C
		public string Type
		{
			get
			{
				return base.GetType().Name;
			}
		}
	}
}
