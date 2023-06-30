using System;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class ScaleTrailRenderer : ScaleRenderer
{
	// Token: 0x06001F75 RID: 8053 RVA: 0x000D4E24 File Offset: 0x000D3024
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (this.myRenderer)
		{
			this.trailRenderer = this.myRenderer.GetComponent<TrailRenderer>();
		}
		else
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		this.startWidth = this.trailRenderer.startWidth;
		this.endWidth = this.trailRenderer.endWidth;
		this.duration = this.trailRenderer.time;
		this.startMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000D4EA8 File Offset: 0x000D30A8
	public override void SetScale_Internal(float scale)
	{
		if (scale == 0f)
		{
			this.trailRenderer.emitting = false;
			this.trailRenderer.enabled = false;
			this.trailRenderer.time = 0f;
			this.trailRenderer.Clear();
			return;
		}
		if (!this.trailRenderer.emitting)
		{
			this.trailRenderer.Clear();
		}
		this.trailRenderer.emitting = true;
		this.trailRenderer.enabled = true;
		base.SetScale_Internal(scale);
		this.trailRenderer.widthMultiplier = this.startMultiplier * scale;
		this.trailRenderer.time = this.duration * scale;
	}

	// Token: 0x040018BB RID: 6331
	private TrailRenderer trailRenderer;

	// Token: 0x040018BC RID: 6332
	[NonSerialized]
	private float startWidth;

	// Token: 0x040018BD RID: 6333
	[NonSerialized]
	private float endWidth;

	// Token: 0x040018BE RID: 6334
	[NonSerialized]
	private float duration;

	// Token: 0x040018BF RID: 6335
	[NonSerialized]
	private float startMultiplier;
}
