using System;
using UnityEngine;

// Token: 0x0200042C RID: 1068
public class TorpedoServerProjectile : ServerProjectile
{
	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06002436 RID: 9270 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06002437 RID: 9271 RVA: 0x000E700C File Offset: 0x000E520C
	protected override int mask
	{
		get
		{
			return 1237003009;
		}
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000E7014 File Offset: 0x000E5214
	public override bool DoMovement()
	{
		if (!base.DoMovement())
		{
			return false;
		}
		float num = WaterLevel.GetWaterInfo(base.transform.position, true, false, null, false).surfaceLevel - base.transform.position.y;
		if (num < -1f)
		{
			this.gravityModifier = 1f;
		}
		else if (num <= this.minWaterDepth)
		{
			Vector3 currentVelocity = base.CurrentVelocity;
			currentVelocity.y = 0f;
			base.CurrentVelocity = currentVelocity;
			this.gravityModifier = 0.1f;
		}
		else if (num > this.minWaterDepth + 0.3f && num <= this.minWaterDepth + 0.7f)
		{
			this.gravityModifier = -0.1f;
		}
		else
		{
			this.gravityModifier = Mathf.Clamp(base.CurrentVelocity.y, -0.1f, 0.1f);
		}
		return true;
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000E70E8 File Offset: 0x000E52E8
	public override void InitializeVelocity(Vector3 overrideVel)
	{
		base.InitializeVelocity(overrideVel);
		float num = WaterLevel.GetWaterInfo(base.transform.position, true, false, null, false).surfaceLevel - base.transform.position.y;
		float num2 = Mathf.InverseLerp(this.shallowWaterCutoff, this.shallowWaterCutoff + 2f, num);
		float num3 = Mathf.Lerp(this.shallowWaterInaccuracy, this.deepWaterInaccuracy, num2);
		this.initialVelocity = this.initialVelocity.GetWithInaccuracy(num3);
		base.CurrentVelocity = this.initialVelocity;
	}

	// Token: 0x04001C31 RID: 7217
	[Tooltip("Make sure to leave some allowance for waves, which affect the true depth.")]
	[SerializeField]
	private float minWaterDepth = 0.5f;

	// Token: 0x04001C32 RID: 7218
	[SerializeField]
	private float shallowWaterInaccuracy;

	// Token: 0x04001C33 RID: 7219
	[SerializeField]
	private float deepWaterInaccuracy;

	// Token: 0x04001C34 RID: 7220
	[SerializeField]
	private float shallowWaterCutoff = 2f;
}
