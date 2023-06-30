using System;

// Token: 0x020004D5 RID: 1237
public class SimpleLight : IOEntity
{
	// Token: 0x0600285F RID: 10335 RVA: 0x000FA7CF File Offset: 0x000F89CF
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000FA69C File Offset: 0x000F889C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
