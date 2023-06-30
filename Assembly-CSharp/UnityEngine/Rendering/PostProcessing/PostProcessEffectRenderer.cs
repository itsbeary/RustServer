using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A95 RID: 2709
	public abstract class PostProcessEffectRenderer
	{
		// Token: 0x06004060 RID: 16480 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void Init()
		{
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x00007A44 File Offset: 0x00005C44
		public virtual DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.None;
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x0017B4D4 File Offset: 0x001796D4
		public virtual void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x0017B4DD File Offset: 0x001796DD
		public virtual void Release()
		{
			this.ResetHistory();
		}

		// Token: 0x06004064 RID: 16484
		public abstract void Render(PostProcessRenderContext context);

		// Token: 0x06004065 RID: 16485
		internal abstract void SetSettings(PostProcessEffectSettings settings);

		// Token: 0x040039E1 RID: 14817
		protected bool m_ResetHistory = true;
	}
}
