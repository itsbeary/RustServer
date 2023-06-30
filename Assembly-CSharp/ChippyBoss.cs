using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class ChippyBoss : SpriteArcadeEntity
{
	// Token: 0x04000FBC RID: 4028
	public Vector2 roamDistance;

	// Token: 0x04000FBD RID: 4029
	public float animationSpeed = 0.5f;

	// Token: 0x04000FBE RID: 4030
	public Sprite[] animationFrames;

	// Token: 0x04000FBF RID: 4031
	public ArcadeEntity bulletTest;

	// Token: 0x04000FC0 RID: 4032
	public SpriteRenderer flashRenderer;

	// Token: 0x04000FC1 RID: 4033
	public ChippyBoss.BossDamagePoint[] damagePoints;

	// Token: 0x02000C3B RID: 3131
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x0400430A RID: 17162
		public BoxCollider hitBox;

		// Token: 0x0400430B RID: 17163
		public float health;

		// Token: 0x0400430C RID: 17164
		public ArcadeEntityController damagePrefab;

		// Token: 0x0400430D RID: 17165
		public ArcadeEntityController damageInstance;

		// Token: 0x0400430E RID: 17166
		public bool destroyed;
	}
}
