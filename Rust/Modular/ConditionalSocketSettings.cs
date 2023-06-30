using System;

namespace Rust.Modular
{
	// Token: 0x02000B32 RID: 2866
	[Serializable]
	public class ConditionalSocketSettings
	{
		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x0600455D RID: 17757 RVA: 0x00195A83 File Offset: 0x00193C83
		public bool HasSocketRestrictions
		{
			get
			{
				return this.restrictOnLocation || this.restrictOnWheel;
			}
		}

		// Token: 0x04003E66 RID: 15974
		public bool restrictOnLocation;

		// Token: 0x04003E67 RID: 15975
		public ConditionalSocketSettings.LocationCondition locationRestriction;

		// Token: 0x04003E68 RID: 15976
		public bool restrictOnWheel;

		// Token: 0x04003E69 RID: 15977
		public ModularVehicleSocket.SocketWheelType wheelRestriction;

		// Token: 0x02000FA6 RID: 4006
		public enum LocationCondition
		{
			// Token: 0x040050E9 RID: 20713
			Middle,
			// Token: 0x040050EA RID: 20714
			Front,
			// Token: 0x040050EB RID: 20715
			Back,
			// Token: 0x040050EC RID: 20716
			NotMiddle,
			// Token: 0x040050ED RID: 20717
			NotFront,
			// Token: 0x040050EE RID: 20718
			NotBack
		}
	}
}
