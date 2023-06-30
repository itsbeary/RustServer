using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4A RID: 2634
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinAttribute : Attribute
	{
		// Token: 0x06003F58 RID: 16216 RVA: 0x001732D7 File Offset: 0x001714D7
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x040038BC RID: 14524
		public readonly float min;
	}
}
