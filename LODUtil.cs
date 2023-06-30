using System;
using UnityEngine;

// Token: 0x02000540 RID: 1344
public static class LODUtil
{
	// Token: 0x06002A24 RID: 10788 RVA: 0x00101D5F File Offset: 0x000FFF5F
	public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		return LODUtil.GetDistance(transform.position, mode);
	}

	// Token: 0x06002A25 RID: 10789 RVA: 0x00101D70 File Offset: 0x000FFF70
	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		if (MainCamera.isValid)
		{
			switch (mode)
			{
			case LODDistanceMode.XYZ:
				return Vector3.Distance(MainCamera.position, worldPos);
			case LODDistanceMode.XZ:
				return Vector3Ex.Distance2D(MainCamera.position, worldPos);
			case LODDistanceMode.Y:
				return Mathf.Abs(MainCamera.position.y - worldPos.y);
			}
		}
		return 1000f;
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x00101DCC File Offset: 0x000FFFCC
	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x00101DD9 File Offset: 0x000FFFD9
	public static LODEnvironmentMode DetermineEnvironmentMode(Transform transform)
	{
		if (transform.CompareTag("OnlyVisibleUnderground") || transform.root.CompareTag("OnlyVisibleUnderground"))
		{
			return LODEnvironmentMode.Underground;
		}
		return LODEnvironmentMode.Default;
	}

	// Token: 0x04002271 RID: 8817
	public const float DefaultDistance = 1000f;
}
