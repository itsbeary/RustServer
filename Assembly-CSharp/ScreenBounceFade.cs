using System;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class ScreenBounceFade : BaseScreenShake
{
	// Token: 0x06001F86 RID: 8070 RVA: 0x000D5221 File Offset: 0x000D3421
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000D5238 File Offset: 0x000D3438
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		float num = Vector3.Distance(cam.position, base.transform.position);
		float num2 = 1f - Mathf.InverseLerp(0f, this.maxDistance, num);
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num3 = this.distanceFalloff.Evaluate(num2);
		float num4 = this.bounceScale.Evaluate(delta) * 0.1f * num3 * this.scale * this.timeFalloff.Evaluate(delta);
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num4;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num4;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		vector *= num2;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}

	// Token: 0x040018CD RID: 6349
	public AnimationCurve bounceScale;

	// Token: 0x040018CE RID: 6350
	public AnimationCurve bounceSpeed;

	// Token: 0x040018CF RID: 6351
	public AnimationCurve bounceViewmodel;

	// Token: 0x040018D0 RID: 6352
	public AnimationCurve distanceFalloff;

	// Token: 0x040018D1 RID: 6353
	public AnimationCurve timeFalloff;

	// Token: 0x040018D2 RID: 6354
	private float bounceTime;

	// Token: 0x040018D3 RID: 6355
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x040018D4 RID: 6356
	public float maxDistance = 10f;

	// Token: 0x040018D5 RID: 6357
	public float scale = 1f;
}
