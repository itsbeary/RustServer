using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A82 RID: 2690
	[Serializable]
	public sealed class VectorscopeMonitor : Monitor
	{
		// Token: 0x06004007 RID: 16391 RVA: 0x00179FC3 File Offset: 0x001781C3
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06004009 RID: 16393 RVA: 0x00179FE5 File Offset: 0x001781E5
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.vectorscope;
		}

		// Token: 0x0600400A RID: 16394 RVA: 0x00179FFC File Offset: 0x001781FC
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.size, this.size);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num = this.size * this.size;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num, 4);
			}
			else if (this.m_Data.count != num)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num, 4);
			}
			ComputeShader vectorscope = context.resources.computeShaders.vectorscope;
			CommandBuffer command = context.command;
			command.BeginSample("Vectorscope");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)this.size, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0));
			int num2 = vectorscope.FindKernel("KVectorscopeClear");
			command.SetComputeBufferParam(vectorscope, num2, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeVectorParam(vectorscope, "_Params", vector);
			command.DispatchCompute(vectorscope, num2, Mathf.CeilToInt((float)this.size / 16f), Mathf.CeilToInt((float)this.size / 16f), 1);
			num2 = vectorscope.FindKernel("KVectorscopeGather");
			command.SetComputeBufferParam(vectorscope, num2, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeTextureParam(vectorscope, num2, "_Source", ShaderIDs.HalfResFinalCopy);
			command.DispatchCompute(vectorscope, num2, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.vectorscope);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.size, (float)this.size, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.VectorscopeBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Vectorscope");
		}

		// Token: 0x040039A6 RID: 14758
		public int size = 256;

		// Token: 0x040039A7 RID: 14759
		public float exposure = 0.12f;

		// Token: 0x040039A8 RID: 14760
		private ComputeBuffer m_Data;

		// Token: 0x040039A9 RID: 14761
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x040039AA RID: 14762
		private const int k_ThreadGroupSizeY = 16;
	}
}
