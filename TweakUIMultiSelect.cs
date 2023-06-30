using System;
using System.Linq;
using UnityEngine.UI;

// Token: 0x0200088E RID: 2190
public class TweakUIMultiSelect : TweakUIBase
{
	// Token: 0x060036B4 RID: 14004 RVA: 0x00149BBC File Offset: 0x00147DBC
	protected override void Init()
	{
		base.Init();
		this.UpdateToggleGroup();
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x00149BCA File Offset: 0x00147DCA
	protected void OnEnable()
	{
		this.UpdateToggleGroup();
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x00149BD2 File Offset: 0x00147DD2
	public void OnChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x00149BDC File Offset: 0x00147DDC
	private void UpdateToggleGroup()
	{
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		foreach (Toggle toggle in this.toggleGroup.GetComponentsInChildren<Toggle>())
		{
			toggle.isOn = toggle.name == @string;
		}
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x00149C2C File Offset: 0x00147E2C
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		Toggle toggle = (from x in this.toggleGroup.GetComponentsInChildren<Toggle>()
			where x.isOn
			select x).FirstOrDefault<Toggle>();
		if (toggle == null)
		{
			return;
		}
		if (this.conVar.String == toggle.name)
		{
			return;
		}
		this.conVar.Set(toggle.name);
	}

	// Token: 0x0400316E RID: 12654
	public ToggleGroup toggleGroup;
}
