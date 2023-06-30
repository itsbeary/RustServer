using System;

// Token: 0x020004D0 RID: 1232
public class ElevatorIOEntity : IOEntity
{
	// Token: 0x0600284A RID: 10314 RVA: 0x000FA59C File Offset: 0x000F879C
	public override int ConsumptionAmount()
	{
		return this.Consumption;
	}

	// Token: 0x040020A9 RID: 8361
	public int Consumption = 5;
}
