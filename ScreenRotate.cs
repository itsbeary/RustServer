using System;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class ScreenRotate : BaseScreenShake
{
	// Token: 0x06001F8C RID: 8076 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Setup()
	{
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000D5428 File Offset: 0x000D3628
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		Vector3 zero = Vector3.zero;
		zero.x = this.Pitch.Evaluate(delta);
		zero.y = this.Yaw.Evaluate(delta);
		zero.z = this.Roll.Evaluate(delta);
		if (cam)
		{
			cam.rotation *= Quaternion.Euler(zero * this.scale);
		}
		if (vm && this.useViewModelEffect)
		{
			vm.rotation *= Quaternion.Euler(zero * this.scale * -1f * (1f - this.ViewmodelEffect.Evaluate(delta)));
		}
	}

	// Token: 0x040018D7 RID: 6359
	public AnimationCurve Pitch;

	// Token: 0x040018D8 RID: 6360
	public AnimationCurve Yaw;

	// Token: 0x040018D9 RID: 6361
	public AnimationCurve Roll;

	// Token: 0x040018DA RID: 6362
	public AnimationCurve ViewmodelEffect;

	// Token: 0x040018DB RID: 6363
	public float scale = 1f;

	// Token: 0x040018DC RID: 6364
	public bool useViewModelEffect = true;
}
