using System;

// Token: 0x020003CC RID: 972
public class EntityFlag_ToggleNotify : EntityFlag_Toggle
{
	// Token: 0x060021E1 RID: 8673 RVA: 0x000DC7C8 File Offset: 0x000DA9C8
	protected override void OnStateToggled(bool state)
	{
		base.OnStateToggled(state);
		IFlagNotify flagNotify;
		if (!this.UseEntityParent && base.baseEntity != null && (flagNotify = base.baseEntity as IFlagNotify) != null)
		{
			flagNotify.OnFlagToggled(state);
		}
		IFlagNotify flagNotify2;
		if (this.UseEntityParent && base.baseEntity != null && base.baseEntity.GetParentEntity() != null && (flagNotify2 = base.baseEntity.GetParentEntity() as IFlagNotify) != null)
		{
			flagNotify2.OnFlagToggled(state);
		}
	}

	// Token: 0x04001A44 RID: 6724
	public bool UseEntityParent;
}
