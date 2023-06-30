using System;
using UnityEngine.Rendering;

// Token: 0x02000510 RID: 1296
public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
	// Token: 0x040021A3 RID: 8611
	public static bool Paused;

	// Token: 0x040021A4 RID: 8612
	public GameObjectRef BatchPrefab;

	// Token: 0x040021A5 RID: 8613
	public float CellSize = 50f;

	// Token: 0x040021A6 RID: 8614
	public LayerSelect FoliageLayer = 0;

	// Token: 0x040021A7 RID: 8615
	public ShadowCastingMode FoliageShadows;
}
