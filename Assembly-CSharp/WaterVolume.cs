using System;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
public class WaterVolume : TriggerBase
{
	// Token: 0x06002C34 RID: 11316 RVA: 0x0010BAE8 File Offset: 0x00109CE8
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x0010BB10 File Offset: 0x00109D10
	public bool Test(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 vector = plane.ClosestPointOnPlane(pos);
		float y = (vector + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (vector + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - pos.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x0010BC1C File Offset: 0x00109E1C
	public bool Test(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(bounds.ClosestPoint(this.cachedBounds.position)))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(bounds.center))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 vector = plane.ClosestPointOnPlane(bounds.center);
		float y = (vector + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (vector + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - bounds.min.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x0010BD4C File Offset: 0x00109F4C
	public bool Test(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		Vector3 vector = (start + end) * 0.5f;
		float num = Mathf.Min(start.y, end.y) - radius;
		if (this.cachedBounds.Distance(start) >= radius && this.cachedBounds.Distance(end) >= radius)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(vector))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 vector2 = plane.ClosestPointOnPlane(vector);
		float y = (vector2 + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (vector2 + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - num);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x0010BE98 File Offset: 0x0010A098
	private bool CheckCutOffPlanes(Vector3 pos)
	{
		int num = this.cutOffPlanes.Length;
		bool flag = true;
		for (int i = 0; i < num; i++)
		{
			if (this.cutOffPlanes[i] != null && this.cutOffPlanes[i].InverseTransformPoint(pos).y > 0f)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x0010BEF0 File Offset: 0x0010A0F0
	private void UpdateCachedTransform()
	{
		if (this.cachedTransform != null && this.cachedTransform.hasChanged)
		{
			this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
			this.cachedTransform.hasChanged = false;
		}
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x0010BF30 File Offset: 0x0010A130
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x040023CF RID: 9167
	public Bounds WaterBounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x040023D0 RID: 9168
	private OBB cachedBounds;

	// Token: 0x040023D1 RID: 9169
	private Transform cachedTransform;

	// Token: 0x040023D2 RID: 9170
	public Transform[] cutOffPlanes = new Transform[0];

	// Token: 0x040023D3 RID: 9171
	public bool waterEnabled = true;
}
