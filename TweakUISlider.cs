using System;
using TMPro;
using UnityEngine.UI;

// Token: 0x0200088F RID: 2191
public class TweakUISlider : TweakUIBase
{
	// Token: 0x060036BA RID: 14010 RVA: 0x00149CB3 File Offset: 0x00147EB3
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x00149824 File Offset: 0x00147A24
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x00149CC1 File Offset: 0x00147EC1
	public void OnChanged()
	{
		this.RefreshSliderDisplay(this.sliderControl.value);
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x00149CE4 File Offset: 0x00147EE4
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		float value = this.sliderControl.value;
		if (this.conVar.AsFloat == value)
		{
			return;
		}
		this.conVar.Set(value);
		this.RefreshSliderDisplay(this.conVar.AsFloat);
		TweakUISlider.lastConVarChanged = this.conVar.FullName;
		TweakUISlider.timeSinceLastConVarChange = 0f;
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x00149D57 File Offset: 0x00147F57
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		this.RefreshSliderDisplay(this.conVar.AsFloat);
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x00149D7C File Offset: 0x00147F7C
	private void RefreshSliderDisplay(float value)
	{
		this.sliderControl.value = value;
		if (this.sliderControl.wholeNumbers)
		{
			this.textControl.text = this.sliderControl.value.ToString("N0");
			return;
		}
		this.textControl.text = this.sliderControl.value.ToString("0.0");
	}

	// Token: 0x0400316F RID: 12655
	public Slider sliderControl;

	// Token: 0x04003170 RID: 12656
	public TextMeshProUGUI textControl;

	// Token: 0x04003171 RID: 12657
	public static string lastConVarChanged;

	// Token: 0x04003172 RID: 12658
	public static TimeSince timeSinceLastConVarChange;
}
