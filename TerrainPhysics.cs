using System;
using UnityEngine;

// Token: 0x020006B2 RID: 1714
public class TerrainPhysics : TerrainExtension
{
	// Token: 0x0600318B RID: 12683 RVA: 0x00128ABD File Offset: 0x00126CBD
	public override void Setup()
	{
		this.splat = this.terrain.GetComponent<TerrainSplatMap>();
		this.materials = this.config.GetPhysicMaterials();
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x00128AE1 File Offset: 0x00126CE1
	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		if (this.splat == null || this.materials.Length == 0)
		{
			return null;
		}
		return this.materials[this.splat.GetSplatMaxIndex(worldPos, -1)];
	}

	// Token: 0x04002847 RID: 10311
	private TerrainSplatMap splat;

	// Token: 0x04002848 RID: 10312
	private PhysicMaterial[] materials;
}
