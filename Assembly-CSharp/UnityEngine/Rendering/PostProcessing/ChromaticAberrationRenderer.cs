using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5C RID: 2652
	[Preserve]
	internal sealed class ChromaticAberrationRenderer : PostProcessEffectRenderer<ChromaticAberration>
	{
		// Token: 0x06003F7D RID: 16253 RVA: 0x001743EC File Offset: 0x001725EC
		public override void Render(PostProcessRenderContext context)
		{
			Texture texture = base.settings.spectralLut.value;
			if (texture == null)
			{
				if (this.m_InternalSpectralLut == null)
				{
					this.m_InternalSpectralLut = new Texture2D(3, 1, TextureFormat.RGB24, false)
					{
						name = "Chromatic Aberration Spectrum Lookup",
						filterMode = FilterMode.Bilinear,
						wrapMode = TextureWrapMode.Clamp,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
					this.m_InternalSpectralLut.SetPixels(new Color[]
					{
						new Color(1f, 0f, 0f),
						new Color(0f, 1f, 0f),
						new Color(0f, 0f, 1f)
					});
					this.m_InternalSpectralLut.Apply();
				}
				texture = this.m_InternalSpectralLut;
			}
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword((base.settings.fastMode || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2) ? "CHROMATIC_ABERRATION_LOW" : "CHROMATIC_ABERRATION");
			uberSheet.properties.SetFloat(ShaderIDs.ChromaticAberration_Amount, base.settings.intensity * 0.05f);
			uberSheet.properties.SetTexture(ShaderIDs.ChromaticAberration_SpectralLut, texture);
		}

		// Token: 0x06003F7E RID: 16254 RVA: 0x00174541 File Offset: 0x00172741
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_InternalSpectralLut);
			this.m_InternalSpectralLut = null;
		}

		// Token: 0x040038F8 RID: 14584
		private Texture2D m_InternalSpectralLut;
	}
}
