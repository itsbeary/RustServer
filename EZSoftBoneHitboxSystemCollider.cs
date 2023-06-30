using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020002FD RID: 765
[RequireComponent(typeof(HitboxSystem))]
public class EZSoftBoneHitboxSystemCollider : EZSoftBoneColliderBase, IClientComponent
{
	// Token: 0x06001E90 RID: 7824 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Collide(ref Vector3 position, float spacing)
	{
	}

	// Token: 0x040017A2 RID: 6050
	public float radius = 2f;
}
