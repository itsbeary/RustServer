using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class FoliageGridBenchmarkScene : BenchmarkScene
{
	// Token: 0x04000025 RID: 37
	private static TerrainMeta terrainMeta;

	// Token: 0x04000026 RID: 38
	public GameObjectRef foliagePrefab;

	// Token: 0x04000027 RID: 39
	private GameObject foliageInstance;

	// Token: 0x04000028 RID: 40
	public GameObjectRef lodPrefab;

	// Token: 0x04000029 RID: 41
	private GameObject lodInstance;

	// Token: 0x0400002A RID: 42
	public GameObjectRef batchingPrefab;

	// Token: 0x0400002B RID: 43
	private GameObject batchingInstance;

	// Token: 0x0400002C RID: 44
	public Terrain terrain;

	// Token: 0x0400002D RID: 45
	public Transform viewpointA;

	// Token: 0x0400002E RID: 46
	public Transform viewpointB;

	// Token: 0x0400002F RID: 47
	public bool moveVantangePoint = true;
}
