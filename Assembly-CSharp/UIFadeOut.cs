using System;
using UnityEngine;

// Token: 0x0200080D RID: 2061
public class UIFadeOut : MonoBehaviour
{
	// Token: 0x060035BD RID: 13757 RVA: 0x00147457 File Offset: 0x00145657
	private void Start()
	{
		this.timeStarted = Time.realtimeSinceStartup;
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x00147464 File Offset: 0x00145664
	private void Update()
	{
		float num = this.timeStarted;
		this.targetGroup.alpha = Mathf.InverseLerp(num + this.secondsToFadeOut, num, Time.realtimeSinceStartup - this.fadeDelay);
		if (this.destroyOnFaded && Time.realtimeSinceStartup - this.fadeDelay > this.timeStarted + this.secondsToFadeOut)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
	}

	// Token: 0x04002E78 RID: 11896
	public float secondsToFadeOut = 3f;

	// Token: 0x04002E79 RID: 11897
	public bool destroyOnFaded = true;

	// Token: 0x04002E7A RID: 11898
	public CanvasGroup targetGroup;

	// Token: 0x04002E7B RID: 11899
	public float fadeDelay;

	// Token: 0x04002E7C RID: 11900
	private float timeStarted;
}
