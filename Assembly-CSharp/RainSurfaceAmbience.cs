using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class RainSurfaceAmbience : SingletonComponent<RainSurfaceAmbience>, IClientComponent
{
	// Token: 0x040013F1 RID: 5105
	public List<RainSurfaceAmbience.SurfaceSound> surfaces = new List<RainSurfaceAmbience.SurfaceSound>();

	// Token: 0x040013F2 RID: 5106
	public GameObjectRef emitterPrefab;

	// Token: 0x040013F3 RID: 5107
	public Dictionary<ParticlePatch, AmbienceEmitter> spawnedEmitters = new Dictionary<ParticlePatch, AmbienceEmitter>();

	// Token: 0x02000C87 RID: 3207
	[Serializable]
	public class SurfaceSound
	{
		// Token: 0x04004403 RID: 17411
		public AmbienceDefinitionList baseAmbience;

		// Token: 0x04004404 RID: 17412
		public List<PhysicMaterial> materials = new List<PhysicMaterial>();
	}
}
