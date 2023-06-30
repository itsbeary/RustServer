using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8F RID: 2703
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture>
	{
		// Token: 0x06004039 RID: 16441 RVA: 0x0017A90C File Offset: 0x00178B0C
		public override void Interp(Texture from, Texture to, float t)
		{
			if (from == null && to == null)
			{
				this.value = null;
				return;
			}
			if (from != null && to != null)
			{
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			if (this.defaultState == TextureParameterDefault.Lut2D)
			{
				Texture lutStrip = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip;
				}
				if (to == null)
				{
					to = lutStrip;
				}
			}
			Color color;
			switch (this.defaultState)
			{
			case TextureParameterDefault.Black:
				color = Color.black;
				break;
			case TextureParameterDefault.White:
				color = Color.white;
				break;
			case TextureParameterDefault.Transparent:
				color = Color.clear;
				break;
			case TextureParameterDefault.Lut2D:
			{
				Texture lutStrip2 = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip2;
				}
				if (to == null)
				{
					to = lutStrip2;
				}
				if (from.width != to.width || from.height != to.height)
				{
					this.value = null;
					return;
				}
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			default:
				base.Interp(from, to, t);
				return;
			}
			if (from == null)
			{
				this.value = TextureLerper.instance.Lerp(to, color, 1f - t);
				return;
			}
			this.value = TextureLerper.instance.Lerp(from, color, t);
		}

		// Token: 0x040039B9 RID: 14777
		public TextureParameterDefault defaultState = TextureParameterDefault.Black;
	}
}
