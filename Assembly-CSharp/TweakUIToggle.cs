using System;
using UnityEngine.UI;

// Token: 0x02000890 RID: 2192
public class TweakUIToggle : TweakUIBase
{
	// Token: 0x060036C1 RID: 14017 RVA: 0x00149CB3 File Offset: 0x00147EB3
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x060036C2 RID: 14018 RVA: 0x00149824 File Offset: 0x00147A24
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x00149970 File Offset: 0x00147B70
	public void OnToggleChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x00149DEC File Offset: 0x00147FEC
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.toggleControl.isOn;
		if (this.inverse)
		{
			flag = !flag;
		}
		if (this.conVar.AsBool == flag)
		{
			return;
		}
		TweakUIToggle.lastConVarChanged = this.conVar.FullName;
		TweakUIToggle.timeSinceLastConVarChange = 0f;
		this.conVar.Set(flag);
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x00149E5C File Offset: 0x0014805C
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.conVar.AsBool;
		if (this.inverse)
		{
			flag = !flag;
		}
		this.toggleControl.isOn = flag;
	}

	// Token: 0x04003173 RID: 12659
	public Toggle toggleControl;

	// Token: 0x04003174 RID: 12660
	public bool inverse;

	// Token: 0x04003175 RID: 12661
	public static string lastConVarChanged;

	// Token: 0x04003176 RID: 12662
	public static TimeSince timeSinceLastConVarChange;
}
