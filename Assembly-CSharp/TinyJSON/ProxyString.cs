using System;

namespace TinyJSON
{
	// Token: 0x020009E0 RID: 2528
	public sealed class ProxyString : Variant
	{
		// Token: 0x06003C36 RID: 15414 RVA: 0x00162B15 File Offset: 0x00160D15
		public ProxyString(string value)
		{
			this.value = value;
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x00162B24 File Offset: 0x00160D24
		public override string ToString(IFormatProvider provider)
		{
			return this.value;
		}

		// Token: 0x040036E5 RID: 14053
		private readonly string value;
	}
}
