using System;

namespace TinyJSON
{
	// Token: 0x020009D7 RID: 2519
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class DecodeAlias : Attribute
	{
		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06003BF9 RID: 15353 RVA: 0x00161C5C File Offset: 0x0015FE5C
		// (set) Token: 0x06003BFA RID: 15354 RVA: 0x00161C64 File Offset: 0x0015FE64
		public string[] Names { get; private set; }

		// Token: 0x06003BFB RID: 15355 RVA: 0x00161C6D File Offset: 0x0015FE6D
		public DecodeAlias(params string[] names)
		{
			this.Names = names;
		}

		// Token: 0x06003BFC RID: 15356 RVA: 0x00161C7C File Offset: 0x0015FE7C
		public bool Contains(string name)
		{
			return Array.IndexOf<string>(this.Names, name) > -1;
		}
	}
}
