using System;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class TerrainPathConnect : MonoBehaviour
{
	// Token: 0x0600300F RID: 12303 RVA: 0x001210A0 File Offset: 0x0011F2A0
	public PathFinder.Point GetPathFinderPoint(int res, Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x001210F8 File Offset: 0x0011F2F8
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		return this.GetPathFinderPoint(res, base.transform.position);
	}

	// Token: 0x040027AD RID: 10157
	public InfrastructureType Type;
}
