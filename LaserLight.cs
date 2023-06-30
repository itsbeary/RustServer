using System;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class LaserLight : AudioVisualisationEntity
{
	// Token: 0x060020CC RID: 8396 RVA: 0x000D89AA File Offset: 0x000D6BAA
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
	}

	// Token: 0x040019B4 RID: 6580
	public Animator LaserAnimator;

	// Token: 0x040019B5 RID: 6581
	public LineRenderer[] LineRenderers;

	// Token: 0x040019B6 RID: 6582
	public MeshRenderer[] DotRenderers;

	// Token: 0x040019B7 RID: 6583
	public MeshRenderer FlareRenderer;

	// Token: 0x040019B8 RID: 6584
	public Light[] LightSources;

	// Token: 0x040019B9 RID: 6585
	public LaserLight.ColourSetting RedSettings;

	// Token: 0x040019BA RID: 6586
	public LaserLight.ColourSetting GreenSettings;

	// Token: 0x040019BB RID: 6587
	public LaserLight.ColourSetting BlueSettings;

	// Token: 0x040019BC RID: 6588
	public LaserLight.ColourSetting YellowSettings;

	// Token: 0x040019BD RID: 6589
	public LaserLight.ColourSetting PinkSettings;

	// Token: 0x02000CCF RID: 3279
	[Serializable]
	public struct ColourSetting
	{
		// Token: 0x04004577 RID: 17783
		public Color PointLightColour;

		// Token: 0x04004578 RID: 17784
		public Material LaserMaterial;

		// Token: 0x04004579 RID: 17785
		public Color DotColour;

		// Token: 0x0400457A RID: 17786
		public Color FlareColour;
	}
}
