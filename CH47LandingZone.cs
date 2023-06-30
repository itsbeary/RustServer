using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public class CH47LandingZone : MonoBehaviour
{
	// Token: 0x06002670 RID: 9840 RVA: 0x000F28E8 File Offset: 0x000F0AE8
	public void Awake()
	{
		if (!CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Add(this);
		}
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000F2904 File Offset: 0x000F0B04
	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47LandingZone ch47LandingZone = null;
		foreach (CH47LandingZone ch47LandingZone2 in CH47LandingZone.landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47LandingZone2.transform.position);
			if (num2 < num)
			{
				num = num2;
				ch47LandingZone = ch47LandingZone2;
			}
		}
		return ch47LandingZone;
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000F2978 File Offset: 0x000F0B78
	public void OnDestroy()
	{
		if (CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Remove(this);
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000F2993 File Offset: 0x000F0B93
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000F29A1 File Offset: 0x000F0BA1
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000F29B0 File Offset: 0x000F0BB0
	public void OnDrawGizmos()
	{
		Color magenta = Color.magenta;
		magenta.a = 0.25f;
		Gizmos.color = magenta;
		GizmosUtil.DrawCircleY(base.transform.position, 6f);
		magenta.a = 1f;
		Gizmos.color = magenta;
		GizmosUtil.DrawWireCircleY(base.transform.position, 6f);
	}

	// Token: 0x04001EC5 RID: 7877
	public float lastDropTime;

	// Token: 0x04001EC6 RID: 7878
	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	// Token: 0x04001EC7 RID: 7879
	public float dropoffScale = 1f;
}
