using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000358 RID: 856
public abstract class BaseScreenShake : MonoBehaviour
{
	// Token: 0x06001F7B RID: 8059 RVA: 0x000D4F94 File Offset: 0x000D3194
	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cachedTransform = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> cachedTransform2 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < BaseScreenShake.list.Count; i++)
		{
			BaseScreenShake.list[i].Run(ref cachedTransform, ref cachedTransform2);
		}
		cachedTransform.Apply();
		cachedTransform2.Apply();
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000D4FE8 File Offset: 0x000D31E8
	protected void OnEnable()
	{
		BaseScreenShake.list.Add(this);
		this.timeTaken = 0f;
		this.Setup();
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000D5006 File Offset: 0x000D3206
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		BaseScreenShake.list.Remove(this);
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000D501C File Offset: 0x000D321C
	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (this.timeTaken > this.length)
		{
			return;
		}
		if (Time.frameCount != this.currentFrame)
		{
			this.timeTaken += Time.deltaTime;
			this.currentFrame = Time.frameCount;
		}
		float num = Mathf.InverseLerp(0f, this.length, this.timeTaken);
		this.Run(num, ref cam, ref vm);
	}

	// Token: 0x06001F7F RID: 8063
	public abstract void Setup();

	// Token: 0x06001F80 RID: 8064
	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);

	// Token: 0x040018C1 RID: 6337
	public static List<BaseScreenShake> list = new List<BaseScreenShake>();

	// Token: 0x040018C2 RID: 6338
	internal static float punchFadeScale = 0f;

	// Token: 0x040018C3 RID: 6339
	internal static float bobScale = 0f;

	// Token: 0x040018C4 RID: 6340
	internal static float animPunchMagnitude = 10f;

	// Token: 0x040018C5 RID: 6341
	public float length = 2f;

	// Token: 0x040018C6 RID: 6342
	internal float timeTaken;

	// Token: 0x040018C7 RID: 6343
	private int currentFrame = -1;
}
