using System;
using UnityEngine;

// Token: 0x020002CB RID: 715
public class LightGroupAtTime : FacepunchBehaviour
{
	// Token: 0x04001697 RID: 5783
	public float IntensityOverride = 1f;

	// Token: 0x04001698 RID: 5784
	public AnimationCurve IntensityScaleOverTime = new AnimationCurve
	{
		keys = new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(8f, 0f),
			new Keyframe(12f, 0f),
			new Keyframe(19f, 1f),
			new Keyframe(24f, 1f)
		}
	};

	// Token: 0x04001699 RID: 5785
	public Transform SearchRoot;

	// Token: 0x0400169A RID: 5786
	[Header("Power Settings")]
	public bool requiresPower;

	// Token: 0x0400169B RID: 5787
	[Tooltip("Can NOT be entity, use new blank gameobject!")]
	public Transform powerOverrideTransform;

	// Token: 0x0400169C RID: 5788
	public LayerMask checkLayers = 1235288065;

	// Token: 0x0400169D RID: 5789
	public GameObject enableWhenLightsOn;

	// Token: 0x0400169E RID: 5790
	public float timeBetweenPowerLookup = 10f;
}
