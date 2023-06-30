using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A54 RID: 2644
	[Preserve]
	internal sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
	{
		// Token: 0x06003F66 RID: 16230 RVA: 0x001735BD File Offset: 0x001717BD
		public override void Init()
		{
			if (this.m_Methods == null)
			{
				this.m_Methods = new IAmbientOcclusionMethod[]
				{
					new ScalableAO(base.settings),
					new MultiScaleVO(base.settings)
				};
			}
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x001735F0 File Offset: 0x001717F0
		public bool IsAmbientOnly(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			return base.settings.ambientOnly.value && camera.actualRenderingPath == RenderingPath.DeferredShading && camera.allowHDR;
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x00173627 File Offset: 0x00171827
		public IAmbientOcclusionMethod Get()
		{
			return this.m_Methods[(int)base.settings.mode.value];
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x00173640 File Offset: 0x00171840
		public override DepthTextureMode GetCameraFlags()
		{
			return this.Get().GetCameraFlags();
		}

		// Token: 0x06003F6A RID: 16234 RVA: 0x00173650 File Offset: 0x00171850
		public override void Release()
		{
			IAmbientOcclusionMethod[] methods = this.m_Methods;
			for (int i = 0; i < methods.Length; i++)
			{
				methods[i].Release();
			}
		}

		// Token: 0x06003F6B RID: 16235 RVA: 0x0017367A File Offset: 0x0017187A
		public ScalableAO GetScalableAO()
		{
			return (ScalableAO)this.m_Methods[0];
		}

		// Token: 0x06003F6C RID: 16236 RVA: 0x00173689 File Offset: 0x00171889
		public MultiScaleVO GetMultiScaleVO()
		{
			return (MultiScaleVO)this.m_Methods[1];
		}

		// Token: 0x06003F6D RID: 16237 RVA: 0x000063A5 File Offset: 0x000045A5
		public override void Render(PostProcessRenderContext context)
		{
		}

		// Token: 0x040038D9 RID: 14553
		private IAmbientOcclusionMethod[] m_Methods;
	}
}
