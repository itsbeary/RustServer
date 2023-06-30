using System;
using TMPro;
using UnityEngine;

// Token: 0x020008DD RID: 2269
public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
	// Token: 0x0600377F RID: 14207 RVA: 0x0014CBF6 File Offset: 0x0014ADF6
	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.visible = true;
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x0014CC14 File Offset: 0x0014AE14
	public void SetVisible(bool b)
	{
		if (this.visible == b)
		{
			return;
		}
		this.visible = b;
		this.canvasGroup.alpha = (this.visible ? 1f : 0f);
		SingletonComponent<UISleepingScreen>.Instance.gameObject.SetChildComponentsEnabled(this.visible);
	}

	// Token: 0x040032E8 RID: 13032
	protected CanvasGroup canvasGroup;

	// Token: 0x040032E9 RID: 13033
	private bool visible;
}
