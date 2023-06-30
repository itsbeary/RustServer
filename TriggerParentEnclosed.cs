using System;
using ConVar;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class TriggerParentEnclosed : TriggerParent
{
	// Token: 0x06002BCC RID: 11212 RVA: 0x00109B18 File Offset: 0x00107D18
	protected void OnEnable()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x00109B26 File Offset: 0x00107D26
	public override bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		return base.ShouldParent(ent, bypassOtherTriggerCheck) && this.IsInside(ent, this.Padding);
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x00109B44 File Offset: 0x00107D44
	internal override bool SkipOnTriggerExit(Collider collider)
	{
		if (!this.CheckBoundsOnUnparent)
		{
			return false;
		}
		if (!Debugging.checkparentingtriggers)
		{
			return false;
		}
		BaseEntity baseEntity = collider.ToBaseEntity();
		return !(baseEntity == null) && this.IsInside(baseEntity, 0f);
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x00109B84 File Offset: 0x00107D84
	private bool IsInside(BaseEntity ent, float padding)
	{
		Bounds bounds = new Bounds(this.boxCollider.center, this.boxCollider.size);
		if (padding > 0f)
		{
			bounds.Expand(padding);
		}
		OBB obb = new OBB(this.boxCollider.transform, bounds);
		Vector3 vector = ((this.intersectionMode == TriggerParentEnclosed.TriggerMode.TriggerPoint) ? ent.TriggerPoint() : ent.PivotPoint());
		return obb.Contains(vector);
	}

	// Token: 0x040023A8 RID: 9128
	public float Padding;

	// Token: 0x040023A9 RID: 9129
	[Tooltip("AnyIntersect: Look for any intersection with the trigger. OriginIntersect: Only consider objects in the trigger if their origin is inside")]
	public TriggerParentEnclosed.TriggerMode intersectionMode;

	// Token: 0x040023AA RID: 9130
	public bool CheckBoundsOnUnparent;

	// Token: 0x040023AB RID: 9131
	private BoxCollider boxCollider;

	// Token: 0x02000D7A RID: 3450
	public enum TriggerMode
	{
		// Token: 0x0400480E RID: 18446
		TriggerPoint,
		// Token: 0x0400480F RID: 18447
		PivotPoint
	}
}
