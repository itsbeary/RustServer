using System;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class EyeController : MonoBehaviour
{
	// Token: 0x0400186D RID: 6253
	public const float MaxLookDot = 0.8f;

	// Token: 0x0400186E RID: 6254
	public bool debug;

	// Token: 0x0400186F RID: 6255
	public Transform LeftEye;

	// Token: 0x04001870 RID: 6256
	public Transform RightEye;

	// Token: 0x04001871 RID: 6257
	public Transform EyeTransform;

	// Token: 0x04001872 RID: 6258
	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	// Token: 0x04001873 RID: 6259
	public Vector3 FlickerRange;

	// Token: 0x04001874 RID: 6260
	private Transform Focus;

	// Token: 0x04001875 RID: 6261
	private float FocusUpdateTime;
}
