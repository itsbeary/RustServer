using System;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public abstract class VehicleModuleButtonComponent : MonoBehaviour
{
	// Token: 0x06002715 RID: 10005
	public abstract void ServerUse(BasePlayer player, BaseVehicleModule parentModule);

	// Token: 0x04001F6F RID: 8047
	public string interactionColliderName = "MyCollider";

	// Token: 0x04001F70 RID: 8048
	public SoundDefinition pressSoundDef;
}
