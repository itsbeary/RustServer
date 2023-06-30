using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA9 RID: 2729
	public static class TextureFormatUtilities
	{
		// Token: 0x0600410D RID: 16653 RVA: 0x0017EBFC File Offset: 0x0017CDFC
		static TextureFormatUtilities()
		{
			foreach (object obj in Enum.GetValues(typeof(RenderTextureFormat)))
			{
				if ((int)obj >= 0 && !TextureFormatUtilities.IsObsolete(obj))
				{
					bool flag = SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)obj);
					TextureFormatUtilities.s_SupportedRenderTextureFormats[(int)obj] = flag;
				}
			}
			TextureFormatUtilities.s_SupportedTextureFormats = new Dictionary<int, bool>();
			foreach (object obj2 in Enum.GetValues(typeof(TextureFormat)))
			{
				if ((int)obj2 >= 0 && !TextureFormatUtilities.IsObsolete(obj2))
				{
					bool flag2 = SystemInfo.SupportsTextureFormat((TextureFormat)obj2);
					TextureFormatUtilities.s_SupportedTextureFormats[(int)obj2] = flag2;
				}
			}
		}

		// Token: 0x0600410E RID: 16654 RVA: 0x0017EE70 File Offset: 0x0017D070
		private static bool IsObsolete(object value)
		{
			ObsoleteAttribute[] array = (ObsoleteAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return array != null && array.Length != 0;
		}

		// Token: 0x0600410F RID: 16655 RVA: 0x0017EEB0 File Offset: 0x0017D0B0
		public static RenderTextureFormat GetUncompressedRenderTextureFormat(Texture texture)
		{
			Assert.IsNotNull<Texture>(texture);
			if (texture is RenderTexture)
			{
				return (texture as RenderTexture).format;
			}
			if (!(texture is Texture2D))
			{
				return RenderTextureFormat.Default;
			}
			TextureFormat format = ((Texture2D)texture).format;
			RenderTextureFormat renderTextureFormat;
			if (!TextureFormatUtilities.s_FormatAliasMap.TryGetValue((int)format, out renderTextureFormat))
			{
				throw new NotSupportedException("Texture format not supported");
			}
			return renderTextureFormat;
		}

		// Token: 0x06004110 RID: 16656 RVA: 0x0017EF08 File Offset: 0x0017D108
		internal static bool IsSupported(this RenderTextureFormat format)
		{
			bool flag;
			TextureFormatUtilities.s_SupportedRenderTextureFormats.TryGetValue((int)format, out flag);
			return flag;
		}

		// Token: 0x06004111 RID: 16657 RVA: 0x0017EF24 File Offset: 0x0017D124
		internal static bool IsSupported(this TextureFormat format)
		{
			bool flag;
			TextureFormatUtilities.s_SupportedTextureFormats.TryGetValue((int)format, out flag);
			return flag;
		}

		// Token: 0x04003AB4 RID: 15028
		private static Dictionary<int, RenderTextureFormat> s_FormatAliasMap = new Dictionary<int, RenderTextureFormat>
		{
			{
				1,
				RenderTextureFormat.ARGB32
			},
			{
				2,
				RenderTextureFormat.ARGB4444
			},
			{
				3,
				RenderTextureFormat.ARGB32
			},
			{
				4,
				RenderTextureFormat.ARGB32
			},
			{
				5,
				RenderTextureFormat.ARGB32
			},
			{
				7,
				RenderTextureFormat.RGB565
			},
			{
				9,
				RenderTextureFormat.RHalf
			},
			{
				10,
				RenderTextureFormat.ARGB32
			},
			{
				12,
				RenderTextureFormat.ARGB32
			},
			{
				13,
				RenderTextureFormat.ARGB4444
			},
			{
				14,
				RenderTextureFormat.ARGB32
			},
			{
				15,
				RenderTextureFormat.RHalf
			},
			{
				16,
				RenderTextureFormat.RGHalf
			},
			{
				17,
				RenderTextureFormat.ARGBHalf
			},
			{
				18,
				RenderTextureFormat.RFloat
			},
			{
				19,
				RenderTextureFormat.RGFloat
			},
			{
				20,
				RenderTextureFormat.ARGBFloat
			},
			{
				22,
				RenderTextureFormat.ARGBHalf
			},
			{
				26,
				RenderTextureFormat.R8
			},
			{
				27,
				RenderTextureFormat.RGHalf
			},
			{
				24,
				RenderTextureFormat.ARGBHalf
			},
			{
				25,
				RenderTextureFormat.ARGB32
			},
			{
				28,
				RenderTextureFormat.ARGB32
			},
			{
				29,
				RenderTextureFormat.ARGB32
			},
			{
				30,
				RenderTextureFormat.ARGB32
			},
			{
				31,
				RenderTextureFormat.ARGB32
			},
			{
				32,
				RenderTextureFormat.ARGB32
			},
			{
				33,
				RenderTextureFormat.ARGB32
			},
			{
				34,
				RenderTextureFormat.ARGB32
			},
			{
				45,
				RenderTextureFormat.ARGB32
			},
			{
				46,
				RenderTextureFormat.ARGB32
			},
			{
				47,
				RenderTextureFormat.ARGB32
			},
			{
				48,
				RenderTextureFormat.ARGB32
			},
			{
				49,
				RenderTextureFormat.ARGB32
			},
			{
				50,
				RenderTextureFormat.ARGB32
			},
			{
				51,
				RenderTextureFormat.ARGB32
			},
			{
				52,
				RenderTextureFormat.ARGB32
			},
			{
				53,
				RenderTextureFormat.ARGB32
			}
		};

		// Token: 0x04003AB5 RID: 15029
		private static Dictionary<int, bool> s_SupportedRenderTextureFormats = new Dictionary<int, bool>();

		// Token: 0x04003AB6 RID: 15030
		private static Dictionary<int, bool> s_SupportedTextureFormats;
	}
}
