using System;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class ArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x04000F7E RID: 3966
	public uint id;

	// Token: 0x04000F7F RID: 3967
	public uint spriteID;

	// Token: 0x04000F80 RID: 3968
	public uint soundID;

	// Token: 0x04000F81 RID: 3969
	public bool visible;

	// Token: 0x04000F82 RID: 3970
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	// Token: 0x04000F83 RID: 3971
	public bool isEnabled;

	// Token: 0x04000F84 RID: 3972
	public bool dirty;

	// Token: 0x04000F85 RID: 3973
	public float alpha = 1f;

	// Token: 0x04000F86 RID: 3974
	public BoxCollider boxCollider;

	// Token: 0x04000F87 RID: 3975
	public bool host;

	// Token: 0x04000F88 RID: 3976
	public bool localAuthorativeOverride;

	// Token: 0x04000F89 RID: 3977
	public ArcadeEntity arcadeEntityParent;

	// Token: 0x04000F8A RID: 3978
	public uint prefabID;

	// Token: 0x04000F8B RID: 3979
	[Header("Health")]
	public bool takesDamage;

	// Token: 0x04000F8C RID: 3980
	public float health = 1f;

	// Token: 0x04000F8D RID: 3981
	public float maxHealth = 1f;

	// Token: 0x04000F8E RID: 3982
	[NonSerialized]
	public bool mapLoadedEntiy;
}
