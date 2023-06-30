using System;

// Token: 0x02000597 RID: 1431
public class TriggerParentElevator : TriggerParentEnclosed
{
	// Token: 0x06002BCA RID: 11210 RVA: 0x00109AEE File Offset: 0x00107CEE
	protected override bool IsClipping(BaseEntity ent)
	{
		return (!this.AllowHorsesToBypassClippingChecks || !(ent is BaseRidableAnimal)) && base.IsClipping(ent);
	}

	// Token: 0x040023A7 RID: 9127
	public bool AllowHorsesToBypassClippingChecks = true;
}
