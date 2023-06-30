using System;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class DeployVolumeEntityBounds : DeployVolume
{
	// Token: 0x0600295C RID: 10588 RVA: 0x000FE824 File Offset: 0x000FCA24
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation), this.layers & mask, this);
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000FE874 File Offset: 0x000FCA74
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	// Token: 0x04002175 RID: 8565
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
