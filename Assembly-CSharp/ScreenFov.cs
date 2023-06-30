using System;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class ScreenFov : BaseScreenShake
{
	// Token: 0x06001F89 RID: 8073 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Setup()
	{
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000D53F2 File Offset: 0x000D35F2
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (cam)
		{
			cam.component.fieldOfView += this.FovAdjustment.Evaluate(delta);
		}
	}

	// Token: 0x040018D6 RID: 6358
	public AnimationCurve FovAdjustment;
}
