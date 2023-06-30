using System;

// Token: 0x020004D2 RID: 1234
public class FuseBox : IOEntity
{
	// Token: 0x06002856 RID: 10326 RVA: 0x000FA69C File Offset: 0x000F889C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
