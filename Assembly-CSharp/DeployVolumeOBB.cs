using System;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public class DeployVolumeOBB : DeployVolume
{
	// Token: 0x06002964 RID: 10596 RVA: 0x000FE9D0 File Offset: 0x000FCBD0
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation * this.worldRotation), this.layers & mask, this);
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000FEA44 File Offset: 0x000FCC44
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		OBB obb = new OBB(position, this.bounds.size, rotation * this.worldRotation);
		return (this.layers & mask) != 0 && obb.Intersects(test);
	}

	// Token: 0x04002178 RID: 8568
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
