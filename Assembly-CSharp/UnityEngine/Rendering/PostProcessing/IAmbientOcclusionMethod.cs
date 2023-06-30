using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A53 RID: 2643
	internal interface IAmbientOcclusionMethod
	{
		// Token: 0x06003F61 RID: 16225
		DepthTextureMode GetCameraFlags();

		// Token: 0x06003F62 RID: 16226
		void RenderAfterOpaque(PostProcessRenderContext context);

		// Token: 0x06003F63 RID: 16227
		void RenderAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003F64 RID: 16228
		void CompositeAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003F65 RID: 16229
		void Release();
	}
}
