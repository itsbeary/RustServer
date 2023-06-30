using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Collection")]
public class GestureCollection : ScriptableObject
{
	// Token: 0x06002C6E RID: 11374 RVA: 0x0010D598 File Offset: 0x0010B798
	public GestureConfig IdToGesture(uint id)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.gestureId == id)
			{
				return gestureConfig;
			}
		}
		return null;
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x0010D5CC File Offset: 0x0010B7CC
	public GestureConfig StringToGesture(string gestureName)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.convarName == gestureName)
			{
				return gestureConfig;
			}
		}
		return null;
	}

	// Token: 0x04002426 RID: 9254
	public GestureConfig[] AllGestures;

	// Token: 0x04002427 RID: 9255
	public float GestureVmInDuration = 0.25f;

	// Token: 0x04002428 RID: 9256
	public AnimationCurve GestureInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002429 RID: 9257
	public float GestureVmOutDuration = 0.25f;

	// Token: 0x0400242A RID: 9258
	public AnimationCurve GestureOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400242B RID: 9259
	public float GestureViewmodelDeployDelay = 0.25f;
}
