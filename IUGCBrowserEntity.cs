using System;
using System.Collections.Generic;

// Token: 0x020003E0 RID: 992
public interface IUGCBrowserEntity
{
	// Token: 0x170002DC RID: 732
	// (get) Token: 0x0600223D RID: 8765
	uint[] GetContentCRCs { get; }

	// Token: 0x0600223E RID: 8766
	void ClearContent();

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x0600223F RID: 8767
	UGCType ContentType { get; }

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06002240 RID: 8768
	List<ulong> EditingHistory { get; }

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06002241 RID: 8769
	BaseNetworkable UgcEntity { get; }
}
