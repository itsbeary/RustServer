using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A81 RID: 2689
	public abstract class Monitor
	{
		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06003FFD RID: 16381 RVA: 0x00179F0A File Offset: 0x0017810A
		// (set) Token: 0x06003FFE RID: 16382 RVA: 0x00179F12 File Offset: 0x00178112
		public RenderTexture output { get; protected set; }

		// Token: 0x06003FFF RID: 16383 RVA: 0x00179F1B File Offset: 0x0017811B
		public bool IsRequestedAndSupported(PostProcessRenderContext context)
		{
			return this.requested && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && this.ShaderResourcesAvailable(context);
		}

		// Token: 0x06004000 RID: 16384
		internal abstract bool ShaderResourcesAvailable(PostProcessRenderContext context);

		// Token: 0x06004001 RID: 16385 RVA: 0x00007A44 File Offset: 0x00005C44
		internal virtual bool NeedsHalfRes()
		{
			return false;
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x00179F3C File Offset: 0x0017813C
		protected void CheckOutput(int width, int height)
		{
			if (this.output == null || !this.output.IsCreated() || this.output.width != width || this.output.height != height)
			{
				RuntimeUtilities.Destroy(this.output);
				this.output = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
				{
					anisoLevel = 0,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false
				};
			}
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x000063A5 File Offset: 0x000045A5
		internal virtual void OnEnable()
		{
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x00179FB6 File Offset: 0x001781B6
		internal virtual void OnDisable()
		{
			RuntimeUtilities.Destroy(this.output);
		}

		// Token: 0x06004005 RID: 16389
		internal abstract void Render(PostProcessRenderContext context);

		// Token: 0x040039A5 RID: 14757
		internal bool requested;
	}
}
