using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class ChippyMainCharacter : SpriteArcadeEntity
{
	// Token: 0x04000FC7 RID: 4039
	public float speed;

	// Token: 0x04000FC8 RID: 4040
	public float maxSpeed = 0.25f;

	// Token: 0x04000FC9 RID: 4041
	public ChippyBulletEntity bulletPrefab;

	// Token: 0x04000FCA RID: 4042
	public float fireRate = 0.1f;

	// Token: 0x04000FCB RID: 4043
	public Vector3 aimDir = Vector3.up;
}
