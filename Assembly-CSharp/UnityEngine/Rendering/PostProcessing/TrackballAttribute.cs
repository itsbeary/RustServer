using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4D RID: 2637
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TrackballAttribute : Attribute
	{
		// Token: 0x06003F5C RID: 16220 RVA: 0x0017334C File Offset: 0x0017154C
		public TrackballAttribute(TrackballAttribute.Mode mode)
		{
			this.mode = mode;
		}

		// Token: 0x040038C4 RID: 14532
		public readonly TrackballAttribute.Mode mode;

		// Token: 0x02000F31 RID: 3889
		public enum Mode
		{
			// Token: 0x04004F0B RID: 20235
			None,
			// Token: 0x04004F0C RID: 20236
			Lift,
			// Token: 0x04004F0D RID: 20237
			Gamma,
			// Token: 0x04004F0E RID: 20238
			Gain
		}
	}
}
