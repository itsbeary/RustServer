using System;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	// Token: 0x0600325E RID: 12894 RVA: 0x00136BB8 File Offset: 0x00134DB8
	public static int GetHeightMapRes()
	{
		return Mathf.Min(4096, Mathf.ClosestPowerOfTwo((int)(World.Size * 1f))) + 1;
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x00136BD9 File Offset: 0x00134DD9
	public static int GetSplatMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.5f)));
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x00136BF8 File Offset: 0x00134DF8
	public static int GetBaseMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.01f)));
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x00136C17 File Offset: 0x00134E17
	public GameObject CreateTerrain()
	{
		return this.CreateTerrain(TerrainGenerator.GetHeightMapRes(), TerrainGenerator.GetSplatMapRes());
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x00136C2C File Offset: 0x00134E2C
	public GameObject CreateTerrain(int heightmapResolution, int alphamapResolution)
	{
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData
		{
			baseMapResolution = TerrainGenerator.GetBaseMapRes(),
			heightmapResolution = heightmapResolution,
			alphamapResolution = alphamapResolution,
			size = new Vector3(World.Size, 1000f, World.Size)
		}).GetComponent<Terrain>();
		component.transform.position = base.transform.position + new Vector3((float)(-(float)((ulong)World.Size)) * 0.5f, 0f, (float)(-(float)((ulong)World.Size)) * 0.5f);
		component.drawInstanced = false;
		component.castShadows = this.config.CastShadows;
		component.materialType = Terrain.MaterialType.Custom;
		component.materialTemplate = this.config.Material;
		component.gameObject.tag = base.gameObject.tag;
		component.gameObject.layer = base.gameObject.layer;
		component.gameObject.GetComponent<TerrainCollider>().sharedMaterial = this.config.GenericMaterial;
		TerrainMeta terrainMeta = component.gameObject.AddComponent<TerrainMeta>();
		component.gameObject.AddComponent<TerrainPhysics>();
		component.gameObject.AddComponent<TerrainColors>();
		component.gameObject.AddComponent<TerrainCollision>();
		component.gameObject.AddComponent<TerrainBiomeMap>();
		component.gameObject.AddComponent<TerrainAlphaMap>();
		component.gameObject.AddComponent<TerrainHeightMap>();
		component.gameObject.AddComponent<TerrainSplatMap>();
		component.gameObject.AddComponent<TerrainTopologyMap>();
		component.gameObject.AddComponent<TerrainWaterMap>();
		component.gameObject.AddComponent<TerrainPlacementMap>();
		component.gameObject.AddComponent<TerrainPath>();
		component.gameObject.AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = this.config;
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		return component.gameObject;
	}

	// Token: 0x04002943 RID: 10563
	public TerrainConfig config;

	// Token: 0x04002944 RID: 10564
	private const float HeightMapRes = 1f;

	// Token: 0x04002945 RID: 10565
	private const float SplatMapRes = 0.5f;

	// Token: 0x04002946 RID: 10566
	private const float BaseMapRes = 0.01f;
}
