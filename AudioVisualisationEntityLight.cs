using System;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class AudioVisualisationEntityLight : AudioVisualisationEntity
{
	// Token: 0x04001983 RID: 6531
	public Light TargetLight;

	// Token: 0x04001984 RID: 6532
	public Light SecondaryLight;

	// Token: 0x04001985 RID: 6533
	public MeshRenderer[] TargetRenderer;

	// Token: 0x04001986 RID: 6534
	public AudioVisualisationEntityLight.LightColourSet RedColour;

	// Token: 0x04001987 RID: 6535
	public AudioVisualisationEntityLight.LightColourSet GreenColour;

	// Token: 0x04001988 RID: 6536
	public AudioVisualisationEntityLight.LightColourSet BlueColour;

	// Token: 0x04001989 RID: 6537
	public AudioVisualisationEntityLight.LightColourSet YellowColour;

	// Token: 0x0400198A RID: 6538
	public AudioVisualisationEntityLight.LightColourSet PinkColour;

	// Token: 0x0400198B RID: 6539
	public float lightMinIntensity = 0.05f;

	// Token: 0x0400198C RID: 6540
	public float lightMaxIntensity = 1f;

	// Token: 0x02000CCE RID: 3278
	[Serializable]
	public struct LightColourSet
	{
		// Token: 0x04004574 RID: 17780
		[ColorUsage(true, true)]
		public Color LightColor;

		// Token: 0x04004575 RID: 17781
		[ColorUsage(true, true)]
		public Color SecondaryLightColour;

		// Token: 0x04004576 RID: 17782
		[ColorUsage(true, true)]
		public Color EmissionColour;
	}
}
