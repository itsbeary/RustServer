using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA1 RID: 2721
	internal sealed class LogHistogram
	{
		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x060040B6 RID: 16566 RVA: 0x0017CEFA File Offset: 0x0017B0FA
		// (set) Token: 0x060040B7 RID: 16567 RVA: 0x0017CF02 File Offset: 0x0017B102
		public ComputeBuffer data { get; private set; }

		// Token: 0x060040B8 RID: 16568 RVA: 0x0017CF0C File Offset: 0x0017B10C
		public void Generate(PostProcessRenderContext context)
		{
			if (this.data == null)
			{
				this.data = new ComputeBuffer(128, 4);
			}
			Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes(context);
			ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("LogHistogram");
			int num = exposureHistogram.FindKernel("KEyeHistogramClear");
			command.SetComputeBufferParam(exposureHistogram, num, "_HistogramBuffer", this.data);
			uint num2;
			uint num3;
			uint num4;
			exposureHistogram.GetKernelThreadGroupSizes(num, out num2, out num3, out num4);
			command.DispatchCompute(exposureHistogram, num, Mathf.CeilToInt(128f / num2), 1, 1);
			num = exposureHistogram.FindKernel("KEyeHistogram");
			command.SetComputeBufferParam(exposureHistogram, num, "_HistogramBuffer", this.data);
			command.SetComputeTextureParam(exposureHistogram, num, "_Source", context.source);
			command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
			exposureHistogram.GetKernelThreadGroupSizes(num, out num2, out num3, out num4);
			command.DispatchCompute(exposureHistogram, num, Mathf.CeilToInt(histogramScaleOffsetRes.z / 2f / num2), Mathf.CeilToInt(histogramScaleOffsetRes.w / 2f / num3), 1);
			command.EndSample("LogHistogram");
		}

		// Token: 0x060040B9 RID: 16569 RVA: 0x0017D040 File Offset: 0x0017B240
		public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
		{
			float num = 18f;
			float num2 = 1f / num;
			float num3 = 9f * num2;
			return new Vector4(num2, num3, (float)context.width, (float)context.height);
		}

		// Token: 0x060040BA RID: 16570 RVA: 0x0017D078 File Offset: 0x0017B278
		public void Release()
		{
			if (this.data != null)
			{
				this.data.Release();
			}
			this.data = null;
		}

		// Token: 0x04003A0E RID: 14862
		public const int rangeMin = -9;

		// Token: 0x04003A0F RID: 14863
		public const int rangeMax = 9;

		// Token: 0x04003A10 RID: 14864
		private const int k_Bins = 128;
	}
}
