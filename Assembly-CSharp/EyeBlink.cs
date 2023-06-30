using System;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class EyeBlink : MonoBehaviour
{
	// Token: 0x04001861 RID: 6241
	public Transform LeftEye;

	// Token: 0x04001862 RID: 6242
	public Transform LeftEyelid;

	// Token: 0x04001863 RID: 6243
	public Vector3 LeftEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x04001864 RID: 6244
	public Transform RightEye;

	// Token: 0x04001865 RID: 6245
	public Transform RightEyelid;

	// Token: 0x04001866 RID: 6246
	public Vector3 RightEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x04001867 RID: 6247
	public Vector3 ClosedEyelidPosition;

	// Token: 0x04001868 RID: 6248
	public Vector3 ClosedEyelidRotation;

	// Token: 0x04001869 RID: 6249
	public Vector2 TimeWithoutBlinking = new Vector2(1f, 10f);

	// Token: 0x0400186A RID: 6250
	public float BlinkSpeed = 0.2f;

	// Token: 0x0400186B RID: 6251
	public Vector3 LeftEyeInitial;

	// Token: 0x0400186C RID: 6252
	public Vector3 RightEyeInitial;
}
