using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class CH47PathFinder : BasePathFinder
{
	// Token: 0x06002678 RID: 9848 RVA: 0x000F2A30 File Offset: 0x000F0C30
	public override Vector3 GetRandomPatrolPoint()
	{
		Vector3 vector = Vector3.zero;
		MonumentInfo monumentInfo = null;
		if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			int count = TerrainMeta.Path.Monuments.Count;
			int num = UnityEngine.Random.Range(0, count);
			for (int i = 0; i < count; i++)
			{
				int num2 = i + num;
				if (num2 >= count)
				{
					num2 -= count;
				}
				MonumentInfo monumentInfo2 = TerrainMeta.Path.Monuments[num2];
				if (monumentInfo2.Type != MonumentType.Cave && monumentInfo2.Type != MonumentType.WaterWell && monumentInfo2.Tier != MonumentTier.Tier0 && !monumentInfo2.IsSafeZone && (monumentInfo2.Tier & MonumentTier.Tier0) <= (MonumentTier)0)
				{
					bool flag = false;
					foreach (Vector3 vector2 in this.visitedPatrolPoints)
					{
						if (Vector3Ex.Distance2D(monumentInfo2.transform.position, vector2) < 100f)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						monumentInfo = monumentInfo2;
						break;
					}
				}
			}
			if (monumentInfo == null)
			{
				this.visitedPatrolPoints.Clear();
				monumentInfo = this.GetRandomValidMonumentInfo();
			}
		}
		if (monumentInfo != null)
		{
			this.visitedPatrolPoints.Add(monumentInfo.transform.position);
			vector = monumentInfo.transform.position;
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float num3 = 30f;
			vector = Vector3Ex.Range(-1f, 1f);
			vector.y = 0f;
			vector.Normalize();
			vector *= x * UnityEngine.Random.Range(0f, 0.75f);
			vector.y = num3;
		}
		float num4 = Mathf.Max(TerrainMeta.WaterMap.GetHeight(vector), TerrainMeta.HeightMap.GetHeight(vector));
		float num5 = num4;
		RaycastHit raycastHit;
		if (Physics.SphereCast(vector + new Vector3(0f, 200f, 0f), 20f, Vector3.down, out raycastHit, 300f, 1218511105))
		{
			num5 = Mathf.Max(raycastHit.point.y, num4);
		}
		vector.y = num5 + 30f;
		return vector;
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000F2C90 File Offset: 0x000F0E90
	private MonumentInfo GetRandomValidMonumentInfo()
	{
		int count = TerrainMeta.Path.Monuments.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = i + num;
			if (num2 >= count)
			{
				num2 -= count;
			}
			MonumentInfo monumentInfo = TerrainMeta.Path.Monuments[num2];
			if (monumentInfo.Type != MonumentType.Cave && monumentInfo.Type != MonumentType.WaterWell && monumentInfo.Tier != MonumentTier.Tier0 && !monumentInfo.IsSafeZone)
			{
				return monumentInfo;
			}
		}
		return null;
	}

	// Token: 0x04001EC8 RID: 7880
	public List<Vector3> visitedPatrolPoints = new List<Vector3>();
}
