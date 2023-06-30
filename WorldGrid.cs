using System;

// Token: 0x020005BE RID: 1470
public class WorldGrid : SingletonComponent<WorldGrid>, IClientComponent
{
	// Token: 0x04002420 RID: 9248
	public static bool Paused;

	// Token: 0x04002421 RID: 9249
	public float CellSize = 50f;

	// Token: 0x04002422 RID: 9250
	public float MaxMilliseconds = 0.1f;

	// Token: 0x04002423 RID: 9251
	public const float MaxRefreshDistance = 500f;

	// Token: 0x04002424 RID: 9252
	public const float MinTimeBetweenRefreshes = 1f;
}
