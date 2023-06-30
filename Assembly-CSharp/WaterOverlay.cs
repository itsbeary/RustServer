using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002A6 RID: 678
public class WaterOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x04001628 RID: 5672
	public PostProcessVolume postProcessVolume;

	// Token: 0x04001629 RID: 5673
	public PostProcessVolume blurPostProcessVolume;

	// Token: 0x0400162A RID: 5674
	public WaterOverlay.EffectParams adminParams = WaterOverlay.EffectParams.DefaultAdmin;

	// Token: 0x0400162B RID: 5675
	public WaterOverlay.EffectParams gogglesParams = WaterOverlay.EffectParams.DefaultGoggles;

	// Token: 0x0400162C RID: 5676
	public WaterOverlay.EffectParams submarineParams = WaterOverlay.EffectParams.DefaultSubmarine;

	// Token: 0x0400162D RID: 5677
	public WaterOverlay.EffectParams underwaterLabParams = WaterOverlay.EffectParams.DefaultUnderwaterLab;

	// Token: 0x0400162E RID: 5678
	public WaterOverlay.EffectParams cinematicParams = WaterOverlay.EffectParams.DefaultCinematic;

	// Token: 0x0400162F RID: 5679
	public Material[] UnderwaterFogMaterials;

	// Token: 0x02000CA9 RID: 3241
	[Serializable]
	public struct EffectParams
	{
		// Token: 0x040044BE RID: 17598
		public float scatterCoefficient;

		// Token: 0x040044BF RID: 17599
		public bool blur;

		// Token: 0x040044C0 RID: 17600
		public float blurDistance;

		// Token: 0x040044C1 RID: 17601
		public float blurSize;

		// Token: 0x040044C2 RID: 17602
		public int blurIterations;

		// Token: 0x040044C3 RID: 17603
		public bool wiggle;

		// Token: 0x040044C4 RID: 17604
		public float wiggleSpeed;

		// Token: 0x040044C5 RID: 17605
		public bool chromaticAberration;

		// Token: 0x040044C6 RID: 17606
		public bool godRays;

		// Token: 0x040044C7 RID: 17607
		public static WaterOverlay.EffectParams DefaultAdmin = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};

		// Token: 0x040044C8 RID: 17608
		public static WaterOverlay.EffectParams DefaultGoggles = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.05f,
			blur = true,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = true,
			wiggleSpeed = 2f,
			chromaticAberration = true,
			godRays = true
		};

		// Token: 0x040044C9 RID: 17609
		public static WaterOverlay.EffectParams DefaultSubmarine = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = false,
			godRays = false
		};

		// Token: 0x040044CA RID: 17610
		public static WaterOverlay.EffectParams DefaultUnderwaterLab = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.005f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};

		// Token: 0x040044CB RID: 17611
		public static WaterOverlay.EffectParams DefaultCinematic = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};
	}
}
