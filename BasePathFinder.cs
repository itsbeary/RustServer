using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class BasePathFinder
{
	// Token: 0x06001BE3 RID: 7139 RVA: 0x0002C05D File Offset: 0x0002A25D
	public virtual Vector3 GetRandomPatrolPoint()
	{
		return Vector3.zero;
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public virtual AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		return null;
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x000C3F50 File Offset: 0x000C2150
	public void DebugDraw()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.chosenPosition, 5f);
		Gizmos.color = Color.blue;
		Vector3[] array = BasePathFinder.topologySamples;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawSphere(array[i], 2.5f);
		}
		Gizmos.color = color;
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x000C3FB4 File Offset: 0x000C21B4
	public virtual Vector3 GetRandomPositionAround(Vector3 position, float minDistFrom = 0f, float maxDistFrom = 2f)
	{
		if (maxDistFrom < 0f)
		{
			maxDistFrom = 0f;
		}
		Vector2 vector = UnityEngine.Random.insideUnitCircle * maxDistFrom;
		float num = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector.x);
		float num2 = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector.y);
		return position + new Vector3(num, 0f, num2);
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000C4038 File Offset: 0x000C2238
	public virtual Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		float num = UnityEngine.Random.Range(minRange, maxRange);
		int num2 = 0;
		int num3 = 0;
		float num4 = UnityEngine.Random.Range(0f, 90f);
		for (float num5 = 0f; num5 < 360f; num5 += 90f)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(navigator.transform.position, num, num5 + num4);
			Vector3 vector;
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out vector, 10f) && navigator.IsPositionABiomeRequirement(vector) && navigator.IsAcceptableWaterDepth(vector) && !navigator.IsPositionPreventTopology(vector))
			{
				BasePathFinder.topologySamples[num2] = vector;
				num2++;
				if (navigator.IsPositionABiomePreference(vector) && navigator.IsPositionATopologyPreference(vector))
				{
					BasePathFinder.preferedTopologySamples[num3] = vector;
					num3++;
				}
			}
		}
		if (num3 > 0)
		{
			this.chosenPosition = BasePathFinder.preferedTopologySamples[UnityEngine.Random.Range(0, num3)];
		}
		else if (num2 > 0)
		{
			this.chosenPosition = BasePathFinder.topologySamples[UnityEngine.Random.Range(0, num2)];
		}
		else
		{
			this.chosenPosition = fallbackPos;
		}
		return this.chosenPosition;
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000C4148 File Offset: 0x000C2348
	public virtual Vector3 GetBestRoamPositionFromAnchor(BaseNavigator navigator, Vector3 anchorPos, Vector3 fallbackPos, float minRange, float maxRange)
	{
		float num = UnityEngine.Random.Range(minRange, maxRange);
		int num2 = 0;
		int num3 = 0;
		float num4 = UnityEngine.Random.Range(0f, 90f);
		for (float num5 = 0f; num5 < 360f; num5 += 90f)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(anchorPos, num, num5 + num4);
			Vector3 vector;
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out vector, 10f) && navigator.IsAcceptableWaterDepth(vector))
			{
				BasePathFinder.topologySamples[num2] = vector;
				num2++;
				if (navigator.IsPositionABiomePreference(vector) && navigator.IsPositionATopologyPreference(vector))
				{
					BasePathFinder.preferedTopologySamples[num3] = vector;
					num3++;
				}
			}
		}
		if (UnityEngine.Random.Range(0f, 1f) <= 0.9f && num3 > 0)
		{
			this.chosenPosition = BasePathFinder.preferedTopologySamples[UnityEngine.Random.Range(0, num3)];
		}
		else if (num2 > 0)
		{
			this.chosenPosition = BasePathFinder.topologySamples[UnityEngine.Random.Range(0, num2)];
		}
		else
		{
			this.chosenPosition = fallbackPos;
		}
		return this.chosenPosition;
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000C4250 File Offset: 0x000C2450
	public virtual bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		if (fleeFrom == null)
		{
			result = navigator.transform.position;
			return false;
		}
		Vector3 vector = Vector3Ex.Direction2D(navigator.transform.position, fleeFrom.transform.position);
		if (this.TestFleeDirection(navigator, vector, 0f, minRange, maxRange, out result))
		{
			return true;
		}
		bool flag = UnityEngine.Random.Range(0, 2) == 1;
		return this.TestFleeDirection(navigator, vector, flag ? 45f : 315f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, vector, flag ? 315f : 45f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, vector, flag ? 90f : 270f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, vector, flag ? 270f : 90f, minRange, maxRange, out result) || this.TestFleeDirection(navigator, vector, 135f + UnityEngine.Random.Range(0f, 90f), minRange, maxRange, out result);
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000C4364 File Offset: 0x000C2564
	private bool TestFleeDirection(BaseNavigator navigator, Vector3 dirFromThreat, float offsetDegrees, float minRange, float maxRange, out Vector3 result)
	{
		result = navigator.transform.position;
		Vector3 vector = Quaternion.Euler(0f, offsetDegrees, 0f) * dirFromThreat;
		Vector3 vector2 = navigator.transform.position + vector * UnityEngine.Random.Range(minRange, maxRange);
		Vector3 vector3;
		if (!navigator.GetNearestNavmeshPosition(vector2, out vector3, 20f))
		{
			return false;
		}
		if (!navigator.IsAcceptableWaterDepth(vector3))
		{
			return false;
		}
		result = vector3;
		return true;
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000C43E0 File Offset: 0x000C25E0
	public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
	{
		float num = center.x + radius * Mathf.Cos(degrees * 0.017453292f);
		float num2 = center.z + radius * Mathf.Sin(degrees * 0.017453292f);
		return new Vector3(num, center.y, num2);
	}

	// Token: 0x040013B2 RID: 5042
	private static Vector3[] preferedTopologySamples = new Vector3[4];

	// Token: 0x040013B3 RID: 5043
	private static Vector3[] topologySamples = new Vector3[4];

	// Token: 0x040013B4 RID: 5044
	private Vector3 chosenPosition;

	// Token: 0x040013B5 RID: 5045
	private const float halfPI = 0.017453292f;
}
