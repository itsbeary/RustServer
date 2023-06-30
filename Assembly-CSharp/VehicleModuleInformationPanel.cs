using System;

// Token: 0x02000861 RID: 2145
public class VehicleModuleInformationPanel : ItemInformationPanel
{
	// Token: 0x0400306C RID: 12396
	public ItemStatValue socketsDisplay;

	// Token: 0x0400306D RID: 12397
	public ItemStatValue hpDisplay;

	// Token: 0x02000E9C RID: 3740
	public interface IVehicleModuleInfo
	{
		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06005306 RID: 21254
		int SocketsTaken { get; }
	}
}
