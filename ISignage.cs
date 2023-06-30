using System;
using UnityEngine;

// Token: 0x020003DF RID: 991
public interface ISignage
{
	// Token: 0x06002235 RID: 8757
	bool CanUpdateSign(BasePlayer player);

	// Token: 0x06002236 RID: 8758
	float Distance(Vector3 position);

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06002237 RID: 8759
	Vector2i TextureSize { get; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06002238 RID: 8760
	int TextureCount { get; }

	// Token: 0x06002239 RID: 8761
	uint[] GetTextureCRCs();

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x0600223A RID: 8762
	NetworkableId NetworkID { get; }

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x0600223B RID: 8763
	FileStorage.Type FileType { get; }

	// Token: 0x0600223C RID: 8764
	void SetTextureCRCs(uint[] crcs);
}
