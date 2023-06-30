using System;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class SnowmobileChassisVisuals : VehicleChassisVisuals<Snowmobile>, IClientComponent
{
	// Token: 0x04001F99 RID: 8089
	[SerializeField]
	private Animator animator;

	// Token: 0x04001F9A RID: 8090
	[SerializeField]
	private SnowmobileAudio audioScript;

	// Token: 0x04001F9B RID: 8091
	[SerializeField]
	private SnowmobileChassisVisuals.TreadRenderer[] treadRenderers;

	// Token: 0x04001F9C RID: 8092
	[SerializeField]
	private float treadSpeedMultiplier = 0.01f;

	// Token: 0x04001F9D RID: 8093
	[SerializeField]
	private bool flipRightSkiExtension;

	// Token: 0x04001F9E RID: 8094
	[SerializeField]
	private Transform leftSki;

	// Token: 0x04001F9F RID: 8095
	[SerializeField]
	private Transform leftSkiPistonIn;

	// Token: 0x04001FA0 RID: 8096
	[SerializeField]
	private Transform leftSkiPistonOut;

	// Token: 0x04001FA1 RID: 8097
	[SerializeField]
	private Transform rightSki;

	// Token: 0x04001FA2 RID: 8098
	[SerializeField]
	private Transform rightSkiPistonIn;

	// Token: 0x04001FA3 RID: 8099
	[SerializeField]
	private Transform rightSkiPistonOut;

	// Token: 0x04001FA4 RID: 8100
	[SerializeField]
	private float skiVisualAdjust;

	// Token: 0x04001FA5 RID: 8101
	[SerializeField]
	private float treadVisualAdjust;

	// Token: 0x04001FA6 RID: 8102
	[SerializeField]
	private float skiVisualMaxExtension;

	// Token: 0x04001FA7 RID: 8103
	[SerializeField]
	private float treadVisualMaxExtension;

	// Token: 0x04001FA8 RID: 8104
	[SerializeField]
	private float wheelSizeVisualMultiplier = 1f;

	// Token: 0x02000D24 RID: 3364
	[Serializable]
	private class TreadRenderer
	{
		// Token: 0x040046E0 RID: 18144
		public Renderer renderer;

		// Token: 0x040046E1 RID: 18145
		public int materialIndex;
	}
}
