using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006E0 RID: 1760
public class PlaceCliffs : ProceduralComponent
{
	// Token: 0x0600322B RID: 12843 RVA: 0x00132A00 File Offset: 0x00130C00
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab[] array2 = array.Where((Prefab prefab) => prefab.Attribute.Find<DecorSocketMale>(prefab.ID) && prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)).ToArray<Prefab>();
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab[] array3 = array.Where((Prefab prefab) => prefab.Attribute.Find<DecorSocketMale>(prefab.ID)).ToArray<Prefab>();
		if (array3 == null || array3.Length == 0)
		{
			return;
		}
		Prefab[] array4 = array.Where((Prefab prefab) => prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)).ToArray<Prefab>();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		int num3 = Mathf.RoundToInt(size.x * size.z * 0.001f * (float)this.RetryMultiplier);
		for (int i = 0; i < num3; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num);
			float num5 = SeedRandom.Range(ref seed, z, num2);
			float num6 = TerrainMeta.NormalizeX(num4);
			float num7 = TerrainMeta.NormalizeZ(num5);
			float num8 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(num6, num7, true);
			Prefab random = array2.GetRandom(ref seed);
			if (factor * factor >= num8)
			{
				Vector3 normal = TerrainMeta.HeightMap.GetNormal(num6, num7);
				if (Vector3.Angle(Vector3.up, normal) >= (float)this.CutoffSlope)
				{
					float height = heightMap.GetHeight(num6, num7);
					Vector3 vector = new Vector3(num4, height, num5);
					Quaternion quaternion = QuaternionEx.LookRotationForcedUp(normal, Vector3.up);
					float num9 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
					for (float num10 = this.MaxScale; num10 >= this.MinScale; num10 -= num9)
					{
						Vector3 vector2 = vector;
						Quaternion quaternion2 = quaternion * random.Object.transform.localRotation;
						Vector3 vector3 = num10 * random.Object.transform.localScale;
						if (random.ApplyTerrainAnchors(ref vector2, quaternion2, vector3, null) && random.ApplyTerrainChecks(vector2, quaternion2, vector3, null) && random.ApplyTerrainFilters(vector2, quaternion2, vector3, null) && random.ApplyWaterChecks(vector2, quaternion2, vector3))
						{
							PlaceCliffs.CliffPlacement cliffPlacement = this.PlaceMale(array3, ref seed, random, vector2, quaternion2, vector3);
							PlaceCliffs.CliffPlacement cliffPlacement2 = this.PlaceFemale(array4, ref seed, random, vector2, quaternion2, vector3);
							World.AddPrefab("Decor", random, vector2, quaternion2, vector3);
							while (cliffPlacement != null)
							{
								if (cliffPlacement.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement.prefab, cliffPlacement.pos, cliffPlacement.rot, cliffPlacement.scale);
								cliffPlacement = cliffPlacement.next;
								i++;
							}
							while (cliffPlacement2 != null)
							{
								if (cliffPlacement2.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement2.prefab, cliffPlacement2.pos, cliffPlacement2.rot, cliffPlacement2.scale);
								cliffPlacement2 = cliffPlacement2.next;
								i++;
							}
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600322C RID: 12844 RVA: 0x00132D9C File Offset: 0x00130F9C
	private PlaceCliffs.CliffPlacement PlaceMale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketFemale, DecorSocketMale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x00132DBC File Offset: 0x00130FBC
	private PlaceCliffs.CliffPlacement PlaceFemale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketMale, DecorSocketFemale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x00132DDC File Offset: 0x00130FDC
	private PlaceCliffs.CliffPlacement Place<ParentSocketType, ChildSocketType>(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale, int parentAngle = 0, int parentCount = 0, int parentScore = 0) where ParentSocketType : PrefabAttribute where ChildSocketType : PrefabAttribute
	{
		PlaceCliffs.CliffPlacement cliffPlacement = null;
		if (parentAngle > 160 || parentAngle < -160)
		{
			return cliffPlacement;
		}
		int num = SeedRandom.Range(ref seed, 0, prefabs.Length);
		ParentSocketType parentSocketType = parentPrefab.Attribute.Find<ParentSocketType>(parentPrefab.ID);
		Vector3 vector = parentPos + parentRot * Vector3.Scale(parentSocketType.worldPosition, parentScale);
		float num2 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
		for (int i = 0; i < prefabs.Length; i++)
		{
			Prefab prefab = prefabs[(num + i) % prefabs.Length];
			if (prefab != parentPrefab)
			{
				ParentSocketType parentSocketType2 = prefab.Attribute.Find<ParentSocketType>(prefab.ID);
				ChildSocketType childSocketType = prefab.Attribute.Find<ChildSocketType>(prefab.ID);
				bool flag = parentSocketType2 != null;
				if (cliffPlacement == null || cliffPlacement.count <= PlaceCliffs.target_count || cliffPlacement.score <= PlaceCliffs.target_length || !flag)
				{
					for (float num3 = this.MaxScale; num3 >= this.MinScale; num3 -= num2)
					{
						for (int j = PlaceCliffs.min_rotation; j <= PlaceCliffs.max_rotation; j += PlaceCliffs.rotation_delta)
						{
							for (int k = -1; k <= 1; k += 2)
							{
								Vector3[] array = PlaceCliffs.offsets;
								int l = 0;
								while (l < array.Length)
								{
									Vector3 vector2 = array[l];
									Vector3 vector3 = parentScale * num3;
									Quaternion quaternion = Quaternion.Euler(0f, (float)(k * j), 0f) * parentRot;
									Vector3 vector4 = vector - quaternion * (Vector3.Scale(childSocketType.worldPosition, vector3) + vector2);
									if (this.Filter.GetFactor(vector4, true) >= 0.5f && prefab.ApplyTerrainAnchors(ref vector4, quaternion, vector3, null) && prefab.ApplyTerrainChecks(vector4, quaternion, vector3, null) && prefab.ApplyTerrainFilters(vector4, quaternion, vector3, null) && prefab.ApplyWaterChecks(vector4, quaternion, vector3))
									{
										int num4 = parentAngle + j;
										int num5 = parentCount + 1;
										int num6 = parentScore + Mathf.CeilToInt(Vector3Ex.Distance2D(parentPos, vector4));
										PlaceCliffs.CliffPlacement cliffPlacement2 = null;
										if (flag)
										{
											cliffPlacement2 = this.Place<ParentSocketType, ChildSocketType>(prefabs, ref seed, prefab, vector4, quaternion, vector3, num4, num5, num6);
											if (cliffPlacement2 != null)
											{
												num5 = cliffPlacement2.count;
												num6 = cliffPlacement2.score;
											}
										}
										else
										{
											num6 *= 2;
										}
										if (cliffPlacement == null)
										{
											cliffPlacement = new PlaceCliffs.CliffPlacement();
										}
										if (cliffPlacement.score < num6)
										{
											cliffPlacement.next = cliffPlacement2;
											cliffPlacement.count = num5;
											cliffPlacement.score = num6;
											cliffPlacement.prefab = prefab;
											cliffPlacement.pos = vector4;
											cliffPlacement.rot = quaternion;
											cliffPlacement.scale = vector3;
											goto IL_2D3;
										}
										goto IL_2D3;
									}
									else
									{
										l++;
									}
								}
							}
						}
					}
				}
			}
			IL_2D3:;
		}
		return cliffPlacement;
	}

	// Token: 0x040028E8 RID: 10472
	public SpawnFilter Filter;

	// Token: 0x040028E9 RID: 10473
	public string ResourceFolder = string.Empty;

	// Token: 0x040028EA RID: 10474
	public int RetryMultiplier = 1;

	// Token: 0x040028EB RID: 10475
	public int CutoffSlope = 10;

	// Token: 0x040028EC RID: 10476
	public float MinScale = 1f;

	// Token: 0x040028ED RID: 10477
	public float MaxScale = 2f;

	// Token: 0x040028EE RID: 10478
	private static int target_count = 4;

	// Token: 0x040028EF RID: 10479
	private static int target_length = 0;

	// Token: 0x040028F0 RID: 10480
	private static float min_scale_delta = 0.1f;

	// Token: 0x040028F1 RID: 10481
	private static int max_scale_attempts = 10;

	// Token: 0x040028F2 RID: 10482
	private static int min_rotation = PlaceCliffs.rotation_delta;

	// Token: 0x040028F3 RID: 10483
	private static int max_rotation = 60;

	// Token: 0x040028F4 RID: 10484
	private static int rotation_delta = 10;

	// Token: 0x040028F5 RID: 10485
	private static float offset_c = 0f;

	// Token: 0x040028F6 RID: 10486
	private static float offset_l = -0.75f;

	// Token: 0x040028F7 RID: 10487
	private static float offset_r = 0.75f;

	// Token: 0x040028F8 RID: 10488
	private static Vector3[] offsets = new Vector3[]
	{
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_l, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_r, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_l),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_r)
	};

	// Token: 0x02000E29 RID: 3625
	private class CliffPlacement
	{
		// Token: 0x04004AAB RID: 19115
		public int count;

		// Token: 0x04004AAC RID: 19116
		public int score;

		// Token: 0x04004AAD RID: 19117
		public Prefab prefab;

		// Token: 0x04004AAE RID: 19118
		public Vector3 pos = Vector3.zero;

		// Token: 0x04004AAF RID: 19119
		public Quaternion rot = Quaternion.identity;

		// Token: 0x04004AB0 RID: 19120
		public Vector3 scale = Vector3.one;

		// Token: 0x04004AB1 RID: 19121
		public PlaceCliffs.CliffPlacement next;
	}
}
