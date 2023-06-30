using System;
using UnityEngine;

// Token: 0x02000442 RID: 1090
public class LifeScale : BaseMonoBehaviour
{
	// Token: 0x060024B5 RID: 9397 RVA: 0x000E93FE File Offset: 0x000E75FE
	protected void Awake()
	{
		this.updateScaleAction = new Action(this.UpdateScale);
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000E9412 File Offset: 0x000E7612
	public void OnEnable()
	{
		this.Init();
		base.transform.localScale = this.initialScale;
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000E942B File Offset: 0x000E762B
	public void SetProgress(float progress)
	{
		this.Init();
		this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
		base.InvokeRepeating(this.updateScaleAction, 0f, 0.015f);
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000E9461 File Offset: 0x000E7661
	public void Init()
	{
		if (!this.initialized)
		{
			this.initialScale = base.transform.localScale;
			this.initialized = true;
		}
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000E9484 File Offset: 0x000E7684
	public void UpdateScale()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetLerpScale, Time.deltaTime);
		if (base.transform.localScale == this.targetLerpScale)
		{
			this.targetLerpScale = Vector3.zero;
			base.CancelInvoke(this.updateScaleAction);
		}
	}

	// Token: 0x04001CA0 RID: 7328
	[NonSerialized]
	private bool initialized;

	// Token: 0x04001CA1 RID: 7329
	[NonSerialized]
	private Vector3 initialScale;

	// Token: 0x04001CA2 RID: 7330
	public Vector3 finalScale = Vector3.one;

	// Token: 0x04001CA3 RID: 7331
	private Vector3 targetLerpScale = Vector3.zero;

	// Token: 0x04001CA4 RID: 7332
	private Action updateScaleAction;
}
