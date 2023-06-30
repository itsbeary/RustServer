using System;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class FirecrackerRepeater : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04000F5A RID: 3930
	public GameObjectRef singleExplosionEffect;

	// Token: 0x04000F5B RID: 3931
	public Transform[] parts;

	// Token: 0x04000F5C RID: 3932
	public float partWidth = 0.2f;

	// Token: 0x04000F5D RID: 3933
	public float partLength = 0.1f;

	// Token: 0x04000F5E RID: 3934
	public Quaternion[] targetRotations;

	// Token: 0x04000F5F RID: 3935
	public Quaternion[] initialRotations;

	// Token: 0x04000F60 RID: 3936
	public Renderer[] renderers;

	// Token: 0x04000F61 RID: 3937
	public Material materialSource;

	// Token: 0x04000F62 RID: 3938
	public float explodeRepeatMin = 0.05f;

	// Token: 0x04000F63 RID: 3939
	public float explodeRepeatMax = 0.15f;

	// Token: 0x04000F64 RID: 3940
	public float explodeLerpSpeed = 30f;

	// Token: 0x04000F65 RID: 3941
	public Vector3 twistAmount;

	// Token: 0x04000F66 RID: 3942
	public float fuseLength = 3f;

	// Token: 0x04000F67 RID: 3943
	public float explodeStrength = 10f;

	// Token: 0x04000F68 RID: 3944
	public float explodeDirBlend = 0.5f;

	// Token: 0x04000F69 RID: 3945
	public float duration = 10f;

	// Token: 0x04000F6A RID: 3946
	public ParticleSystemContainer smokeParticle;
}
