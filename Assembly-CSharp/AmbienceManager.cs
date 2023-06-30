using System;
using System.Collections.Generic;

// Token: 0x02000225 RID: 549
public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001C00 RID: 7168 RVA: 0x000C488C File Offset: 0x000C2A8C
	// (set) Token: 0x06001C01 RID: 7169 RVA: 0x000C4894 File Offset: 0x000C2A94
	public float ambienceZoneGain { get; private set; } = 1f;

	// Token: 0x040013DD RID: 5085
	public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits = new List<AmbienceManager.EmitterTypeLimit>();

	// Token: 0x040013DE RID: 5086
	public AmbienceManager.EmitterTypeLimit catchallEmitterLimit = new AmbienceManager.EmitterTypeLimit();

	// Token: 0x040013DF RID: 5087
	public int maxActiveLocalEmitters = 5;

	// Token: 0x040013E0 RID: 5088
	public int activeLocalEmitters;

	// Token: 0x040013E1 RID: 5089
	public List<AmbienceEmitter> cameraEmitters = new List<AmbienceEmitter>();

	// Token: 0x040013E2 RID: 5090
	public List<AmbienceEmitter> emittersInRange = new List<AmbienceEmitter>();

	// Token: 0x040013E3 RID: 5091
	public List<AmbienceEmitter> activeEmitters = new List<AmbienceEmitter>();

	// Token: 0x040013E4 RID: 5092
	public float localEmitterRange = 30f;

	// Token: 0x040013E5 RID: 5093
	public List<AmbienceZone> currentAmbienceZones = new List<AmbienceZone>();

	// Token: 0x040013E6 RID: 5094
	public bool isUnderwater;

	// Token: 0x02000C86 RID: 3206
	[Serializable]
	public class EmitterTypeLimit
	{
		// Token: 0x04004400 RID: 17408
		public List<AmbienceDefinitionList> ambience;

		// Token: 0x04004401 RID: 17409
		public int limit = 1;

		// Token: 0x04004402 RID: 17410
		public int active;
	}
}
