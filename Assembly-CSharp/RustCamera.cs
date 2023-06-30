using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002E6 RID: 742
public abstract class RustCamera<T> : SingletonComponent<T> where T : RustCamera<T>
{
	// Token: 0x04001751 RID: 5969
	[SerializeField]
	private AmplifyOcclusionEffect ssao;

	// Token: 0x04001752 RID: 5970
	[SerializeField]
	private SEScreenSpaceShadows contactShadows;

	// Token: 0x04001753 RID: 5971
	[SerializeField]
	private VisualizeTexelDensity visualizeTexelDensity;

	// Token: 0x04001754 RID: 5972
	[SerializeField]
	private EnvironmentVolumePropertiesCollection environmentVolumeProperties;

	// Token: 0x04001755 RID: 5973
	[SerializeField]
	private PostProcessLayer post;

	// Token: 0x04001756 RID: 5974
	[SerializeField]
	private PostProcessVolume baseEffectVolume;
}
