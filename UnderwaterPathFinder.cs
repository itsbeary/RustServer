using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class UnderwaterPathFinder : BasePathFinder
{
	// Token: 0x06001BEE RID: 7150 RVA: 0x000C443C File Offset: 0x000C263C
	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000C4448 File Offset: 0x000C2648
	public override Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		List<Vector3> list = Pool.GetList<Vector3>();
		float height = TerrainMeta.WaterMap.GetHeight(navigator.transform.position);
		float height2 = TerrainMeta.HeightMap.GetHeight(navigator.transform.position);
		for (int i = 0; i < 8; i++)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(fallbackPos, UnityEngine.Random.Range(1f, navigator.MaxRoamDistanceFromHome), UnityEngine.Random.Range(0f, 359f));
			pointOnCircle.y += UnityEngine.Random.Range(-2f, 2f);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
			list.Add(pointOnCircle);
		}
		float num = -1f;
		int num2 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 vector = list[j];
			if (this.npc.IsVisible(vector, float.PositiveInfinity))
			{
				float num3 = 0f;
				Vector3 vector2 = Vector3Ex.Direction2D(vector, navigator.transform.position);
				float num4 = Vector3.Dot(navigator.transform.forward, vector2);
				num3 += Mathf.InverseLerp(0.25f, 0.8f, num4) * 5f;
				float num5 = Mathf.Abs(vector.y - navigator.transform.position.y);
				num3 += 1f - Mathf.InverseLerp(1f, 3f, num5) * 5f;
				if (num3 > num || num2 == -1)
				{
					num = num3;
					num2 = j;
				}
			}
		}
		Vector3 vector3 = list[num2];
		Pool.FreeList<Vector3>(ref list);
		return vector3;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000C45E8 File Offset: 0x000C27E8
	public override bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		if (fleeFrom == null)
		{
			result = navigator.transform.position;
			return false;
		}
		Vector3 vector = Vector3Ex.Direction2D(navigator.transform.position, fleeFrom.transform.position);
		result = navigator.transform.position + vector * UnityEngine.Random.Range(minRange, maxRange);
		return true;
	}

	// Token: 0x040013B6 RID: 5046
	private BaseEntity npc;
}
