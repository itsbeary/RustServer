using System;

namespace TinyJSON
{
	// Token: 0x020009CF RID: 2511
	[Flags]
	public enum EncodeOptions
	{
		// Token: 0x040036C7 RID: 14023
		None = 0,
		// Token: 0x040036C8 RID: 14024
		PrettyPrint = 1,
		// Token: 0x040036C9 RID: 14025
		NoTypeHints = 2,
		// Token: 0x040036CA RID: 14026
		IncludePublicProperties = 4,
		// Token: 0x040036CB RID: 14027
		EnforceHierarchyOrder = 8,
		// Token: 0x040036CC RID: 14028
		[Obsolete("Use EncodeOptions.EnforceHierarchyOrder instead.")]
		EnforceHeirarchyOrder = 8
	}
}
