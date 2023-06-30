using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class AmbienceSpawnEmitters : MonoBehaviour, IClientComponent
{
	// Token: 0x040013E8 RID: 5096
	public int baseEmitterCount = 5;

	// Token: 0x040013E9 RID: 5097
	public int baseEmitterDistance = 10;

	// Token: 0x040013EA RID: 5098
	public GameObjectRef emitterPrefab;
}
