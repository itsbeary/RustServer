using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A78 RID: 2680
	[Preserve]
	[Serializable]
	public sealed class SubpixelMorphologicalAntialiasing
	{
		// Token: 0x06003FD9 RID: 16345 RVA: 0x00179054 File Offset: 0x00177254
		public bool IsSupported()
		{
			return !RuntimeUtilities.isSinglePassStereoEnabled;
		}

		// Token: 0x06003FDA RID: 16346 RVA: 0x00179060 File Offset: 0x00177260
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.subpixelMorphologicalAntialiasing);
			propertySheet.properties.SetTexture("_AreaTex", context.resources.smaaLuts.area);
			propertySheet.properties.SetTexture("_SearchTex", context.resources.smaaLuts.search);
			CommandBuffer command = context.command;
			command.BeginSample("SubpixelMorphologicalAntialiasing");
			command.GetTemporaryRT(ShaderIDs.SMAA_Flip, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.GetTemporaryRT(ShaderIDs.SMAA_Flop, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.SMAA_Flip, propertySheet, (int)this.quality, true, null);
			command.BlitFullscreenTriangle(ShaderIDs.SMAA_Flip, ShaderIDs.SMAA_Flop, propertySheet, (int)(3 + this.quality), false, null);
			command.SetGlobalTexture("_BlendTex", ShaderIDs.SMAA_Flop);
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 6, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flip);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flop);
			command.EndSample("SubpixelMorphologicalAntialiasing");
		}

		// Token: 0x04003979 RID: 14713
		[Tooltip("Lower quality is faster at the expense of visual quality (Low = ~60%, Medium = ~80%).")]
		public SubpixelMorphologicalAntialiasing.Quality quality = SubpixelMorphologicalAntialiasing.Quality.High;

		// Token: 0x02000F3C RID: 3900
		private enum Pass
		{
			// Token: 0x04004F50 RID: 20304
			EdgeDetection,
			// Token: 0x04004F51 RID: 20305
			BlendWeights = 3,
			// Token: 0x04004F52 RID: 20306
			NeighborhoodBlending = 6
		}

		// Token: 0x02000F3D RID: 3901
		public enum Quality
		{
			// Token: 0x04004F54 RID: 20308
			Low,
			// Token: 0x04004F55 RID: 20309
			Medium,
			// Token: 0x04004F56 RID: 20310
			High
		}
	}
}
