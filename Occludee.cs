using System;
using UnityEngine;

// Token: 0x020009A3 RID: 2467
public class Occludee : MonoBehaviour
{
	// Token: 0x06003A81 RID: 14977 RVA: 0x001591A4 File Offset: 0x001573A4
	protected virtual void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.collider = base.GetComponent<Collider>();
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x001591BE File Offset: 0x001573BE
	public void OnEnable()
	{
		if (this.autoRegister && this.collider != null)
		{
			this.Register();
		}
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x001591DC File Offset: 0x001573DC
	public void OnDisable()
	{
		if (this.autoRegister && this.occludeeId >= 0)
		{
			this.Unregister();
		}
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x001591F8 File Offset: 0x001573F8
	public void Register()
	{
		this.center = this.collider.bounds.center;
		this.radius = Mathf.Max(Mathf.Max(this.collider.bounds.extents.x, this.collider.bounds.extents.y), this.collider.bounds.extents.z);
		this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.enabled, this.minTimeVisible, this.isStatic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (this.occludeeId < 0)
		{
			Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
		}
		this.state = OcclusionCulling.GetStateById(this.occludeeId);
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x001592F0 File Offset: 0x001574F0
	public void Unregister()
	{
		OcclusionCulling.UnregisterOccludee(this.occludeeId);
	}

	// Token: 0x06003A86 RID: 14982 RVA: 0x001592FD File Offset: 0x001574FD
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.renderer != null)
		{
			this.renderer.enabled = visible;
		}
	}

	// Token: 0x04003545 RID: 13637
	public float minTimeVisible = 0.1f;

	// Token: 0x04003546 RID: 13638
	public bool isStatic = true;

	// Token: 0x04003547 RID: 13639
	public bool autoRegister;

	// Token: 0x04003548 RID: 13640
	public bool stickyGizmos;

	// Token: 0x04003549 RID: 13641
	public OccludeeState state;

	// Token: 0x0400354A RID: 13642
	protected int occludeeId = -1;

	// Token: 0x0400354B RID: 13643
	protected Vector3 center;

	// Token: 0x0400354C RID: 13644
	protected float radius;

	// Token: 0x0400354D RID: 13645
	protected Renderer renderer;

	// Token: 0x0400354E RID: 13646
	protected Collider collider;
}
