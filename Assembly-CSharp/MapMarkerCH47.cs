using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class MapMarkerCH47 : MapMarker
{
	// Token: 0x0600191E RID: 6430 RVA: 0x000B964C File Offset: 0x000B784C
	private float GetRotation()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity)
		{
			return 0f;
		}
		Vector3 forward = parentEntity.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		return Mathf.Atan2(forward.x, -forward.z) * 57.29578f + 180f;
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x000B96AB File Offset: 0x000B78AB
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.rotation = this.GetRotation();
		return appMarkerData;
	}

	// Token: 0x040011B5 RID: 4533
	private GameObject createdMarker;
}
