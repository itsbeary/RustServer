using System;
using UnityEngine;

// Token: 0x0200070B RID: 1803
[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	// Token: 0x0400298A RID: 10634
	[SerializeField]
	private Texture2D texture;

	// Token: 0x0400298B RID: 10635
	[Range(0f, 1f)]
	public float Displacement = 1f;

	// Token: 0x0400298C RID: 10636
	[Range(0f, 1f)]
	public float Disturbance = 0.5f;
}
