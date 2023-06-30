using System;
using UnityEngine;

// Token: 0x02000619 RID: 1561
public interface IMissionProvider
{
	// Token: 0x06002E3F RID: 11839
	NetworkableId ProviderID();

	// Token: 0x06002E40 RID: 11840
	Vector3 ProviderPosition();

	// Token: 0x06002E41 RID: 11841
	BaseEntity Entity();
}
