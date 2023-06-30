using System;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public static class WaterCheckEx
{
	// Token: 0x060032F5 RID: 13045 RVA: 0x001390EC File Offset: 0x001372EC
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (WaterCheck waterCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(waterCheck.worldPosition, scale);
			if (waterCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 vector2 = pos + vector;
			if (!waterCheck.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
