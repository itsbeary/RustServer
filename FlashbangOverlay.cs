using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200029D RID: 669
public class FlashbangOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x04001614 RID: 5652
	public static FlashbangOverlay Instance;

	// Token: 0x04001615 RID: 5653
	public PostProcessVolume postProcessVolume;

	// Token: 0x04001616 RID: 5654
	public AnimationCurve burnIntensityCurve;

	// Token: 0x04001617 RID: 5655
	public AnimationCurve whiteoutIntensityCurve;

	// Token: 0x04001618 RID: 5656
	public SoundDefinition deafLoopDef;
}
