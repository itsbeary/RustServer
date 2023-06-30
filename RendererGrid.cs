using System;

// Token: 0x02000533 RID: 1331
public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
	// Token: 0x0400224B RID: 8779
	public static bool Paused;

	// Token: 0x0400224C RID: 8780
	public GameObjectRef BatchPrefab;

	// Token: 0x0400224D RID: 8781
	public float CellSize = 50f;

	// Token: 0x0400224E RID: 8782
	public float MaxMilliseconds = 0.1f;

	// Token: 0x0400224F RID: 8783
	public const float MinTimeBetweenRefreshes = 1f;
}
