using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000511 RID: 1297
[CreateAssetMenu(menuName = "Rust/Foliage Placement")]
public class FoliagePlacement : ScriptableObject
{
	// Token: 0x040021A8 RID: 8616
	[Header("Placement")]
	public float Density = 2f;

	// Token: 0x040021A9 RID: 8617
	[Header("Filter")]
	public SpawnFilter Filter;

	// Token: 0x040021AA RID: 8618
	[FormerlySerializedAs("Cutoff")]
	public float FilterCutoff = 0.5f;

	// Token: 0x040021AB RID: 8619
	public float FilterFade = 0.1f;

	// Token: 0x040021AC RID: 8620
	[FormerlySerializedAs("Scaling")]
	public float FilterScaling = 1f;

	// Token: 0x040021AD RID: 8621
	[Header("Randomization")]
	public float RandomScaling = 0.2f;

	// Token: 0x040021AE RID: 8622
	[Header("Placement Range")]
	[MinMax(0f, 1f)]
	public MinMax Range = new MinMax(0f, 1f);

	// Token: 0x040021AF RID: 8623
	public float RangeFade = 0.1f;

	// Token: 0x040021B0 RID: 8624
	[Header("LOD")]
	[Range(0f, 1f)]
	public float DistanceDensity;

	// Token: 0x040021B1 RID: 8625
	[Range(1f, 2f)]
	public float DistanceScaling = 2f;

	// Token: 0x040021B2 RID: 8626
	[Header("Visuals")]
	public Material material;

	// Token: 0x040021B3 RID: 8627
	[FormerlySerializedAs("mesh")]
	public Mesh mesh0;

	// Token: 0x040021B4 RID: 8628
	[FormerlySerializedAs("mesh")]
	public Mesh mesh1;

	// Token: 0x040021B5 RID: 8629
	[FormerlySerializedAs("mesh")]
	public Mesh mesh2;

	// Token: 0x040021B6 RID: 8630
	public const int lods = 5;

	// Token: 0x040021B7 RID: 8631
	public const int octaves = 1;

	// Token: 0x040021B8 RID: 8632
	public const float frequency = 0.05f;

	// Token: 0x040021B9 RID: 8633
	public const float amplitude = 0.5f;

	// Token: 0x040021BA RID: 8634
	public const float offset = 0.5f;
}
