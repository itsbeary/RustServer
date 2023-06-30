using System;

// Token: 0x0200039D RID: 925
public interface ICassettePlayer
{
	// Token: 0x060020BD RID: 8381
	void OnCassetteInserted(Cassette c);

	// Token: 0x060020BE RID: 8382
	void OnCassetteRemoved(Cassette c);

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x060020BF RID: 8383
	BaseEntity ToBaseEntity { get; }
}
