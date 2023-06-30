using System;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class MLRSAudio : MonoBehaviour
{
	// Token: 0x04001ECF RID: 7887
	[SerializeField]
	private MLRS mlrs;

	// Token: 0x04001ED0 RID: 7888
	[SerializeField]
	private Transform pitchTransform;

	// Token: 0x04001ED1 RID: 7889
	[SerializeField]
	private Transform yawTransform;

	// Token: 0x04001ED2 RID: 7890
	[SerializeField]
	private float pitchDeltaSmoothRate = 5f;

	// Token: 0x04001ED3 RID: 7891
	[SerializeField]
	private float yawDeltaSmoothRate = 5f;

	// Token: 0x04001ED4 RID: 7892
	[SerializeField]
	private float pitchDeltaThreshold = 0.5f;

	// Token: 0x04001ED5 RID: 7893
	[SerializeField]
	private float yawDeltaThreshold = 0.5f;

	// Token: 0x04001ED6 RID: 7894
	private float lastPitch;

	// Token: 0x04001ED7 RID: 7895
	private float lastYaw;

	// Token: 0x04001ED8 RID: 7896
	private float pitchDelta;

	// Token: 0x04001ED9 RID: 7897
	private float yawDelta;

	// Token: 0x04001EDA RID: 7898
	public SoundDefinition turretMovementStartDef;

	// Token: 0x04001EDB RID: 7899
	public SoundDefinition turretMovementLoopDef;

	// Token: 0x04001EDC RID: 7900
	public SoundDefinition turretMovementStopDef;

	// Token: 0x04001EDD RID: 7901
	private Sound turretMovementLoop;
}
