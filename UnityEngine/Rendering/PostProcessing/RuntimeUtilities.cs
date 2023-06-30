using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA5 RID: 2725
	public static class RuntimeUtilities
	{
		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x060040CD RID: 16589 RVA: 0x0017D340 File Offset: 0x0017B540
		public static Texture2D whiteTexture
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture == null)
				{
					RuntimeUtilities.m_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture"
					};
					RuntimeUtilities.m_WhiteTexture.SetPixel(0, 0, Color.white);
					RuntimeUtilities.m_WhiteTexture.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x060040CE RID: 16590 RVA: 0x0017D394 File Offset: 0x0017B594
		public static Texture3D whiteTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture3D == null)
				{
					RuntimeUtilities.m_WhiteTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture 3D"
					};
					RuntimeUtilities.m_WhiteTexture3D.SetPixels(new Color[] { Color.white });
					RuntimeUtilities.m_WhiteTexture3D.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture3D;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x060040CF RID: 16591 RVA: 0x0017D3F4 File Offset: 0x0017B5F4
		public static Texture2D blackTexture
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture == null)
				{
					RuntimeUtilities.m_BlackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture"
					};
					RuntimeUtilities.m_BlackTexture.SetPixel(0, 0, Color.black);
					RuntimeUtilities.m_BlackTexture.Apply();
				}
				return RuntimeUtilities.m_BlackTexture;
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060040D0 RID: 16592 RVA: 0x0017D448 File Offset: 0x0017B648
		public static Texture3D blackTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture3D == null)
				{
					RuntimeUtilities.m_BlackTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture 3D"
					};
					RuntimeUtilities.m_BlackTexture3D.SetPixels(new Color[] { Color.black });
					RuntimeUtilities.m_BlackTexture3D.Apply();
				}
				return RuntimeUtilities.m_BlackTexture3D;
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060040D1 RID: 16593 RVA: 0x0017D4A8 File Offset: 0x0017B6A8
		public static Texture2D transparentTexture
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture == null)
				{
					RuntimeUtilities.m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture"
					};
					RuntimeUtilities.m_TransparentTexture.SetPixel(0, 0, Color.clear);
					RuntimeUtilities.m_TransparentTexture.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060040D2 RID: 16594 RVA: 0x0017D4FC File Offset: 0x0017B6FC
		public static Texture3D transparentTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture3D == null)
				{
					RuntimeUtilities.m_TransparentTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture 3D"
					};
					RuntimeUtilities.m_TransparentTexture3D.SetPixels(new Color[] { Color.clear });
					RuntimeUtilities.m_TransparentTexture3D.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture3D;
			}
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x0017D55C File Offset: 0x0017B75C
		public static Texture2D GetLutStrip(int size)
		{
			Texture2D texture2D;
			if (!RuntimeUtilities.m_LutStrips.TryGetValue(size, out texture2D))
			{
				int num = size * size;
				Color[] array = new Color[num * size];
				float num2 = 1f / ((float)size - 1f);
				for (int i = 0; i < size; i++)
				{
					int num3 = i * size;
					float num4 = (float)i * num2;
					for (int j = 0; j < size; j++)
					{
						float num5 = (float)j * num2;
						for (int k = 0; k < size; k++)
						{
							float num6 = (float)k * num2;
							array[j * num + num3 + k] = new Color(num6, num5, num4);
						}
					}
				}
				TextureFormat textureFormat = TextureFormat.RGBAHalf;
				if (!textureFormat.IsSupported())
				{
					textureFormat = TextureFormat.ARGB32;
				}
				texture2D = new Texture2D(size * size, size, textureFormat, false, true)
				{
					name = "Strip Lut" + size,
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0
				};
				texture2D.SetPixels(array);
				texture2D.Apply();
				RuntimeUtilities.m_LutStrips.Add(size, texture2D);
			}
			return texture2D;
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060040D4 RID: 16596 RVA: 0x0017D670 File Offset: 0x0017B870
		public static Mesh fullscreenTriangle
		{
			get
			{
				if (RuntimeUtilities.s_FullscreenTriangle != null)
				{
					return RuntimeUtilities.s_FullscreenTriangle;
				}
				RuntimeUtilities.s_FullscreenTriangle = new Mesh
				{
					name = "Fullscreen Triangle"
				};
				RuntimeUtilities.s_FullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				RuntimeUtilities.s_FullscreenTriangle.SetIndices(new int[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
				RuntimeUtilities.s_FullscreenTriangle.UploadMeshData(false);
				return RuntimeUtilities.s_FullscreenTriangle;
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060040D5 RID: 16597 RVA: 0x0017D730 File Offset: 0x0017B930
		public static Material copyStdMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStd)
				{
					name = "PostProcess - CopyStd",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdMaterial;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060040D6 RID: 16598 RVA: 0x0017D78C File Offset: 0x0017B98C
		public static Material copyStdFromDoubleWideMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdFromDoubleWideMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdFromDoubleWideMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromDoubleWide)
				{
					name = "PostProcess - CopyStdFromDoubleWide",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060040D7 RID: 16599 RVA: 0x0017D7E8 File Offset: 0x0017B9E8
		public static Material copyMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyMaterial != null)
				{
					return RuntimeUtilities.s_CopyMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copy)
				{
					name = "PostProcess - Copy",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyMaterial;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060040D8 RID: 16600 RVA: 0x0017D844 File Offset: 0x0017BA44
		public static Material copyFromTexArrayMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArrayMaterial != null)
				{
					return RuntimeUtilities.s_CopyFromTexArrayMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyFromTexArrayMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromTexArray)
				{
					name = "PostProcess - CopyFromTexArray",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyFromTexArrayMaterial;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060040D9 RID: 16601 RVA: 0x0017D89F File Offset: 0x0017BA9F
		public static PropertySheet copySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopySheet == null)
				{
					RuntimeUtilities.s_CopySheet = new PropertySheet(RuntimeUtilities.copyMaterial);
				}
				return RuntimeUtilities.s_CopySheet;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060040DA RID: 16602 RVA: 0x0017D8BC File Offset: 0x0017BABC
		public static PropertySheet copyFromTexArraySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArraySheet == null)
				{
					RuntimeUtilities.s_CopyFromTexArraySheet = new PropertySheet(RuntimeUtilities.copyFromTexArrayMaterial);
				}
				return RuntimeUtilities.s_CopyFromTexArraySheet;
			}
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x0017D8D9 File Offset: 0x0017BAD9
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			cmd.SetRenderTarget(rt, loadAction, storeAction);
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0017D8E4 File Offset: 0x0017BAE4
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			cmd.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x0017D8F8 File Offset: 0x0017BAF8
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, RuntimeUtilities.copyMaterial, 0, 0);
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x0017D964 File Offset: 0x0017BB64
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, RenderBufferLoadAction loadAction, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			bool flag = loadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				loadAction = RenderBufferLoadAction.DontCare;
			}
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? loadAction : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (flag)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040DF RID: 16607 RVA: 0x0017D9E2 File Offset: 0x0017BBE2
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, pass, clear ? RenderBufferLoadAction.Clear : RenderBufferLoadAction.DontCare, viewport);
		}

		// Token: 0x060040E0 RID: 16608 RVA: 0x0017D9FC File Offset: 0x0017BBFC
		public static void BlitFullscreenTriangleFromDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass, int eye)
		{
			Vector4 vector = new Vector4(0.5f, 1f, 0f, 0f);
			if (eye == 1)
			{
				vector.z = 0.5f;
			}
			cmd.SetGlobalVector(ShaderIDs.UVScaleOffset, vector);
			cmd.BuiltinBlit(source, destination, material, pass);
		}

		// Token: 0x060040E1 RID: 16609 RVA: 0x0017DA4C File Offset: 0x0017BC4C
		public static void BlitFullscreenTriangleToDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, int eye)
		{
			Vector4 vector = new Vector4(0.5f, 1f, -0.5f, 0f);
			if (eye == 1)
			{
				vector.z = 0.5f;
			}
			propertySheet.EnableKeyword("STEREO_DOUBLEWIDE_TARGET");
			propertySheet.properties.SetVector(ShaderIDs.PosScaleOffset, vector);
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, 0, false, null);
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x0017DAB8 File Offset: 0x0017BCB8
		public static void BlitFullscreenTriangleFromTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x0017DB18 File Offset: 0x0017BD18
		public static void BlitFullscreenTriangleToTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x0017DB7C File Offset: 0x0017BD7C
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			RenderBufferLoadAction renderBufferLoadAction = ((viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load);
			if (clear)
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, renderBufferLoadAction, RenderBufferStoreAction.Store);
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			else
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			}
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040E5 RID: 16613 RVA: 0x0017DC08 File Offset: 0x0017BE08
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destinations, depth);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x0017DC71 File Offset: 0x0017BE71
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination);
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x0017DC8C File Offset: 0x0017BE8C
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass = 0)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination, mat, pass);
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0017DCAC File Offset: 0x0017BEAC
		public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
			{
				cmd.CopyTexture(source, destination);
				return;
			}
			cmd.BlitFullscreenTriangle(source, destination, false, null);
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060040E9 RID: 16617 RVA: 0x0017DCDC File Offset: 0x0017BEDC
		public static bool scriptableRenderPipelineActive
		{
			get
			{
				return GraphicsSettings.renderPipelineAsset != null;
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060040EA RID: 16618 RVA: 0x0017DCE9 File Offset: 0x0017BEE9
		public static bool supportsDeferredShading
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredShading) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060040EB RID: 16619 RVA: 0x0017DCFD File Offset: 0x0017BEFD
		public static bool supportsDepthNormals
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DepthNormals) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060040EC RID: 16620 RVA: 0x00007A44 File Offset: 0x00005C44
		public static bool isSinglePassStereoEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060040ED RID: 16621 RVA: 0x00007A44 File Offset: 0x00005C44
		public static bool isVREnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060040EE RID: 16622 RVA: 0x0017DD11 File Offset: 0x0017BF11
		public static bool isAndroidOpenGL
		{
			get
			{
				return Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060040EF RID: 16623 RVA: 0x00041065 File Offset: 0x0003F265
		public static RenderTextureFormat defaultHDRRenderTextureFormat
		{
			get
			{
				return RenderTextureFormat.DefaultHDR;
			}
		}

		// Token: 0x060040F0 RID: 16624 RVA: 0x0017DD2A File Offset: 0x0017BF2A
		public static bool isFloatingPointFormat(RenderTextureFormat format)
		{
			return format == RenderTextureFormat.DefaultHDR || format == RenderTextureFormat.ARGBHalf || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.RGFloat || format == RenderTextureFormat.RGHalf || format == RenderTextureFormat.RFloat || format == RenderTextureFormat.RHalf || format == RenderTextureFormat.RGB111110Float;
		}

		// Token: 0x060040F1 RID: 16625 RVA: 0x0017DD55 File Offset: 0x0017BF55
		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060040F2 RID: 16626 RVA: 0x0017DD66 File Offset: 0x0017BF66
		public static bool isLinearColorSpace
		{
			get
			{
				return QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
		}

		// Token: 0x060040F3 RID: 16627 RVA: 0x0017DD70 File Offset: 0x0017BF70
		public static bool IsResolvedDepthAvailable(Camera camera)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			return camera.actualRenderingPath == RenderingPath.DeferredShading && (graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne);
		}

		// Token: 0x060040F4 RID: 16628 RVA: 0x0017DDA0 File Offset: 0x0017BFA0
		public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
		{
			if (destroyEffects)
			{
				foreach (PostProcessEffectSettings postProcessEffectSettings in profile.settings)
				{
					RuntimeUtilities.Destroy(postProcessEffectSettings);
				}
			}
			RuntimeUtilities.Destroy(profile);
		}

		// Token: 0x060040F5 RID: 16629 RVA: 0x0017DDFC File Offset: 0x0017BFFC
		public static void DestroyVolume(PostProcessVolume volume, bool destroyProfile, bool destroyGameObject = false)
		{
			if (destroyProfile)
			{
				RuntimeUtilities.DestroyProfile(volume.profileRef, true);
			}
			GameObject gameObject = volume.gameObject;
			RuntimeUtilities.Destroy(volume);
			if (destroyGameObject)
			{
				RuntimeUtilities.Destroy(gameObject);
			}
		}

		// Token: 0x060040F6 RID: 16630 RVA: 0x0017DE2E File Offset: 0x0017C02E
		public static bool IsPostProcessingActive(PostProcessLayer layer)
		{
			return layer != null && layer.enabled;
		}

		// Token: 0x060040F7 RID: 16631 RVA: 0x0017DE41 File Offset: 0x0017C041
		public static bool IsTemporalAntialiasingActive(PostProcessLayer layer)
		{
			return RuntimeUtilities.IsPostProcessingActive(layer) && layer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing && layer.temporalAntialiasing.IsSupported();
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x0017DE61 File Offset: 0x0017C061
		public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
		{
			Queue<Transform> queue = new Queue<Transform>();
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				queue.Enqueue(gameObject.transform);
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					yield return component;
				}
			}
			GameObject[] array = null;
			while (queue.Count > 0)
			{
				foreach (object obj in queue.Dequeue())
				{
					Transform transform = (Transform)obj;
					queue.Enqueue(transform);
					T component2 = transform.GetComponent<T>();
					if (component2 != null)
					{
						yield return component2;
					}
				}
				IEnumerator enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x060040F9 RID: 16633 RVA: 0x0017DE6A File Offset: 0x0017C06A
		public static void CreateIfNull<T>(ref T obj) where T : class, new()
		{
			if (obj == null)
			{
				obj = new T();
			}
		}

		// Token: 0x060040FA RID: 16634 RVA: 0x0017DE84 File Offset: 0x0017C084
		public static float Exp2(float x)
		{
			return Mathf.Exp(x * 0.6931472f);
		}

		// Token: 0x060040FB RID: 16635 RVA: 0x0017DE94 File Offset: 0x0017C094
		public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
		{
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			float num = Mathf.Tan(0.008726646f * camera.fieldOfView) * nearClipPlane;
			float num2 = num * camera.aspect;
			offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
			offset.y *= num / (0.5f * (float)camera.pixelHeight);
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			ref Matrix4x4 ptr = ref projectionMatrix;
			ptr[0, 2] = ptr[0, 2] + offset.x / num2;
			ptr = ref projectionMatrix;
			ptr[1, 2] = ptr[1, 2] + offset.y / num;
			return projectionMatrix;
		}

		// Token: 0x060040FC RID: 16636 RVA: 0x0017DF48 File Offset: 0x0017C148
		public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
		{
			float orthographicSize = camera.orthographicSize;
			float num = orthographicSize * camera.aspect;
			offset.x *= num / (0.5f * (float)camera.pixelWidth);
			offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
			float num2 = offset.x - num;
			float num3 = offset.x + num;
			float num4 = offset.y + orthographicSize;
			float num5 = offset.y - orthographicSize;
			return Matrix4x4.Ortho(num2, num3, num5, num4, camera.nearClipPlane, camera.farClipPlane);
		}

		// Token: 0x060040FD RID: 16637 RVA: 0x0017DFD4 File Offset: 0x0017C1D4
		public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
		{
			FrustumPlanes decomposeProjection = origProj.decomposeProjection;
			float num = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
			float num2 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
			Vector2 vector = new Vector2(jitter.x * num2 / (float)context.screenWidth, jitter.y * num / (float)context.screenHeight);
			decomposeProjection.left += vector.x;
			decomposeProjection.right += vector.x;
			decomposeProjection.top += vector.y;
			decomposeProjection.bottom += vector.y;
			return Matrix4x4.Frustum(decomposeProjection);
		}

		// Token: 0x060040FE RID: 16638 RVA: 0x0017E08C File Offset: 0x0017C28C
		public static IEnumerable<Type> GetAllAssemblyTypes()
		{
			if (RuntimeUtilities.m_AssemblyTypes == null)
			{
				RuntimeUtilities.m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
				{
					Type[] array = new Type[0];
					try
					{
						array = t.GetTypes();
					}
					catch
					{
					}
					return array;
				});
			}
			return RuntimeUtilities.m_AssemblyTypes;
		}

		// Token: 0x060040FF RID: 16639 RVA: 0x0017E0D8 File Offset: 0x0017C2D8
		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
			return (T)((object)type.GetCustomAttributes(typeof(T), false)[0]);
		}

		// Token: 0x06004100 RID: 16640 RVA: 0x0017E110 File Offset: 0x0017C310
		public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			Expression expression = expr;
			if (expression is LambdaExpression)
			{
				expression = ((LambdaExpression)expression).Body;
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				return ((FieldInfo)((MemberExpression)expression).Member).GetCustomAttributes(false).Cast<Attribute>().ToArray<Attribute>();
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x0017E168 File Offset: 0x0017C368
		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			ExpressionType nodeType = expr.Body.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = expr.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = memberExpression.Expression as MemberExpression;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = list.Count - 1; i >= 0; i--)
				{
					stringBuilder.Append(list[i]);
					if (i > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}

		// Token: 0x04003A17 RID: 14871
		private static Texture2D m_WhiteTexture;

		// Token: 0x04003A18 RID: 14872
		private static Texture3D m_WhiteTexture3D;

		// Token: 0x04003A19 RID: 14873
		private static Texture2D m_BlackTexture;

		// Token: 0x04003A1A RID: 14874
		private static Texture3D m_BlackTexture3D;

		// Token: 0x04003A1B RID: 14875
		private static Texture2D m_TransparentTexture;

		// Token: 0x04003A1C RID: 14876
		private static Texture3D m_TransparentTexture3D;

		// Token: 0x04003A1D RID: 14877
		private static Dictionary<int, Texture2D> m_LutStrips = new Dictionary<int, Texture2D>();

		// Token: 0x04003A1E RID: 14878
		internal static PostProcessResources s_Resources;

		// Token: 0x04003A1F RID: 14879
		private static Mesh s_FullscreenTriangle;

		// Token: 0x04003A20 RID: 14880
		private static Material s_CopyStdMaterial;

		// Token: 0x04003A21 RID: 14881
		private static Material s_CopyStdFromDoubleWideMaterial;

		// Token: 0x04003A22 RID: 14882
		private static Material s_CopyMaterial;

		// Token: 0x04003A23 RID: 14883
		private static Material s_CopyFromTexArrayMaterial;

		// Token: 0x04003A24 RID: 14884
		private static PropertySheet s_CopySheet;

		// Token: 0x04003A25 RID: 14885
		private static PropertySheet s_CopyFromTexArraySheet;

		// Token: 0x04003A26 RID: 14886
		private static IEnumerable<Type> m_AssemblyTypes;
	}
}
