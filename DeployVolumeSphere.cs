using System;
using UnityEngine;

// Token: 0x02000500 RID: 1280
public class DeployVolumeSphere : DeployVolume
{
	// Token: 0x06002967 RID: 10599 RVA: 0x000FEADC File Offset: 0x000FCCDC
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return DeployVolume.CheckSphere(position, this.radius, this.layers & mask, this);
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000FEB34 File Offset: 0x000FCD34
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return (this.layers & mask) != 0 && Vector3.Distance(position, obb.ClosestPoint(position)) <= this.radius;
	}

	// Token: 0x04002179 RID: 8569
	public Vector3 center = Vector3.zero;

	// Token: 0x0400217A RID: 8570
	public float radius = 0.5f;
}
