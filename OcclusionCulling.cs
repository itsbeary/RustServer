using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RustNative;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009A5 RID: 2469
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	// Token: 0x06003A89 RID: 14985 RVA: 0x0015933A File Offset: 0x0015753A
	public static bool DebugFilterIsDynamic(int filter)
	{
		return filter == 1 || filter == 4;
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x00159346 File Offset: 0x00157546
	public static bool DebugFilterIsStatic(int filter)
	{
		return filter == 2 || filter == 4;
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x00159352 File Offset: 0x00157552
	public static bool DebugFilterIsGrid(int filter)
	{
		return filter == 3 || filter == 4;
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x0015935E File Offset: 0x0015755E
	private void DebugInitialize()
	{
		this.debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x0015937D File Offset: 0x0015757D
	private void DebugShutdown()
	{
		if (this.debugMipMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.debugMipMat);
			this.debugMipMat = null;
		}
	}

	// Token: 0x06003A8E RID: 14990 RVA: 0x0015939F File Offset: 0x0015759F
	private void DebugUpdate()
	{
		if (this.HiZReady)
		{
			this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, this.hiZLevels.Length - 1);
		}
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x000063A5 File Offset: 0x000045A5
	private void DebugDraw()
	{
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x001593D0 File Offset: 0x001575D0
	public static void NormalizePlane(ref Vector4 plane)
	{
		float num = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= num;
		plane.y /= num;
		plane.z /= num;
		plane.w /= num;
	}

	// Token: 0x06003A91 RID: 14993 RVA: 0x00159438 File Offset: 0x00157638
	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[5]);
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06003A92 RID: 14994 RVA: 0x001596E7 File Offset: 0x001578E7
	public bool HiZReady
	{
		get
		{
			return this.hiZTexture != null && this.hiZWidth > 0 && this.hiZHeight > 0;
		}
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x0015970C File Offset: 0x0015790C
	public void CheckResizeHiZMap()
	{
		int pixelWidth = this.camera.pixelWidth;
		int pixelHeight = this.camera.pixelHeight;
		if (pixelWidth > 0 && pixelHeight > 0)
		{
			int num = pixelWidth / 4;
			int num2 = pixelHeight / 4;
			if (this.hiZLevels == null || this.hiZWidth != num || this.hiZHeight != num2)
			{
				this.InitializeHiZMap(num, num2);
				this.hiZWidth = num;
				this.hiZHeight = num2;
				if (this.debugSettings.log)
				{
					Debug.Log(string.Concat(new object[] { "[OcclusionCulling] Resized HiZ Map to ", this.hiZWidth, " x ", this.hiZHeight }));
				}
			}
		}
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x001597C0 File Offset: 0x001579C0
	private void InitializeHiZMap()
	{
		Shader shader = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader shader2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		this.downscaleMat = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blitCopyMat = new Material(shader2)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckResizeHiZMap();
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x00159814 File Offset: 0x00157A14
	private void FinalizeHiZMap()
	{
		this.DestroyHiZMap();
		if (this.downscaleMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.downscaleMat);
			this.downscaleMat = null;
		}
		if (this.blitCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitCopyMat);
			this.blitCopyMat = null;
		}
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x00159868 File Offset: 0x00157A68
	private void InitializeHiZMap(int width, int height)
	{
		this.DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		this.hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		this.hiZLevels = new RenderTexture[this.hiZLevelCount];
		this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
		this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
		for (int i = 0; i < this.hiZLevelCount; i++)
		{
			this.hiZLevels[i] = this.CreateDepthTextureMip("HiZMap" + i, width, height, i);
		}
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x00159924 File Offset: 0x00157B24
	private void DestroyHiZMap()
	{
		if (this.depthTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.depthTexture);
			this.depthTexture = null;
		}
		if (this.hiZTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.hiZTexture);
			this.hiZTexture = null;
		}
		if (this.hiZLevels != null)
		{
			for (int i = 0; i < this.hiZLevels.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.hiZLevels[i]);
			}
			this.hiZLevels = null;
		}
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x001599AC File Offset: 0x00157BAC
	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = mips;
		renderTexture.autoGenerateMips = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003A99 RID: 15001 RVA: 0x001599E4 File Offset: 0x00157BE4
	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		int num = width >> mip;
		int num2 = height >> mip;
		RenderTexture renderTexture = new RenderTexture(num, num2, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x00159A2D File Offset: 0x00157C2D
	public void GrabDepthTexture()
	{
		if (this.depthTexture != null)
		{
			UnityEngine.Graphics.Blit(null, this.depthTexture, this.depthCopyMat, 0);
		}
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x00159A50 File Offset: 0x00157C50
	public void GenerateHiZMipChain()
	{
		if (this.HiZReady)
		{
			bool flag = true;
			this.depthCopyMat.SetMatrix("_CameraReprojection", this.prevViewProjMatrix * this.invViewProjMatrix);
			this.depthCopyMat.SetFloat("_FrustumNoDataDepth", flag ? 1f : 0f);
			UnityEngine.Graphics.Blit(this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
			for (int i = 1; i < this.hiZLevels.Length; i++)
			{
				RenderTexture renderTexture = this.hiZLevels[i - 1];
				RenderTexture renderTexture2 = this.hiZLevels[i];
				int num = (((renderTexture.width & 1) == 0 && (renderTexture.height & 1) == 0) ? 0 : 1);
				this.downscaleMat.SetTexture("_MainTex", renderTexture);
				UnityEngine.Graphics.Blit(renderTexture, renderTexture2, this.downscaleMat, num);
			}
			for (int j = 0; j < this.hiZLevels.Length; j++)
			{
				UnityEngine.Graphics.SetRenderTarget(this.hiZTexture, j);
				UnityEngine.Graphics.Blit(this.hiZLevels[j], this.blitCopyMat);
			}
		}
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x00159B60 File Offset: 0x00157D60
	private void DebugDrawGizmos()
	{
		Camera component = base.GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
		Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(component.projectionMatrix, false) * worldToCameraMatrix;
		Vector4[] array = new Vector4[6];
		OcclusionCulling.ExtractFrustum(matrix4x, ref array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = new Vector3(array[i].x, array[i].y, array[i].z);
			float w = array[i].w;
			Vector3 vector2 = -vector * w;
			Gizmos.DrawLine(vector2, vector2 * 2f);
		}
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x00159C78 File Offset: 0x00157E78
	private static int floor(float x)
	{
		int num = (int)x;
		if (x >= (float)num)
		{
			return num;
		}
		return num - 1;
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x00159C94 File Offset: 0x00157E94
	public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num4 = Mathf.Clamp(num, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		int num6 = Mathf.Clamp(num3, -1048575, 1048575);
		ulong num7 = (ulong)((long)((num4 >= 0) ? num4 : (num4 + 1048575)));
		ulong num8 = (ulong)((long)((num5 >= 0) ? num5 : (num5 + 1048575)));
		ulong num9 = (ulong)((long)((num6 >= 0) ? num6 : (num6 + 1048575)));
		ulong num10 = (num7 << 42) | (num8 << 21) | num9;
		OcclusionCulling.Cell cell;
		bool flag = OcclusionCulling.grid.TryGetValue(num10, out cell);
		if (!flag)
		{
			Vector3 vector = default(Vector3);
			vector.x = (float)num * 100f + 50f;
			vector.y = (float)num2 * 100f + 50f;
			vector.z = (float)num3 * 100f + 50f;
			Vector3 vector2 = new Vector3(100f, 100f, 100f);
			cell = OcclusionCulling.grid.Add(num10, 16).Initialize(num, num2, num3, new Bounds(vector, vector2));
		}
		OcclusionCulling.SmartList smartList = (occludee.isStatic ? cell.staticBucket : cell.dynamicBucket);
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = cell;
			smartList.Add(occludee, 16);
			OcclusionCulling.gridChanged.Enqueue(cell);
		}
		return cell;
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x00159E74 File Offset: 0x00158074
	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num2 != occludee.cell.y || num3 != occludee.cell.z)
		{
			OcclusionCulling.UnregisterFromGrid(occludee);
			OcclusionCulling.RegisterToGrid(occludee);
		}
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x00159F3C File Offset: 0x0015813C
	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		OcclusionCulling.Cell cell = occludee.cell;
		OcclusionCulling.SmartList smartList = (occludee.isStatic ? cell.staticBucket : cell.dynamicBucket);
		OcclusionCulling.gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			OcclusionCulling.grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x00159FAC File Offset: 0x001581AC
	public void UpdateGridBuffers()
	{
		if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
		{
			if (this.debugSettings.log)
			{
				Debug.Log("[OcclusionCulling] Resized grid to " + OcclusionCulling.grid.Size);
			}
			for (int i = 0; i < OcclusionCulling.grid.Size; i++)
			{
				if (OcclusionCulling.grid[i] != null)
				{
					OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[i]);
				}
			}
		}
		bool flag = OcclusionCulling.gridChanged.Count > 0;
		while (OcclusionCulling.gridChanged.Count > 0)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
			OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = cell.sphereBounds;
		}
		if (flag)
		{
			OcclusionCulling.gridSet.UploadData();
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x06003AA2 RID: 15010 RVA: 0x0015A08B File Offset: 0x0015828B
	public static OcclusionCulling Instance
	{
		get
		{
			return OcclusionCulling.instance;
		}
	}

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06003AA3 RID: 15011 RVA: 0x0015A092 File Offset: 0x00158292
	public static bool Supported
	{
		get
		{
			return OcclusionCulling.supportedDeviceTypes.Contains(SystemInfo.graphicsDeviceType);
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06003AA4 RID: 15012 RVA: 0x0015A0A3 File Offset: 0x001582A3
	// (set) Token: 0x06003AA5 RID: 15013 RVA: 0x0015A0AA File Offset: 0x001582AA
	public static bool Enabled
	{
		get
		{
			return OcclusionCulling._enabled;
		}
		set
		{
			OcclusionCulling._enabled = value;
			if (OcclusionCulling.instance != null)
			{
				OcclusionCulling.instance.enabled = value;
			}
		}
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06003AA6 RID: 15014 RVA: 0x0015A0CA File Offset: 0x001582CA
	// (set) Token: 0x06003AA7 RID: 15015 RVA: 0x0015A0D1 File Offset: 0x001582D1
	public static bool SafeMode
	{
		get
		{
			return OcclusionCulling._safeMode;
		}
		set
		{
			OcclusionCulling._safeMode = value;
		}
	}

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06003AA8 RID: 15016 RVA: 0x0015A0D9 File Offset: 0x001582D9
	// (set) Token: 0x06003AA9 RID: 15017 RVA: 0x0015A0E0 File Offset: 0x001582E0
	public static OcclusionCulling.DebugFilter DebugShow
	{
		get
		{
			return OcclusionCulling._debugShow;
		}
		set
		{
			OcclusionCulling._debugShow = value;
		}
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x0015A0E8 File Offset: 0x001582E8
	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			OcclusionCulling.statePool.Enqueue(new OccludeeState());
		}
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x0015A114 File Offset: 0x00158314
	private static OccludeeState Allocate()
	{
		if (OcclusionCulling.statePool.Count == 0)
		{
			OcclusionCulling.GrowStatePool();
		}
		return OcclusionCulling.statePool.Dequeue();
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x0015A131 File Offset: 0x00158331
	private static void Release(OccludeeState state)
	{
		OcclusionCulling.statePool.Enqueue(state);
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x0015A140 File Offset: 0x00158340
	private void Awake()
	{
		OcclusionCulling.instance = this;
		this.camera = base.GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			this.frustumPropNames[i] = "_FrustumPlane" + i;
		}
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x0015A184 File Offset: 0x00158384
	private void OnEnable()
	{
		if (!OcclusionCulling.Enabled)
		{
			OcclusionCulling.Enabled = false;
			return;
		}
		if (!OcclusionCulling.Supported)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to graphics device type " + SystemInfo.graphicsDeviceType + " not supported.");
			OcclusionCulling.Enabled = false;
			return;
		}
		this.usePixelShaderFallback = this.usePixelShaderFallback || !SystemInfo.supportsComputeShaders || this.computeShader == null || !this.computeShader.HasKernel("compute_cull");
		this.useNativePath = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 && this.SupportsNativePath();
		this.useAsyncReadAPI = !this.useNativePath && SystemInfo.supportsAsyncGPUReadback;
		if (!this.useNativePath && !this.useAsyncReadAPI)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType);
			OcclusionCulling.Enabled = false;
			return;
		}
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			OcclusionCulling.staticChanged.Add(i);
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			OcclusionCulling.dynamicChanged.Add(j);
		}
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		OcclusionCulling.staticSet.Attach(this);
		OcclusionCulling.dynamicSet.Attach(this);
		OcclusionCulling.gridSet.Attach(this);
		this.depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.InitializeHiZMap();
		this.UpdateCameraMatrices(true);
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x0015A314 File Offset: 0x00158514
	private bool SupportsNativePath()
	{
		bool flag = true;
		try
		{
			OccludeeState.State state = default(OccludeeState.State);
			Color32 color = new Color32(0, 0, 0, 0);
			Vector4 zero = Vector4.zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			OcclusionCulling.ProcessOccludees_Native(ref state, ref num, 0, ref color, 0, ref num2, ref num3, ref zero, 0f, 0U);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			flag = false;
		}
		return flag;
	}

	// Token: 0x06003AB0 RID: 15024 RVA: 0x0015A384 File Offset: 0x00158584
	private void OnDisable()
	{
		if (this.fallbackMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.fallbackMat);
			this.fallbackMat = null;
		}
		if (this.depthCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.depthCopyMat);
			this.depthCopyMat = null;
		}
		OcclusionCulling.staticSet.Dispose(true);
		OcclusionCulling.dynamicSet.Dispose(true);
		OcclusionCulling.gridSet.Dispose(true);
		this.FinalizeHiZMap();
	}

	// Token: 0x06003AB1 RID: 15025 RVA: 0x0015A3F8 File Offset: 0x001585F8
	public static void MakeAllVisible()
	{
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			if (OcclusionCulling.staticOccludees[i] != null)
			{
				OcclusionCulling.staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			if (OcclusionCulling.dynamicOccludees[j] != null)
			{
				OcclusionCulling.dynamicOccludees[j].MakeVisible();
			}
		}
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x0015A469 File Offset: 0x00158669
	private void Update()
	{
		if (!OcclusionCulling.Enabled)
		{
			base.enabled = false;
			return;
		}
		this.CheckResizeHiZMap();
		this.DebugUpdate();
		this.DebugDraw();
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x0015A48C File Offset: 0x0015868C
	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false) where T : Occludee
	{
		Renderer component = transform.GetComponent<Renderer>();
		Collider component2 = transform.GetComponent<Collider>();
		if (component != null && component2 != null)
		{
			T t = component.gameObject.GetComponent<T>();
			t = ((t == null) ? component.gameObject.AddComponent<T>() : t);
			t.minTimeVisible = minTimeVisible;
			t.isStatic = isStatic;
			t.stickyGizmos = stickyGizmos;
			t.Register();
		}
		foreach (object obj in transform)
		{
			OcclusionCulling.RecursiveAddOccludees<T>((Transform)obj, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	// Token: 0x06003AB4 RID: 15028 RVA: 0x0015A55C File Offset: 0x0015875C
	private static int FindFreeSlot(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int num;
		if (recycled.Count > 0)
		{
			num = recycled.Dequeue();
		}
		else
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num2 = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num2 > 0)
				{
					occludees.Capacity = num2;
					states.Capacity = num2;
				}
			}
			if (occludees.Count < occludees.Capacity)
			{
				num = occludees.Count;
				occludees.Add(null);
				states.Add(default(OccludeeState.State));
			}
			else
			{
				num = -1;
			}
		}
		return num;
	}

	// Token: 0x06003AB5 RID: 15029 RVA: 0x0015A5E4 File Offset: 0x001587E4
	public static OccludeeState GetStateById(int id)
	{
		if (id < 0 || id >= 2097152)
		{
			return null;
		}
		bool flag = id < 1048576;
		int num = (flag ? id : (id - 1048576));
		if (flag)
		{
			return OcclusionCulling.staticOccludees[num];
		}
		return OcclusionCulling.dynamicOccludees[num];
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x0015A630 File Offset: 0x00158830
	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
	{
		int num;
		if (isStatic)
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged);
		}
		else
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged);
		}
		if (num >= 0 && !isStatic)
		{
			return num + 1048576;
		}
		return num;
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x0015A6B4 File Offset: 0x001588B4
	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged, OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, OcclusionCulling.BufferSet set, OcclusionCulling.SimpleList<int> visibilityChanged)
	{
		int num = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 vector = new Vector4(center.x, center.y, center.z, radius);
			OccludeeState occludeeState = OcclusionCulling.Allocate().Initialize(states, set, num, vector, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			occludeeState.cell = OcclusionCulling.RegisterToGrid(occludeeState);
			occludees[num] = occludeeState;
			changed.Add(num);
			if (states.array[num].isVisible > 0 != occludeeState.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x0015A74C File Offset: 0x0015894C
	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int num = (flag ? id : (id - 1048576));
			if (flag)
			{
				OcclusionCulling.UnregisterOccludee(num, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
				return;
			}
			OcclusionCulling.UnregisterOccludee(num, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
		}
	}

	// Token: 0x06003AB9 RID: 15033 RVA: 0x0015A7A8 File Offset: 0x001589A8
	private static void UnregisterOccludee(int slot, OcclusionCulling.SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState occludeeState = occludees[slot];
		OcclusionCulling.UnregisterFromGrid(occludeeState);
		recycled.Enqueue(slot);
		changed.Add(slot);
		OcclusionCulling.Release(occludeeState);
		occludees[slot] = null;
		occludeeState.Invalidate();
	}

	// Token: 0x06003ABA RID: 15034 RVA: 0x0015A7D8 File Offset: 0x001589D8
	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			OcclusionCulling.dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OcclusionCulling.dynamicChanged.Add(num);
		}
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x0015A834 File Offset: 0x00158A34
	private void UpdateBuffers(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, List<int> changed, bool isStatic)
	{
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		set.CheckResize(count, 2048);
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				if (!isStatic)
				{
					OcclusionCulling.UpdateInGrid(occludeeState);
				}
				set.inputData[num] = states[num].sphereBounds;
			}
			else
			{
				set.inputData[num] = Vector4.zero;
			}
		}
		changed.Clear();
		if (flag)
		{
			set.UploadData();
		}
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x0015A8D8 File Offset: 0x00158AD8
	private void UpdateCameraMatrices(bool starting = false)
	{
		if (!starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
		Matrix4x4 matrix4x = Matrix4x4.Perspective(this.camera.fieldOfView, this.camera.aspect, this.camera.nearClipPlane, this.camera.farClipPlane);
		this.viewMatrix = this.camera.worldToCameraMatrix;
		this.projMatrix = GL.GetGPUProjectionMatrix(matrix4x, false);
		this.viewProjMatrix = this.projMatrix * this.viewMatrix;
		this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
		if (starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x0015A97C File Offset: 0x00158B7C
	private void OnPreCull()
	{
		this.UpdateCameraMatrices(false);
		this.GenerateHiZMipChain();
		this.PrepareAndDispatch();
		this.IssueRead();
		if (OcclusionCulling.grid.Size <= OcclusionCulling.gridSet.resultData.Length)
		{
			this.RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[OcclusionCulling] Grid size and result capacity are out of sync: ",
			OcclusionCulling.grid.Size,
			", ",
			OcclusionCulling.gridSet.resultData.Length
		}));
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x0015AA08 File Offset: 0x00158C08
	private void OnPostRender()
	{
		bool sRGBWrite = GL.sRGBWrite;
		RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
		this.GrabDepthTexture();
		UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		GL.sRGBWrite = sRGBWrite;
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x0015AA38 File Offset: 0x00158C38
	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int i = 0;
		int num = 0;
		while (i < 4)
		{
			for (int j = 0; j < 4; j++)
			{
				this.matrixToFloatTemp[num++] = m[j, i];
			}
			i++;
		}
		return this.matrixToFloatTemp;
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x0015AA7C File Offset: 0x00158C7C
	private void PrepareAndDispatch()
	{
		Vector2 vector = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
		OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
		bool flag = true;
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat.SetTexture("_HiZMap", this.hiZTexture);
			this.fallbackMat.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
			this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
			this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
			this.fallbackMat.SetVector("_CameraWorldPos", base.transform.position);
			this.fallbackMat.SetVector("_ViewportSize", vector);
			this.fallbackMat.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int i = 0; i < 6; i++)
			{
				this.fallbackMat.SetVector(this.frustumPropNames[i], this.frustumPlanes[i]);
			}
		}
		else
		{
			this.computeShader.SetTexture(0, "_HiZMap", this.hiZTexture);
			this.computeShader.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
			this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
			this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
			this.computeShader.SetVector("_CameraWorldPos", base.transform.position);
			this.computeShader.SetVector("_ViewportSize", vector);
			this.computeShader.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int j = 0; j < 6; j++)
			{
				this.computeShader.SetVector(this.frustumPropNames[j], this.frustumPlanes[j]);
			}
		}
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
			OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
			OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
		}
		this.UpdateGridBuffers();
		OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x0015AD48 File Offset: 0x00158F48
	private void IssueRead()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.IssueRead();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.IssueRead();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.IssueRead();
		}
		GL.IssuePluginEvent(RustNative.Graphics.GetRenderEventFunc(), 2);
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x0015ADA8 File Offset: 0x00158FA8
	public void ResetTiming(OcclusionCulling.SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null)
			{
				occludeeState.states.array[occludeeState.slot].waitTime = 0f;
			}
		}
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x0015ADF4 File Offset: 0x00158FF4
	public void ResetTiming()
	{
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null)
			{
				this.ResetTiming(cell.staticBucket);
				this.ResetTiming(cell.dynamicBucket);
			}
		}
	}

	// Token: 0x06003AC4 RID: 15044 RVA: 0x0015AE40 File Offset: 0x00159040
	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		for (int i = 0; i < 6; i++)
		{
			if (planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w < -testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003AC5 RID: 15045 RVA: 0x0015AEB0 File Offset: 0x001590B0
	private static int ProcessOccludees_Safe(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SmartList bucket, Color32[] results, OcclusionCulling.SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null && occludeeState.slot < results.Length)
			{
				int slot = occludeeState.slot;
				OccludeeState.State state = states[slot];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[slot].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = time < state.waitTime;
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						changed.Add(slot);
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[slot] = state;
				num += (int)state.isVisible;
			}
		}
		return num;
	}

	// Token: 0x06003AC6 RID: 15046 RVA: 0x0015AF9C File Offset: 0x0015919C
	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num2 = bucket[i];
			if (num2 >= 0 && num2 < resultCount && states[num2].active != 0)
			{
				OccludeeState.State state = states[num2];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[num2].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = time < state.waitTime;
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						int num3 = changedCount;
						changedCount = num3 + 1;
						changed[num3] = num2;
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[num2] = state;
				num += (flag2 ? 0 : 1);
			}
		}
		return num;
	}

	// Token: 0x06003AC7 RID: 15047
	[DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	// Token: 0x06003AC8 RID: 15048 RVA: 0x0015B088 File Offset: 0x00159288
	private void ApplyVisibility_Safe(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, cell.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, cell.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
					}
					cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
				}
			}
		}
	}

	// Token: 0x06003AC9 RID: 15049 RVA: 0x0015B1CC File Offset: 0x001593CC
	private void ApplyVisibility_Fast(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
				}
			}
		}
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x0015B370 File Offset: 0x00159570
	private void ApplyVisibility_Native(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
				}
			}
		}
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x0015B558 File Offset: 0x00159758
	private void ProcessCallbacks(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SimpleList<int> changed)
	{
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				bool flag = states.array[num].isVisible == 0;
				OcclusionCulling.OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (UnityEngine.Object)onVisibilityChanged.Target != null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (flag ? 1 : 0);
				}
			}
		}
		changed.Clear();
	}

	// Token: 0x06003ACC RID: 15052 RVA: 0x0015B5F8 File Offset: 0x001597F8
	public void RetrieveAndApplyVisibility()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.GetResults();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.GetResults();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.GetResults();
		}
		if (this.debugSettings.showAllVisible)
		{
			for (int i = 0; i < OcclusionCulling.staticSet.resultData.Length; i++)
			{
				OcclusionCulling.staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < OcclusionCulling.dynamicSet.resultData.Length; j++)
			{
				OcclusionCulling.dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < OcclusionCulling.gridSet.resultData.Length; k++)
			{
				OcclusionCulling.gridSet.resultData[k].r = 1;
			}
		}
		OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
		OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
		float time = Time.time;
		uint frameCount = (uint)Time.frameCount;
		if (this.useNativePath)
		{
			this.ApplyVisibility_Native(time, frameCount);
		}
		else
		{
			this.ApplyVisibility_Fast(time, frameCount);
		}
		this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
		this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
	}

	// Token: 0x0400354F RID: 13647
	public OcclusionCulling.DebugSettings debugSettings = new OcclusionCulling.DebugSettings();

	// Token: 0x04003550 RID: 13648
	private Material debugMipMat;

	// Token: 0x04003551 RID: 13649
	private const float debugDrawDuration = 0.0334f;

	// Token: 0x04003552 RID: 13650
	private Material downscaleMat;

	// Token: 0x04003553 RID: 13651
	private Material blitCopyMat;

	// Token: 0x04003554 RID: 13652
	private int hiZLevelCount;

	// Token: 0x04003555 RID: 13653
	private int hiZWidth;

	// Token: 0x04003556 RID: 13654
	private int hiZHeight;

	// Token: 0x04003557 RID: 13655
	private RenderTexture depthTexture;

	// Token: 0x04003558 RID: 13656
	private RenderTexture hiZTexture;

	// Token: 0x04003559 RID: 13657
	private RenderTexture[] hiZLevels;

	// Token: 0x0400355A RID: 13658
	private const int GridCellsPerAxis = 2097152;

	// Token: 0x0400355B RID: 13659
	private const int GridHalfCellsPerAxis = 1048576;

	// Token: 0x0400355C RID: 13660
	private const int GridMinHalfCellsPerAxis = -1048575;

	// Token: 0x0400355D RID: 13661
	private const int GridMaxHalfCellsPerAxis = 1048575;

	// Token: 0x0400355E RID: 13662
	private const float GridCellSize = 100f;

	// Token: 0x0400355F RID: 13663
	private const float GridHalfCellSize = 50f;

	// Token: 0x04003560 RID: 13664
	private const float GridRcpCellSize = 0.01f;

	// Token: 0x04003561 RID: 13665
	private const int GridPoolCapacity = 16384;

	// Token: 0x04003562 RID: 13666
	private const int GridPoolGranularity = 4096;

	// Token: 0x04003563 RID: 13667
	private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid = new OcclusionCulling.HashedPool<OcclusionCulling.Cell>(16384, 4096);

	// Token: 0x04003564 RID: 13668
	private static Queue<OcclusionCulling.Cell> gridChanged = new Queue<OcclusionCulling.Cell>();

	// Token: 0x04003565 RID: 13669
	public ComputeShader computeShader;

	// Token: 0x04003566 RID: 13670
	public bool usePixelShaderFallback = true;

	// Token: 0x04003567 RID: 13671
	public bool useAsyncReadAPI;

	// Token: 0x04003568 RID: 13672
	private Camera camera;

	// Token: 0x04003569 RID: 13673
	private const int ComputeThreadsPerGroup = 64;

	// Token: 0x0400356A RID: 13674
	private const int InputBufferStride = 16;

	// Token: 0x0400356B RID: 13675
	private const int ResultBufferStride = 4;

	// Token: 0x0400356C RID: 13676
	private const int OccludeeMaxSlotsPerPool = 1048576;

	// Token: 0x0400356D RID: 13677
	private const int OccludeePoolGranularity = 2048;

	// Token: 0x0400356E RID: 13678
	private const int StateBufferGranularity = 2048;

	// Token: 0x0400356F RID: 13679
	private const int GridBufferGranularity = 256;

	// Token: 0x04003570 RID: 13680
	private static Queue<OccludeeState> statePool = new Queue<OccludeeState>();

	// Token: 0x04003571 RID: 13681
	private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x04003572 RID: 13682
	private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x04003573 RID: 13683
	private static OcclusionCulling.SimpleList<int> staticVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x04003574 RID: 13684
	private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x04003575 RID: 13685
	private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x04003576 RID: 13686
	private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x04003577 RID: 13687
	private static List<int> staticChanged = new List<int>(256);

	// Token: 0x04003578 RID: 13688
	private static Queue<int> staticRecycled = new Queue<int>();

	// Token: 0x04003579 RID: 13689
	private static List<int> dynamicChanged = new List<int>(1024);

	// Token: 0x0400357A RID: 13690
	private static Queue<int> dynamicRecycled = new Queue<int>();

	// Token: 0x0400357B RID: 13691
	private static OcclusionCulling.BufferSet staticSet = new OcclusionCulling.BufferSet();

	// Token: 0x0400357C RID: 13692
	private static OcclusionCulling.BufferSet dynamicSet = new OcclusionCulling.BufferSet();

	// Token: 0x0400357D RID: 13693
	private static OcclusionCulling.BufferSet gridSet = new OcclusionCulling.BufferSet();

	// Token: 0x0400357E RID: 13694
	private Vector4[] frustumPlanes = new Vector4[6];

	// Token: 0x0400357F RID: 13695
	private string[] frustumPropNames = new string[6];

	// Token: 0x04003580 RID: 13696
	private float[] matrixToFloatTemp = new float[16];

	// Token: 0x04003581 RID: 13697
	private Material fallbackMat;

	// Token: 0x04003582 RID: 13698
	private Material depthCopyMat;

	// Token: 0x04003583 RID: 13699
	private Matrix4x4 viewMatrix;

	// Token: 0x04003584 RID: 13700
	private Matrix4x4 projMatrix;

	// Token: 0x04003585 RID: 13701
	private Matrix4x4 viewProjMatrix;

	// Token: 0x04003586 RID: 13702
	private Matrix4x4 prevViewProjMatrix;

	// Token: 0x04003587 RID: 13703
	private Matrix4x4 invViewProjMatrix;

	// Token: 0x04003588 RID: 13704
	private bool useNativePath = true;

	// Token: 0x04003589 RID: 13705
	private static OcclusionCulling instance;

	// Token: 0x0400358A RID: 13706
	private static GraphicsDeviceType[] supportedDeviceTypes = new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11 };

	// Token: 0x0400358B RID: 13707
	private static bool _enabled = false;

	// Token: 0x0400358C RID: 13708
	private static bool _safeMode = false;

	// Token: 0x0400358D RID: 13709
	private static OcclusionCulling.DebugFilter _debugShow = OcclusionCulling.DebugFilter.Off;

	// Token: 0x02000EE7 RID: 3815
	public class BufferSet
	{
		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060053A8 RID: 21416 RVA: 0x001B302B File Offset: 0x001B122B
		public bool Ready
		{
			get
			{
				return this.resultData.Length != 0;
			}
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x001B3037 File Offset: 0x001B1237
		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x001B3040 File Offset: 0x001B1240
		public void Dispose(bool data = true)
		{
			if (this.inputBuffer != null)
			{
				this.inputBuffer.Dispose();
				this.inputBuffer = null;
			}
			if (this.resultBuffer != null)
			{
				this.resultBuffer.Dispose();
				this.resultBuffer = null;
			}
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (this.resultReadTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.resultReadTexture);
				this.resultReadTexture = null;
			}
			if (this.readbackInst != IntPtr.Zero)
			{
				RustNative.Graphics.BufferReadback.Destroy(this.readbackInst);
				this.readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
				this.capacity = 0;
				this.count = 0;
			}
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x001B3144 File Offset: 0x001B1344
		public bool CheckResize(int count, int granularity)
		{
			if (count > this.capacity || (this.culling.usePixelShaderFallback && this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				int num = this.capacity;
				int num2 = count / granularity * granularity + granularity;
				if (this.culling.usePixelShaderFallback)
				{
					this.width = Mathf.CeilToInt(Mathf.Sqrt((float)num2));
					this.height = Mathf.CeilToInt((float)num2 / (float)this.width);
					this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
					this.inputTexture.name = "_Input";
					this.inputTexture.filterMode = FilterMode.Point;
					this.inputTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					this.resultTexture.name = "_Result";
					this.resultTexture.filterMode = FilterMode.Point;
					this.resultTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture.useMipMap = false;
					this.resultTexture.Create();
					this.resultReadTexture = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false, true);
					this.resultReadTexture.name = "_ResultRead";
					this.resultReadTexture.filterMode = FilterMode.Point;
					this.resultReadTexture.wrapMode = TextureWrapMode.Clamp;
					if (!this.culling.useAsyncReadAPI)
					{
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForTexture(this.resultTexture.GetNativeTexturePtr(), (uint)this.width, (uint)this.height, (uint)this.resultTexture.format);
					}
					this.capacity = this.width * this.height;
				}
				else
				{
					this.inputBuffer = new ComputeBuffer(num2, 16);
					this.resultBuffer = new ComputeBuffer(num2, 4);
					if (!this.culling.useAsyncReadAPI)
					{
						uint num3 = (uint)(this.capacity * 4);
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), num3);
					}
					this.capacity = num2;
				}
				Array.Resize<Color>(ref this.inputData, this.capacity);
				Array.Resize<Color32>(ref this.resultData, this.capacity);
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				for (int i = num; i < this.capacity; i++)
				{
					this.resultData[i] = color;
				}
				this.count = count;
				return true;
			}
			return false;
		}

		// Token: 0x060053AC RID: 21420 RVA: 0x001B33B8 File Offset: 0x001B15B8
		public void UploadData()
		{
			if (this.culling.usePixelShaderFallback)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
				return;
			}
			this.inputBuffer.SetData(this.inputData);
		}

		// Token: 0x060053AD RID: 21421 RVA: 0x001B33F5 File Offset: 0x001B15F5
		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		// Token: 0x060053AE RID: 21422 RVA: 0x001B3400 File Offset: 0x001B1600
		public void Dispatch(int count)
		{
			if (this.culling.usePixelShaderFallback)
			{
				RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
				this.culling.fallbackMat.SetTexture("_Input", this.inputTexture);
				UnityEngine.Graphics.Blit(this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
				UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
				return;
			}
			if (this.inputBuffer != null)
			{
				this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
				this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
				this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
			}
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x001B34C0 File Offset: 0x001B16C0
		public void IssueRead()
		{
			if (!OcclusionCulling.SafeMode)
			{
				if (this.culling.useAsyncReadAPI)
				{
					if (this.asyncRequests.Count < 10)
					{
						AsyncGPUReadbackRequest asyncGPUReadbackRequest;
						if (this.culling.usePixelShaderFallback)
						{
							asyncGPUReadbackRequest = AsyncGPUReadback.Request(this.resultTexture, 0, null);
						}
						else
						{
							asyncGPUReadbackRequest = AsyncGPUReadback.Request(this.resultBuffer, null);
						}
						this.asyncRequests.Enqueue(asyncGPUReadbackRequest);
						return;
					}
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.IssueRead(this.readbackInst);
				}
			}
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x001B3544 File Offset: 0x001B1744
		public void GetResults()
		{
			if (this.resultData != null && this.resultData.Length != 0)
			{
				if (!OcclusionCulling.SafeMode)
				{
					if (this.culling.useAsyncReadAPI)
					{
						while (this.asyncRequests.Count > 0)
						{
							AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
							if (asyncGPUReadbackRequest.hasError)
							{
								this.asyncRequests.Dequeue();
							}
							else
							{
								if (!asyncGPUReadbackRequest.done)
								{
									return;
								}
								NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
								for (int i = 0; i < data.Length; i++)
								{
									this.resultData[i] = data[i];
								}
								this.asyncRequests.Dequeue();
							}
						}
						return;
					}
					if (this.readbackInst != IntPtr.Zero)
					{
						RustNative.Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
						return;
					}
				}
				else
				{
					if (this.culling.usePixelShaderFallback)
					{
						RenderTexture.active = this.resultTexture;
						this.resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)this.width, (float)this.height), 0, 0);
						this.resultReadTexture.Apply();
						Array.Copy(this.resultReadTexture.GetPixels32(), this.resultData, this.resultData.Length);
						return;
					}
					this.resultBuffer.GetData(this.resultData);
				}
			}
		}

		// Token: 0x04004DD0 RID: 19920
		public ComputeBuffer inputBuffer;

		// Token: 0x04004DD1 RID: 19921
		public ComputeBuffer resultBuffer;

		// Token: 0x04004DD2 RID: 19922
		public int width;

		// Token: 0x04004DD3 RID: 19923
		public int height;

		// Token: 0x04004DD4 RID: 19924
		public int capacity;

		// Token: 0x04004DD5 RID: 19925
		public int count;

		// Token: 0x04004DD6 RID: 19926
		public Texture2D inputTexture;

		// Token: 0x04004DD7 RID: 19927
		public RenderTexture resultTexture;

		// Token: 0x04004DD8 RID: 19928
		public Texture2D resultReadTexture;

		// Token: 0x04004DD9 RID: 19929
		public Color[] inputData = new Color[0];

		// Token: 0x04004DDA RID: 19930
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004DDB RID: 19931
		private OcclusionCulling culling;

		// Token: 0x04004DDC RID: 19932
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004DDD RID: 19933
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x04004DDE RID: 19934
		public IntPtr readbackInst = IntPtr.Zero;
	}

	// Token: 0x02000EE8 RID: 3816
	public enum DebugFilter
	{
		// Token: 0x04004DE0 RID: 19936
		Off,
		// Token: 0x04004DE1 RID: 19937
		Dynamic,
		// Token: 0x04004DE2 RID: 19938
		Static,
		// Token: 0x04004DE3 RID: 19939
		Grid,
		// Token: 0x04004DE4 RID: 19940
		All
	}

	// Token: 0x02000EE9 RID: 3817
	[Flags]
	public enum DebugMask
	{
		// Token: 0x04004DE6 RID: 19942
		Off = 0,
		// Token: 0x04004DE7 RID: 19943
		Dynamic = 1,
		// Token: 0x04004DE8 RID: 19944
		Static = 2,
		// Token: 0x04004DE9 RID: 19945
		Grid = 4,
		// Token: 0x04004DEA RID: 19946
		All = 7
	}

	// Token: 0x02000EEA RID: 3818
	[Serializable]
	public class DebugSettings
	{
		// Token: 0x04004DEB RID: 19947
		public bool log;

		// Token: 0x04004DEC RID: 19948
		public bool showAllVisible;

		// Token: 0x04004DED RID: 19949
		public bool showMipChain;

		// Token: 0x04004DEE RID: 19950
		public bool showMain;

		// Token: 0x04004DEF RID: 19951
		public int showMainLod;

		// Token: 0x04004DF0 RID: 19952
		public bool showFallback;

		// Token: 0x04004DF1 RID: 19953
		public bool showStats;

		// Token: 0x04004DF2 RID: 19954
		public bool showScreenBounds;

		// Token: 0x04004DF3 RID: 19955
		public OcclusionCulling.DebugMask showMask;

		// Token: 0x04004DF4 RID: 19956
		public LayerMask layerFilter = -1;
	}

	// Token: 0x02000EEB RID: 3819
	public class HashedPoolValue
	{
		// Token: 0x04004DF5 RID: 19957
		public ulong hashedPoolKey = ulong.MaxValue;

		// Token: 0x04004DF6 RID: 19958
		public int hashedPoolIndex = -1;
	}

	// Token: 0x02000EEC RID: 3820
	public class HashedPool<ValueType> where ValueType : OcclusionCulling.HashedPoolValue, new()
	{
		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x060053B4 RID: 21428 RVA: 0x001B3705 File Offset: 0x001B1905
		public int Size
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x060053B5 RID: 21429 RVA: 0x001B3712 File Offset: 0x001B1912
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x17000712 RID: 1810
		public ValueType this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x060053B8 RID: 21432 RVA: 0x001B373C File Offset: 0x001B193C
		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			this.dict = new Dictionary<ulong, ValueType>(capacity);
			this.pool = new List<ValueType>(capacity);
			this.list = new List<ValueType>(capacity);
			this.recycled = new Queue<ValueType>();
		}

		// Token: 0x060053B9 RID: 21433 RVA: 0x001B377A File Offset: 0x001B197A
		public void Clear()
		{
			this.dict.Clear();
			this.pool.Clear();
			this.list.Clear();
			this.recycled.Clear();
		}

		// Token: 0x060053BA RID: 21434 RVA: 0x001B37A8 File Offset: 0x001B19A8
		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType valueType;
			if (this.recycled.Count > 0)
			{
				valueType = this.recycled.Dequeue();
				this.list[valueType.hashedPoolIndex] = valueType;
			}
			else
			{
				int count = this.pool.Count;
				if (count == this.pool.Capacity)
				{
					this.pool.Capacity += this.granularity;
				}
				valueType = new ValueType();
				valueType.hashedPoolIndex = count;
				this.pool.Add(valueType);
				this.list.Add(valueType);
			}
			valueType.hashedPoolKey = key;
			this.dict.Add(key, valueType);
			return valueType;
		}

		// Token: 0x060053BB RID: 21435 RVA: 0x001B3860 File Offset: 0x001B1A60
		public void Remove(ValueType value)
		{
			this.dict.Remove(value.hashedPoolKey);
			this.list[value.hashedPoolIndex] = default(ValueType);
			this.recycled.Enqueue(value);
			value.hashedPoolKey = ulong.MaxValue;
		}

		// Token: 0x060053BC RID: 21436 RVA: 0x001B38BC File Offset: 0x001B1ABC
		public bool TryGetValue(ulong key, out ValueType value)
		{
			return this.dict.TryGetValue(key, out value);
		}

		// Token: 0x060053BD RID: 21437 RVA: 0x001B38CB File Offset: 0x001B1ACB
		public bool ContainsKey(ulong key)
		{
			return this.dict.ContainsKey(key);
		}

		// Token: 0x04004DF7 RID: 19959
		private int granularity;

		// Token: 0x04004DF8 RID: 19960
		private Dictionary<ulong, ValueType> dict;

		// Token: 0x04004DF9 RID: 19961
		private List<ValueType> pool;

		// Token: 0x04004DFA RID: 19962
		private List<ValueType> list;

		// Token: 0x04004DFB RID: 19963
		private Queue<ValueType> recycled;
	}

	// Token: 0x02000EED RID: 3821
	public class SimpleList<T>
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060053BE RID: 21438 RVA: 0x001B38D9 File Offset: 0x001B1AD9
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x060053BF RID: 21439 RVA: 0x001B38E1 File Offset: 0x001B1AE1
		// (set) Token: 0x060053C0 RID: 21440 RVA: 0x001B38EC File Offset: 0x001B1AEC
		public int Capacity
		{
			get
			{
				return this.array.Length;
			}
			set
			{
				if (value != this.array.Length)
				{
					if (value > 0)
					{
						T[] array = new T[value];
						if (this.count > 0)
						{
							Array.Copy(this.array, 0, array, 0, this.count);
						}
						this.array = array;
						return;
					}
					this.array = OcclusionCulling.SimpleList<T>.emptyArray;
				}
			}
		}

		// Token: 0x17000715 RID: 1813
		public T this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		// Token: 0x060053C3 RID: 21443 RVA: 0x001B395C File Offset: 0x001B1B5C
		public SimpleList()
		{
			this.array = OcclusionCulling.SimpleList<T>.emptyArray;
		}

		// Token: 0x060053C4 RID: 21444 RVA: 0x001B396F File Offset: 0x001B1B6F
		public SimpleList(int capacity)
		{
			this.array = ((capacity == 0) ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity]);
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x001B3990 File Offset: 0x001B1B90
		public void Add(T item)
		{
			if (this.count == this.array.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			T[] array = this.array;
			int num = this.count;
			this.count = num + 1;
			array[num] = item;
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x001B39D8 File Offset: 0x001B1BD8
		public void Clear()
		{
			if (this.count > 0)
			{
				Array.Clear(this.array, 0, this.count);
				this.count = 0;
			}
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x001B39FC File Offset: 0x001B1BFC
		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x001B3A3E File Offset: 0x001B1C3E
		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, this.count);
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x001B3A54 File Offset: 0x001B1C54
		public void EnsureCapacity(int min)
		{
			if (this.array.Length < min)
			{
				int num = ((this.array.Length == 0) ? 16 : (this.array.Length * 2));
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}

		// Token: 0x04004DFC RID: 19964
		private const int defaultCapacity = 16;

		// Token: 0x04004DFD RID: 19965
		private static readonly T[] emptyArray = new T[0];

		// Token: 0x04004DFE RID: 19966
		public T[] array;

		// Token: 0x04004DFF RID: 19967
		public int count;
	}

	// Token: 0x02000EEE RID: 3822
	public class SmartListValue
	{
		// Token: 0x04004E00 RID: 19968
		public int hashedListIndex = -1;
	}

	// Token: 0x02000EEF RID: 3823
	public class SmartList
	{
		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x060053CC RID: 21452 RVA: 0x001B3AB0 File Offset: 0x001B1CB0
		public OccludeeState[] List
		{
			get
			{
				return this.list;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x060053CD RID: 21453 RVA: 0x001B3AB8 File Offset: 0x001B1CB8
		public int[] Slots
		{
			get
			{
				return this.slots;
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060053CE RID: 21454 RVA: 0x001B3AC0 File Offset: 0x001B1CC0
		public int Size
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060053CF RID: 21455 RVA: 0x001B3AC8 File Offset: 0x001B1CC8
		public int Count
		{
			get
			{
				return this.count - this.recycled.Count;
			}
		}

		// Token: 0x1700071A RID: 1818
		public OccludeeState this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060053D2 RID: 21458 RVA: 0x001B3AF1 File Offset: 0x001B1CF1
		// (set) Token: 0x060053D3 RID: 21459 RVA: 0x001B3AFC File Offset: 0x001B1CFC
		public int Capacity
		{
			get
			{
				return this.list.Length;
			}
			set
			{
				if (value != this.list.Length)
				{
					if (value > 0)
					{
						OccludeeState[] array = new OccludeeState[value];
						int[] array2 = new int[value];
						if (this.count > 0)
						{
							Array.Copy(this.list, array, this.count);
							Array.Copy(this.slots, array2, this.count);
						}
						this.list = array;
						this.slots = array2;
						return;
					}
					this.list = OcclusionCulling.SmartList.emptyList;
					this.slots = OcclusionCulling.SmartList.emptySlots;
				}
			}
		}

		// Token: 0x060053D4 RID: 21460 RVA: 0x001B3B78 File Offset: 0x001B1D78
		public SmartList(int capacity)
		{
			this.list = new OccludeeState[capacity];
			this.slots = new int[capacity];
			this.recycled = new Queue<int>();
			this.count = 0;
		}

		// Token: 0x060053D5 RID: 21461 RVA: 0x001B3BAC File Offset: 0x001B1DAC
		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (this.recycled.Count > 0)
			{
				num = this.recycled.Dequeue();
				this.list[num] = value;
				this.slots[num] = value.slot;
			}
			else
			{
				num = this.count;
				if (num == this.list.Length)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.list[num] = value;
				this.slots[num] = value.slot;
				this.count++;
			}
			value.hashedListIndex = num;
		}

		// Token: 0x060053D6 RID: 21462 RVA: 0x001B3C38 File Offset: 0x001B1E38
		public void Remove(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			this.list[hashedListIndex] = null;
			this.slots[hashedListIndex] = -1;
			this.recycled.Enqueue(hashedListIndex);
			value.hashedListIndex = -1;
		}

		// Token: 0x060053D7 RID: 21463 RVA: 0x001B3C74 File Offset: 0x001B1E74
		public bool Contains(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			return hashedListIndex >= 0 && this.list[hashedListIndex] != null;
		}

		// Token: 0x060053D8 RID: 21464 RVA: 0x001B3C9C File Offset: 0x001B1E9C
		public void EnsureCapacity(int min)
		{
			if (this.list.Length < min)
			{
				int num = ((this.list.Length == 0) ? 16 : (this.list.Length * 2));
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}

		// Token: 0x04004E01 RID: 19969
		private const int defaultCapacity = 16;

		// Token: 0x04004E02 RID: 19970
		private static readonly OccludeeState[] emptyList = new OccludeeState[0];

		// Token: 0x04004E03 RID: 19971
		private static readonly int[] emptySlots = new int[0];

		// Token: 0x04004E04 RID: 19972
		private OccludeeState[] list;

		// Token: 0x04004E05 RID: 19973
		private int[] slots;

		// Token: 0x04004E06 RID: 19974
		private Queue<int> recycled;

		// Token: 0x04004E07 RID: 19975
		private int count;
	}

	// Token: 0x02000EF0 RID: 3824
	[Serializable]
	public class Cell : OcclusionCulling.HashedPoolValue
	{
		// Token: 0x060053DA RID: 21466 RVA: 0x001B3CF4 File Offset: 0x001B1EF4
		public void Reset()
		{
			this.x = (this.y = (this.z = 0));
			this.bounds = default(Bounds);
			this.sphereBounds = Vector4.zero;
			this.isVisible = true;
			this.staticBucket = null;
			this.dynamicBucket = null;
		}

		// Token: 0x060053DB RID: 21467 RVA: 0x001B3D48 File Offset: 0x001B1F48
		public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			this.sphereBounds = new Vector4(bounds.center.x, bounds.center.y, bounds.center.z, bounds.extents.magnitude);
			this.isVisible = true;
			this.staticBucket = new OcclusionCulling.SmartList(32);
			this.dynamicBucket = new OcclusionCulling.SmartList(32);
			return this;
		}

		// Token: 0x04004E08 RID: 19976
		public int x;

		// Token: 0x04004E09 RID: 19977
		public int y;

		// Token: 0x04004E0A RID: 19978
		public int z;

		// Token: 0x04004E0B RID: 19979
		public Bounds bounds;

		// Token: 0x04004E0C RID: 19980
		public Vector4 sphereBounds;

		// Token: 0x04004E0D RID: 19981
		public bool isVisible;

		// Token: 0x04004E0E RID: 19982
		public OcclusionCulling.SmartList staticBucket;

		// Token: 0x04004E0F RID: 19983
		public OcclusionCulling.SmartList dynamicBucket;
	}

	// Token: 0x02000EF1 RID: 3825
	public struct Sphere
	{
		// Token: 0x060053DD RID: 21469 RVA: 0x001B3DDA File Offset: 0x001B1FDA
		public bool IsValid()
		{
			return this.radius > 0f;
		}

		// Token: 0x060053DE RID: 21470 RVA: 0x001B3DE9 File Offset: 0x001B1FE9
		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}

		// Token: 0x04004E10 RID: 19984
		public Vector3 position;

		// Token: 0x04004E11 RID: 19985
		public float radius;
	}

	// Token: 0x02000EF2 RID: 3826
	// (Invoke) Token: 0x060053E0 RID: 21472
	public delegate void OnVisibilityChanged(bool visible);
}
