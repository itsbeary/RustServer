using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A58 RID: 2648
	[Preserve]
	internal sealed class AutoExposureRenderer : PostProcessEffectRenderer<AutoExposure>
	{
		// Token: 0x06003F72 RID: 16242 RVA: 0x001737BC File Offset: 0x001719BC
		public AutoExposureRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_AutoExposurePool[i] = new RenderTexture[2];
				this.m_AutoExposurePingPong[i] = 0;
			}
		}

		// Token: 0x06003F73 RID: 16243 RVA: 0x0017380C File Offset: 0x00171A0C
		private void CheckTexture(int eye, int id)
		{
			if (this.m_AutoExposurePool[eye][id] == null || !this.m_AutoExposurePool[eye][id].IsCreated())
			{
				this.m_AutoExposurePool[eye][id] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat)
				{
					enableRandomWrite = true
				};
				this.m_AutoExposurePool[eye][id].Create();
			}
		}

		// Token: 0x06003F74 RID: 16244 RVA: 0x00173868 File Offset: 0x00171A68
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("AutoExposureLookup");
			this.CheckTexture(context.xrActiveEye, 0);
			this.CheckTexture(context.xrActiveEye, 1);
			float num = base.settings.filtering.value.x;
			float num2 = base.settings.filtering.value.y;
			num2 = Mathf.Clamp(num2, 1.01f, 99f);
			num = Mathf.Clamp(num, 1f, num2 - 0.01f);
			float value = base.settings.minLuminance.value;
			float value2 = base.settings.maxLuminance.value;
			base.settings.minLuminance.value = Mathf.Min(value, value2);
			base.settings.maxLuminance.value = Mathf.Max(value, value2);
			bool flag = this.m_ResetHistory || !Application.isPlaying;
			string text;
			if (flag || base.settings.eyeAdaptation.value == EyeAdaptation.Fixed)
			{
				text = "KAutoExposureAvgLuminance_fixed";
			}
			else
			{
				text = "KAutoExposureAvgLuminance_progressive";
			}
			ComputeShader autoExposure = context.resources.computeShaders.autoExposure;
			int num3 = autoExposure.FindKernel(text);
			command.SetComputeBufferParam(autoExposure, num3, "_HistogramBuffer", context.logHistogram.data);
			command.SetComputeVectorParam(autoExposure, "_Params1", new Vector4(num * 0.01f, num2 * 0.01f, RuntimeUtilities.Exp2(base.settings.minLuminance.value), RuntimeUtilities.Exp2(base.settings.maxLuminance.value)));
			command.SetComputeVectorParam(autoExposure, "_Params2", new Vector4(base.settings.speedDown.value, base.settings.speedUp.value, base.settings.keyValue.value, Time.deltaTime));
			command.SetComputeVectorParam(autoExposure, "_ScaleOffsetRes", context.logHistogram.GetHistogramScaleOffsetRes(context));
			if (flag)
			{
				this.m_CurrentAutoExposure = this.m_AutoExposurePool[context.xrActiveEye][0];
				command.SetComputeTextureParam(autoExposure, num3, "_Destination", this.m_CurrentAutoExposure);
				command.DispatchCompute(autoExposure, num3, 1, 1, 1);
				RuntimeUtilities.CopyTexture(command, this.m_AutoExposurePool[context.xrActiveEye][0], this.m_AutoExposurePool[context.xrActiveEye][1]);
				this.m_ResetHistory = false;
			}
			else
			{
				int num4 = this.m_AutoExposurePingPong[context.xrActiveEye];
				RenderTexture renderTexture = this.m_AutoExposurePool[context.xrActiveEye][++num4 % 2];
				RenderTexture renderTexture2 = this.m_AutoExposurePool[context.xrActiveEye][++num4 % 2];
				command.SetComputeTextureParam(autoExposure, num3, "_Source", renderTexture);
				command.SetComputeTextureParam(autoExposure, num3, "_Destination", renderTexture2);
				command.DispatchCompute(autoExposure, num3, 1, 1, 1);
				this.m_AutoExposurePingPong[context.xrActiveEye] = (num4 + 1) % 2;
				this.m_CurrentAutoExposure = renderTexture2;
			}
			command.EndSample("AutoExposureLookup");
			context.autoExposureTexture = this.m_CurrentAutoExposure;
			context.autoExposure = base.settings;
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x00173B94 File Offset: 0x00171D94
		public override void Release()
		{
			foreach (RenderTexture[] array in this.m_AutoExposurePool)
			{
				for (int j = 0; j < array.Length; j++)
				{
					RuntimeUtilities.Destroy(array[j]);
				}
			}
		}

		// Token: 0x040038E4 RID: 14564
		private const int k_NumEyes = 2;

		// Token: 0x040038E5 RID: 14565
		private const int k_NumAutoExposureTextures = 2;

		// Token: 0x040038E6 RID: 14566
		private readonly RenderTexture[][] m_AutoExposurePool = new RenderTexture[2][];

		// Token: 0x040038E7 RID: 14567
		private int[] m_AutoExposurePingPong = new int[2];

		// Token: 0x040038E8 RID: 14568
		private RenderTexture m_CurrentAutoExposure;
	}
}
