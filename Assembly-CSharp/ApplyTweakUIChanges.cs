using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000884 RID: 2180
public class ApplyTweakUIChanges : MonoBehaviour
{
	// Token: 0x06003684 RID: 13956 RVA: 0x00149462 File Offset: 0x00147662
	private void OnEnable()
	{
		this.SetClean();
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x0014946C File Offset: 0x0014766C
	public void Apply()
	{
		if (this.Options == null)
		{
			return;
		}
		foreach (TweakUIBase tweakUIBase in this.Options)
		{
			if (!(tweakUIBase == null))
			{
				tweakUIBase.OnApplyClicked();
			}
		}
		this.SetClean();
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x001494B0 File Offset: 0x001476B0
	public void SetDirty()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = true;
		}
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x001494CC File Offset: 0x001476CC
	public void SetClean()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = false;
		}
	}

	// Token: 0x0400314F RID: 12623
	public Button ApplyButton;

	// Token: 0x04003150 RID: 12624
	public TweakUIBase[] Options;
}
