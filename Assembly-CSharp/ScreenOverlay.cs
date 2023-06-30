using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200064A RID: 1610
[PostProcess(typeof(ScreenOverlayRenderer), PostProcessEvent.AfterStack, "Custom/ScreenOverlay", true)]
[Serializable]
public class ScreenOverlay : PostProcessEffectSettings
{
	// Token: 0x040026A7 RID: 9895
	public OverlayBlendModeParameter blendMode = new OverlayBlendModeParameter
	{
		value = OverlayBlendMode.Multiply
	};

	// Token: 0x040026A8 RID: 9896
	public FloatParameter intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040026A9 RID: 9897
	public TextureParameter texture = new TextureParameter
	{
		value = null
	};

	// Token: 0x040026AA RID: 9898
	public TextureParameter normals = new TextureParameter
	{
		value = null
	};
}
