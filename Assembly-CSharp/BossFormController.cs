using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class BossFormController : ArcadeEntityController
{
	// Token: 0x04000FA3 RID: 4003
	public float animationSpeed = 0.5f;

	// Token: 0x04000FA4 RID: 4004
	public Sprite[] animationFrames;

	// Token: 0x04000FA5 RID: 4005
	public Vector2 roamDistance;

	// Token: 0x04000FA6 RID: 4006
	public Transform colliderParent;

	// Token: 0x04000FA7 RID: 4007
	public BossFormController.BossDamagePoint[] damagePoints;

	// Token: 0x04000FA8 RID: 4008
	public ArcadeEntityController flashController;

	// Token: 0x04000FA9 RID: 4009
	public float health = 50f;

	// Token: 0x02000C3A RID: 3130
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x04004305 RID: 17157
		public BoxCollider hitBox;

		// Token: 0x04004306 RID: 17158
		public float health;

		// Token: 0x04004307 RID: 17159
		public ArcadeEntityController damagePrefab;

		// Token: 0x04004308 RID: 17160
		public ArcadeEntityController damageInstance;

		// Token: 0x04004309 RID: 17161
		public bool destroyed;
	}
}
