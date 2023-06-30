using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A96 RID: 2710
	public abstract class PostProcessEffectRenderer<T> : PostProcessEffectRenderer where T : PostProcessEffectSettings
	{
		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06004067 RID: 16487 RVA: 0x0017B4F4 File Offset: 0x001796F4
		// (set) Token: 0x06004068 RID: 16488 RVA: 0x0017B4FC File Offset: 0x001796FC
		public T settings { get; internal set; }

		// Token: 0x06004069 RID: 16489 RVA: 0x0017B505 File Offset: 0x00179705
		internal override void SetSettings(PostProcessEffectSettings settings)
		{
			this.settings = (T)((object)settings);
		}
	}
}
