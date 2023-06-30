using System;
using Rust;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	// Token: 0x0600318F RID: 12687 RVA: 0x000063A5 File Offset: 0x000045A5
	private void InitializeBasePyramid()
	{
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ReleaseBasePyramid()
	{
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateBasePyramid()
	{
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x000063A5 File Offset: 0x000045A5
	private void InitializeCoarseHeightSlope()
	{
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ReleaseCoarseHeightSlope()
	{
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateCoarseHeightSlope()
	{
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06003195 RID: 12693 RVA: 0x00128B10 File Offset: 0x00126D10
	public int ShoreMapSize
	{
		get
		{
			return this.shoreMapSize;
		}
	}

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x06003196 RID: 12694 RVA: 0x00128B18 File Offset: 0x00126D18
	public Vector3[] ShoreMap
	{
		get
		{
			return this.shoreVectors;
		}
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x00128B20 File Offset: 0x00126D20
	private void InitializeShoreVector()
	{
		int num = Mathf.ClosestPowerOfTwo(this.terrain.terrainData.heightmapResolution) >> 1;
		int num2 = num * num;
		this.terrainSize = Mathf.Max(this.terrain.terrainData.size.x, this.terrain.terrainData.size.z);
		this.shoreMapSize = num;
		this.shoreDistanceScale = this.terrainSize / (float)this.shoreMapSize;
		this.shoreDistances = new float[num * num];
		this.shoreVectors = new Vector3[num * num];
		for (int i = 0; i < num2; i++)
		{
			this.shoreDistances[i] = 10000f;
			this.shoreVectors[i] = Vector3.one;
		}
	}

	// Token: 0x06003198 RID: 12696 RVA: 0x00128BE0 File Offset: 0x00126DE0
	private void GenerateShoreVector()
	{
		using (TimeWarning.New("GenerateShoreVector", 500))
		{
			this.GenerateShoreVector(out this.shoreDistances, out this.shoreVectors);
		}
	}

	// Token: 0x06003199 RID: 12697 RVA: 0x00128C2C File Offset: 0x00126E2C
	private void ReleaseShoreVector()
	{
		this.shoreDistances = null;
		this.shoreVectors = null;
	}

	// Token: 0x0600319A RID: 12698 RVA: 0x00128C3C File Offset: 0x00126E3C
	private void GenerateShoreVector(out float[] distances, out Vector3[] vectors)
	{
		float num = this.terrainSize / (float)this.shoreMapSize;
		Vector3 position = this.terrain.GetPosition();
		int num2 = LayerMask.NameToLayer("Terrain");
		NativeArray<RaycastHit> nativeArray = new NativeArray<RaycastHit>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		NativeArray<RaycastCommand> nativeArray2 = new NativeArray<RaycastCommand>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		for (int i = 0; i < this.shoreMapSize; i++)
		{
			for (int j = 0; j < this.shoreMapSize; j++)
			{
				float num3 = ((float)j + 0.5f) * num;
				float num4 = ((float)i + 0.5f) * num;
				Vector3 vector = new Vector3(position.x, 0f, position.z) + new Vector3(num3, 1000f, num4);
				Vector3 down = Vector3.down;
				nativeArray2[i * this.shoreMapSize + j] = new RaycastCommand(vector, down, float.MaxValue, -5, 1);
			}
		}
		RaycastCommand.ScheduleBatch(nativeArray2, nativeArray, 1, default(JobHandle)).Complete();
		byte[] array = new byte[this.shoreMapSize * this.shoreMapSize];
		distances = new float[this.shoreMapSize * this.shoreMapSize];
		vectors = new Vector3[this.shoreMapSize * this.shoreMapSize];
		int k = 0;
		int num5 = 0;
		while (k < this.shoreMapSize)
		{
			int l = 0;
			while (l < this.shoreMapSize)
			{
				RaycastHit raycastHit = nativeArray[k * this.shoreMapSize + l];
				bool flag = raycastHit.collider.gameObject.layer == num2;
				if (flag && raycastHit.point.y <= 0f)
				{
					flag = false;
				}
				array[num5] = (flag ? byte.MaxValue : 0);
				distances[num5] = (float)(flag ? 256 : 0);
				l++;
				num5++;
			}
			k++;
		}
		byte b = 127;
		DistanceField.Generate(this.shoreMapSize, b, array, ref distances);
		DistanceField.ApplyGaussianBlur(this.shoreMapSize, distances, 1);
		DistanceField.GenerateVectors(this.shoreMapSize, distances, ref vectors);
		nativeArray.Dispose();
		nativeArray2.Dispose();
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x00128E74 File Offset: 0x00127074
	public float GetCoarseDistanceToShore(Vector3 pos)
	{
		Vector2 vector;
		vector.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		vector.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseDistanceToShore(vector);
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x00128ED0 File Offset: 0x001270D0
	public float GetCoarseDistanceToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = ((num3 < (float)num2) ? 1 : 0);
		int num10 = ((num4 < (float)num2) ? num : 0);
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		float num15 = this.shoreDistances[num11];
		float num16 = this.shoreDistances[num12];
		float num17 = this.shoreDistances[num13];
		float num18 = this.shoreDistances[num14];
		float num19 = (num16 - num15) * num7 + num15;
		return (((num18 - num17) * num7 + num17 - num19) * num8 + num19) * this.shoreDistanceScale;
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x00128FBC File Offset: 0x001271BC
	public Vector3 GetCoarseVectorToShore(Vector3 pos)
	{
		Vector2 vector;
		vector.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		vector.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseVectorToShore(vector);
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x00129018 File Offset: 0x00127218
	public Vector3 GetCoarseVectorToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = ((num3 < (float)num2) ? 1 : 0);
		int num10 = ((num4 < (float)num2) ? num : 0);
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		Vector3 vector = this.shoreVectors[num11];
		Vector3 vector2 = this.shoreVectors[num12];
		Vector3 vector3 = this.shoreVectors[num13];
		Vector3 vector4 = this.shoreVectors[num14];
		Vector3 vector5;
		vector5.x = (vector2.x - vector.x) * num7 + vector.x;
		vector5.y = (vector2.y - vector.y) * num7 + vector.y;
		vector5.z = (vector2.z - vector.z) * num7 + vector.z;
		Vector3 vector6;
		vector6.x = (vector4.x - vector3.x) * num7 + vector3.x;
		vector6.y = (vector4.y - vector3.y) * num7 + vector3.y;
		vector6.z = (vector4.z - vector3.z) * num7 + vector3.z;
		float num15 = (vector6.x - vector5.x) * num8 + vector5.x;
		float num16 = (vector6.y - vector5.y) * num8 + vector5.y;
		float num17 = (vector6.z - vector5.z) * num8 + vector5.z;
		return new Vector3(num15, num16, num17 * this.shoreDistanceScale);
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x0600319F RID: 12703 RVA: 0x00129217 File Offset: 0x00127417
	public static TerrainTexturing Instance
	{
		get
		{
			return TerrainTexturing.instance;
		}
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x0012921E File Offset: 0x0012741E
	private void CheckInstance()
	{
		TerrainTexturing.instance = ((TerrainTexturing.instance != null) ? TerrainTexturing.instance : this);
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x0012923A File Offset: 0x0012743A
	private void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x00129242 File Offset: 0x00127442
	public override void Setup()
	{
		this.InitializeShoreVector();
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x0012924C File Offset: 0x0012744C
	public override void PostSetup()
	{
		TerrainMeta component = base.GetComponent<TerrainMeta>();
		if (component == null || component.config == null)
		{
			Debug.LogError("[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		this.Shutdown();
		this.InitializeCoarseHeightSlope();
		this.GenerateShoreVector();
		this.initialized = true;
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x0012929B File Offset: 0x0012749B
	private void Shutdown()
	{
		this.ReleaseBasePyramid();
		this.ReleaseCoarseHeightSlope();
		this.ReleaseShoreVector();
		this.initialized = false;
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x0012923A File Offset: 0x0012743A
	private void OnEnable()
	{
		this.CheckInstance();
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x001292B6 File Offset: 0x001274B6
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x001292C6 File Offset: 0x001274C6
	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		this.UpdateBasePyramid();
		this.UpdateCoarseHeightSlope();
	}

	// Token: 0x04002849 RID: 10313
	private const int ShoreVectorDownscale = 1;

	// Token: 0x0400284A RID: 10314
	private const int ShoreVectorBlurPasses = 1;

	// Token: 0x0400284B RID: 10315
	private float terrainSize;

	// Token: 0x0400284C RID: 10316
	private int shoreMapSize;

	// Token: 0x0400284D RID: 10317
	private float shoreDistanceScale;

	// Token: 0x0400284E RID: 10318
	private float[] shoreDistances;

	// Token: 0x0400284F RID: 10319
	private Vector3[] shoreVectors;

	// Token: 0x04002850 RID: 10320
	public bool debugFoliageDisplacement;

	// Token: 0x04002851 RID: 10321
	private bool initialized;

	// Token: 0x04002852 RID: 10322
	private static TerrainTexturing instance;
}
