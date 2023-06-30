using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006CD RID: 1741
public class GenerateRailTerrain : ProceduralComponent
{
	// Token: 0x060031FA RID: 12794 RVA: 0x00130320 File Offset: 0x0012E520
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Func<int, float> func = (int i) => Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 8f, (float)i));
		for (int l = 0; l < 8; l++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
			{
				PathInterpolator path = pathList.Path;
				Vector3[] points = path.Points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector3 vector = points[j];
					float num = (pathList.Start ? func(j) : 1f);
					vector.y = Mathf.SmoothStep(vector.y, heightMap.GetHeight(vector), num);
					points[j] = vector;
				}
				path.Smoothen(8, Vector3.up, pathList.Start ? func : null);
				path.RecalculateTangents();
				heightMap.Push();
				float num2 = 1f;
				float num3 = Mathf.InverseLerp(8f, 0f, (float)l);
				pathList.AdjustTerrainHeight(num2, num3);
				heightMap.Pop();
			}
		}
		foreach (PathList pathList2 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path2 = pathList2.Path;
			Vector3[] points2 = path2.Points;
			for (int k = 0; k < points2.Length; k++)
			{
				Vector3 vector2 = points2[k];
				float num4 = (pathList2.Start ? func(k) : 1f);
				vector2.y = Mathf.SmoothStep(vector2.y, heightMap.GetHeight(vector2), num4);
				points2[k] = vector2;
			}
			path2.RecalculateTangents();
		}
	}

	// Token: 0x040028B1 RID: 10417
	public const int SmoothenLoops = 8;

	// Token: 0x040028B2 RID: 10418
	public const int SmoothenIterations = 8;

	// Token: 0x040028B3 RID: 10419
	public const int SmoothenY = 64;

	// Token: 0x040028B4 RID: 10420
	public const int SmoothenXZ = 32;

	// Token: 0x040028B5 RID: 10421
	public const int TransitionSteps = 8;
}
