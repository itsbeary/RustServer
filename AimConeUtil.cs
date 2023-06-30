using System;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class AimConeUtil
{
	// Token: 0x06001BF2 RID: 7154 RVA: 0x000C4658 File Offset: 0x000C2858
	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		Quaternion quaternion = Quaternion.LookRotation(inputVec);
		Vector2 vector = (anywhereInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		return quaternion * Quaternion.Euler(vector.x * aimCone * 0.5f, vector.y * aimCone * 0.5f, 0f) * Vector3.forward;
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000C46B8 File Offset: 0x000C28B8
	public static Quaternion GetAimConeQuat(float aimCone)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		return Quaternion.Euler(insideUnitSphere.x * aimCone * 0.5f, insideUnitSphere.y * aimCone * 0.5f, 0f);
	}
}
