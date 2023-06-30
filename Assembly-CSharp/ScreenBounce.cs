using System;
using UnityEngine;

// Token: 0x02000359 RID: 857
public class ScreenBounce : BaseScreenShake
{
	// Token: 0x06001F83 RID: 8067 RVA: 0x000D50C6 File Offset: 0x000D32C6
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000D50E0 File Offset: 0x000D32E0
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num = this.bounceScale.Evaluate(delta) * 0.1f;
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}

	// Token: 0x040018C8 RID: 6344
	public AnimationCurve bounceScale;

	// Token: 0x040018C9 RID: 6345
	public AnimationCurve bounceSpeed;

	// Token: 0x040018CA RID: 6346
	public AnimationCurve bounceViewmodel;

	// Token: 0x040018CB RID: 6347
	private float bounceTime;

	// Token: 0x040018CC RID: 6348
	private Vector3 bounceVelocity = Vector3.zero;
}
