using System;

namespace Rust.Modular
{
	// Token: 0x02000B34 RID: 2868
	public static class EngineItemTypeEx
	{
		// Token: 0x06004562 RID: 17762 RVA: 0x00195ABF File Offset: 0x00193CBF
		public static bool BoostsAcceleration(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.SparkPlug || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x06004563 RID: 17763 RVA: 0x00195ACB File Offset: 0x00193CCB
		public static bool BoostsTopSpeed(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Crankshaft || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x06004564 RID: 17764 RVA: 0x0015933A File Offset: 0x0015753A
		public static bool BoostsFuelEconomy(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Valve;
		}
	}
}
