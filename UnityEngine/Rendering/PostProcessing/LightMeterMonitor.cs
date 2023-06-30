using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7F RID: 2687
	[Serializable]
	public sealed class LightMeterMonitor : Monitor
	{
		// Token: 0x06003FFA RID: 16378 RVA: 0x00179CE9 File Offset: 0x00177EE9
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.shaders.lightMeter && context.resources.shaders.lightMeter.isSupported;
		}

		// Token: 0x06003FFB RID: 16379 RVA: 0x00179D1C File Offset: 0x00177F1C
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.width, this.height);
			LogHistogram logHistogram = context.logHistogram;
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lightMeter);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, logHistogram.data);
			Vector4 histogramScaleOffsetRes = logHistogram.GetHistogramScaleOffsetRes(context);
			histogramScaleOffsetRes.z = 1f / (float)this.width;
			histogramScaleOffsetRes.w = 1f / (float)this.height;
			propertySheet.properties.SetVector(ShaderIDs.ScaleOffsetRes, histogramScaleOffsetRes);
			if (context.logLut != null && this.showCurves)
			{
				propertySheet.EnableKeyword("COLOR_GRADING_HDR");
				propertySheet.properties.SetTexture(ShaderIDs.Lut3D, context.logLut);
			}
			AutoExposure autoExposure = context.autoExposure;
			if (autoExposure != null)
			{
				float num = autoExposure.filtering.value.x;
				float num2 = autoExposure.filtering.value.y;
				num2 = Mathf.Clamp(num2, 1.01f, 99f);
				num = Mathf.Clamp(num, 1f, num2 - 0.01f);
				Vector4 vector = new Vector4(num * 0.01f, num2 * 0.01f, RuntimeUtilities.Exp2(autoExposure.minLuminance.value), RuntimeUtilities.Exp2(autoExposure.maxLuminance.value));
				propertySheet.EnableKeyword("AUTO_EXPOSURE");
				propertySheet.properties.SetVector(ShaderIDs.Params, vector);
			}
			CommandBuffer command = context.command;
			command.BeginSample("LightMeter");
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("LightMeter");
		}

		// Token: 0x0400399C RID: 14748
		public int width = 512;

		// Token: 0x0400399D RID: 14749
		public int height = 256;

		// Token: 0x0400399E RID: 14750
		public bool showCurves = true;
	}
}
