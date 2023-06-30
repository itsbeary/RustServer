using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A1B RID: 2587
	public static class ImageEffectHelper
	{
		// Token: 0x06003D74 RID: 15732 RVA: 0x00168CAC File Offset: 0x00166EAC
		public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
		{
			if (s == null || !s.isSupported)
			{
				Debug.LogWarningFormat("Missing shader for image effect {0}", new object[] { effect });
				return false;
			}
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[] { effect });
				return false;
			}
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[] { effect });
				return false;
			}
			if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
			{
				Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[] { effect });
				return false;
			}
			return true;
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x001579BC File Offset: 0x00155BBC
		public static Material CheckShaderAndCreateMaterial(Shader s)
		{
			if (s == null || !s.isSupported)
			{
				return null;
			}
			return new Material(s)
			{
				hideFlags = HideFlags.DontSave
			};
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06003D76 RID: 15734 RVA: 0x001579DF File Offset: 0x00155BDF
		public static bool supportsDX11
		{
			get
			{
				return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			}
		}
	}
}
