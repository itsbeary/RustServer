using System;
using UnityEngine;

// Token: 0x02000727 RID: 1831
[CreateAssetMenu(menuName = "Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
	// Token: 0x040029EA RID: 10730
	public float TransitionSpeed = 1f;

	// Token: 0x040029EB RID: 10731
	public LayerMask ReflectionMask = 1084293120;

	// Token: 0x040029EC RID: 10732
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] ReflectionMultipliers;

	// Token: 0x040029ED RID: 10733
	public float DefaultReflectionMultiplier = 1f;

	// Token: 0x040029EE RID: 10734
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] AmbientMultipliers;

	// Token: 0x040029EF RID: 10735
	public float DefaultAmbientMultiplier = 1f;

	// Token: 0x040029F0 RID: 10736
	public EnvironmentVolumePropertiesCollection.OceanParameters OceanOverrides;

	// Token: 0x02000E4E RID: 3662
	[Serializable]
	public class EnvironmentMultiplier
	{
		// Token: 0x04004B5C RID: 19292
		public EnvironmentType Type;

		// Token: 0x04004B5D RID: 19293
		public float Multiplier;
	}

	// Token: 0x02000E4F RID: 3663
	[Serializable]
	public class OceanParameters
	{
		// Token: 0x04004B5E RID: 19294
		public AnimationCurve TransitionCurve = AnimationCurve.Linear(0f, 0f, 40f, 1f);

		// Token: 0x04004B5F RID: 19295
		[Range(0f, 1f)]
		public float DirectionalLightMultiplier = 0.25f;

		// Token: 0x04004B60 RID: 19296
		[Range(0f, 1f)]
		public float AmbientLightMultiplier;

		// Token: 0x04004B61 RID: 19297
		[Range(0f, 1f)]
		public float ReflectionMultiplier = 1f;

		// Token: 0x04004B62 RID: 19298
		[Range(0f, 1f)]
		public float SunMeshBrightnessMultiplier = 1f;

		// Token: 0x04004B63 RID: 19299
		[Range(0f, 1f)]
		public float MoonMeshBrightnessMultiplier = 1f;

		// Token: 0x04004B64 RID: 19300
		[Range(0f, 1f)]
		public float AtmosphereBrightnessMultiplier = 1f;

		// Token: 0x04004B65 RID: 19301
		[Range(0f, 1f)]
		public float LightColorMultiplier = 1f;

		// Token: 0x04004B66 RID: 19302
		public Color LightColor = Color.black;

		// Token: 0x04004B67 RID: 19303
		[Range(0f, 1f)]
		public float SunRayColorMultiplier = 1f;

		// Token: 0x04004B68 RID: 19304
		public Color SunRayColor = Color.black;

		// Token: 0x04004B69 RID: 19305
		[Range(0f, 1f)]
		public float MoonRayColorMultiplier = 1f;

		// Token: 0x04004B6A RID: 19306
		public Color MoonRayColor = Color.black;
	}
}
