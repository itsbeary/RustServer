using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BD RID: 2493
	public static class Noise3D
	{
		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06003B48 RID: 15176 RVA: 0x0015EBEC File Offset: 0x0015CDEC
		public static bool isSupported
		{
			get
			{
				if (!Noise3D.ms_IsSupportedChecked)
				{
					Noise3D.ms_IsSupported = SystemInfo.graphicsShaderLevel >= 35;
					if (!Noise3D.ms_IsSupported)
					{
						Debug.LogWarning(Noise3D.isNotSupportedString);
					}
					Noise3D.ms_IsSupportedChecked = true;
				}
				return Noise3D.ms_IsSupported;
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06003B49 RID: 15177 RVA: 0x0015EC22 File Offset: 0x0015CE22
		public static bool isProperlyLoaded
		{
			get
			{
				return Noise3D.ms_NoiseTexture != null;
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06003B4A RID: 15178 RVA: 0x0015EC2F File Offset: 0x0015CE2F
		public static string isNotSupportedString
		{
			get
			{
				return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", SystemInfo.graphicsShaderLevel, 35);
			}
		}

		// Token: 0x06003B4B RID: 15179 RVA: 0x0015EC4C File Offset: 0x0015CE4C
		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			Noise3D.LoadIfNeeded();
		}

		// Token: 0x06003B4C RID: 15180 RVA: 0x0015EC54 File Offset: 0x0015CE54
		public static void LoadIfNeeded()
		{
			if (!Noise3D.isSupported)
			{
				return;
			}
			if (Noise3D.ms_NoiseTexture == null)
			{
				Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Noise3D.ms_NoiseTexture)
				{
					Noise3D.ms_NoiseTexture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", Noise3D.ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		// Token: 0x06003B4D RID: 15181 RVA: 0x0015ECD0 File Offset: 0x0015CED0
		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			if (textData == null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] bytes = textData.bytes;
			Debug.Assert(bytes != null);
			int num = Mathf.Max(0, size * size * size);
			if (bytes.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[] { size });
				return null;
			}
			Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.Alpha8, false);
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Color32(0, 0, 0, bytes[i]);
			}
			texture3D.SetPixels(array);
			texture3D.Apply();
			return texture3D;
		}

		// Token: 0x04003649 RID: 13897
		private static bool ms_IsSupportedChecked;

		// Token: 0x0400364A RID: 13898
		private static bool ms_IsSupported;

		// Token: 0x0400364B RID: 13899
		private static Texture3D ms_NoiseTexture;

		// Token: 0x0400364C RID: 13900
		private const HideFlags kHideFlags = HideFlags.HideAndDontSave;

		// Token: 0x0400364D RID: 13901
		private const int kMinShaderLevel = 35;
	}
}
