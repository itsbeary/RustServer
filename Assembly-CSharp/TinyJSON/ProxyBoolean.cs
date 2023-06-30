using System;

namespace TinyJSON
{
	// Token: 0x020009DD RID: 2525
	public sealed class ProxyBoolean : Variant
	{
		// Token: 0x06003C17 RID: 15383 RVA: 0x0016285A File Offset: 0x00160A5A
		public ProxyBoolean(bool value)
		{
			this.value = value;
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x00162869 File Offset: 0x00160A69
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value;
		}

		// Token: 0x06003C19 RID: 15385 RVA: 0x00162871 File Offset: 0x00160A71
		public override string ToString(IFormatProvider provider)
		{
			if (!this.value)
			{
				return "false";
			}
			return "true";
		}

		// Token: 0x040036E0 RID: 14048
		private readonly bool value;
	}
}
