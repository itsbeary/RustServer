using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002A1 RID: 673
public class OxygenOverlay : MonoBehaviour
{
	// Token: 0x04001620 RID: 5664
	[SerializeField]
	private PostProcessVolume postProcessVolume;

	// Token: 0x04001621 RID: 5665
	[SerializeField]
	private float smoothTime = 1f;

	// Token: 0x04001622 RID: 5666
	[Tooltip("If true, only show this effect when the player is mounted in a submarine.")]
	[SerializeField]
	private bool submarinesOnly;
}
