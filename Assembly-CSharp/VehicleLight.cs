using System;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class VehicleLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04001F86 RID: 8070
	public bool IsBrake;

	// Token: 0x04001F87 RID: 8071
	public GameObject toggleObject;

	// Token: 0x04001F88 RID: 8072
	public VehicleLight.LightRenderer[] renderers;

	// Token: 0x04001F89 RID: 8073
	[ColorUsage(true, true)]
	public Color lightOnColour;

	// Token: 0x04001F8A RID: 8074
	[ColorUsage(true, true)]
	public Color brakesOnColour;

	// Token: 0x02000D23 RID: 3363
	[Serializable]
	public class LightRenderer
	{
		// Token: 0x040046DE RID: 18142
		public Renderer renderer;

		// Token: 0x040046DF RID: 18143
		public int matIndex;
	}
}
