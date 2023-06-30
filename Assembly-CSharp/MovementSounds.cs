using System;
using UnityEngine;

// Token: 0x02000627 RID: 1575
public class MovementSounds : MonoBehaviour
{
	// Token: 0x0400262D RID: 9773
	public SoundDefinition waterMovementDef;

	// Token: 0x0400262E RID: 9774
	public float waterMovementFadeInSpeed = 1f;

	// Token: 0x0400262F RID: 9775
	public float waterMovementFadeOutSpeed = 1f;

	// Token: 0x04002630 RID: 9776
	public SoundDefinition enterWaterSmall;

	// Token: 0x04002631 RID: 9777
	public SoundDefinition enterWaterMedium;

	// Token: 0x04002632 RID: 9778
	public SoundDefinition enterWaterLarge;

	// Token: 0x04002633 RID: 9779
	private Sound waterMovement;

	// Token: 0x04002634 RID: 9780
	private SoundModulation.Modulator waterGainMod;

	// Token: 0x04002635 RID: 9781
	public bool inWater;

	// Token: 0x04002636 RID: 9782
	public float waterLevel;

	// Token: 0x04002637 RID: 9783
	public bool mute;
}
