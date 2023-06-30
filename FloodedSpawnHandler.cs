using System;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public static class FloodedSpawnHandler
{
	// Token: 0x06002ADB RID: 10971 RVA: 0x00105614 File Offset: 0x00103814
	public static bool GetSpawnPoint(BasePlayer.SpawnPoint spawnPoint, float searchHeight)
	{
		SpawnHandler instance = SingletonComponent<SpawnHandler>.Instance;
		if (TerrainMeta.HeightMap == null || instance == null)
		{
			return false;
		}
		LayerMask placementMask = instance.PlacementMask;
		LayerMask placementCheckMask = instance.PlacementCheckMask;
		float placementCheckHeight = instance.PlacementCheckHeight;
		LayerMask radiusCheckMask = instance.RadiusCheckMask;
		float radiusCheckDistance = instance.RadiusCheckDistance;
		int i = 0;
		while (i < 10)
		{
			Vector3 vector = FloodedSpawnHandler.FindSpawnPoint(searchHeight);
			RaycastHit raycastHit;
			if (placementCheckMask == 0 || !Physics.Raycast(vector + Vector3.up * placementCheckHeight, Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
			{
				goto IL_B4;
			}
			if (((1 << raycastHit.transform.gameObject.layer) & placementMask) != 0)
			{
				vector.y = raycastHit.point.y;
				goto IL_B4;
			}
			IL_FD:
			i++;
			continue;
			IL_B4:
			if (radiusCheckMask == 0 || !Physics.CheckSphere(vector, radiusCheckDistance, radiusCheckMask))
			{
				spawnPoint.pos = vector;
				spawnPoint.rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
				return true;
			}
			goto IL_FD;
		}
		return false;
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x00105730 File Offset: 0x00103930
	private static Vector3 FindSpawnPoint(float searchHeight)
	{
		Vector3 vector = (TerrainMeta.Size / 2f).WithY(0f);
		float magnitude = vector.magnitude;
		float num = magnitude / 50f;
		float num2 = FloodedSpawnHandler.RandomAngle();
		float num3 = num2 + 3.1415927f;
		Vector3 vector2 = TerrainMeta.Position + vector + FloodedSpawnHandler.Step(num2, magnitude);
		for (int i = 0; i < 50; i++)
		{
			float num4 = float.MinValue;
			Vector3 vector3 = Vector3.zero;
			float num5 = 0f;
			foreach (int num6 in FloodedSpawnHandler.SpreadSteps)
			{
				float num7 = num3 + (float)num6 * 0.17453292f;
				Vector3 vector4 = vector2 + FloodedSpawnHandler.Step(num7, num);
				float height = TerrainMeta.HeightMap.GetHeight(vector4);
				if (height > num4)
				{
					num4 = height;
					vector3 = vector4;
					num5 = num7;
				}
			}
			vector2 = vector3.WithY(num4);
			num3 = (num3 + num5) / 2f;
			if (num4 >= searchHeight)
			{
				break;
			}
		}
		return vector2;
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x0010583A File Offset: 0x00103A3A
	private static Vector3 Step(float angle, float distance)
	{
		return new Vector3(distance * Mathf.Cos(angle), 0f, distance * -Mathf.Sin(angle));
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x00105857 File Offset: 0x00103A57
	private static float RandomAngle()
	{
		return UnityEngine.Random.value * 6.2831855f;
	}

	// Token: 0x0400230F RID: 8975
	private static readonly int[] SpreadSteps = new int[] { 0, 1, -1, 2, -2, 3, -3 };
}
