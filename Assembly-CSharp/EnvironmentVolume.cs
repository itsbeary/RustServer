using System;
using UnityEngine;

// Token: 0x0200050D RID: 1293
public class EnvironmentVolume : MonoBehaviour
{
	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06002986 RID: 10630 RVA: 0x000FEF0C File Offset: 0x000FD10C
	// (set) Token: 0x06002987 RID: 10631 RVA: 0x000FEF14 File Offset: 0x000FD114
	public Collider trigger { get; private set; }

	// Token: 0x06002988 RID: 10632 RVA: 0x000FEF1D File Offset: 0x000FD11D
	protected virtual void Awake()
	{
		this.UpdateTrigger();
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000FEF25 File Offset: 0x000FD125
	protected void OnEnable()
	{
		if (this.trigger && !this.trigger.enabled)
		{
			this.trigger.enabled = true;
		}
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000FEF4D File Offset: 0x000FD14D
	protected void OnDisable()
	{
		if (this.trigger && this.trigger.enabled)
		{
			this.trigger.enabled = false;
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x000FEF78 File Offset: 0x000FD178
	public void UpdateTrigger()
	{
		if (!this.trigger)
		{
			this.trigger = base.gameObject.GetComponent<Collider>();
		}
		if (!this.trigger)
		{
			this.trigger = base.gameObject.AddComponent<BoxCollider>();
		}
		this.trigger.isTrigger = true;
		BoxCollider boxCollider = this.trigger as BoxCollider;
		if (boxCollider)
		{
			boxCollider.center = this.Center;
			boxCollider.size = this.Size;
		}
	}

	// Token: 0x0400219C RID: 8604
	[InspectorFlags]
	public EnvironmentType Type = EnvironmentType.Underground;

	// Token: 0x0400219D RID: 8605
	public Vector3 Center = Vector3.zero;

	// Token: 0x0400219E RID: 8606
	public Vector3 Size = Vector3.one;
}
