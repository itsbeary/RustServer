using System;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class HideIfOwnerFirstPerson : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged
{
	// Token: 0x04001A4B RID: 6731
	public GameObject[] disableGameObjects;

	// Token: 0x04001A4C RID: 6732
	public bool worldModelEffect;
}
