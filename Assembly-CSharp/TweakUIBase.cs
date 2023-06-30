using System;
using UnityEngine;

// Token: 0x0200088C RID: 2188
public class TweakUIBase : MonoBehaviour
{
	// Token: 0x060036A1 RID: 13985 RVA: 0x0014979E File Offset: 0x0014799E
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x001497A8 File Offset: 0x001479A8
	protected virtual void Init()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar == null)
		{
			Debug.LogWarning("TweakUI Convar Missing: " + this.convarName, base.gameObject);
			return;
		}
		this.conVar.OnValueChanged += this.OnConVarChanged;
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x00149802 File Offset: 0x00147A02
	public virtual void OnApplyClicked()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.SetConvarValue();
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x00149813 File Offset: 0x00147A13
	public virtual void UnapplyChanges()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.ResetToConvar();
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x00149824 File Offset: 0x00147A24
	protected virtual void OnConVarChanged(ConsoleSystem.Command obj)
	{
		this.ResetToConvar();
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ResetToConvar()
	{
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void SetConvarValue()
	{
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x0014982C File Offset: 0x00147A2C
	private void OnDestroy()
	{
		if (this.conVar != null)
		{
			this.conVar.OnValueChanged -= this.OnConVarChanged;
		}
	}

	// Token: 0x04003161 RID: 12641
	public string convarName = "effects.motionblur";

	// Token: 0x04003162 RID: 12642
	public bool ApplyImmediatelyOnChange = true;

	// Token: 0x04003163 RID: 12643
	internal ConsoleSystem.Command conVar;
}
