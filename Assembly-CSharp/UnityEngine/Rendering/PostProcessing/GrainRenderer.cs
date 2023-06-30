using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6B RID: 2667
	[Preserve]
	internal sealed class GrainRenderer : PostProcessEffectRenderer<Grain>
	{
		// Token: 0x06003FA4 RID: 16292 RVA: 0x001767E0 File Offset: 0x001749E0
		public override void Render(PostProcessRenderContext context)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = HaltonSeq.Get(this.m_SampleIndex & 1023, 2);
			float num2 = HaltonSeq.Get(this.m_SampleIndex & 1023, 3);
			int num3 = this.m_SampleIndex + 1;
			this.m_SampleIndex = num3;
			if (num3 >= 1024)
			{
				this.m_SampleIndex = 0;
			}
			if (this.m_GrainLookupRT == null || !this.m_GrainLookupRT.IsCreated())
			{
				RuntimeUtilities.Destroy(this.m_GrainLookupRT);
				this.m_GrainLookupRT = new RenderTexture(128, 128, 0, this.GetLookupFormat())
				{
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Repeat,
					anisoLevel = 0,
					name = "Grain Lookup Texture"
				};
				this.m_GrainLookupRT.Create();
			}
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.grainBaker);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Phase, realtimeSinceStartup % 10f);
			propertySheet.properties.SetVector(ShaderIDs.GrainNoiseParameters, new Vector3(12.9898f, 78.233f, 43758.547f));
			context.command.BeginSample("GrainLookup");
			context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_GrainLookupRT, propertySheet, base.settings.colored.value ? 1 : 0, false, null);
			context.command.EndSample("GrainLookup");
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("GRAIN");
			uberSheet.properties.SetTexture(ShaderIDs.GrainTex, this.m_GrainLookupRT);
			uberSheet.properties.SetVector(ShaderIDs.Grain_Params1, new Vector2(base.settings.lumContrib.value, base.settings.intensity.value * 20f));
			uberSheet.properties.SetVector(ShaderIDs.Grain_Params2, new Vector4((float)context.width / (float)this.m_GrainLookupRT.width / base.settings.size.value, (float)context.height / (float)this.m_GrainLookupRT.height / base.settings.size.value, num, num2));
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x00176A35 File Offset: 0x00174C35
		private RenderTextureFormat GetLookupFormat()
		{
			if (RenderTextureFormat.ARGBHalf.IsSupported())
			{
				return RenderTextureFormat.ARGBHalf;
			}
			return RenderTextureFormat.ARGB32;
		}

		// Token: 0x06003FA6 RID: 16294 RVA: 0x00176A42 File Offset: 0x00174C42
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_GrainLookupRT);
			this.m_GrainLookupRT = null;
			this.m_SampleIndex = 0;
		}

		// Token: 0x04003947 RID: 14663
		private RenderTexture m_GrainLookupRT;

		// Token: 0x04003948 RID: 14664
		private const int k_SampleCount = 1024;

		// Token: 0x04003949 RID: 14665
		private int m_SampleIndex;
	}
}
