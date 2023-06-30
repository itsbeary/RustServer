using System;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public class GlassPane : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001F47 RID: 8007
	public Renderer glassRendereer;

	// Token: 0x04001F48 RID: 8008
	[SerializeField]
	private BaseVehicleModule module;

	// Token: 0x04001F49 RID: 8009
	[SerializeField]
	private float showFullDamageAt = 0.75f;
}
