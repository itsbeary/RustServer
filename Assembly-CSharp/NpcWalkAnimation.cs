using System;
using UnityEngine;

// Token: 0x02000292 RID: 658
public class NpcWalkAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040015E8 RID: 5608
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	// Token: 0x040015E9 RID: 5609
	public BaseNpc Npc;

	// Token: 0x040015EA RID: 5610
	public Animator Animator;

	// Token: 0x040015EB RID: 5611
	public Transform HipBone;

	// Token: 0x040015EC RID: 5612
	public Transform LookBone;

	// Token: 0x040015ED RID: 5613
	public bool UpdateWalkSpeed = true;

	// Token: 0x040015EE RID: 5614
	public bool UpdateFacingDirection = true;

	// Token: 0x040015EF RID: 5615
	public bool UpdateGroundNormal = true;

	// Token: 0x040015F0 RID: 5616
	public Transform alignmentRoot;

	// Token: 0x040015F1 RID: 5617
	public bool LaggyAss = true;

	// Token: 0x040015F2 RID: 5618
	public bool LookAtTarget;

	// Token: 0x040015F3 RID: 5619
	public float MaxLaggyAssRotation = 70f;

	// Token: 0x040015F4 RID: 5620
	public float MaxWalkAnimSpeed = 25f;

	// Token: 0x040015F5 RID: 5621
	public bool UseDirectionBlending;

	// Token: 0x040015F6 RID: 5622
	public bool useTurnPosing;

	// Token: 0x040015F7 RID: 5623
	public float turnPoseScale = 0.5f;

	// Token: 0x040015F8 RID: 5624
	public float laggyAssLerpScale = 15f;

	// Token: 0x040015F9 RID: 5625
	public bool skeletonChainInverted;
}
