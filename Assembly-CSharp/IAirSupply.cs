using System;

// Token: 0x020005F7 RID: 1527
public interface IAirSupply
{
	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x06002DB3 RID: 11699
	ItemModGiveOxygen.AirSupplyType AirType { get; }

	// Token: 0x06002DB4 RID: 11700
	float GetAirTimeRemaining();
}
