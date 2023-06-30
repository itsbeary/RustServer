using System;
using UnityEngine;

// Token: 0x020006AE RID: 1710
[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06003135 RID: 12597 RVA: 0x001273B9 File Offset: 0x001255B9
	// (set) Token: 0x06003136 RID: 12598 RVA: 0x001273C0 File Offset: 0x001255C0
	public static TerrainConfig Config { get; private set; }

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06003137 RID: 12599 RVA: 0x001273C8 File Offset: 0x001255C8
	// (set) Token: 0x06003138 RID: 12600 RVA: 0x001273CF File Offset: 0x001255CF
	public static Terrain Terrain { get; private set; }

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06003139 RID: 12601 RVA: 0x001273D7 File Offset: 0x001255D7
	// (set) Token: 0x0600313A RID: 12602 RVA: 0x001273DE File Offset: 0x001255DE
	public static Transform Transform { get; private set; }

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x0600313B RID: 12603 RVA: 0x001273E6 File Offset: 0x001255E6
	// (set) Token: 0x0600313C RID: 12604 RVA: 0x001273ED File Offset: 0x001255ED
	public static Vector3 Position { get; private set; }

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x0600313D RID: 12605 RVA: 0x001273F5 File Offset: 0x001255F5
	// (set) Token: 0x0600313E RID: 12606 RVA: 0x001273FC File Offset: 0x001255FC
	public static Vector3 Size { get; private set; }

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x0600313F RID: 12607 RVA: 0x00127404 File Offset: 0x00125604
	public static Vector3 Center
	{
		get
		{
			return TerrainMeta.Position + TerrainMeta.Size * 0.5f;
		}
	}

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x06003140 RID: 12608 RVA: 0x0012741F File Offset: 0x0012561F
	// (set) Token: 0x06003141 RID: 12609 RVA: 0x00127426 File Offset: 0x00125626
	public static Vector3 OneOverSize { get; private set; }

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x06003142 RID: 12610 RVA: 0x0012742E File Offset: 0x0012562E
	// (set) Token: 0x06003143 RID: 12611 RVA: 0x00127435 File Offset: 0x00125635
	public static Vector3 HighestPoint { get; set; }

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x06003144 RID: 12612 RVA: 0x0012743D File Offset: 0x0012563D
	// (set) Token: 0x06003145 RID: 12613 RVA: 0x00127444 File Offset: 0x00125644
	public static Vector3 LowestPoint { get; set; }

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06003146 RID: 12614 RVA: 0x0012744C File Offset: 0x0012564C
	// (set) Token: 0x06003147 RID: 12615 RVA: 0x00127453 File Offset: 0x00125653
	public static float LootAxisAngle { get; private set; }

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06003148 RID: 12616 RVA: 0x0012745B File Offset: 0x0012565B
	// (set) Token: 0x06003149 RID: 12617 RVA: 0x00127462 File Offset: 0x00125662
	public static float BiomeAxisAngle { get; private set; }

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x0600314A RID: 12618 RVA: 0x0012746A File Offset: 0x0012566A
	// (set) Token: 0x0600314B RID: 12619 RVA: 0x00127471 File Offset: 0x00125671
	public static TerrainData Data { get; private set; }

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x0600314C RID: 12620 RVA: 0x00127479 File Offset: 0x00125679
	// (set) Token: 0x0600314D RID: 12621 RVA: 0x00127480 File Offset: 0x00125680
	public static TerrainCollider Collider { get; private set; }

	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x0600314E RID: 12622 RVA: 0x00127488 File Offset: 0x00125688
	// (set) Token: 0x0600314F RID: 12623 RVA: 0x0012748F File Offset: 0x0012568F
	public static TerrainCollision Collision { get; private set; }

	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x06003150 RID: 12624 RVA: 0x00127497 File Offset: 0x00125697
	// (set) Token: 0x06003151 RID: 12625 RVA: 0x0012749E File Offset: 0x0012569E
	public static TerrainPhysics Physics { get; private set; }

	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06003152 RID: 12626 RVA: 0x001274A6 File Offset: 0x001256A6
	// (set) Token: 0x06003153 RID: 12627 RVA: 0x001274AD File Offset: 0x001256AD
	public static TerrainColors Colors { get; private set; }

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x06003154 RID: 12628 RVA: 0x001274B5 File Offset: 0x001256B5
	// (set) Token: 0x06003155 RID: 12629 RVA: 0x001274BC File Offset: 0x001256BC
	public static TerrainQuality Quality { get; private set; }

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x06003156 RID: 12630 RVA: 0x001274C4 File Offset: 0x001256C4
	// (set) Token: 0x06003157 RID: 12631 RVA: 0x001274CB File Offset: 0x001256CB
	public static TerrainPath Path { get; private set; }

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06003158 RID: 12632 RVA: 0x001274D3 File Offset: 0x001256D3
	// (set) Token: 0x06003159 RID: 12633 RVA: 0x001274DA File Offset: 0x001256DA
	public static TerrainBiomeMap BiomeMap { get; private set; }

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x0600315A RID: 12634 RVA: 0x001274E2 File Offset: 0x001256E2
	// (set) Token: 0x0600315B RID: 12635 RVA: 0x001274E9 File Offset: 0x001256E9
	public static TerrainAlphaMap AlphaMap { get; private set; }

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x0600315C RID: 12636 RVA: 0x001274F1 File Offset: 0x001256F1
	// (set) Token: 0x0600315D RID: 12637 RVA: 0x001274F8 File Offset: 0x001256F8
	public static TerrainBlendMap BlendMap { get; private set; }

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x0600315E RID: 12638 RVA: 0x00127500 File Offset: 0x00125700
	// (set) Token: 0x0600315F RID: 12639 RVA: 0x00127507 File Offset: 0x00125707
	public static TerrainHeightMap HeightMap { get; private set; }

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x06003160 RID: 12640 RVA: 0x0012750F File Offset: 0x0012570F
	// (set) Token: 0x06003161 RID: 12641 RVA: 0x00127516 File Offset: 0x00125716
	public static TerrainSplatMap SplatMap { get; private set; }

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06003162 RID: 12642 RVA: 0x0012751E File Offset: 0x0012571E
	// (set) Token: 0x06003163 RID: 12643 RVA: 0x00127525 File Offset: 0x00125725
	public static TerrainTopologyMap TopologyMap { get; private set; }

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x06003164 RID: 12644 RVA: 0x0012752D File Offset: 0x0012572D
	// (set) Token: 0x06003165 RID: 12645 RVA: 0x00127534 File Offset: 0x00125734
	public static TerrainWaterMap WaterMap { get; private set; }

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06003166 RID: 12646 RVA: 0x0012753C File Offset: 0x0012573C
	// (set) Token: 0x06003167 RID: 12647 RVA: 0x00127543 File Offset: 0x00125743
	public static TerrainDistanceMap DistanceMap { get; private set; }

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06003168 RID: 12648 RVA: 0x0012754B File Offset: 0x0012574B
	// (set) Token: 0x06003169 RID: 12649 RVA: 0x00127552 File Offset: 0x00125752
	public static TerrainPlacementMap PlacementMap { get; private set; }

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x0600316A RID: 12650 RVA: 0x0012755A File Offset: 0x0012575A
	// (set) Token: 0x0600316B RID: 12651 RVA: 0x00127561 File Offset: 0x00125761
	public static TerrainTexturing Texturing { get; private set; }

	// Token: 0x0600316C RID: 12652 RVA: 0x0012756C File Offset: 0x0012576C
	public static bool OutOfBounds(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x || worldPos.z < TerrainMeta.Position.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z;
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x001275E0 File Offset: 0x001257E0
	public static bool OutOfMargin(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x || worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z;
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x00127680 File Offset: 0x00125880
	public static Vector3 RandomPointOffshore()
	{
		float num = UnityEngine.Random.Range(-1f, 1f);
		float num2 = UnityEngine.Random.Range(0f, 100f);
		Vector3 vector = new Vector3(Mathf.Min(TerrainMeta.Size.x, 4000f) - 100f, 0f, Mathf.Min(TerrainMeta.Size.z, 4000f) - 100f);
		if (num2 < 25f)
		{
			return TerrainMeta.Center + new Vector3(-vector.x, 0f, num * vector.z);
		}
		if (num2 < 50f)
		{
			return TerrainMeta.Center + new Vector3(vector.x, 0f, num * vector.z);
		}
		if (num2 < 75f)
		{
			return TerrainMeta.Center + new Vector3(num * vector.x, 0f, -vector.z);
		}
		return TerrainMeta.Center + new Vector3(num * vector.x, 0f, vector.z);
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x00127794 File Offset: 0x00125994
	public static Vector3 Normalize(Vector3 worldPos)
	{
		float num = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float num2 = (worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
		float num3 = (worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return new Vector3(num, num2, num3);
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x001277FE File Offset: 0x001259FE
	public static float NormalizeX(float x)
	{
		return (x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x00127817 File Offset: 0x00125A17
	public static float NormalizeY(float y)
	{
		return (y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
	}

	// Token: 0x06003172 RID: 12658 RVA: 0x00127830 File Offset: 0x00125A30
	public static float NormalizeZ(float z)
	{
		return (z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x0012784C File Offset: 0x00125A4C
	public static Vector3 Denormalize(Vector3 normPos)
	{
		float num = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
		float num2 = TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y;
		float num3 = TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z;
		return new Vector3(num, num2, num3);
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x001278B6 File Offset: 0x00125AB6
	public static float DenormalizeX(float normX)
	{
		return TerrainMeta.Position.x + normX * TerrainMeta.Size.x;
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x001278CF File Offset: 0x00125ACF
	public static float DenormalizeY(float normY)
	{
		return TerrainMeta.Position.y + normY * TerrainMeta.Size.y;
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x001278E8 File Offset: 0x00125AE8
	public static float DenormalizeZ(float normZ)
	{
		return TerrainMeta.Position.z + normZ * TerrainMeta.Size.z;
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x00127901 File Offset: 0x00125B01
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Shader.DisableKeyword("TERRAIN_PAINTING");
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x00127918 File Offset: 0x00125B18
	public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		if (terrainOverride != null)
		{
			this.terrain = terrainOverride;
		}
		if (configOverride != null)
		{
			this.config = configOverride;
		}
		TerrainMeta.Terrain = this.terrain;
		TerrainMeta.Config = this.config;
		TerrainMeta.Transform = this.terrain.transform;
		TerrainMeta.Data = this.terrain.terrainData;
		TerrainMeta.Size = this.terrain.terrainData.size;
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = this.terrain.GetPosition();
		TerrainMeta.Collider = this.terrain.GetComponent<TerrainCollider>();
		TerrainMeta.Collision = this.terrain.GetComponent<TerrainCollision>();
		TerrainMeta.Physics = this.terrain.GetComponent<TerrainPhysics>();
		TerrainMeta.Colors = this.terrain.GetComponent<TerrainColors>();
		TerrainMeta.Quality = this.terrain.GetComponent<TerrainQuality>();
		TerrainMeta.Path = this.terrain.GetComponent<TerrainPath>();
		TerrainMeta.BiomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
		TerrainMeta.AlphaMap = this.terrain.GetComponent<TerrainAlphaMap>();
		TerrainMeta.BlendMap = this.terrain.GetComponent<TerrainBlendMap>();
		TerrainMeta.HeightMap = this.terrain.GetComponent<TerrainHeightMap>();
		TerrainMeta.SplatMap = this.terrain.GetComponent<TerrainSplatMap>();
		TerrainMeta.TopologyMap = this.terrain.GetComponent<TerrainTopologyMap>();
		TerrainMeta.WaterMap = this.terrain.GetComponent<TerrainWaterMap>();
		TerrainMeta.DistanceMap = this.terrain.GetComponent<TerrainDistanceMap>();
		TerrainMeta.PlacementMap = this.terrain.GetComponent<TerrainPlacementMap>();
		TerrainMeta.Texturing = this.terrain.GetComponent<TerrainTexturing>();
		this.terrain.drawInstanced = false;
		TerrainMeta.HighestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y + TerrainMeta.Size.y, TerrainMeta.Position.z);
		TerrainMeta.LowestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y, TerrainMeta.Position.z);
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init(this.terrain, this.config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num2 = SeedRandom.Range(ref seed, -45, 46);
		int num3 = SeedRandom.Sign(ref seed);
		TerrainMeta.LootAxisAngle = (float)num;
		TerrainMeta.BiomeAxisAngle = (float)(num + num2 + num3 * 90);
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x00127B7E File Offset: 0x00125D7E
	public static void InitNoTerrain(bool createPath = false)
	{
		TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = -0.5f * TerrainMeta.Size;
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x00127BBC File Offset: 0x00125DBC
	public void SetupComponents()
	{
		foreach (TerrainExtension terrainExtension in base.GetComponents<TerrainExtension>())
		{
			terrainExtension.Setup();
			terrainExtension.isInitialized = true;
		}
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x00127BF0 File Offset: 0x00125DF0
	public void PostSetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PostSetup();
		}
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x00127C1C File Offset: 0x00125E1C
	public void BindShaderProperties()
	{
		if (this.config)
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling()));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist));
			Shader.SetGlobalVector("Splat1_UVMIX", new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist));
			Shader.SetGlobalVector("Splat2_UVMIX", new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist));
			Shader.SetGlobalVector("Splat3_UVMIX", new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist));
			Shader.SetGlobalVector("Splat4_UVMIX", new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist));
			Shader.SetGlobalVector("Splat5_UVMIX", new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist));
			Shader.SetGlobalVector("Splat6_UVMIX", new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist));
			Shader.SetGlobalVector("Splat7_UVMIX", new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist));
		}
		if (TerrainMeta.HeightMap)
		{
			Shader.SetGlobalTexture("Terrain_Normal", TerrainMeta.HeightMap.NormalTexture);
		}
		if (TerrainMeta.AlphaMap)
		{
			Shader.SetGlobalTexture("Terrain_Alpha", TerrainMeta.AlphaMap.AlphaTexture);
		}
		if (TerrainMeta.BiomeMap)
		{
			Shader.SetGlobalTexture("Terrain_Biome", TerrainMeta.BiomeMap.BiomeTexture);
		}
		if (TerrainMeta.SplatMap)
		{
			Shader.SetGlobalTexture("Terrain_Control0", TerrainMeta.SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", TerrainMeta.SplatMap.SplatTexture1);
		}
		TerrainMeta.WaterMap;
		if (TerrainMeta.DistanceMap)
		{
			Shader.SetGlobalTexture("Terrain_Distance", TerrainMeta.DistanceMap.DistanceTexture);
		}
		if (this.terrain)
		{
			Shader.SetGlobalVector("Terrain_Position", TerrainMeta.Position);
			Shader.SetGlobalVector("Terrain_Size", TerrainMeta.Size);
			Shader.SetGlobalVector("Terrain_RcpSize", TerrainMeta.OneOverSize);
			if (this.terrain.materialTemplate)
			{
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_BLEND_LINEAR");
				}
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_VERTEX_NORMALS");
				}
			}
		}
	}

	// Token: 0x04002810 RID: 10256
	public Terrain terrain;

	// Token: 0x04002811 RID: 10257
	public TerrainConfig config;

	// Token: 0x04002812 RID: 10258
	public TerrainMeta.PaintMode paint;

	// Token: 0x04002813 RID: 10259
	[HideInInspector]
	public TerrainMeta.PaintMode currentPaintMode;

	// Token: 0x02000DEF RID: 3567
	public enum PaintMode
	{
		// Token: 0x04004A01 RID: 18945
		None,
		// Token: 0x04004A02 RID: 18946
		Splats,
		// Token: 0x04004A03 RID: 18947
		Biomes,
		// Token: 0x04004A04 RID: 18948
		Alpha,
		// Token: 0x04004A05 RID: 18949
		Blend,
		// Token: 0x04004A06 RID: 18950
		Field,
		// Token: 0x04004A07 RID: 18951
		Cliff,
		// Token: 0x04004A08 RID: 18952
		Summit,
		// Token: 0x04004A09 RID: 18953
		Beachside,
		// Token: 0x04004A0A RID: 18954
		Beach,
		// Token: 0x04004A0B RID: 18955
		Forest,
		// Token: 0x04004A0C RID: 18956
		Forestside,
		// Token: 0x04004A0D RID: 18957
		Ocean,
		// Token: 0x04004A0E RID: 18958
		Oceanside,
		// Token: 0x04004A0F RID: 18959
		Decor,
		// Token: 0x04004A10 RID: 18960
		Monument,
		// Token: 0x04004A11 RID: 18961
		Road,
		// Token: 0x04004A12 RID: 18962
		Roadside,
		// Token: 0x04004A13 RID: 18963
		Bridge,
		// Token: 0x04004A14 RID: 18964
		River,
		// Token: 0x04004A15 RID: 18965
		Riverside,
		// Token: 0x04004A16 RID: 18966
		Lake,
		// Token: 0x04004A17 RID: 18967
		Lakeside,
		// Token: 0x04004A18 RID: 18968
		Offshore,
		// Token: 0x04004A19 RID: 18969
		Rail,
		// Token: 0x04004A1A RID: 18970
		Railside,
		// Token: 0x04004A1B RID: 18971
		Building,
		// Token: 0x04004A1C RID: 18972
		Cliffside,
		// Token: 0x04004A1D RID: 18973
		Mountain,
		// Token: 0x04004A1E RID: 18974
		Clutter,
		// Token: 0x04004A1F RID: 18975
		Alt,
		// Token: 0x04004A20 RID: 18976
		Tier0,
		// Token: 0x04004A21 RID: 18977
		Tier1,
		// Token: 0x04004A22 RID: 18978
		Tier2,
		// Token: 0x04004A23 RID: 18979
		Mainland,
		// Token: 0x04004A24 RID: 18980
		Hilltop
	}
}
