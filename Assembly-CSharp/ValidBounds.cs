using System;
using UnityEngine;

// Token: 0x020005A5 RID: 1445
public class ValidBounds : SingletonComponent<ValidBounds>
{
	// Token: 0x06002C0D RID: 11277 RVA: 0x0010A9C5 File Offset: 0x00108BC5
	public static bool Test(Vector3 vPos)
	{
		return !SingletonComponent<ValidBounds>.Instance || SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x0010A9E0 File Offset: 0x00108BE0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this.worldBounds.center, this.worldBounds.size);
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x0010AA08 File Offset: 0x00108C08
	internal bool IsInside(Vector3 vPos)
	{
		if (vPos.IsNaNOrInfinity())
		{
			return false;
		}
		if (!this.worldBounds.Contains(vPos))
		{
			return false;
		}
		if (TerrainMeta.Terrain != null)
		{
			if (World.Procedural && vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040023CC RID: 9164
	public Bounds worldBounds;
}
