using System;

// Token: 0x0200053D RID: 1341
public class LODGrid : SingletonComponent<LODGrid>, IClientComponent
{
	// Token: 0x04002265 RID: 8805
	public static bool Paused = false;

	// Token: 0x04002266 RID: 8806
	public float CellSize = 50f;

	// Token: 0x04002267 RID: 8807
	public float MaxMilliseconds = 0.1f;

	// Token: 0x04002268 RID: 8808
	public const float MaxRefreshDistance = 500f;

	// Token: 0x04002269 RID: 8809
	public static float TreeMeshDistance = 500f;

	// Token: 0x0400226A RID: 8810
	public const float MinTimeBetweenRefreshes = 1f;
}
