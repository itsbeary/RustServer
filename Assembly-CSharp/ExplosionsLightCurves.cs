using System;
using UnityEngine;

// Token: 0x02000999 RID: 2457
public class ExplosionsLightCurves : MonoBehaviour
{
	// Token: 0x06003A53 RID: 14931 RVA: 0x0015850A File Offset: 0x0015670A
	private void Awake()
	{
		this.lightSource = base.GetComponent<Light>();
		this.lightSource.intensity = this.LightCurve.Evaluate(0f);
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x00158533 File Offset: 0x00156733
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x00158548 File Offset: 0x00156748
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float num2 = this.LightCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier;
			this.lightSource.intensity = num2;
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x040034FE RID: 13566
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040034FF RID: 13567
	public float GraphTimeMultiplier = 1f;

	// Token: 0x04003500 RID: 13568
	public float GraphIntensityMultiplier = 1f;

	// Token: 0x04003501 RID: 13569
	private bool canUpdate;

	// Token: 0x04003502 RID: 13570
	private float startTime;

	// Token: 0x04003503 RID: 13571
	private Light lightSource;
}
