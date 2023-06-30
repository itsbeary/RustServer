using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public class ViewShake
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001E59 RID: 7769 RVA: 0x000CEB36 File Offset: 0x000CCD36
	// (set) Token: 0x06001E5A RID: 7770 RVA: 0x000CEB3E File Offset: 0x000CCD3E
	public Vector3 PositionOffset { get; protected set; }

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001E5B RID: 7771 RVA: 0x000CEB47 File Offset: 0x000CCD47
	// (set) Token: 0x06001E5C RID: 7772 RVA: 0x000CEB4F File Offset: 0x000CCD4F
	public Vector3 AnglesOffset { get; protected set; }

	// Token: 0x06001E5D RID: 7773 RVA: 0x000CEB58 File Offset: 0x000CCD58
	public void AddShake(float amplitude, float frequency, float duration)
	{
		this.Entries.Add(new ViewShake.ShakeParameters
		{
			amplitude = amplitude,
			frequency = Mathf.Max(frequency, 0.01f),
			duration = duration,
			endTime = Time.time + duration,
			nextShake = 0f,
			angle = 0f,
			infinite = (duration <= 0f)
		});
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000CEBC8 File Offset: 0x000CCDC8
	public void Update()
	{
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		this.Entries.RemoveAll((ViewShake.ShakeParameters i) => !i.infinite && Time.time > i.endTime);
		foreach (ViewShake.ShakeParameters shakeParameters in this.Entries)
		{
			if (Time.time > shakeParameters.nextShake)
			{
				shakeParameters.nextShake = Time.time + 1f / shakeParameters.frequency;
				shakeParameters.offset = new Vector3(UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude));
				shakeParameters.angle = UnityEngine.Random.Range(-shakeParameters.amplitude * 0.25f, shakeParameters.amplitude * 0.25f);
			}
			float num = 0f;
			float num2 = (shakeParameters.infinite ? 1f : ((shakeParameters.endTime - Time.time) / shakeParameters.duration));
			if (num2 != 0f)
			{
				num = shakeParameters.frequency / num2;
			}
			num2 *= num2;
			float num3 = Time.time * num;
			num2 *= Mathf.Sin(num3);
			vector += shakeParameters.offset * num2;
			zero.z += shakeParameters.angle * num2;
			if (!shakeParameters.infinite)
			{
				shakeParameters.amplitude -= shakeParameters.amplitude * Time.deltaTime / (shakeParameters.duration * shakeParameters.frequency);
			}
		}
		this.PositionOffset = vector * 0.01f;
		this.AnglesOffset = zero;
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x000CEDB4 File Offset: 0x000CCFB4
	public void Stop()
	{
		this.Entries.Clear();
		this.PositionOffset = Vector3.zero;
		this.AnglesOffset = Vector3.zero;
	}

	// Token: 0x04001793 RID: 6035
	protected List<ViewShake.ShakeParameters> Entries = new List<ViewShake.ShakeParameters>();

	// Token: 0x02000CB6 RID: 3254
	protected class ShakeParameters
	{
		// Token: 0x040044F3 RID: 17651
		public float endTime;

		// Token: 0x040044F4 RID: 17652
		public float duration;

		// Token: 0x040044F5 RID: 17653
		public float amplitude;

		// Token: 0x040044F6 RID: 17654
		public float frequency;

		// Token: 0x040044F7 RID: 17655
		public float nextShake;

		// Token: 0x040044F8 RID: 17656
		public float angle;

		// Token: 0x040044F9 RID: 17657
		public Vector3 offset;

		// Token: 0x040044FA RID: 17658
		public bool infinite;
	}
}
