using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200044F RID: 1103
public class PlayerNameTag : MonoBehaviour
{
	// Token: 0x04001D2E RID: 7470
	public CanvasGroup canvasGroup;

	// Token: 0x04001D2F RID: 7471
	public Text text;

	// Token: 0x04001D30 RID: 7472
	public Gradient color;

	// Token: 0x04001D31 RID: 7473
	public float minDistance = 3f;

	// Token: 0x04001D32 RID: 7474
	public float maxDistance = 10f;

	// Token: 0x04001D33 RID: 7475
	public Vector3 positionOffset;

	// Token: 0x04001D34 RID: 7476
	public Transform parentBone;
}
