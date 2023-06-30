using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7E RID: 2686
	[Serializable]
	public sealed class HistogramMonitor : Monitor
	{
		// Token: 0x06003FF5 RID: 16373 RVA: 0x00179AD7 File Offset: 0x00177CD7
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003FF6 RID: 16374 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x00179AF9 File Offset: 0x00177CF9
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.gammaHistogram;
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x00179B10 File Offset: 0x00177D10
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.width, this.height);
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(256, 4);
			}
			ComputeShader gammaHistogram = context.resources.computeShaders.gammaHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("GammaHistogram");
			int num = gammaHistogram.FindKernel("KHistogramClear");
			command.SetComputeBufferParam(gammaHistogram, num, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, num, Mathf.CeilToInt(16f), 1, 1);
			num = gammaHistogram.FindKernel("KHistogramGather");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), (float)this.channel);
			command.SetComputeVectorParam(gammaHistogram, "_Params", vector);
			command.SetComputeTextureParam(gammaHistogram, num, "_Source", ShaderIDs.HalfResFinalCopy);
			command.SetComputeBufferParam(gammaHistogram, num, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, num, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.gammaHistogram);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.width, (float)this.height, 0f, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("GammaHistogram");
		}

		// Token: 0x04003995 RID: 14741
		public int width = 512;

		// Token: 0x04003996 RID: 14742
		public int height = 256;

		// Token: 0x04003997 RID: 14743
		public HistogramMonitor.Channel channel = HistogramMonitor.Channel.Master;

		// Token: 0x04003998 RID: 14744
		private ComputeBuffer m_Data;

		// Token: 0x04003999 RID: 14745
		private const int k_NumBins = 256;

		// Token: 0x0400399A RID: 14746
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x0400399B RID: 14747
		private const int k_ThreadGroupSizeY = 16;

		// Token: 0x02000F3F RID: 3903
		public enum Channel
		{
			// Token: 0x04004F5B RID: 20315
			Red,
			// Token: 0x04004F5C RID: 20316
			Green,
			// Token: 0x04004F5D RID: 20317
			Blue,
			// Token: 0x04004F5E RID: 20318
			Master
		}
	}
}
