using System;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class ParticleEmitFromParentObject : MonoBehaviour
{
	// Token: 0x04001896 RID: 6294
	public string bonename;

	// Token: 0x04001897 RID: 6295
	private Bounds bounds;

	// Token: 0x04001898 RID: 6296
	private Transform bone;

	// Token: 0x04001899 RID: 6297
	private BaseEntity entity;

	// Token: 0x0400189A RID: 6298
	private float lastBoundsUpdate;
}
