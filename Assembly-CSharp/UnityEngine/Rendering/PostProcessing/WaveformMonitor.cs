using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A83 RID: 2691
	[Serializable]
	public sealed class WaveformMonitor : Monitor
	{
		// Token: 0x0600400C RID: 16396 RVA: 0x0017A234 File Offset: 0x00178434
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x0600400E RID: 16398 RVA: 0x0017A256 File Offset: 0x00178456
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.waveform;
		}

		// Token: 0x0600400F RID: 16399 RVA: 0x0017A270 File Offset: 0x00178470
		internal override void Render(PostProcessRenderContext context)
		{
			float num = (float)context.width / 2f / ((float)context.height / 2f);
			int num2 = Mathf.FloorToInt((float)this.height * num);
			base.CheckOutput(num2, this.height);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num3 = num2 * this.height;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			else if (this.m_Data.count < num3)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			ComputeShader waveform = context.resources.computeShaders.waveform;
			CommandBuffer command = context.command;
			command.BeginSample("Waveform");
			Vector4 vector = new Vector4((float)num2, (float)this.height, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), 0f);
			int num4 = waveform.FindKernel("KWaveformClear");
			command.SetComputeBufferParam(waveform, num4, "_WaveformBuffer", this.m_Data);
			command.SetComputeVectorParam(waveform, "_Params", vector);
			command.DispatchCompute(waveform, num4, Mathf.CeilToInt((float)num2 / 16f), Mathf.CeilToInt((float)this.height / 16f), 1);
			command.GetTemporaryRT(ShaderIDs.WaveformSource, num2, this.height, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(ShaderIDs.HalfResFinalCopy, ShaderIDs.WaveformSource, false, null);
			num4 = waveform.FindKernel("KWaveformGather");
			command.SetComputeBufferParam(waveform, num4, "_WaveformBuffer", this.m_Data);
			command.SetComputeTextureParam(waveform, num4, "_Source", ShaderIDs.WaveformSource);
			command.SetComputeVectorParam(waveform, "_Params", vector);
			command.DispatchCompute(waveform, num4, num2, Mathf.CeilToInt((float)this.height / 256f), 1);
			command.ReleaseTemporaryRT(ShaderIDs.WaveformSource);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.waveform);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)num2, (float)this.height, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.WaveformBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Waveform");
		}

		// Token: 0x040039AB RID: 14763
		public float exposure = 0.12f;

		// Token: 0x040039AC RID: 14764
		public int height = 256;

		// Token: 0x040039AD RID: 14765
		private ComputeBuffer m_Data;

		// Token: 0x040039AE RID: 14766
		private const int k_ThreadGroupSize = 256;

		// Token: 0x040039AF RID: 14767
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x040039B0 RID: 14768
		private const int k_ThreadGroupSizeY = 16;
	}
}
