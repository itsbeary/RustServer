using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A90 RID: 2704
	public sealed class PostProcessBundle
	{
		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x0600403B RID: 16443 RVA: 0x0017AA92 File Offset: 0x00178C92
		// (set) Token: 0x0600403C RID: 16444 RVA: 0x0017AA9A File Offset: 0x00178C9A
		public PostProcessAttribute attribute { get; private set; }

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x0600403D RID: 16445 RVA: 0x0017AAA3 File Offset: 0x00178CA3
		// (set) Token: 0x0600403E RID: 16446 RVA: 0x0017AAAB File Offset: 0x00178CAB
		public PostProcessEffectSettings settings { get; private set; }

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x0600403F RID: 16447 RVA: 0x0017AAB4 File Offset: 0x00178CB4
		internal PostProcessEffectRenderer renderer
		{
			get
			{
				if (this.m_Renderer == null)
				{
					Assert.IsNotNull<Type>(this.attribute.renderer);
					Type renderer = this.attribute.renderer;
					this.m_Renderer = (PostProcessEffectRenderer)Activator.CreateInstance(renderer);
					this.m_Renderer.SetSettings(this.settings);
					this.m_Renderer.Init();
				}
				return this.m_Renderer;
			}
		}

		// Token: 0x06004040 RID: 16448 RVA: 0x0017AB18 File Offset: 0x00178D18
		internal PostProcessBundle(PostProcessEffectSettings settings)
		{
			Assert.IsNotNull<PostProcessEffectSettings>(settings);
			this.settings = settings;
			this.attribute = settings.GetType().GetAttribute<PostProcessAttribute>();
		}

		// Token: 0x06004041 RID: 16449 RVA: 0x0017AB3E File Offset: 0x00178D3E
		internal void Release()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.Release();
			}
			RuntimeUtilities.Destroy(this.settings);
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x0017AB5E File Offset: 0x00178D5E
		internal void ResetHistory()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.ResetHistory();
			}
		}

		// Token: 0x06004043 RID: 16451 RVA: 0x0017AB73 File Offset: 0x00178D73
		internal T CastSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.settings);
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x0017AB80 File Offset: 0x00178D80
		internal T CastRenderer<T>() where T : PostProcessEffectRenderer
		{
			return (T)((object)this.renderer);
		}

		// Token: 0x040039BC RID: 14780
		private PostProcessEffectRenderer m_Renderer;
	}
}
