using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000737 RID: 1847
[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	// Token: 0x06003354 RID: 13140 RVA: 0x0013A186 File Offset: 0x00138386
	private void CreateMeshes()
	{
		if (this.blitMesh == null)
		{
			this.blitMesh = ReflectionProbeEx.CreateBlitMesh();
		}
		if (this.skyboxMesh == null)
		{
			this.skyboxMesh = ReflectionProbeEx.CreateSkyboxMesh();
		}
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x0013A1BC File Offset: 0x001383BC
	private void DestroyMeshes()
	{
		if (this.blitMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitMesh);
			this.blitMesh = null;
		}
		if (this.skyboxMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.skyboxMesh);
			this.skyboxMesh = null;
		}
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x0013A20C File Offset: 0x0013840C
	private static Mesh CreateBlitMesh()
	{
		return new Mesh
		{
			vertices = new Vector3[]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			},
			uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			},
			triangles = new int[] { 0, 1, 2, 0, 2, 3 }
		};
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x0013A314 File Offset: 0x00138514
	private static ReflectionProbeEx.CubemapSkyboxVertex SubDivVert(ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2)
	{
		Vector3 vector = new Vector3(v1.x, v1.y, v1.z);
		Vector3 vector2 = new Vector3(v2.x, v2.y, v2.z);
		Vector3 vector3 = Vector3.Normalize(Vector3.Lerp(vector, vector2, 0.5f));
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
		cubemapSkyboxVertex.x = (cubemapSkyboxVertex.tu = vector3.x);
		cubemapSkyboxVertex.y = (cubemapSkyboxVertex.tv = vector3.y);
		cubemapSkyboxVertex.z = (cubemapSkyboxVertex.tw = vector3.z);
		cubemapSkyboxVertex.color = Color.white;
		return cubemapSkyboxVertex;
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x0013A3B4 File Offset: 0x001385B4
	private static void Subdivide(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = ReflectionProbeEx.SubDivVert(v1, v2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2 = ReflectionProbeEx.SubDivVert(v2, v3);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3 = ReflectionProbeEx.SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v2);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v3);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex2);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x0013A430 File Offset: 0x00138630
	private static void SubdivideYOnly(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		float num = Mathf.Abs(v2.y - v1.y);
		float num2 = Mathf.Abs(v2.y - v3.y);
		float num3 = Mathf.Abs(v3.y - v1.y);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3;
		if (num < num2 && num < num3)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex2 = v1;
			cubemapSkyboxVertex3 = v2;
		}
		else if (num2 < num && num2 < num3)
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex2 = v2;
			cubemapSkyboxVertex3 = v3;
		}
		else
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex2 = v3;
			cubemapSkyboxVertex3 = v1;
		}
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex4 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex5 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex5);
		Vector3 vector = new Vector3(cubemapSkyboxVertex5.x - cubemapSkyboxVertex2.x, cubemapSkyboxVertex5.y - cubemapSkyboxVertex2.y, cubemapSkyboxVertex5.z - cubemapSkyboxVertex2.z);
		Vector3 vector2 = new Vector3(cubemapSkyboxVertex4.x - cubemapSkyboxVertex3.x, cubemapSkyboxVertex4.y - cubemapSkyboxVertex3.y, cubemapSkyboxVertex4.z - cubemapSkyboxVertex3.z);
		if (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z > vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z)
		{
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(cubemapSkyboxVertex5);
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex3);
			return;
		}
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex3);
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x0013A5F8 File Offset: 0x001387F8
	private static Mesh CreateSkyboxMesh()
	{
		List<ReflectionProbeEx.CubemapSkyboxVertex> list = new List<ReflectionProbeEx.CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = default(ReflectionProbeEx.CubemapSkyboxVertex);
			Vector3 vector = Vector3.Normalize(new Vector3(ReflectionProbeEx.octaVerts[i * 3], ReflectionProbeEx.octaVerts[i * 3 + 1], ReflectionProbeEx.octaVerts[i * 3 + 2]));
			cubemapSkyboxVertex.x = (cubemapSkyboxVertex.tu = vector.x);
			cubemapSkyboxVertex.y = (cubemapSkyboxVertex.tv = vector.y);
			cubemapSkyboxVertex.z = (cubemapSkyboxVertex.tw = vector.z);
			cubemapSkyboxVertex.color = Color.white;
			list.Add(cubemapSkyboxVertex);
		}
		for (int j = 0; j < 3; j++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list2 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list2.AddRange(list);
			int count = list2.Count;
			list.Clear();
			list.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				ReflectionProbeEx.Subdivide(list, list2[k], list2[k + 1], list2[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list3 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list3.AddRange(list);
			int count2 = list3.Count;
			float num = Mathf.Pow(0.5f, (float)l + 1f);
			list.Clear();
			list.Capacity = count2 * 4;
			for (int m = 0; m < count2; m += 3)
			{
				if (Mathf.Max(Mathf.Max(Mathf.Abs(list3[m].y), Mathf.Abs(list3[m + 1].y)), Mathf.Abs(list3[m + 2].y)) > num)
				{
					list.Add(list3[m]);
					list.Add(list3[m + 1]);
					list.Add(list3[m + 2]);
				}
				else
				{
					ReflectionProbeEx.SubdivideYOnly(list, list3[m], list3[m + 1], list3[m + 2]);
				}
			}
		}
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[list.Count];
		Vector2[] array2 = new Vector2[list.Count];
		int[] array3 = new int[list.Count];
		for (int n = 0; n < list.Count; n++)
		{
			array[n] = new Vector3(list[n].x, list[n].y, list[n].z);
			array2[n] = new Vector3(list[n].tu, list[n].tv);
			array3[n] = n;
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		return mesh;
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x0013A900 File Offset: 0x00138B00
	private bool InitializeCubemapFaceMatrices()
	{
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		if (graphicsDeviceType != GraphicsDeviceType.Direct3D11)
		{
			switch (graphicsDeviceType)
			{
			case GraphicsDeviceType.Metal:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.OpenGLCore:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatrices;
				goto IL_75;
			case GraphicsDeviceType.Direct3D12:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.Vulkan:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			}
			this.platformCubemapFaceMatrices = null;
		}
		else
		{
			this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
		}
		IL_75:
		if (this.platformCubemapFaceMatrices == null)
		{
			Debug.LogError("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + SystemInfo.graphicsDeviceType);
			return false;
		}
		return true;
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x0013A9A6 File Offset: 0x00138BA6
	private int FastLog2(int value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return ReflectionProbeEx.tab32[(int)((uint)((long)value * 130329821L) >> 27)];
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x0013A9E0 File Offset: 0x00138BE0
	private uint ReverseBits(uint bits)
	{
		bits = (bits << 16) | (bits >> 16);
		bits = ((bits & 16711935U) << 8) | ((bits & 4278255360U) >> 8);
		bits = ((bits & 252645135U) << 4) | ((bits & 4042322160U) >> 4);
		bits = ((bits & 858993459U) << 2) | ((bits & 3435973836U) >> 2);
		bits = ((bits & 1431655765U) << 1) | ((bits & 2863311530U) >> 1);
		return bits;
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x0013AA4D File Offset: 0x00138C4D
	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		if (mat == null)
		{
			mat = new Material(shader);
		}
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x0013AA61 File Offset: 0x00138C61
	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if (mat == null)
		{
			this.SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x0013AA7C File Offset: 0x00138C7C
	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear)
	{
		if (rt == null || !rt.IsCreated())
		{
			this.SafeDestroy<RenderTexture>(ref rt);
			rt = new RenderTexture(size, size, depth, format, readWrite)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.dimension = dim;
			if (dim == TextureDimension.Tex2DArray)
			{
				rt.volumeDepth = 6;
			}
			rt.useMipMap = mips;
			rt.autoGenerateMips = false;
			rt.filterMode = filter;
			rt.anisoLevel = 0;
			rt.Create();
		}
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x0013AB02 File Offset: 0x00138D02
	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		if (cb == null)
		{
			cb = new CommandBuffer();
			cb.name = name;
		}
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x0013AB17 File Offset: 0x00138D17
	private void SafeDestroy<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x0013AB43 File Offset: 0x00138D43
	private void SafeDispose<T>(ref T obj) where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x04002A1A RID: 10778
	private Mesh blitMesh;

	// Token: 0x04002A1B RID: 10779
	private Mesh skyboxMesh;

	// Token: 0x04002A1C RID: 10780
	private static float[] octaVerts = new float[]
	{
		0f, 1f, 0f, 0f, 0f, -1f, 1f, 0f, 0f, 0f,
		1f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f,
		0f, 0f, 0f, 1f, -1f, 0f, 0f, 0f, 1f, 0f,
		-1f, 0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 1f,
		0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f, 0f,
		1f, 1f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f,
		0f, 0f, 1f, 0f, -1f, 0f, 0f, 0f, -1f, -1f,
		0f, 0f
	};

	// Token: 0x04002A1D RID: 10781
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04002A1E RID: 10782
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04002A1F RID: 10783
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] shadowCubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f))
	};

	// Token: 0x04002A20 RID: 10784
	private ReflectionProbeEx.CubemapFaceMatrices[] platformCubemapFaceMatrices;

	// Token: 0x04002A21 RID: 10785
	private static readonly int[] tab32 = new int[]
	{
		0, 9, 1, 10, 13, 21, 2, 29, 11, 14,
		16, 18, 22, 25, 3, 30, 8, 12, 20, 28,
		15, 17, 24, 7, 19, 27, 23, 6, 26, 5,
		4, 31
	};

	// Token: 0x04002A22 RID: 10786
	public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.EveryFrame;

	// Token: 0x04002A23 RID: 10787
	public bool timeSlicing;

	// Token: 0x04002A24 RID: 10788
	public int resolution = 128;

	// Token: 0x04002A25 RID: 10789
	[global::InspectorName("HDR")]
	public bool hdr = true;

	// Token: 0x04002A26 RID: 10790
	public float shadowDistance;

	// Token: 0x04002A27 RID: 10791
	public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;

	// Token: 0x04002A28 RID: 10792
	public Color background = new Color(0.192f, 0.301f, 0.474f);

	// Token: 0x04002A29 RID: 10793
	public float nearClip = 0.3f;

	// Token: 0x04002A2A RID: 10794
	public float farClip = 1000f;

	// Token: 0x04002A2B RID: 10795
	public Transform attachToTarget;

	// Token: 0x04002A2C RID: 10796
	public Light directionalLight;

	// Token: 0x04002A2D RID: 10797
	public float textureMipBias = 2f;

	// Token: 0x04002A2E RID: 10798
	public bool highPrecision;

	// Token: 0x04002A2F RID: 10799
	public bool enableShadows;

	// Token: 0x04002A30 RID: 10800
	public ReflectionProbeEx.ConvolutionQuality convolutionQuality;

	// Token: 0x04002A31 RID: 10801
	public List<ReflectionProbeEx.RenderListEntry> staticRenderList = new List<ReflectionProbeEx.RenderListEntry>();

	// Token: 0x04002A32 RID: 10802
	public Cubemap reflectionCubemap;

	// Token: 0x04002A33 RID: 10803
	public float reflectionIntensity = 1f;

	// Token: 0x02000E51 RID: 3665
	private struct CubemapSkyboxVertex
	{
		// Token: 0x04004B6D RID: 19309
		public float x;

		// Token: 0x04004B6E RID: 19310
		public float y;

		// Token: 0x04004B6F RID: 19311
		public float z;

		// Token: 0x04004B70 RID: 19312
		public Color color;

		// Token: 0x04004B71 RID: 19313
		public float tu;

		// Token: 0x04004B72 RID: 19314
		public float tv;

		// Token: 0x04004B73 RID: 19315
		public float tw;
	}

	// Token: 0x02000E52 RID: 3666
	private struct CubemapFaceMatrices
	{
		// Token: 0x06005283 RID: 21123 RVA: 0x001B0584 File Offset: 0x001AE784
		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			this.worldToView = Matrix4x4.identity;
			this.worldToView[0, 0] = x[0];
			this.worldToView[0, 1] = x[1];
			this.worldToView[0, 2] = x[2];
			this.worldToView[1, 0] = y[0];
			this.worldToView[1, 1] = y[1];
			this.worldToView[1, 2] = y[2];
			this.worldToView[2, 0] = z[0];
			this.worldToView[2, 1] = z[1];
			this.worldToView[2, 2] = z[2];
			this.viewToWorld = this.worldToView.inverse;
		}

		// Token: 0x04004B74 RID: 19316
		public Matrix4x4 worldToView;

		// Token: 0x04004B75 RID: 19317
		public Matrix4x4 viewToWorld;
	}

	// Token: 0x02000E53 RID: 3667
	[Serializable]
	public enum ConvolutionQuality
	{
		// Token: 0x04004B77 RID: 19319
		Lowest,
		// Token: 0x04004B78 RID: 19320
		Low,
		// Token: 0x04004B79 RID: 19321
		Medium,
		// Token: 0x04004B7A RID: 19322
		High,
		// Token: 0x04004B7B RID: 19323
		VeryHigh
	}

	// Token: 0x02000E54 RID: 3668
	[Serializable]
	public struct RenderListEntry
	{
		// Token: 0x06005284 RID: 21124 RVA: 0x001B066A File Offset: 0x001AE86A
		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}

		// Token: 0x04004B7C RID: 19324
		public Renderer renderer;

		// Token: 0x04004B7D RID: 19325
		public bool alwaysEnabled;
	}
}
