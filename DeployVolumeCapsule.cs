using System;
using UnityEngine;

// Token: 0x020004FC RID: 1276
public class DeployVolumeCapsule : DeployVolume
{
	// Token: 0x06002959 RID: 10585 RVA: 0x000FE740 File Offset: 0x000FC940
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		Vector3 vector = position + rotation * this.worldRotation * Vector3.up * this.height * 0.5f;
		Vector3 vector2 = position + rotation * this.worldRotation * Vector3.down * this.height * 0.5f;
		return DeployVolume.CheckCapsule(vector, vector2, this.radius, this.layers & mask, this);
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x04002172 RID: 8562
	public Vector3 center = Vector3.zero;

	// Token: 0x04002173 RID: 8563
	public float radius = 0.5f;

	// Token: 0x04002174 RID: 8564
	public float height = 1f;
}
