using System;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class TreadAnimator : MonoBehaviour, IClientComponent
{
	// Token: 0x04001185 RID: 4485
	public Animator mainBodyAnimator;

	// Token: 0x04001186 RID: 4486
	public Transform[] wheelBones;

	// Token: 0x04001187 RID: 4487
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x04001188 RID: 4488
	public Vector3[] wheelBoneOrigin;

	// Token: 0x04001189 RID: 4489
	public float wheelBoneDistMax = 0.26f;

	// Token: 0x0400118A RID: 4490
	public Material leftTread;

	// Token: 0x0400118B RID: 4491
	public Material rightTread;

	// Token: 0x0400118C RID: 4492
	public TreadEffects treadEffects;

	// Token: 0x0400118D RID: 4493
	public float traceThickness = 0.25f;

	// Token: 0x0400118E RID: 4494
	public float heightFudge = 0.13f;

	// Token: 0x0400118F RID: 4495
	public bool useWheelYOrigin;

	// Token: 0x04001190 RID: 4496
	public Vector2 treadTextureDirection = new Vector2(1f, 0f);

	// Token: 0x04001191 RID: 4497
	public bool isMetallic;
}
