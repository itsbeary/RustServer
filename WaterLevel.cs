using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020005A7 RID: 1447
public static class WaterLevel
{
	// Token: 0x06002C22 RID: 11298 RVA: 0x0010B098 File Offset: 0x00109298
	public static float Factor(Vector3 start, Vector3 end, float radius, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		float num;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(start, end, radius, waves, volumes, forEntity);
			num = (waterInfo.isValid ? Mathf.InverseLerp(Mathf.Min(start.y, end.y) - radius, Mathf.Max(start.y, end.y) + radius, waterInfo.surfaceLevel) : 0f);
		}
		return num;
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x0010B120 File Offset: 0x00109320
	public static float Factor(Bounds bounds, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		float num;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			if (bounds.size == Vector3.zero)
			{
				bounds.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds, waves, volumes, forEntity);
			num = (waterInfo.isValid ? Mathf.InverseLerp(bounds.min.y, bounds.max.y, waterInfo.surfaceLevel) : 0f);
		}
		return num;
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x0010B1C4 File Offset: 0x001093C4
	public static bool Test(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		bool isValid;
		using (TimeWarning.New("WaterLevel.Test", 0))
		{
			isValid = WaterLevel.GetWaterInfo(pos, waves, volumes, forEntity, false).isValid;
		}
		return isValid;
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x0010B20C File Offset: 0x0010940C
	public static float GetWaterDepth(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		float currentDepth;
		using (TimeWarning.New("WaterLevel.GetWaterDepth", 0))
		{
			currentDepth = WaterLevel.GetWaterInfo(pos, waves, volumes, forEntity, false).currentDepth;
		}
		return currentDepth;
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x0010B254 File Offset: 0x00109454
	public static float GetOverallWaterDepth(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		float overallDepth;
		using (TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0))
		{
			overallDepth = WaterLevel.GetWaterInfo(pos, waves, volumes, forEntity, noEarlyExit).overallDepth;
		}
		return overallDepth;
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x0010B29C File Offset: 0x0010949C
	public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight, bool doDeepwaterChecks, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo waterInfo2;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			if (pos.y > waterHeight)
			{
				waterInfo2 = WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
			}
			else
			{
				bool flag = pos.y < terrainHeight - 1f;
				if (flag)
				{
					waterHeight = 0f;
					if (pos.y > waterHeight)
					{
						return waterInfo;
					}
				}
				bool flag2 = doDeepwaterChecks && pos.y < waterHeight - 10f;
				int num = (TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0);
				if ((flag || flag2 || (num & 246144) == 0) && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					waterInfo2 = waterInfo;
				}
				else
				{
					RaycastHit raycastHit;
					if (flag2 && Physics.Raycast(pos, Vector3.up, out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
					{
						waterHeight = Mathf.Min(waterHeight, raycastHit.point.y);
					}
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
					waterInfo.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
					waterInfo.surfaceLevel = waterHeight;
					waterInfo2 = waterInfo;
				}
			}
		}
		return waterInfo2;
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x0010B400 File Offset: 0x00109600
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		WaterLevel.WaterInfo waterInfo2;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = WaterLevel.GetWaterLevel(pos);
			if (pos.y > num)
			{
				if (!noEarlyExit)
				{
					return volumes ? WaterLevel.GetWaterInfoFromVolumes(pos, forEntity) : waterInfo;
				}
				waterInfo = (volumes ? WaterLevel.GetWaterInfoFromVolumes(pos, forEntity) : waterInfo);
			}
			float num2 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
			if (pos.y < num2 - 1f)
			{
				num = 0f;
				if (pos.y > num && !noEarlyExit)
				{
					return waterInfo;
				}
			}
			if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
			{
				waterInfo2 = waterInfo;
			}
			else
			{
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num - pos.y);
				waterInfo.overallDepth = Mathf.Max(0f, num - num2);
				waterInfo.surfaceLevel = num;
				waterInfo2 = waterInfo;
			}
		}
		return waterInfo2;
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x0010B520 File Offset: 0x00109720
	public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		WaterLevel.WaterInfo waterInfo2;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = WaterLevel.GetWaterLevel(bounds.center);
			if (bounds.min.y > num)
			{
				waterInfo2 = (volumes ? WaterLevel.GetWaterInfoFromVolumes(bounds, forEntity) : waterInfo);
			}
			else
			{
				float num2 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(bounds.center) : 0f);
				if (bounds.max.y < num2 - 1f)
				{
					num = 0f;
					if (bounds.min.y > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(bounds))
				{
					waterInfo2 = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num - bounds.min.y);
					waterInfo.overallDepth = Mathf.Max(0f, num - num2);
					waterInfo.surfaceLevel = num;
					waterInfo2 = waterInfo;
				}
			}
		}
		return waterInfo2;
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x0010B648 File Offset: 0x00109848
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 start, Vector3 end, float radius, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		WaterLevel.WaterInfo waterInfo2;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			Vector3 vector = (start + end) * 0.5f;
			float num = Mathf.Min(start.y, end.y) - radius;
			float num2 = Mathf.Max(start.y, end.y) + radius;
			float num3 = WaterLevel.GetWaterLevel(vector);
			if (num > num3)
			{
				waterInfo2 = (volumes ? WaterLevel.GetWaterInfoFromVolumes(start, end, radius, forEntity) : waterInfo);
			}
			else
			{
				float num4 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(vector) : 0f);
				if (num2 < num4 - 1f)
				{
					num3 = 0f;
					if (num > num3)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(start, end, radius))
				{
					Vector3 vector2 = vector.WithY(Mathf.Lerp(num, num2, 0.75f));
					if (WaterSystem.Collision.GetIgnore(vector2, 0.01f))
					{
						return waterInfo;
					}
					num3 = Mathf.Min(num3, vector2.y);
				}
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num3 - num);
				waterInfo.overallDepth = Mathf.Max(0f, num3 - num4);
				waterInfo.surfaceLevel = num3;
				waterInfo2 = waterInfo;
			}
		}
		return waterInfo2;
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x0010B7CC File Offset: 0x001099CC
	public static WaterLevel.WaterInfo GetWaterInfo(Camera cam, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		WaterLevel.WaterInfo waterInfo;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			if (cam.transform.position.y < WaterSystem.MinLevel() - 1f)
			{
				waterInfo = WaterLevel.GetWaterInfo(cam.transform.position, waves, volumes, forEntity, noEarlyExit);
			}
			else
			{
				waterInfo = WaterLevel.GetWaterInfo(cam.transform.position - Vector3.up, waves, volumes, forEntity, noEarlyExit);
			}
		}
		return waterInfo;
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x0010B858 File Offset: 0x00109A58
	private static float GetWaterLevel(Vector3 pos)
	{
		float num = (TerrainMeta.HeightMap ? TerrainMeta.WaterMap.GetHeight(pos) : 0f);
		if (num >= WaterSystem.MaxLevel())
		{
			return num;
		}
		float height = WaterSystem.GetHeight(pos);
		if (num > WaterSystem.OceanLevel)
		{
			return Mathf.Max(num, height);
		}
		return height;
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x0010B8A8 File Offset: 0x00109AA8
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Bounds bounds, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(new OBB(bounds), list, 262144, QueryTriggerInteraction.Collide);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Test(bounds, out waterInfo))
					{
						break;
					}
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return waterInfo;
		}
		forEntity.WaterTestFromVolumes(bounds, out waterInfo);
		return waterInfo;
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x0010B938 File Offset: 0x00109B38
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 pos, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(pos, 0.1f, list, 262144, QueryTriggerInteraction.Collide);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Test(pos, out waterInfo))
					{
						break;
					}
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return waterInfo;
		}
		forEntity.WaterTestFromVolumes(pos, out waterInfo);
		return waterInfo;
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x0010B9C8 File Offset: 0x00109BC8
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 start, Vector3 end, float radius, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(start, end, radius, list, 262144, QueryTriggerInteraction.Collide);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Test(start, end, radius, out waterInfo))
					{
						break;
					}
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return waterInfo;
		}
		forEntity.WaterTestFromVolumes(start, end, radius, out waterInfo);
		return waterInfo;
	}

	// Token: 0x02000D7C RID: 3452
	public struct WaterInfo
	{
		// Token: 0x04004815 RID: 18453
		public bool isValid;

		// Token: 0x04004816 RID: 18454
		public float currentDepth;

		// Token: 0x04004817 RID: 18455
		public float overallDepth;

		// Token: 0x04004818 RID: 18456
		public float surfaceLevel;
	}
}
